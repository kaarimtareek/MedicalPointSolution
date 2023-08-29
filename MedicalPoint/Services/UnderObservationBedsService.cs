using MedicalPoint.Common;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public class UnderObservationBedsService
    {
        private readonly ApplicationDbContext _context;

        public UnderObservationBedsService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<UnderObservationBed>> GetAll(int departmentId, CancellationToken cancellationToken = default)
        {
            var result = await _context.UnderObservationBeds.AsNoTracking().Where(x=> x.DepartmentId == departmentId).ToListAsync(cancellationToken);
            return result;
        }

        public async Task<UnderObservationBed> Get(int id, CancellationToken cancellationToken = default)
        {
            var result = await _context.UnderObservationBeds
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return result;
        }

        public async Task<OperationResult<UnderObservationBed>> AddPatientToBed (int bedId, int visitId, int patientId, int doctorId, string notes, CancellationToken cancellationToken = default)
        {
            var bed = await _context.UnderObservationBeds.FirstOrDefaultAsync(x=> x.Id == bedId, cancellationToken);
            if(bed == null || !bed.IsAvailable)
            {
                return OperationResult<UnderObservationBed>.Failed("");
            }
            var patient = await _context.Patients.FirstOrDefaultAsync(x=> x.Id==patientId, cancellationToken);
            if(patient == null)
            {
                return OperationResult<UnderObservationBed>.Failed("");
            }
            //TODO:add this validation in compiled queries
            if(await _context.UnderObservationBeds.AnyAsync(x=> x.PatientId == patientId, cancellationToken))
            {
                return OperationResult<UnderObservationBed>.Failed("");
            }
            bed.PatientId = patientId;
            bed.EnterDate = DateTime.Now;
            bed.Notes = notes;
            bed.DoctorId = doctorId;
            bed.VisitId = visitId;
            _context.SaveChangesAsync(cancellationToken);
            return OperationResult<UnderObservationBed>.Succeeded(bed);
        }


    }
}
