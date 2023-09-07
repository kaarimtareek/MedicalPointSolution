using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IDepartmentsService
    {
        Task<OperationResult<UnderObservationDepartment>> Create(string name, int userId, int bedsCount = 0);
        Task<OperationResult<UnderObservationDepartment>> Edit(int id, string name);
        Task<UnderObservationDepartment> Get(int id, bool withDetails = true);
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
                .Include(x=> x.Beds)
                .AsNoTracking().Where(x=> x.AvailableBedsCount > 0).ToListAsync();
            return result;
        }
        public async Task<UnderObservationDepartment> Get(int id, bool withDetails = true)
        {
            var query = _context.UnderObservationDepartments.AsNoTracking();
            if(withDetails)
            {
                query = query.Include(x => x.Beds.OrderBy(x => x.BedNumber))
                    .ThenInclude(x => x.Patient)
                .Include(x => x.Beds)
                .ThenInclude(x => x.Doctor);
            }
            var result = await query.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }

        public async Task<OperationResult<UnderObservationDepartment>> Create(string name, int userId, int bedsCount = 0)
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
                    DoctorId = userId,
                };
                await _context.UnderObservationBeds.AddAsync(bed);
                var history = new UnderObservationBedHistory
                {
                    ActionDate = DateTime.Now,
                    ActionType = ConstantObservationBedActionType.CREATE,
                    Bed = bed,
                    Notes = "",
                    DoctorId = userId,
                };
                await _context.UnderObservationBedHistories.AddAsync(history);
            }
            await _context.SaveChangesAsync();
            return OperationResult<UnderObservationDepartment>.Succeeded(department);
        }

        public async Task<OperationResult<UnderObservationDepartment>> Edit(int id, string name)
        {
            var department = await _context.UnderObservationDepartments.FirstOrDefaultAsync(x => x.Id == id);
            if(department == null)
            {
                return OperationResult<UnderObservationDepartment>.Failed(ConstantMessageCodes.DepartmentNotFound);
            }
            department.Name = name;
            await _context.SaveChangesAsync();
            return OperationResult<UnderObservationDepartment>.Succeeded(department);
        }
    }
}
