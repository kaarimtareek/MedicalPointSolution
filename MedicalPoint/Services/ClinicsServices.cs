using MedicalPoint.Common;
using MedicalPoint.Data;

namespace MedicalPoint.Services
{
    public interface IClinicsServices
    {
        Task<List<Clinic>> GetAll(bool activeOnly = true);
    }

    public class ClinicsServices : IClinicsServices
    {
        private readonly ApplicationDbContext _context;

        public ClinicsServices(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Clinic>> GetAll(bool activeOnly = true)
        {
            return await QueryFinder.GetClinics(_context, activeOnly);
        }
    }
}
