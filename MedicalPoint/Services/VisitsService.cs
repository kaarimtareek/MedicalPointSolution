using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

namespace MedicalPoint.Services
{
    //TODO: add delete function
    public interface IVisitsService
    {
        Task<OperationResult<Visit>> ChangeStatus(int visitId, string status, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Create(int userId, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, CancellationToken cancellationToken = default);
        Task<OperationResult<Visit>> Edit(int visitId, string diagnosis, string notes, DateTime? exitTime = null, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, DateTime? followDate = null, bool hasFollowingVisit = false, CancellationToken cancellationToken = default);
    }

    public class VisitsService : IVisitsService
    {
        private readonly ApplicationDbContext _context;

        public VisitsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult<Visit>> Create(int userId, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            if (QueryValidator.PatientHasAlreadyActiveVisit(_context, userId))
            {
                return OperationResult<Visit>.Failed("");
            }
            var numberOfVisitsToday = QueryFinder.GetVisitsCountForDay(_context);
            var currentVisitNumber = ++numberOfVisitsToday;
            string visitNumber = $"{DateTime.Today.ToString("yyyyMMdd")}{currentVisitNumber:D4}";
            //we can add checks for the existence of clinic & doctor but will skip it for simplicity 
            var visit = new Visit
            {
                ClinicId = clinicId,
                DoctorId = doctorId,
                CreatedAt = DateTime.Now,
                PatientId = userId,
                PreviousVisitId = previousVisitId,
                Status = ConstantVisitStatus.IN_RECIEPTION,
                Type = type ?? string.Empty,
                VisitTime = visitTime ?? DateTime.Now,
                VisitNumber = visitNumber
            };
            await _context.Visits.AddAsync(visit, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }

        public async Task<OperationResult<Visit>> Edit(int visitId, string diagnosis, string notes, DateTime? exitTime = null, DateTime? visitTime = null, int? clinicId = null, int? doctorId = null, string? type = null, int? previousVisitId = null, DateTime? followDate = null, bool hasFollowingVisit = false, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            visit.Diagnosis = diagnosis;
            visit.Notes = notes;
            visit.ExitTime = exitTime;
            visit.VisitTime = visitTime ?? visit.VisitTime;
            visit.ClinicId = clinicId;
            visit.DoctorId = doctorId;
            visit.FollowingVisitDate = followDate;
            visit.PreviousVisitId = previousVisitId;
            visit.Type = type ?? string.Empty;
            visit.HasFollowingVisit = hasFollowingVisit;
            visit.DoctorId = doctorId;

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }

        public async Task<OperationResult<Visit>> ChangeStatus(int visitId, string status, CancellationToken cancellationToken = default)
        {
            //check if user has already active visit
            var visit = await _context.Visits.FindAsync(new object?[] { visitId }, cancellationToken: cancellationToken);
            if (visit == null)
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }
            if (!ConstantVisitStatus.CanChangeStatus(visit.Status, status))
            {
                return OperationResult<Visit>.Failed(nameof(Visit));
            }

            visit.Status = status;

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<Visit>.Succeeded(visit);
        }

    }
}
