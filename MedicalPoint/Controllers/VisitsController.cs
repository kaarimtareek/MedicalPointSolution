using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Visits;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace MedicalPoint.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly IVisitsService _visitsService;
        private readonly IPatientsService _patientsService;
        private readonly IClinicsServices _clinicsServices;
        private readonly IVisitImagesService _visitImagesService;
        private readonly IVisitMedicinesService _visitMedicinesService;

        public VisitsController(IVisitsService visitsService, IPatientsService patientsService, IClinicsServices clinicsServices, IVisitImagesService visitImagesService, IVisitMedicinesService visitMedicinesService)
        {
            _visitsService = visitsService;
            _patientsService = patientsService;
            _clinicsServices = clinicsServices;
            _visitImagesService = visitImagesService;
            _visitMedicinesService = visitMedicinesService;
        }
        public async Task<IActionResult> Index(DateTime? date)
        {
            var visits = await _visitsService.GetAll(null, null, date, date.HasValue? date.Value.AddDays(1): null);
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
                PatientDegree = x.Patient?.Degree?.Name,
            });
            return View(viewModel);
        }
        public async Task<IActionResult> Patient(int patientId)
        {
            var visits = await _visitsService.GetAll(null, patientId);
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
                ClinicName = x.Clinic?.Name ?? "",
                DoctorName = x.Doctor?.FullName ?? "",
                PatientName = x.Patient?.Name ?? "",
                PatientDegree = x.Patient?.Degree?.Name,
            });
            return View("Index",viewModel);
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
                    ContentType = x.Format
                }).ToList(),
                Medicines = visit.Medicines == null? null : visit.Medicines.Select(x=> new VisitMedicineViewModel
                {
                    Id = x.Id,
                    InventoryQuantity = x.Medicine.Quantity,
                    Quantity = x.Quantity,
                    MedicineId = x.Medicine.Id,
                    MedicineName = x.Medicine.Name,

                }).ToList()
            };
            ViewBag.AvailableMedicines =(await _visitMedicinesService.GetAvailableMedicinesForVisit(id)).Select(x=> new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            return View(viewModel);
        }
        public async Task<IActionResult> Create(int patientId)
        {
            var patient = await _patientsService.GetById(patientId);
            if(patient == null)
            {
                return NotFound();
            }
            ViewBag.Clinics = _clinicsServices.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            ViewBag.VisitTypes = ConstantVisitType.ALL.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
            });
            var patientViewModel = new PatientViewModel
            {
                Id = patientId,
                Name= patient.Name,
            };
            var viewModel = new AddVisitViewModel 
            {
                PatientId = patientId,
                Patient = patientViewModel,
            };

            return View(viewModel);
        }
        public async Task<IActionResult> FinishVisitDiagnosis(int id)
        {
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return NotFound();
            }
            var result = await _visitsService.ChangeStatus(id, userId.Value, ConstantVisitStatus.TAKING_MEDICINE);

            return RedirectToAction("Details", new { id });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddVisitViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return NotFound();
            }
            var result = await _visitsService.Create(userId.Value, viewModel.PatientId, viewModel.VisitTime, viewModel.ClinicId, viewModel.DoctorId, viewModel.Type);
           
           if(!result.Success) 
            {
                return RedirectToAction("Create",new { patientId= viewModel.PatientId });
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> WriteDiagnosis([FromForm] WriteVisitDiagnosisViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return NotFound();
            }
            var result = await _visitsService.WriteDiagnosis( viewModel.VisitId, userId.Value, viewModel.Diagnosis);
           
           if(!result.Success) 
            {
                return RedirectToAction("Details",new { id= viewModel.VisitId });
            }
            return RedirectToAction("Details", new { id = viewModel.VisitId });
        }
        [HttpPost]
        public async Task<IActionResult> UploadVisitImage([FromForm] int id)
        {

            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return NotFound();
            }
            var file = HttpContext.Request.Form.Files.FirstOrDefault();
            if(file == null)
            {
                return RedirectToAction("Details",new { id });
            }
            var isValidExtension = AllowedFileExtensions.IsValid(file);
            if(!isValidExtension)
            {
                return RedirectToAction("Details", new { id });

            }
            var result = await _visitImagesService.Add(file, id);
           
           if(!result.Success) 
            {
                return RedirectToAction("Details", new { id });
            }
            return RedirectToAction("Details", new { id });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteVisitImage( int id , [FromForm] int visitId)
        {

            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return NotFound();
            }
           
            var result = await _visitImagesService.Remove(id);
           if(!result.Success) 
            {
                return RedirectToAction("Details", new {id= visitId });
            }
            return RedirectToAction("Details", new { id = visitId });
        }

        [HttpPost]
        public async Task<IActionResult> AddVisitMedicine([FromForm]int VisitId, [FromForm] int MedicineId, [FromForm] int Quantity)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return NotFound();
            }
            var result = await _visitMedicinesService.Add(userId.Value, VisitId, MedicineId, Quantity, "");

            if (!result.Success)
            {
                return RedirectToAction("Details", new { id = VisitId });
            }
            return RedirectToAction("Details", new { id = VisitId });
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveVisitMedicine([FromForm] int id, [FromForm] int visitId)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return NotFound();
            }
            var result = await _visitMedicinesService.Remove(userId.Value, id);

            if (!result.Success)
            {
                return RedirectToAction("Details", new { id = visitId });
            }
            return RedirectToAction("Details", new { id = visitId });
        }

        public async Task<IActionResult> PatientVisits(int patientId)
        {

            
            var visits = await _visitsService.GetAll(null, patientId);
            var viewModel = visits.Select(x => new VisitsViewModel
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
                ClinicName = x.Clinic?.Name ?? "",
                DoctorName = x.Doctor?.FullName ?? "",
                PatientName = x.Patient?.Name ?? "",
                PatientDegree = x.Patient?.Degree?.Name ?? "",
            });
            return View("Index", viewModel);
        }


    }
}
