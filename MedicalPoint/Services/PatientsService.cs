using MedicalPoint.Common;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IPatientsService
    {
        Task<OperationResult<Patient>> Add(string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", CancellationToken cancellationToken = default);
        Task<OperationResult<Patient>> Edit(int id, string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", CancellationToken cancellationToken = default);
        Task<List<Patient>> GetPatients(string searchValue = "", int? degree = null, CancellationToken cancellationToken = default);
    }

    public class PatientsService : IPatientsService
    {
        private readonly ApplicationDbContext _context;

        public PatientsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Patient>> GetPatients(string searchValue = "", int? degree = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Patients.AsNoTracking().Include(x=> x.Degree).AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(x => x.GeneralNumber.Contains(searchValue));
                query = query.Where(x => x.MilitaryNumber.Contains(searchValue));
                query = query.Where(x => x.NationalNumber.Contains(searchValue));
                query = query.Where(x => x.Name.Contains(searchValue));
            }
            if (degree.HasValue)
            {
                query = query.Where(x => x.DegreeId == degree.Value);
            }
            var patients = await query.ToListAsync(cancellationToken);

            return patients;
        }
        public async Task<OperationResult<Patient>> Add(string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", CancellationToken cancellationToken = default)
        {
            if (QueryValidator.IsPatientNameExist(_context, name))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }
            if (!string.IsNullOrEmpty(militaryNumber) && QueryValidator.IsPatientMilitaryNumberExist(_context, militaryNumber))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }

            if (!string.IsNullOrEmpty(generalNumber) && QueryValidator.IsPatientGeneralNumberExist(_context, generalNumber))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }

            if (!string.IsNullOrEmpty(nationalNumber) && QueryValidator.IsPatientNationalNumberExist(_context, nationalNumber))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }
            var patient = new Patient
            {
                MilitaryNumber = militaryNumber,
                CreatedAt = DateTime.Now,
                DegreeId = degreeId,
                GeneralNumber = generalNumber,
                LastUpdatedAt = DateTime.Now,
                Major = major,
                SaryaNumber = sarayNumber,
                Name = name,
                NationalNumber = nationalNumber,
            };

            await _context.Patients.AddAsync(patient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<Patient>.Succeeded(patient, "");
        }
        public async Task<OperationResult<Patient>> Edit(int id, string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", CancellationToken cancellationToken = default)
        {
            var patient = QueryFinder.GetPatientById(_context, id);
            if (patient == null)
            {
                return OperationResult<Patient>.Failed("patient not found");
            }
            if (QueryValidator.IsPatientNameExist(_context, name, id))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }
            if (!string.IsNullOrEmpty(militaryNumber) && QueryValidator.IsPatientMilitaryNumberExist(_context, militaryNumber, id))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }

            if (!string.IsNullOrEmpty(generalNumber) && QueryValidator.IsPatientGeneralNumberExist(_context, generalNumber, id))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }

            if (!string.IsNullOrEmpty(nationalNumber) && QueryValidator.IsPatientNationalNumberExist(_context, nationalNumber, id))
            {
                return OperationResult<Patient>.Failed("Name Already exist");
            }
            patient.Major = major;
            patient.SaryaNumber = sarayNumber;
            patient.NationalNumber = nationalNumber;
            patient.MilitaryNumber = militaryNumber;
            patient.GeneralNumber = generalNumber;
            patient.Name = name;
            patient.LastUpdatedAt = DateTime.Now;
            patient.DegreeId = degreeId;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<Patient>.Succeeded(patient, "");
        }

    }
}
