using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IUnderObservationBedsService
    {
        Task<OperationResult<UnderObservationBed>> AddBedToDepartment(int departmentId, CancellationToken cancellationToken = default);
        Task<OperationResult<UnderObservationBed>> AddPatientToBed(int bedId,  int patientId, int doctorId, string notes, DateTime? enterDate = null, int? visitId = null, CancellationToken cancellationToken = default);
        Task<OperationResult<UnderObservationBed>> Edit(int bedId, int doctorId, string notes,  CancellationToken cancellationToken = default);
        Task<UnderObservationBed> Get(int id, bool withHistory = false, CancellationToken cancellationToken = default);
        Task<List<UnderObservationBed>> GetAll(int departmentId, CancellationToken cancellationToken = default);
        Task<List<UnderObservationBed>> GetAllAvailable(int departmentId, CancellationToken cancellationToken = default);
        Task<List<UnderObservationBed>> GetAllAvailable(List<int> departmentsIds, CancellationToken cancellationToken = default);
        Task<int?> GetBedIdByPatientId(int patientId, CancellationToken cancellationToken = default);
        Task<OperationResult<UnderObservationBed>> RemovePatientFromBed(int bedId, int doctorId, CancellationToken cancellationToken = default);
    }

    public class UnderObservationBedsService : IUnderObservationBedsService
    {
        private readonly ApplicationDbContext _context;

        public UnderObservationBedsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<UnderObservationBed>> GetAll(int departmentId, CancellationToken cancellationToken = default)
        {
            var result = await _context.UnderObservationBeds.AsNoTracking().Where(x => x.DepartmentId == departmentId).ToListAsync(cancellationToken);
            return result;
        }
        
        public async Task<List<UnderObservationBed>> GetAllAvailable(int departmentId, CancellationToken cancellationToken = default)
        {
            var result = await _context.UnderObservationBeds.AsNoTracking().Where(x => x.DepartmentId == departmentId && x.IsActive && x.PatientId == null).ToListAsync(cancellationToken);
            return result;
        }
         public async Task<List<UnderObservationBed>> GetAllAvailable(List<int> departmentsIds, CancellationToken cancellationToken = default)
        {
            var result = await _context.UnderObservationBeds.AsNoTracking().Where(x => departmentsIds.Contains( x.DepartmentId ) && x.IsActive && x.PatientId == null).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<UnderObservationBed> Get(int id, bool withHistory = false, CancellationToken cancellationToken = default)
        {
            var query = _context.UnderObservationBeds
                .AsNoTracking()
                .Include(x => x.Patient)
                    .ThenInclude(x => x.Degree)
                .Include(x => x.Doctor)
                    .ThenInclude(x => x.Degree)
                    .Include(x => x.Department)
                    .AsQueryable();
            if (withHistory)
            {
                query = query
                    .Include(x => x.History.OrderByDescending(x=> x.ActionDate))
                        .ThenInclude(x => x.Patient)
                    .Include(x => x.History.OrderByDescending(x => x.ActionDate))
                        .ThenInclude(x => x.Doctor);
            }
            else
            {
                query = query
                    .Include(x => x.History.OrderByDescending(x => x.ActionDate).Take(5))
                        .ThenInclude(x => x.Patient)
                    .Include(x => x.History.OrderByDescending(x => x.ActionDate).Take(5))
                        .ThenInclude(x => x.Doctor);

            }
              var result = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return result;
        }
        public async Task<OperationResult<UnderObservationBed>> AddPatientToBed(int bedId,  int patientId, int doctorId, string notes, DateTime? enterDate = null, int? visitId = null, CancellationToken cancellationToken = default)
        {
            var bed = await _context.UnderObservationBeds.FirstOrDefaultAsync(x => x.Id == bedId, cancellationToken);
            if (bed == null)
            {
                return OperationResult<UnderObservationBed>.Failed(ConstantMessageCodes.BedNotFound);
            }
            if ( !bed.CanAddPatient)
            {
                return OperationResult<UnderObservationBed>.Failed(ConstantMessageCodes.CannotAddPatientToBed);
            }
            var patient = await _context.Patients.FirstOrDefaultAsync(x => x.Id == patientId, cancellationToken);
            if (patient == null)
            {
                return OperationResult<UnderObservationBed>.Failed(ConstantMessageCodes.PatientNotFound);
            }
            //TODO:add this validation in compiled queries
            if (await _context.UnderObservationBeds.AnyAsync(x => x.PatientId == patientId, cancellationToken))
            {
                return OperationResult<UnderObservationBed>.Failed(ConstantMessageCodes.PatientAlreadyUnderObservation);
            }
            bed.PatientId = patientId;
            bed.EnterDate = enterDate ?? DateTime.Now;
            bed.Notes = notes??"";
            bed.DoctorId = doctorId;
            bed.VisitId = visitId;

            var bedHistory = new UnderObservationBedHistory
            {
                PatientId = bed.PatientId.Value,
                Notes = bed.Notes,
                VisitId = bed.VisitId,
                DoctorId = doctorId,
                ActionDate = enterDate ?? DateTime.Now,
                BedId = bedId,
                ActionType = ConstantObservationBedActionType.ENTER,
            };
            await _context.UnderObservationBedHistories.AddAsync(bedHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await RefreshAavailableBedCount(bed.DepartmentId);
            return OperationResult<UnderObservationBed>.Succeeded(bed);
        }

        public async Task<OperationResult<UnderObservationBed>> RemovePatientFromBed(int bedId, int doctorId, CancellationToken cancellationToken = default)
        {
            var bed = await _context.UnderObservationBeds.FirstOrDefaultAsync(x => x.Id == bedId, cancellationToken);
            if (bed == null || !bed.CanRemovePatient)
            {
                return OperationResult<UnderObservationBed>.Failed(ConstantMessageCodes.BedNotFound);
            }
            if ( !bed.CanRemovePatient)
            {
                return OperationResult<UnderObservationBed>.Failed(ConstantMessageCodes.CannotRemovePatientFromBed);
            }
            var bedHistory = new UnderObservationBedHistory
            {
                PatientId = bed.PatientId.Value,
                Notes = bed.Notes??"",
                VisitId = bed.VisitId,
                DoctorId = doctorId,
                ActionDate = DateTime.Now,
                BedId = bedId,
                ActionType = ConstantObservationBedActionType.EXIT,
            };
            bed.PatientId = null;
            bed.EnterDate = null;
            bed.DoctorId = null;
            bed.VisitId = null;
            bed.Notes = "";

            await _context.UnderObservationBedHistories.AddAsync(bedHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await RefreshAavailableBedCount(bed.DepartmentId);

            return OperationResult<UnderObservationBed>.Succeeded(bed);
        }
        public async Task<OperationResult<UnderObservationBed>> AddBedToDepartment(int departmentId, CancellationToken cancellationToken = default)
        {
            var bedsCount = await _context.UnderObservationBeds.CountAsync(x=> x.DepartmentId == departmentId);
            var bed = new UnderObservationBed
            {
                IsActive = true,
                DepartmentId = departmentId,
                BedNumber = bedsCount +1,
                Notes =""
            };
            var bedHistory = new UnderObservationBedHistory
            {
                Notes = bed.Notes,
                ActionDate = DateTime.Now,
                ActionType = ConstantObservationBedActionType.CREATE,
                Bed = bed
            };
            
            await _context.UnderObservationBeds.AddAsync(bed, cancellationToken);
            await _context.UnderObservationBedHistories.AddAsync(bedHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await RefreshAavailableBedCount(bed.DepartmentId);

            return OperationResult<UnderObservationBed>.Succeeded(bed);
        }

        public async Task<OperationResult<UnderObservationBed>> Edit(int bedId, int doctorId, string notes,  CancellationToken cancellationToken = default)
        {
            var bed = await _context.UnderObservationBeds.FirstOrDefaultAsync(x => x.Id == bedId, cancellationToken);
            if (bed == null || bed.IsAvailable)
            {
                return OperationResult<UnderObservationBed>.Failed("");
            }


            bed.Notes = notes??"";

            var bedHistory = new UnderObservationBedHistory
            {
                PatientId = bed.PatientId.Value,
                Notes = bed.Notes,
                VisitId = bed.VisitId,
                DoctorId = doctorId,
                ActionDate = DateTime.Now,
                EnterDate = bed.EnterDate,
                BedId = bedId,
                ActionType = ConstantObservationBedActionType.EDIT,
            };

            await _context.UnderObservationBedHistories.AddAsync(bedHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<UnderObservationBed>.Succeeded(bed);
        }
        public async Task<int?> GetBedIdByPatientId(int patientId, CancellationToken cancellationToken = default)
        {
            var bedId = await _context.UnderObservationBeds.AsNoTracking().Where(x => x.PatientId == patientId).Select(x=> x.Id).FirstOrDefaultAsync(cancellationToken);
            if (bedId == 0)
                return null;
            return bedId;
        }

        private async Task RefreshAavailableBedCount(int departmentId)
        {
            var department = await _context.UnderObservationDepartments.FirstOrDefaultAsync(x => x.Id == departmentId);
            if (department == null)
            {
                return;
            }
            var availableBedsCount = await _context.UnderObservationBeds.CountAsync(x => x.DepartmentId == departmentId && !x.PatientId.HasValue);
            var bedsCount = await _context.UnderObservationBeds.CountAsync(x => x.DepartmentId == departmentId);
            department.BedsCount = bedsCount;
            department.AvailableBedsCount = availableBedsCount;
            await _context.SaveChangesAsync();
        }

    }
}
