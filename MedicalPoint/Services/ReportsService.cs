using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

using MedicalPoint.Constants;
using MedicalPoint.Data;
using MedicalPoint.ViewModels.Visits;

using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Services
{
    public interface IReportsService
    {
        Task<DailyMedicineReport> GenerateDailyMedicineReport(DateTime date);
        Task<PatientReport> GeneratePatientReport(int patientId);
        Task<VisitsReport> GenerateVisitsReport(DateTime startDate, DateTime endDate, bool includeAllPatients = false, CancellationToken cancellationToken = default);
        Task<VisitsReport> GenerateVisitsReport(DateTime startDate, DateTime endDate);
    }
    public static class StringExtensions
    {
        public const string Dashes = "----";
        public static bool IsNumber(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            return int.TryParse(s, out int value);
        }

   
    }

    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        //daily report for medicines
        public ReportsService(ApplicationDbContext context, ICacheService  cacheService)
        {
            _context = context;
            _cacheService = cacheService;
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
                PatientsCount = patientsIds.Count,
                VisitsCount = visits.Count,
                Visits = visits.ConvertAll(x => new VisitsViewModel
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
        public async Task<VisitsReport> GenerateVisitsReport(DateTime startDate, DateTime endDate, bool includeAllPatients = false, CancellationToken cancellationToken = default)
        {
            startDate = startDate.Date;
            //largest time span is 1 month
            startDate = endDate.Subtract(startDate).TotalDays > 31 ? endDate.AddDays(-31) : startDate.Date;
            endDate = endDate.Date.AddDays(1);
            //var startDate = date.Date;
            //var endDate = date.Date.AddDays(1);
            var query = _context.Visits.AsNoTracking().Where(x => x.VisitTime >=startDate && x.VisitTime<=endDate );
            if(!includeAllPatients)
            {
                query = query.Where(x => x.Patient.DegreeId == 1);    
            }
            var visits = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(cancellationToken);

            var emergencyVisitTypeCount = visits.Count(x => x.Type == ConstantVisitType.EMERGENCY);
            var patientsIds = visits.Select(x => x.PatientId).Distinct().ToList();
            var doctorsIds = visits.Select(x => x.DoctorId).Distinct().Where(x => x.HasValue).Select(x => x.Value).ToList();
            var patients = await _context.Patients.Include(x => x.Degree).AsNoTracking().Where(x => patientsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
            var doctors = await _context.Users.Include(x => x.Degree).AsNoTracking().Where(x => doctorsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
            var clinicsIds = visits.Select(x => x.ClinicId).Distinct().ToList();
            var clinics = await _context.Clinics.AsNoTracking().Where(x => clinicsIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
            var visitsPatientsGroup = visits.GroupBy(x => x.PatientId);
            var visitsDoctorsGroup = visits.GroupBy(x => x.DoctorId);
            var visitsClinicsGroup = visits.GroupBy(x => x.ClinicId);
            var visitsTypesGroup = visits.GroupBy(x => x.Type);
            var visitsReport = new VisitsReport
            {
                FromDate = startDate,
                //because we added 1 day for filteration, we revert it back here
                ToDate = endDate.AddDays(-1),
                ReportDate = DateTime.Now,
                PatientsCount = patientsIds.Count,
                VisitsCount = visits.Count,
                EmergencyVisitTypeCount = emergencyVisitTypeCount,
                Visits = visits.ConvertAll(x => new VisitsViewModel
                {
                    PatientDegree = patients.GetValueOrDefault(x.PatientId)?.Degree?.Name??StringExtensions.Dashes,
                    ClinicId = x.ClinicId,
                    ClinicName = x.ClinicId.HasValue ? clinics.GetValueOrDefault(x.ClinicId.Value)?.Name ?? StringExtensions.Dashes : StringExtensions.Dashes,
                    Diagnosis = x.Diagnosis,
                    DoctorId = x.DoctorId,
                    ExitTime = x.ExitTime,
                    PatientId = x.PatientId,
                    Status = x.Status,
                    Type = x.Type,
                    VisitNumber = x.VisitNumber,
                    VisitTime = x.VisitTime,
                    Id = x.Id,
                    DoctorName = x.DoctorId.HasValue ? doctors.GetValueOrDefault(x.DoctorId.Value)?.FullName ?? StringExtensions.Dashes : StringExtensions.Dashes,
                    PatientName = patients.GetValueOrDefault(x.PatientId)?.Name ?? StringExtensions.Dashes,
                    PatientSaryaNumber = patients.GetValueOrDefault(x.PatientId)?.SaryaNumber?? StringExtensions.Dashes,
                    PatientGeneralNumber = patients.GetValueOrDefault(x.PatientId)?.GeneralNumber ?? StringExtensions.Dashes,
                }),

            };
            return visitsReport;
        }
        public async Task<PatientReport> GeneratePatientReport (int patientId)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
              var patient = await _context.Patients.AsNoTracking()
                .Include(x=> x.RegisteredUser)
                .Include(x=> x.Degree)
                .FirstOrDefaultAsync (x=> x.Id == patientId);
            var visits = await _context.Visits.AsNoTracking()
                .Where(x => x.PatientId == patientId).ToListAsync();
            var doctorsIds = visits.Select(x=> x.DoctorId).ToList();
            var doctors = await _context.Users.AsNoTracking()
                .Include(x=> x.Degree)
                .ToDictionaryAsync(x=> x.Id);
            var clinics = _cacheService.GetClinics().ToDictionary(x=> x.Id);

            var bedHistorys = (await _context.UnderObservationBedHistories.AsNoTracking()
                .Where(x => x.PatientId == patientId).ToListAsync()).GroupBy(x=> x.BedId);
            var bedIds = bedHistorys.Select(x=> x.Key).ToList();
            var beds = await _context.UnderObservationBeds.AsNoTracking()
                .Include(x=> x.Department)
                .Where(x=> bedIds.Contains(x.Id))
                .OrderByDescending(x=> x.DepartmentId)
                .ThenBy(x=> x.Id)
                .ToListAsync();
            var visitRestTypes = _cacheService.GetVisitRestTypes().ToDictionary(x=> x.Id);
            
            var visitRests = await _context.VisitRests
                .AsNoTracking()
                .Where(x=> x.PatientId == patientId).ToListAsync();
            var visitsIds = visits.Select(x=> x.Id).ToList();
            var visitsDictionary = visits.ToDictionary(x => x.Id);
            var visitMedicines = await _context.VisitMedicines.AsNoTracking().Where(x => visitsIds.Contains(x.VisitId)).ToListAsync();
            var medicinesIds = visitMedicines.Select(x=> x.MedicineId).ToList();
            var medicines = await _context.Medicines.AsNoTracking().Where(x => medicinesIds.Contains(x.Id)).ToDictionaryAsync(x=> x.Id);


            var report = new PatientReport
            {
                Id = patientId,
                Name = patient.Name ?? "",
                CreatedAt = DateTime.Now,
                Degree = patient?.Degree?.Name?? StringExtensions.Dashes,
                GeneralNumber = patient.GeneralNumber??StringExtensions.Dashes,
                SarayNumber = patient.SaryaNumber??StringExtensions.Dashes,
                MilitaryNumber = patient.MilitaryNumber??StringExtensions.Dashes,
                VisitsCount = visits.Count,
                VisitRestCount = visitRests.Count,
                VisitRests = visitRests.OrderByDescending(x=> x.CreatedAt)
                .Select(x=> new VisitRestForPatientReport
                {
                    Id = x.Id,
                    CreatedAt= x.CreatedAt,
                    DoctorId = x.DoctorId,
                    DoctorName = doctors.GetValueOrDefault(x.DoctorId)?.FullName?? StringExtensions.Dashes,
                    EndDate = x.EndDate,
                    Notes = x.Notes ?? StringExtensions.Dashes,
                    RestType = visitRestTypes.GetValueOrDefault(x.RestTypeId)?.Name?? StringExtensions.Dashes,
                    StartDate = x.StartDate ,
                    VisitId = x.VisitId,
                }).ToList(),
                VisitHistory = visits.OrderByDescending(x=> x.VisitTime).Select(x=> new VisitForPatientReport
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Clinic = x.ClinicId.HasValue? clinics.GetValueOrDefault(x.ClinicId.Value)?.Name??StringExtensions.Dashes: StringExtensions.Dashes,
                    Diagnosis = x.Diagnosis?? StringExtensions.Dashes,
                    DoctorId = x.Id,
                    DoctorName = x.DoctorId.HasValue? doctors.GetValueOrDefault(x.DoctorId.Value)?.FullName??StringExtensions.Dashes: StringExtensions.Dashes,
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
                        DoctorName = doctors.GetValueOrDefault(history.DoctorId)?.FullName?? StringExtensions.Dashes,
                        Notes = history.Notes?? StringExtensions.Dashes,
                        PatientId = history.PatientId??0,
                    })).OrderByDescending(x=> x.CreatedAt).ToList(),
                    
                }),
               MedicinesTaken = visitMedicines.GroupBy(x=> x.VisitId).ToList().Select(x=> new MedicinesTakenForPatientReport
               {
                   VisitId = x.Key,
                   CreatedAt = visitsDictionary.GetValueOrDefault(x.Key)?.CreatedAt?? DateTime.Now,
                   VisitNumber = visitsDictionary.GetValueOrDefault(x.Key)?.VisitNumber??StringExtensions.Dashes,
                   VisitMedicines = x.Select(xx=> new VisitMedicineForPatientReport
                   {
                       MedicineId =  xx.MedicineId,
                       Quantity = xx.Quantity,
                       VisitMedicineId = xx.Id,
                       MedicineName = medicines.GetValueOrDefault(xx.MedicineId)?.Name??StringExtensions.Dashes,
                   } ).ToList()
               }).ToList()
            };
            stopwatch.Stop();
            var value = stopwatch.Elapsed.TotalMilliseconds;
            return report;
        }
        public async Task<DailyMedicineReport> GenerateDailyMedicineReport(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1);
            var medicines = await _context.Medicines.AsNoTracking().ToListAsync();
            
            var givenMedicinesHistory = await _context.MedicineHistories
                .Include(x=> x.User)
                .AsNoTracking().Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate ).ToListAsync();
            var visitsIds = givenMedicinesHistory.Where(x => x.VisitId.HasValue).Select(x => x.VisitId.Value).ToList();
            var visits = await _context.Visits
                .Include(x=> x.Patient)
                .AsNoTracking().Where(x=> visitsIds.Contains(x.Id)).ToDictionaryAsync(x=> x.Id);

            var givenMedicinesIds = givenMedicinesHistory.Select(x=> x.MedicineId).Distinct().ToList();
            var givenMedicines = medicines.Where(x => givenMedicinesIds.Contains(x.Id)).ToList();
            var report = new DailyMedicineReport
            {
                ReportDate = startDate,
                AllMedicines = medicines.OrderBy(x=> x.Name).Select(x=> new MedicineForDailyMedicineReport
                {
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    LastUpdatedAt = x.LastUpdatedAt,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Status = x.Status
                }).ToList(),
                AvailableMedicines = medicines.Where(x => x.Status == ConstantMedicineStatus.AVAILABLE).OrderBy(x=> x.Name).Select(x => new MedicineForDailyMedicineReport
                {
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    LastUpdatedAt = x.LastUpdatedAt,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Status = x.Status
                }).ToList(),
                NearFinishMedicines = medicines.Where(x => x.Status == ConstantMedicineStatus.NEAR_FINISH).OrderBy(x => x.Name).Select(x => new MedicineForDailyMedicineReport
                {
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    LastUpdatedAt = x.LastUpdatedAt,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Status = x.Status
                }).ToList(),
                NotAvailableMedicines = medicines.Where(x => x.Status == ConstantMedicineStatus.NOT_AVAILABLE).OrderBy(x => x.Name).Select(x => new MedicineForDailyMedicineReport
                {
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    LastUpdatedAt = x.LastUpdatedAt,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Status = x.Status
                }).ToList(),
                MedicinesGivenToday = givenMedicines.OrderBy(x => x.Name).Select(x => new MedicineForDailyMedicineReport
                {
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    LastUpdatedAt = x.LastUpdatedAt,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    Name = x.Name,
                    Quantity = x.Quantity,
                    Status = x.Status,
                    History  = givenMedicinesHistory.Where(xx=> xx.MedicineId == x.Id).Select(xx=> new MedicineHistoryForDailyMedicineReport
                    {
                        ActionType = xx.ActionType,
                        CreatedAt = xx.CreatedAt,
                        Id = xx.Id,
                        MedicineId = xx.MedicineId,
                        MedicineName =xx.MedicineName,
                        MedicineQuantity =xx.MedicineQuantity,
                        MinimumQuantityThreshold = xx.MinimumQuantityThreshold,
                        UserId = xx.UserId,
                        UserName = xx.User?.FullName?? StringExtensions.Dashes,
                        VisitId = xx.VisitId,
                        PatientName = xx.VisitId.HasValue?  visits.GetValueOrDefault(xx.VisitId.Value)?.Patient?.Name?? StringExtensions.Dashes : StringExtensions.Dashes,
                        PatientId = xx.VisitId.HasValue? visits.GetValueOrDefault(xx.VisitId.Value)?.PatientId : null,
                    }).OrderBy(x=> x.PatientName).ToList()
                }).ToList()

            };
            return report;
        }
    }
    public class DailyMedicineReport
    {
        public DateTime ReportDate { get; set; }
        public int MedicinesCount => AllMedicines?.Count ?? 0;
        public int AvailableMedicinesCount => AvailableMedicines?.Count ?? 0;
        public int NotAvailableMedicinesCount => NotAvailableMedicines?.Count ?? 0;
        public int NearFinishMedicineCount => NearFinishMedicines?.Count ?? 0;
        public int MedicinesGivenTodayCount => MedicinesGivenToday?.Count ?? 0;
        public List<MedicineForDailyMedicineReport> AllMedicines { get; set; }
        public List<MedicineForDailyMedicineReport> AvailableMedicines { get; set; }
        public List<MedicineForDailyMedicineReport> NotAvailableMedicines { get; set; }
        public List<MedicineForDailyMedicineReport> NearFinishMedicines { get; set; }
        public List<MedicineForDailyMedicineReport> MedicinesGivenToday { get; set; }

    }
    public class MedicineForDailyMedicineReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public int ChangeInQuantity
            => History == null || History.Count == 0 ? 0
            : TotalOfMedicineQuantityActionType();

        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        public List<MedicineHistoryForDailyMedicineReport> History { get; set; }
        private int TotalOfMedicineQuantityActionType()
        {
            int result = 0;
            var groups = History.GroupBy(x => x.ActionType );
            foreach (var group in groups)
            {
                var type = group.Key;
                var sum = group.Sum(x => x.MedicineQuantity);
                var sumResult = sum ?? 0;
                if (type == ConstantMedicineActionType.EXPORT)
                    result -= sumResult;
                else if (type == ConstantMedicineActionType.EDIT)
                    continue;
                else
                    result += sumResult;

            }
            return result;
            
        }
    }
    public class MedicineHistoryForDailyMedicineReport
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public int UserId { get; set; }
        public string? MedicineName { get; set; }
        public int? MedicineQuantity { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        [MaxLength(100)]
        public string ActionType { get; set; }
        public string UserName { get; set; }
        public int? VisitId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PatientName { get; set; }
        public int? PatientId { get; set; }
    }
    public class VisitsReport
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public Dictionary<int, int> SaryasCount =>
            Visits?.Where(x=> x.PatientSaryaNumber.IsNumber()).GroupBy(x => x.PatientSaryaNumber).Where(x => x.Key != StringExtensions.Dashes  ).ToDictionary(x => int.Parse(x.Key), x => x.Count()).OrderBy(x => x.Key).ToDictionary(x=> x.Key, x=> x.Value);
        public DateTime ReportDate { get; set; }
        public int VisitsCount { get; set; }
        public int PatientsCount { get; set; }
        public int EmergencyVisitTypeCount { get; set; }
        public List<VisitsViewModel> Visits { get; set; }

    }
    public class PatientReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Degree { get; set; }
        public string GeneralNumber { get; set; }
        public string SarayNumber { get; set; }
        public string MilitaryNumber { get; set; }
        public int VisitRestCount { get; set; }
        public int VisitsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<int, int> ClinicsCount { get; set; }
        public int UnderObservationsCount { get; set; }
        public int MedicinesTakenCount =>  MedicinesTaken?.Sum(x=> x.VisitMedicinesCount)?? 0;
        public int MedicinesQuantityTakenCount =>  MedicinesTaken?.Sum(x=> x.VisitMedicinesQuantityCount) ?? 0;
        public List<UnderObservarionForPatientReport> UnderObservarionsHistory { get; set; }
        public List<VisitForPatientReport> VisitHistory { get; set; }
        public List<VisitRestForPatientReport> VisitRests { get; set; }
        public List<MedicinesTakenForPatientReport> MedicinesTaken { get; set; }
    }
    public class MedicinesTakenForPatientReport
    {
        public int VisitId { get; set; }
        public string VisitNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public int VisitMedicinesCount => VisitMedicines?.Count?? 0;
        public int VisitMedicinesQuantityCount => VisitMedicines?.Sum(x=> x.Quantity)?? 0;
        public List<VisitMedicineForPatientReport> VisitMedicines { get; set; }
    }
    public class VisitMedicineForPatientReport
    {
        public string MedicineName { get; set; }
        public int MedicineId { get; set; }
        public int VisitMedicineId { get; set; }
        public int Quantity { get; set; }
    }
    public class VisitRestForPatientReport
    {
        public int VisitId { get; set; }
        public int Id { get; set; }
        public string RestType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Notes { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
