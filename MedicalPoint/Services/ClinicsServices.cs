using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IClinicsServices
    {
        Task<OperationResult<Clinic>> Add(string name, CancellationToken cancellationToken = default);
        Task<OperationResult<Clinic>> Edit(int id, string name, bool isActive, CancellationToken cancellationToken = default);
        List<Clinic> GetAll(bool activeOnly = true);
        Task<OperationResult<Clinic>> Remove(int id, CancellationToken cancellationToken = default);
        Task<Clinic> GetById(int id, CancellationToken cancellationToken = default);

    }

    public class ClinicsServices : IClinicsServices
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public ClinicsServices(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<Clinic> GetById(int id, CancellationToken cancellationToken = default)
        {
            var clinic = await _context.Clinics.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);


            return clinic;
        }
        public List<Clinic> GetAll(bool activeOnly = true)
        {
            return  _cacheService.GetClinics();
        }
        public async Task<OperationResult<Clinic>> Add(string name, CancellationToken cancellationToken = default)
        {
            name = name.Trim();
            if (await _context.Clinics.AnyAsync(x => x.Name == name, cancellationToken))
            {
                return OperationResult<Clinic>.Failed(ConstantMessageCodes.DegreeAlreadyExist);
            }
            var clinic = new Clinic
            {
                Name = name,
                IsActive = true,
            };
            await _context.Clinics.AddAsync(clinic, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _cacheService.UpdateClinics();
            return OperationResult<Clinic>.Succeeded(clinic);
        }
        public async Task<OperationResult<Clinic>> Edit(int id, string name, bool isActive, CancellationToken cancellationToken = default)
        {
            name = name.Trim();
            if (await _context.Clinics.AnyAsync(x => x.Name == name && x.Id != id, cancellationToken))
            {
                return OperationResult<Clinic>.Failed(ConstantMessageCodes.ClinicNameAlreadyExist);
            }
            var clinic = await _context.Clinics.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (clinic == null)
            {
                return OperationResult<Clinic>.Failed(ConstantMessageCodes.ClinicNotFound);
            }
            clinic.Name = name;
            clinic.IsActive = isActive;

            await _context.SaveChangesAsync(cancellationToken);
            _cacheService.UpdateClinics();
            return OperationResult<Clinic>.Succeeded(clinic);
        }

        public async Task<OperationResult<Clinic>> Remove(int id, CancellationToken cancellationToken = default)
        {

            var clinic = await _context.Clinics.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (clinic == null)
            {
                return OperationResult<Clinic>.Failed(ConstantMessageCodes.ClinicNotFound);
            }
            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync(cancellationToken);
            _cacheService.UpdateClinics();
            return OperationResult<Clinic>.Succeeded(clinic);
        }
    }
}
