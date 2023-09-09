using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IPatientsService
    {
        Task<OperationResult<Patient>> Add(string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", int? userId = null, CancellationToken cancellationToken = default);
        Task<OperationResult<Patient>> Edit(int id, string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", int? userId = null, CancellationToken cancellationToken = default);
        Task<PaginatedList<Patient>> GetPatients(int pageNumber = 1, int pageSize = 20, string searchValue = "", int? degree = null, bool? hasCheckVisit = null, CancellationToken cancellationToken = default);
        Task<Patient> GetById( int id , CancellationToken cancellationToken = default);
        bool IsUnderObservation(int id, CancellationToken cancellationToken = default);
        Task<List<Patient>> GetAvailableToAddToBed(CancellationToken cancellationToken = default);
    }

    public class PatientsService : IPatientsService
    {
        private readonly ApplicationDbContext _context;

        public PatientsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Patient>> GetPatients(int pageNumber = 1, int pageSize = 20, string searchValue = "", int? degree = null, bool? hasCheckVisit = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Patients.AsNoTracking()
                .Include(x => x.Degree)
                .Include(x => x.RegisteredUser)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchValue))
            {
                query = query.Where(x => x.GeneralNumber.Contains(searchValue) || x.MilitaryNumber.Contains(searchValue) || x.NationalNumber.Contains(searchValue) || x.Name.Contains(searchValue));
            }
            if (degree.HasValue && degree !=0)
            {
                query = query.Where(x => x.DegreeId == degree.Value);
            }
            query = query.OrderByDescending(x => x.CreatedAt);
            var patients = await PaginatedList<Patient>.CreateAsync(query, pageNumber, pageSize);

            return patients;
        }
        public async Task<List<Patient>> GetAvailableToAddToBed(CancellationToken cancellationToken = default)
        {
            var patientIds = await _context.UnderObservationBeds.AsNoTracking().Where(x=> x.PatientId !=null).Select(x=> x.PatientId.Value).ToListAsync(cancellationToken);

            var query = _context.Patients.AsNoTracking()
                .Include(x=> x.Degree)
                .Where(x=>  !patientIds.Contains(x.Id))
                .AsQueryable();
            
            var patients = await query.ToListAsync(cancellationToken);

            return patients;
        }
        public async Task<OperationResult<Patient>> Add(string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", int? userId = null, CancellationToken cancellationToken = default)
        {
            if(userId ==null)
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.NameAlreadyExist);

            }
            //if (QueryValidator.IsPatientNameExist(_context, name))
            //{
            //    return OperationResult<Patient>.Failed(ConstantMessageCodes.NameAlreadyExist);
            //}
            if (!string.IsNullOrEmpty(militaryNumber) && QueryValidator.IsPatientMilitaryNumberExist(_context, militaryNumber))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.MilitaryNumberAlreadyExist);
            }

            if (!string.IsNullOrEmpty(generalNumber) && QueryValidator.IsPatientGeneralNumberExist(_context, generalNumber))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.GeneralNumberAlreadyExist);
            }

            if (!string.IsNullOrEmpty(nationalNumber) && QueryValidator.IsPatientNationalNumberExist(_context, nationalNumber))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.NationalNumberAlreadyExist);
            }
            if(!await _context.Degrees.AnyAsync(x => x.Id == degreeId,  cancellationToken))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.DegreeNotFound);

            }
            var patient = new Patient
            {
                MilitaryNumber = militaryNumber ?? "",
                CreatedAt = DateTime.Now,
                DegreeId = degreeId,
                GeneralNumber = generalNumber??"",
                LastUpdatedAt = DateTime.Now,
                Major = major ?? "",
                SaryaNumber = sarayNumber??"",
                Name = name,
                NationalNumber = nationalNumber ?? "",
                RegisteredUserId = userId.Value
            };

            await _context.Patients.AddAsync(patient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<Patient>.Succeeded(patient);
        }
        public async Task<OperationResult<Patient>> Edit(int id, string name, int degreeId, string militaryNumber = "", string nationalNumber = "", string generalNumber = "", string sarayNumber = "", string major = "", int? userId = null, CancellationToken cancellationToken = default)
        {
            var patient = QueryFinder.GetPatientById(_context, id);
            if (patient == null)
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.PatientNotFound);
            }
            if (QueryValidator.IsPatientNameExist(_context, name, id))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.NameAlreadyExist);
            }
            if (!string.IsNullOrEmpty(militaryNumber) && QueryValidator.IsPatientMilitaryNumberExist(_context, militaryNumber, id))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.MilitaryNumberAlreadyExist);
            }

            if (!string.IsNullOrEmpty(generalNumber) && QueryValidator.IsPatientGeneralNumberExist(_context, generalNumber, id))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.GeneralNumberAlreadyExist);
            }

            if (!string.IsNullOrEmpty(nationalNumber) && QueryValidator.IsPatientNationalNumberExist(_context, nationalNumber, id))
            {
                return OperationResult<Patient>.Failed(ConstantMessageCodes.NationalNumberAlreadyExist);
            }
            patient.Major = major??"";
            patient.SaryaNumber = sarayNumber??"";
            patient.NationalNumber = nationalNumber??"";
            patient.MilitaryNumber = militaryNumber??"";
            patient.GeneralNumber = generalNumber??"";
            patient.Name = name;
            patient.LastUpdatedAt = DateTime.Now;
            patient.DegreeId = degreeId;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync(cancellationToken);

            return OperationResult<Patient>.Succeeded(patient);
        }

        public async Task<Patient> GetById(int id, CancellationToken cancellationToken = default)
        {
            var patient = await _context.Patients.AsNoTracking()
                .Include(x => x.Degree)
                .Include(x => x.RegisteredUser)
                .Include(x => x.Visits)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);


            return patient;
        }
          public  bool IsUnderObservation(int id, CancellationToken cancellationToken = default)
        {
            return QueryValidator.IsPatientInBed(_context, id);
        }

    }
}
