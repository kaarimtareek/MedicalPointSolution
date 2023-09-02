using System.Security.Cryptography;
using System.Text;

using MedicalPoint.Common;
using MedicalPoint.Data;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IMedicalPointUsersService
    {
        Task<OperationResult<MedicalPointUser>> ChangePassword(int userId, string oldPassword, string newPassword, CancellationToken cancellationToken = default);
        Task<OperationResult<MedicalPointUser>> ChangePassword(int userId, string newPassword, CancellationToken cancellationToken = default);
        Task<OperationResult<MedicalPointUser>> ChangeUserActiveStatus(int userId, CancellationToken cancellationToken = default);
        Task<OperationResult<MedicalPointUser>> Create(string email, string password, string accountType, string name, int degree, string militaryNumber, string? phonenumber, CancellationToken cancellationToken = default);
        Task<OperationResult<MedicalPointUser>> Edit(int userId, string email, string name, int degree, string militaryNumber, string? phonenumber, string? Accounttype, CancellationToken cancellationToken = default);
        Task<MedicalPointUser> Get(int id);
        Task<List<MedicalPointUser>> GetUsers(string searchValue = "", int? degree = null, CancellationToken cancellationToken = default);

        Task<OperationResult<MedicalPointUser>> Login(string email, string password);
    }

    public class MedicalPointUsersService : IMedicalPointUsersService
    {
        private readonly int saltSize = 128 / 8;
        private readonly ApplicationDbContext _context;

        public MedicalPointUsersService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<List<MedicalPointUser>> GetUsers(string searchValue = "", int? degree = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Users.AsNoTracking()
                .Include(x => x.Degree)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(x => x.FullName.Contains(searchValue));
                query = query.Where(x => x.Email.Contains(searchValue));
                query = query.Where(x => x.MilitaryNumber.Contains(searchValue));

            }
            if (degree.HasValue)
            {
                query = query.Where(x => x.DegreeId == degree.Value);
            }
            var users = await query.ToListAsync(cancellationToken);

            return users;
        }
        public async Task<MedicalPointUser> Get(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(x=> x.Degree)
                .FirstOrDefaultAsync(x => x.Id == id);  

            return user;

        }
        public async Task<OperationResult<MedicalPointUser>> Login(string email, string password)
        {
            var user = QueryFinder.GetUserByEmail(_context, email);
            if (user == null || !VerifyHashedPassword(password, user.Password, user.Salt))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            if (!user.IsActive)
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            //if (user.AccoutType != accountType)
            //{
            //    return OperationResult<MedicalPointUser>.Failed("");

            //}
            return OperationResult<MedicalPointUser>.Succeeded(user);

        }

        public async Task<OperationResult<MedicalPointUser>> Create(string email, string password, string accountType, string name, int degree, string militaryNumber, string? phonenumber, CancellationToken cancellationToken = default)
        {
            //check if military number, email or phonenumber already exist
            if (QueryValidator.IsUserEmailExist(_context, email))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            if (QueryValidator.IsUserMilitaryNumberExist(_context, militaryNumber))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            if (!string.IsNullOrEmpty(phonenumber) && QueryValidator.IsUserPhoneNumberExist(_context, phonenumber))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            var generatedSalt = GenerateRandomSalt(saltSize);
            var hashedPassword = HashPassword(password, generatedSalt);
            var user = new MedicalPointUser
            {
                Email = email,
                Password = hashedPassword,
                AccoutType = accountType,
                DegreeId = degree,
                FullName = name,
                IsActive = true,
                MilitaryNumber = militaryNumber??"",
                PhoneNumber = phonenumber??"",
                Salt = generatedSalt,
            };
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<MedicalPointUser>.Succeeded(user);
        }
        public async Task<OperationResult<MedicalPointUser>> Edit(int userId, string email, string name, int degree, string militaryNumber, string? phonenumber, string?Accounttype, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=> x.Id == userId);
            if (user == null)
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            //check if military number, email or phonenumber already exist
            if (QueryValidator.IsUserEmailExist(_context, email, userId))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            if (QueryValidator.IsUserMilitaryNumberExist(_context, militaryNumber, userId))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            if (!string.IsNullOrEmpty(phonenumber) && QueryValidator.IsUserPhoneNumberExist(_context, phonenumber, userId))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            user.Email = email;
            user.PhoneNumber = phonenumber??"";
            user.MilitaryNumber = militaryNumber??"";
            user.FullName = name;
            user.DegreeId = degree;
          
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<MedicalPointUser>.Succeeded(user);
        }
        public async Task<OperationResult<MedicalPointUser>> ChangePassword(int userId, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            if (!VerifyHashedPassword(oldPassword, user.Password, user.Salt))
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            var generatedSalt = GenerateRandomSalt(saltSize);
            var generatedHash = HashPassword(newPassword, generatedSalt);
            user.Password = generatedHash;
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<MedicalPointUser>.Succeeded(user);

        }
        public async Task<OperationResult<MedicalPointUser>> ChangePassword(int userId,  string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }
            
            var generatedSalt = GenerateRandomSalt(saltSize);
            var generatedHash = HashPassword(newPassword, generatedSalt);
            user.Password = generatedHash;
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<MedicalPointUser>.Succeeded(user);

        }
        public async Task<OperationResult<MedicalPointUser>> ChangeUserActiveStatus(int userId,  CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
            if (user == null)
            {
                return OperationResult<MedicalPointUser>.Failed("");
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<MedicalPointUser>.Succeeded(user);

        }
        private string HashPassword(string password, byte[] salt)
        {
            // divide by 8 to convert bits to bytes
            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return hashed;
        }
        private bool VerifyHashedPassword(string providedPassword, string hashed, byte[] salt)
        {
            return HashPassword(providedPassword, salt) == hashed;
        }
        private byte[] GenerateRandomSalt(int byteSize)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(byteSize);
            return salt;
        }
    }
}
