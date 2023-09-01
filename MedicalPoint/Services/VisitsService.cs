using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

using static Azure.Core.HttpHeader;

namespace MedicalPoint.Services
{
    public interface IVisitsService
    {
        Task<OperationResult<Visit>> ChangeStatus(int visitId, int userId, string status, bool forceChange = false, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Create(int userId, int patientId, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Delete(int visitId, int userId, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Edit(int visitId, int userId,  string notes, DateTime? exitTime = null, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, DateTime? followDate = null, bool hasFollowingVisit = false, CancellationToken cancellationToken = default);
        Task<Visit> Get(int visitId, CancellationToken cancellationToken = default);
        Task<List<Visit>> GetAll(int? doctorId = null, int? patientId = null, DateTime? from = null, DateTime? to = null, string? type = null, int? clinicId = null, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> WriteDiagnosis(int visitId, int userId, string diagnosis, bool forceChange = false, CancellationToken cancellationToken = default);
    }

    public class VisitsService : IVisitsService
    {
        private readonly ApplicationDbContext _context;

        public VisitsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<Visit>> Create(int userId, int patientId, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            if (QueryValidator.PatientHasAlreadyActiveVisit(_context, patientId))
            {
                return OperationResult<Visit>.Failed("");
            }
            var numberOfVisitsToday = QueryFinder.GetVisitsCountForDay(_context);
            var currentVisitNumber = ++numberOfVisitsToday;
            string visitNumber = $"{DateTime.Today:yyyyMMdd}{currentVisitNumber:D3}";
            //we can add checks for the existence of clinic & doctor but will skip it for simplicity 
            var visit = new Visit
            {
                ClinicId = clinicId,
                DoctorId = doctorId,
                CreatedAt = DateTime.Now,
                PatientId = patientId,
                PreviousVisitId = previousVisitId,
                Status = ConstantVisitStatus.IN_RECIEPTION,
                Type = type ?? string.Empty,
                VisitTime = visitTime ?? DateTime.Now,
                VisitNumber = visitNumber,
                Diagnosis = string.Empty,
                Notes = string.Empty,
                RegisteredUserId = userId,
                
            };
            await _context.Visits.AddAsync(visit, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var visitHistory = new VisitHistory
            {
                VisitId = visit.Id,
                CreatedAt = DateTime.Now,
                UserId = userId,
                ClinicId = clinicId,
                Type = type ?? string.Empty,
                VisitTime = visitTime ?? DateTime.Now,
                DoctorId = doctorId,
                PreviousVisitId = previousVisitId,
                PatientId = patientId,
                Status = ConstantVisitStatus.IN_RECIEPTION,
                VisitNumber = visitNumber,
                Notes= string.Empty,
                Diagnosis= string.Empty,

            };
            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }

        public async Task<OperationResult<Visit>> Edit(int visitId, int userId,  string notes, DateTime? exitTime = null, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, DateTime? followDate = null, bool hasFollowingVisit = false, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            visit.Diagnosis = "";
            visit.Notes = notes??"";
            visit.ExitTime = exitTime;
            visit.VisitTime = visitTime ?? visit.VisitTime;
            visit.ClinicId = clinicId;
            visit.DoctorId = doctorId;
            visit.FollowingVisitDate = followDate;
            visit.PreviousVisitId = previousVisitId;
            visit.Type = type ?? string.Empty;
            visit.HasFollowingVisit = hasFollowingVisit;
            visit.DoctorId = doctorId;

            var visitHistory = new VisitHistory
            {
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                UserId = userId,
                ClinicId = clinicId,
                Type = type?? string.Empty,
                VisitTime = visitTime,
                Diagnosis = "",
                Notes = notes??"",
                ExitTime = exitTime,
                DoctorId = doctorId,
                HasFollowingVisit = hasFollowingVisit,
                PreviousVisitId = previousVisitId,
                FollowingVisitDate = followDate,
                
            };

            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }

        public async Task<OperationResult<Visit>> ChangeStatus(int visitId, int userId, string status, bool forceChange = false, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            if (!forceChange && !ConstantVisitStatus.CanChangeStatus(visit.Status, status))
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            visit.Status = status;
            var visitHistory = new VisitHistory
            {
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                UserId = userId,
                 Status = status,
                Diagnosis =  "",
                Notes = "",
                Type = "",
                VisitNumber = "",
            };
            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }
        
        public async Task<OperationResult<Visit>> WriteDiagnosis(int visitId, int userId, string diagnosis, bool forceChange = false, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(diagnosis))
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            if (!forceChange && !visit.CanEditVisit(forceChange))
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            visit.Diagnosis = diagnosis;
            visit.DoctorId = userId;
            if(visit.Status == ConstantVisitStatus.IN_RECIEPTION)
            {
                visit.Status = ConstantVisitStatus.IN_CLINIC_DIAGNOSIS;
            }
            var visitHistory = new VisitHistory
            {
                VisitId = visitId,
                CreatedAt = DateTime.Now,
                UserId = userId,
                 Status = visit.Status,
                Diagnosis =  diagnosis,
                Notes = "",
                Type = "",
                VisitNumber = "",
            };
            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }
        
       
        public async Task<OperationResult<Visit>> Delete(int visitId, int userId, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }

            visit.IsDeleted = true;
            var visitHistory = new VisitHistory
            {
                 VisitId = visitId,
                 IsDeleted = true,
                 CreatedAt = DateTime.Now,
                 UserId = userId,
                Diagnosis = "",
                Notes = "",
                Type = "",
                VisitNumber = "",
            };

            await _context.VisitHistories.AddAsync(visitHistory, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }
        public async Task<List<Visit>> GetAll(int? doctorId = null, int? patientId = null, DateTime? from = null, DateTime? to = null, string? type = null, int? clinicId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Visits
                .Include(x=> x.Doctor)
                .Include(x=> x.Patient)
                    .ThenInclude(x=> x.Degree)
                 .Include(x=> x.Clinic)
                .AsNoTracking().AsQueryable();

            if (doctorId != null)
            {
                query = query.Where(x => x.DoctorId == doctorId);
            }
            if (patientId != null)
            {
                query = query.Where(x => x.PatientId == patientId);
            }
            if (from != null)
            {
                from = from.Value.Date;
                query = query.Where(x => x.VisitTime >= from);
            }
            if (to != null)
            {
                to = to.Value.Date;
                query = query.Where(x => x.VisitTime <= to);
            }
            if (type != null)
            {
                query.Where(x => x.Type == type);
            }
            if (clinicId != null)
            {
                query.Where(x => x.ClinicId == clinicId);
            }
            var result = await query.ToListAsync(cancellationToken);
            return result;
        }
        public async Task<Visit> Get(int visitId, CancellationToken cancellationToken = default)
        {
            var visit = await _context.Visits.AsNoTracking()
                .Include(x=> x.Clinic)
                .Include(x=> x.Doctor)
                .Include(x=> x.Patient)
                    .ThenInclude(x=> x.Degree)
                .Include(x=> x.Medicines)
                    .ThenInclude(x=> x.Medicine)
                .Include(x=> x.PreviousVisit)
                .Include(x=> x.Images)
                .Include(x=> x.History)
                .FirstOrDefaultAsync(x=> x.Id == visitId, cancellationToken);
            return visit;
        }
    }
}
