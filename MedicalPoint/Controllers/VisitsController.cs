using MedicalPoint.Data;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Visits;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalPoint.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly IVisitsService _visitsService;
        private readonly IPatientsService _patientsService;

        public VisitsController(IVisitsService visitsService, IPatientsService patientsService)
        {
            _visitsService = visitsService;
            _patientsService = patientsService;
        }
        public async Task<IActionResult> Index()
        {
            var visits = await _visitsService.GetAll();
            var viewModel = visits.ConvertAll(x => new VisitsViewModel
            {
                ClinicId = x.Id,
                Diagnosis = x.Diagnosis,
                DoctorId = x.DoctorId,
                ExitTime = x.ExitTime,
                FollowingVisitDate = x.FollowingVisitDate,
                Id = x.Id,
                IsDeleted = x.IsDeleted,
                PatientId = x.PatientId,
                PreviousVisitId = x.PreviousVisitId,
                Status = x.Status,
                Type = x.Type,
                VisitNumber = x.VisitNumber,
                VisitTime = x.VisitTime,
                ClinicName = x.Clinic?.Name??"",
                DoctorName = x.Doctor?.FullName??"",
                PatientName = x.Patient?.Name??"",
            });
            return View(viewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            var visit = await _visitsService.Get(id);
            if(visit == null)
                return NotFound();
            var viewModel = new VisitViewModel
            {
                ClinicId= visit.ClinicId,
                Clinic = visit.Clinic== null? null: new ViewModels.Clinics.ClinicViewModel
                {
                    Name = visit.Clinic.Name,
                    Id = visit.Clinic.Id,
                    IsActive = visit.Clinic.IsActive,
                },
                ExitTime = visit.ExitTime,
                Id = visit.Id,
                CreatedAt = visit.CreatedAt,
                Diagnosis = visit.Diagnosis,
                DoctorId = visit.DoctorId,
                PatientId = visit.PatientId,
                FollowingVisitDate = visit.FollowingVisitDate,
                HasFollowingVisit = visit.HasFollowingVisit,
                Notes = visit.Notes,
                PreviousVisitId = visit.PreviousVisitId,
                Type = visit.Type,
                Status = visit.Status,
                VisitTime = visit.VisitTime,
                VisitNumber = visit.VisitNumber,
                Doctor = visit.Doctor == null? null : new DoctorViewModel
                {
                    Id = visit.DoctorId.Value,
                    FullName = visit.Doctor.FullName,
                    IsActive= visit.Doctor.IsActive,
                },
                Patient = visit.Patient == null? null : new PatientViewModel
                {
                    CreatedAt = visit.Patient.CreatedAt,
                    DegreeId = visit.Patient.DegreeId,
                    GeneralNumber = visit.Patient.GeneralNumber,
                    Id = visit.Patient.Id,
                    LastUpdatedAt = visit.Patient.LastUpdatedAt,
                    Name = visit.Patient.Name,
                    SaryaNumber = visit.Patient.SaryaNumber,
                    MilitaryNumber = visit.Patient.MilitaryNumber,
                    Major = visit.Patient.Major,
                    NationalNumber = visit.Patient.NationalNumber,
                    LastVisitAt = visit.Patient.LastVisitAt,
                    Degree = visit.Patient.Degree?.Name??"",
                },
                Images = visit.Images == null? null : visit.Images.Select(x=> new VisitImageViewModel
                {
                    Name = x.Name,
                    Content = x.Content,
                    Id = x.Id,
                }).ToList()
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Create(int patientId)
        {
            var patient = new PatientViewModel();
            var viewModel = new AddVisitViewModel 
            {
                PatientId = patientId,
                Patient = patient,
            };

            return View(viewModel);
        }
    }
}
