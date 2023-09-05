using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IVisitRestsService
    {
        Task<OperationResult<VisitRest>> Add(int visitId, int doctorId, int restType, string notes, DateTime startTime, int numberOfRestDays, CancellationToken cancellationToken = default);
        Task<OperationResult<VisitRest>> Edit(int visitRestId, int doctorId, int restType, string notes, DateTime startTime, int numberOfRestDays, CancellationToken cancellationToken = default);
        Task<VisitRest> Get(int visitRestId, CancellationToken cancellationToken = default);
        Task<List<VisitRest>> GetAll(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
        Task<VisitRest> GetByVisitId(int visitId, CancellationToken cancellationToken = default);
        Task<int> GetCountForPatient(int patientId, CancellationToken cancellationToken = default);
    }

    public class VisitRestsService : IVisitRestsService
    {
        private readonly ApplicationDbContext _context;

        public VisitRestsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<VisitRest>> GetAll(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
        {
            var query = _context.VisitRests.AsNoTracking().AsQueryable();
            if (from.HasValue)
            {
                from = from.Value.Date;
                query = query.Where(x => x.CreatedAt >= from.Value);
            }
            if (to.HasValue)
            {
                to = to.Value.Date;
                query = query.Where(x => x.CreatedAt <= to.Value);
            }
            var result = await query.ToListAsync(cancellationToken);
            return result;
        }
        public async Task<VisitRest> Get(int visitRestId, CancellationToken cancellationToken = default)
        {
            var result = await _context.VisitRests.AsNoTracking().FirstOrDefaultAsync(x => x.Id == visitRestId, cancellationToken);
            return result;
        }
        public async Task<VisitRest> GetByVisitId(int visitId, CancellationToken cancellationToken = default)
        {
            var result = await _context.VisitRests.AsNoTracking().FirstOrDefaultAsync(x => x.VisitId == visitId, cancellationToken);
            return result;
        }

        public async Task<int> GetCountForPatient(int patientId, CancellationToken cancellationToken = default)
        {
            var result = await _context.VisitRests.AsNoTracking().CountAsync(x => x.PatientId == patientId, cancellationToken);
            return result;
        }
        public async Task<OperationResult<VisitRest>> Add(int visitId, int doctorId, int restType, string notes, DateTime startTime, int numberOfRestDays, CancellationToken cancellationToken = default)
        {
            if (numberOfRestDays < 1)
            {
                return OperationResult<VisitRest>.Failed(ConstantMessageCodes.InvalidRestDaysNumber);
            }
            var visit = QueryFinder.GetVisitById(_context, visitId);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitRest>.Failed(ConstantMessageCodes.VisitNotFound);
            }
            //return if there's already visit rest for this visit
            if (await _context.VisitRests.AnyAsync(x => x.VisitId == visitId, cancellationToken))
            {
                return OperationResult<VisitRest>.Failed(ConstantMessageCodes.VisitRestAlreadyExist);
            }
            var visitRest = new VisitRest
            {
                PatientId = visit.PatientId,
                CreatedAt = DateTime.Now,
                DoctorId = doctorId,
                Notes = notes??"",
                RestDaysNumber = numberOfRestDays,
                StartDate = startTime,
                EndDate = startTime.AddDays(numberOfRestDays),
                RestTypeId = restType,
                VisitId = visitId,
            };
            await _context.VisitRests.AddAsync(visitRest, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitRest>.Succeeded(visitRest);
        }
        public async Task<OperationResult<VisitRest>> Edit(int visitRestId, int doctorId, int restType, string notes, DateTime startTime, int numberOfRestDays, CancellationToken cancellationToken = default)
        {
            if (numberOfRestDays < 1)
            {
                return OperationResult<VisitRest>.Failed(ConstantMessageCodes.InvalidRestDaysNumber);
            }
            var visitRest = await _context.VisitRests.FirstOrDefaultAsync(x => x.Id == visitRestId, cancellationToken);
            if (visitRest == null)
            {
                return OperationResult<VisitRest>.Failed(ConstantMessageCodes.VisitRestNotFound);
            }
            var visit = QueryFinder.GetVisitById(_context, visitRest.VisitId);
            if (visit == null || visit.IsDeleted)
            {
                return OperationResult<VisitRest>.Failed(ConstantMessageCodes.VisitNotFound);
            }
            if (!visit.CanEditVisit())
            {
                return OperationResult<VisitRest>.Failed(ConstantMessageCodes.CannotEditVisit);
            }
            visitRest.Notes = notes??"";
            visitRest.StartDate = startTime;
            visitRest.RestTypeId = restType;
            visitRest.RestDaysNumber = numberOfRestDays;
            visitRest.EndDate = startTime.AddDays(numberOfRestDays);
            visitRest.DoctorId = doctorId;

            await _context.SaveChangesAsync(cancellationToken);
            return OperationResult<VisitRest>.Succeeded(visitRest);
        }


    }
}
