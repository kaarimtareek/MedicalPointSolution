using MedicalPoint.Data;
using MedicalPoint.ViewModels.Visits;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IReportsService
    {
        Task<PatientReport> GeneratePatientReport(int patientId);
        Task<VisitsReport> GenerateTodayStudentsVisitsReport(DateTime date);
        Task<VisitsReport> GenerateVisitsReport(DateTime startDate, DateTime endDate);
    }
    public static class StringExtensions
    {
        public const string Dashes = "----";
   
    }

    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext _context;

        public ReportsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<VisitsReport> GenerateVisitsReport(DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date;
            //largest time span is 1 month
            endDate = startDate.Subtract(endDate).TotalDays > 31 ? startDate.AddDays(-31) : endDate.Date;
            var visits = await _context.Visits.AsNoTracking().Where(x => x.VisitTime >= startDate && x.VisitTime <= endDate).ToListAsync();
            var patientsIds = visits.Select(x => x.PatientId).Distinct().ToList();
            var doctorsIds = visits.Select(x => x.DoctorId).Distinct().Where(x => x.HasValue).Select(x => x.Value).ToList();
            var patients = await _context.Patients.Include(x => x.Degree).AsNoTracking().Where(x => patientsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
            var doctors = await _context.Users.Include(x => x.Degree).AsNoTracking().Where(x => doctorsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
            var clinicsIds = visits.Select(x => x.ClinicId).Distinct().ToList();
            var clinics = await _context.Clinics.AsNoTracking().Where(x => clinicsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
            var visitsPatientsGroup = visits.GroupBy(x => x.PatientId);
            var visitsDoctorsGroup = visits.GroupBy(x => x.DoctorId);
            var visitsClinicsGroup = visits.GroupBy(x => x.ClinicId);
            var visitsTypesGroup = visits.GroupBy(x => x.Type);
            var patientsDegreesGroup = patients.GroupBy(x => x.Value.DegreeId);
            var visitsReport = new VisitsReport
            {
                ClinicsCount = visitsClinicsGroup.ToDictionary(x => x.Key, x => x.Count()),
                PatientsCount = patientsIds.Count,
                DoctorsCount = visitsDoctorsGroup.ToDictionary(x => x.Key, x => x.Count()),
                VisitsCount = visits.Count,
                VisitTypesCount = visitsTypesGroup.ToDictionary(x => x.Key, x => x.Count()),
                Vists = visits.ConvertAll(x => new VisitsViewModel
                {
                    ClinicId = x.ClinicId,
                    ClinicName = x.ClinicId.HasValue ? clinics[x.ClinicId.Value]?.Name ?? StringExtensions.Dashes : StringExtensions.Dashes,
                    Diagnosis = x.Diagnosis,
                    DoctorId = x.DoctorId,
                    ExitTime = x.ExitTime,
                    PatientId = x.PatientId,
                    Status = x.Status,
                    Type = x.Type,
                    VisitNumber = x.VisitNumber,
                    VisitTime = x.VisitTime,
                    Id = x.Id,
                    DoctorName = x.DoctorId.HasValue ? doctors[x.DoctorId.Value]?.FullName ?? StringExtensions.Dashes : StringExtensions.Dashes,
                    PatientName = patients[x.PatientId]?.Name ?? StringExtensions.Dashes,
                }),
                DegreesCount = patientsDegreesGroup.ToDictionary(x => x.Key, x => x.Count())

            };
            return visitsReport;
        }
        public async Task<VisitsReport> GenerateTodayStudentsVisitsReport(DateTime date)
        {

            var visits = await _context.Visits.AsNoTracking().Where(x => x.VisitTime == date && x.Patient.Degree.Id == 1).ToListAsync();
            var patientsIds = visits.Select(x => x.PatientId).Distinct().ToList();
            var doctorsIds = visits.Select(x => x.DoctorId).Distinct().Where(x => x.HasValue).Select(x => x.Value).ToList();
            var patients = await _context.Patients.Include(x => x.Degree).AsNoTracking().Where(x => patientsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
            var doctors = await _context.Users.Include(x => x.Degree).AsNoTracking().Where(x => doctorsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
            var clinicsIds = visits.Select(x => x.ClinicId).Distinct().ToList();
            var clinics = await _context.Clinics.AsNoTracking().Where(x => clinicsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
            var visitsPatientsGroup = visits.GroupBy(x => x.PatientId);
            var visitsDoctorsGroup = visits.GroupBy(x => x.DoctorId);
            var visitsClinicsGroup = visits.GroupBy(x => x.ClinicId);
            var visitsTypesGroup = visits.GroupBy(x => x.Type);
            var patientsDegreesGroup = patients.GroupBy(x => x.Value.DegreeId);
            var visitsReport = new VisitsReport
            {
                DegreesCount = patientsDegreesGroup.ToDictionary(x => x.Key, x => x.Count()),
                ClinicsCount = visitsClinicsGroup.ToDictionary(x => x.Key, x => x.Count()),
                PatientsCount = patientsIds.Count,
                DoctorsCount = visitsDoctorsGroup.ToDictionary(x => x.Key, x => x.Count()),
                VisitsCount = visits.Count,
                VisitTypesCount = visitsTypesGroup.ToDictionary(x => x.Key, x => x.Count()),
                Vists = visits.ConvertAll(x => new VisitsViewModel
                {
                    ClinicId = x.ClinicId,
                    ClinicName = x.ClinicId.HasValue ? clinics[x.ClinicId.Value]?.Name ?? StringExtensions.Dashes : StringExtensions.Dashes,
                    Diagnosis = x.Diagnosis,
                    DoctorId = x.DoctorId,
                    ExitTime = x.ExitTime,
                    PatientId = x.PatientId,
                    Status = x.Status,
                    Type = x.Type,
                    VisitNumber = x.VisitNumber,
                    VisitTime = x.VisitTime,
                    Id = x.Id,
                    DoctorName = x.DoctorId.HasValue ? doctors[x.DoctorId.Value]?.FullName ?? StringExtensions.Dashes : StringExtensions.Dashes,
                    PatientName = patients[x.PatientId]?.Name ?? StringExtensions.Dashes,
                }),

            };
            return visitsReport;
        }
        public async Task<PatientReport> GeneratePatientReport (int patientId)
        {
              var patient = await _context.Patients.AsNoTracking()
                .Include(x=> x.RegisteredUser)
                .Include(x=> x.Degree)
                .FirstOrDefaultAsync (x=> x.Id == patientId);
            var visits = await _context.Visits.AsNoTracking()
                .Where(x => x.PatientId == patientId).ToListAsync();
            var clinicsIds = visits.Select(x=> x.ClinicId).ToList();
            var doctorsIds = visits.Select(x=> x.DoctorId).ToList();
            var doctors = await _context.Users.AsNoTracking()
                .Include(x=> x.Degree)
                .Where(x=> doctorsIds.Contains(x.Id)).ToDictionaryAsync(x=> x.Id);
            var clinics = await _context.Clinics.AsNoTracking()
                .Where(x => clinicsIds.Contains(x.Id)).ToDictionaryAsync(x=> x.Id);

            var bedHistorys = (await _context.UnderObservationBedHistories.AsNoTracking()
                .Where(x => x.PatientId == patientId).ToListAsync()).GroupBy(x=> x.BedId);
            var bedIds = bedHistorys.Select(x=> x.Key).ToList();
            var beds = await _context.UnderObservationBeds.AsNoTracking()
                .Include(x=> x.Department)
                .Where(x=> bedIds.Contains(x.Id))
                .OrderByDescending(x=> x.DepartmentId)
                .ThenBy(x=> x.Id)
                .ToListAsync();
            var report = new PatientReport
            {
                Id = patientId,
                CreatedAt = DateTime.Now,
                Degree = patient?.Degree?.Name?? StringExtensions.Dashes,
                GeneralNumber = patient.GeneralNumber??StringExtensions.Dashes,
                SarayNumber = patient.SaryaNumber??StringExtensions.Dashes,
                MilitaryNumber = patient.MilitaryNumber??StringExtensions.Dashes,
                VisitsCount = visits.Count,
                VisitHistory = visits.OrderByDescending(x=> x.VisitTime).Select(x=> new VisitForPatientReport
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Clinic = x.ClinicId.HasValue? clinics[x.ClinicId.Value]?.Name??StringExtensions.Dashes: StringExtensions.Dashes,
                    Diagnosis = x.Diagnosis?? StringExtensions.Dashes,
                    DoctorId = x.Id,
                    DoctorName = x.DoctorId.HasValue? doctors[x.DoctorId.Value]?.FullName??StringExtensions.Dashes: StringExtensions.Dashes,
                    ExitTime = x.ExitTime,
                    PatientId =x.PatientId,
                    Status = x.Status,
                    Type = x.Type,
                    VisitNumber = x.VisitNumber,
                    VisitTime = x.VisitTime
                }).ToList(),
                UnderObservarionsHistory = beds.ConvertAll(x=> new UnderObservarionForPatientReport
                {
                    DepartmentId = x.DepartmentId,
                    BedNumber = x.BedNumber,
                    DepratmentName = x.Department?.Name?? StringExtensions.Dashes,
                    Id = x.Id,
                    BedHistory = bedHistorys.Where(xx=>xx.Key == x.Id).SelectMany(historyGroup=> historyGroup.Select(history=> new UnderObservarionHistoryForPatientReport
                    {
                        Id = history.Id,
                        ActionType = history.ActionType,
                        BedId = history.BedId,
                        BedNumber = x.BedNumber,
                        CreatedAt = history.ActionDate,
                        DoctorId = history.DoctorId,
                        DoctorName = doctors[history.DoctorId]?.FullName?? StringExtensions.Dashes,
                        Notes = history.Notes?? StringExtensions.Dashes,
                        PatientId = history.PatientId??0,
                    })).OrderByDescending(x=> x.CreatedAt).ToList(),
                    
                })

                
            };
            return report;
        }
    }
    public class VisitsReport
    {
        public int VisitsCount { get; set; }
        public int PatientsCount { get; set; }
        public Dictionary<int?, int> ClinicsCount { get; set; }
        public Dictionary<int?, int> DoctorsCount { get; set; }
        public Dictionary<string, int> VisitTypesCount { get; set; }
        public Dictionary<int, int> DegreesCount { get; set; }
        public List<VisitsViewModel> Vists { get; set; }

    }
    public class PatientReport
    {
        public int Id { get; set; }
        public string Degree { get; set; }
        public string GeneralNumber { get; set; }
        public string SarayNumber { get; set; }
        public string MilitaryNumber { get; set; }
        public int VisitsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<int, int> ClinicsCount { get; set; }
        public int UnderObservationsCount { get; set; }
        public List<UnderObservarionForPatientReport> UnderObservarionsHistory { get; set; }
        public List<VisitForPatientReport> VisitHistory { get; set; }
    }
    public class VisitForPatientReport
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public string VisitNumber { get; set; }
        public string Diagnosis { get; set; }
        public string Clinic { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
    public class UnderObservarionForPatientReport
    {
        public int DepartmentId { get; set; }
        public string DepratmentName { get; set; }
        public int BedNumber { get; set; }
        public int Id { get; set; }
        public List<UnderObservarionHistoryForPatientReport> BedHistory { get; set; }
    }
    public class UnderObservarionHistoryForPatientReport
    {
        public int BedId { get; set; }
        public int BedNumber { get; set; }
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ActionType { get; set; }
        public string Notes { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
    }
    public class PatientsReport
    {
        public int PatientsCount { get; set; }
        public Dictionary<int, int> DegreesCount { get; set; }
        public List<Patient> Patients { get; set; }

    }
}
