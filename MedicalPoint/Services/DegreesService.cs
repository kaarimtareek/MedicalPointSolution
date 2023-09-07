using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IDegreesService
    {
        List<Degree> GetAll(bool activeOnly = true);
        Task<OperationResult<Degree>> Add(string name, CancellationToken cancellationToken = default);
        Task<OperationResult<Degree>> Edit(int id, string name, CancellationToken cancellationToken = default);
        Task<OperationResult<Degree>> Remove(int id, CancellationToken cancellationToken = default);
        Task<Degree> GetById(int id, CancellationToken cancellationToken = default);

    }

    public class DegreesService : IDegreesService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public DegreesService(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }
        public async Task<Degree> GetById(int id, CancellationToken cancellationToken = default)
        {
            var clinic = await _context.Degrees.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);


            return clinic;
        }
        public List<Degree> GetAll(bool activeOnly = true)
        {
            return _cacheService.GetDegrees(); 
                //QueryFinder.GetDegrees(_context, activeOnly);
        }
        public async Task<OperationResult<Degree>> Add(string name, CancellationToken cancellationToken = default)
        {
            name = name.Trim();
            if(await _context.Degrees.AnyAsync(x=> x.Name == name, cancellationToken))
            {
                return OperationResult<Degree>.Failed(ConstantMessageCodes.DegreeAlreadyExist);
            }
            var degree = new Degree
            {
                Name = name,
            };
            await _context.Degrees.AddAsync(degree, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _cacheService.UpdateDegrees();
            return OperationResult<Degree>.Succeeded(degree);
        }
        public async Task<OperationResult<Degree>> Edit(int id, string name, CancellationToken cancellationToken = default)
        {
            name = name.Trim();
            if (await _context.Degrees.AnyAsync(x => x.Name == name && x.Id != id, cancellationToken))
            {
                return OperationResult<Degree>.Failed(ConstantMessageCodes.DegreeAlreadyExist);
            }
            var degree = await _context.Degrees.FirstOrDefaultAsync(x=> x.Id == id, cancellationToken);
            if (degree == null)
            {
                return OperationResult<Degree>.Failed(ConstantMessageCodes.DegreeAlreadyExist);
            }
            degree.Name = name;
            await _context.SaveChangesAsync(cancellationToken);
            _cacheService.UpdateDegrees();
            return OperationResult<Degree>.Succeeded(degree);
        }

        public async Task<OperationResult<Degree>> Remove(int id, CancellationToken cancellationToken = default)
        {
           
            var degree = await _context.Degrees.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (degree == null)
            {
                return OperationResult<Degree>.Failed(ConstantMessageCodes.DegreeNotFound);
            }
            _context.Degrees.Remove(degree);
            await _context.SaveChangesAsync(cancellationToken);
            _cacheService.UpdateDegrees();
            return OperationResult<Degree>.Succeeded(degree);
        }
    }
}
