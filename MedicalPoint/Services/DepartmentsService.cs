using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IDepartmentsService
    {
        Task<OperationResult<UnderObservationDepartment>> Create(string name, int bedsCount = 0);
        Task<UnderObservationDepartment> Get(int id);
        Task<List<UnderObservationDepartment>> GetAll();
        Task<List<UnderObservationDepartment>> GetAllAvailable();
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
        public async Task<List<UnderObservationDepartment>> GetAllAvailable()
        {
            var result = await _context.UnderObservationDepartments
                .AsNoTracking().Where(x=> x.AvailableBedsCount > 0).ToListAsync();
            return result;
        }
        public async Task<UnderObservationDepartment> Get(int id)
        {
            var result = await _context.UnderObservationDepartments
                .Include(x => x.Beds)
                    .ThenInclude(x=> x.Patient)
                .Include(x=> x.Beds)
                .ThenInclude(x=> x.Doctor)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<OperationResult<UnderObservationDepartment>> Create(string name, int bedsCount = 0)
        {
            var department = new UnderObservationDepartment
            {
                AvailableBedsCount = bedsCount,
                BedsCount = bedsCount,
                Name = name,
            };
            await _context.UnderObservationDepartments.AddAsync(department);
            for(int i = 0;i< bedsCount;i++)
            {
                var bed = new UnderObservationBed
                {
                    BedNumber = i + 1,
                    Department = department,
                    IsActive = true,
                    Notes= "",
                };
                await _context.UnderObservationBeds.AddAsync(bed);
                var history = new UnderObservationBedHistory
                {
                    ActionDate = DateTime.Now,
                    ActionType = ConstantObservationBedActionType.CREATE,
                    Bed = bed,
                    Notes = "",
                };
                await _context.UnderObservationBedHistories.AddAsync(history);
            }
            await _context.SaveChangesAsync();
            return OperationResult<UnderObservationDepartment>.Succeeded(department);
        }
    }
}
