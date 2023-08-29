using MedicalPoint.Common;
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
    }

    public class ClinicsServices : IClinicsServices
    {
        private readonly ApplicationDbContext _context;

        public ClinicsServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Clinic> GetAll(bool activeOnly = true)
        {
            return  QueryFinder.GetClinics(_context, activeOnly);
        }
        public async Task<OperationResult<Clinic>> Add(string name, CancellationToken cancellationToken = default)
        {
            name = name.Trim();
            if (await _context.Clinics.AnyAsync(x => x.Name == name, cancellationToken))
            {
                return OperationResult<Clinic>.Failed("");
            }
            var clinic = new Clinic
            {
                Name = name,
                IsActive = true,
            };
            await _context.Clinics.AddAsync(clinic, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Clinic>.Succeeded(clinic);
        }
        public async Task<OperationResult<Clinic>> Edit(int id, string name, bool isActive, CancellationToken cancellationToken = default)
        {
            name = name.Trim();
            if (await _context.Clinics.AnyAsync(x => x.Name == name && x.Id != id, cancellationToken))
            {
                return OperationResult<Clinic>.Failed("");
            }
            var clinic = await _context.Clinics.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (clinic == null)
            {
                return OperationResult<Clinic>.Failed("");
            }
            clinic.Name = name;
            clinic.IsActive = isActive;

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Clinic>.Succeeded(clinic);
        }

        public async Task<OperationResult<Clinic>> Remove(int id, CancellationToken cancellationToken = default)
        {

            var clinic = await _context.Clinics.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (clinic == null)
            {
                return OperationResult<Clinic>.Failed("");
            }
            _context.Clinics.Remove(clinic);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Clinic>.Succeeded(clinic);
        }
    }
}
