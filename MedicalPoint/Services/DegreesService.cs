using MedicalPoint.Common;
using MedicalPoint.Data;

namespace MedicalPoint.Services
{
    public interface IDegreesService
    {
        Task<List<Degree>> GetAll(bool activeOnly = true);
    }

    public class DegreesService : IDegreesService
    {
        private readonly ApplicationDbContext _context;

        public DegreesService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Degree>> GetAll(bool activeOnly = true)
        {
            return await QueryFinder.GetDegrees(_context, activeOnly);
        }
    }
}
