using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IDepartmentsService
    {
        Task<UnderObservationDepartment> Get(int id);
        Task<List<UnderObservationDepartment>> GetAll();
    }

    public class DepartmentsService : IDepartmentsService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<UnderObservationDepartment>> GetAll()
        {
            var result = await _context.UnderObservationDepartments.AsNoTracking().ToListAsync();
            return result;
        }
        public async Task<UnderObservationDepartment> Get(int id)
        {
            var result = await _context.UnderObservationDepartments
                .Include(x => x.Beds)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
    }
}
