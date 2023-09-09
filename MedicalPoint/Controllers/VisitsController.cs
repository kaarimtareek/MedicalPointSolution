
using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;
using MedicalPoint.Models;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Medicines;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Visits;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace MedicalPoint.Controllers
{
    [Authorize(Roles = $"{ConstantUserType.Recieptionist},{ConstantUserType.SUPER_ADMIN},{ConstantUserType.Doctor}")]
    public class VisitsController : Controller
    {
        private readonly IVisitsService _visitsService;
        private readonly IPatientsService _patientsService;
        private readonly IClinicsServices _clinicsServices;
        private readonly IVisitImagesService _visitImagesService;
        private readonly IVisitMedicinesService _visitMedicinesService;
        private readonly IVisitRestsService _visitRestsService;
        private readonly ICacheService _cacheService;
        private readonly IUploadService _uploadService;

        public VisitsController(IVisitsService visitsService, IPatientsService patientsService, IClinicsServices clinicsServices, IVisitImagesService visitImagesService, IVisitMedicinesService visitMedicinesService, IVisitRestsService visitRestsService, ICacheService cacheService, IUploadService uploadService)
        {
            _visitsService = visitsService;
            _patientsService = patientsService;
            _clinicsServices = clinicsServices;
            _visitImagesService = visitImagesService;
            _visitMedicinesService = visitMedicinesService;
            _visitRestsService = visitRestsService;
            _cacheService = cacheService;
            _uploadService = uploadService;
        }
        public async Task<IActionResult> Index( string? searchValue, DateTime? date, string? type, int? clinicId = null, int? doctorId = null, int pageNumber = 1, int pageSize = 20 )
        {
            type = !string.IsNullOrEmpty(type) && type == "null" ? null : type;
            var visits = await _visitsService.GetAll(pageNumber, pageSize, doctorId, null, date, date.HasValue? date.Value.AddDays(1): null, type, clinicId, searchValue);
            var clinics = _cacheService.GetClinics();
            ViewBag.SearchValue = searchValue;
            ViewBag.Date = date?.ToShortDateString();
            ViewBag.Clinics = clinics.ConvertAll(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = clinicId.HasValue &&  x.Id == clinicId.Value
            });
            ViewBag.SelectedClinic = clinicId;
            ViewBag.VisitTypes = ConstantVisitType.ALL.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = !string.IsNullOrEmpty(type) && type == x
            }).ToList();
            ViewBag.SelectedVisitType = type;
            ViewBag.PageSizeList = new List<int>() { 10, 20, 50, 100 }.Select(x => new SelectListItem
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = pageSize == x
            });
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
                IsMedicinesGiven = x.IsMedicinesGiven,
                ClinicName = x.Clinic?.Name??"",
                DoctorName = x.Doctor?.FullName??"",
                PatientName = x.Patient?.Name??"",
                PatientDegree = x.Patient?.Degree?.Name,
                PatientGeneralNumber = x.Patient?.GeneralNumber?? "",
            });
            var paginatedViewModel = PaginatedList< VisitsViewModel >.Create(viewModel, pageNumber, pageSize);
            SendErrorMessageToViewBagAndResetTempData();
            return View(paginatedViewModel);
        }
        public async Task<IActionResult> Patient(int patientId)
        {
            var visits = await _visitsService.GetAll(1, int.MaxValue, null, patientId);
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
                IsMedicinesGiven = x.IsMedicinesGiven,
                VisitTime = x.VisitTime,
                ClinicName = x.Clinic?.Name ?? "",
                DoctorName = x.Doctor?.FullName ?? "",
                PatientName = x.Patient?.Name ?? "",
                PatientGeneralNumber = x.Patient?.GeneralNumber?? "",
                PatientDegree = x.Patient?.Degree?.Name,
            });
            var paginatedViewMode = PaginatedList<VisitsViewModel>.Create(viewModel, 1, int.MaxValue);
            return View("Index", paginatedViewMode);
        }
    [Authorize(Roles = $"{ConstantUserType.Recieptionist},{ConstantUserType.SUPER_ADMIN},{ConstantUserType.Doctor},{ConstantUserType.Pharmacist}")]
        public async Task<IActionResult> Details(int id)
        {
            var visit = await _visitsService.Get(id);
            if(visit == null)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = ConstantMessageCodes.VisitNotFound;
                TempData[ConstantMessageCodes.ACTION_MESSAGE_KEY] = nameof(Index);
                TempData[ConstantMessageCodes.CONTROLLER_MESSAGE_KEY] = nameof(VisitsController);

                return NotFound();
            }
            var hasVisitRest = await _visitsService.IsVisitHasRest(id);
            var viewModel = new VisitViewModel
            {
                ClinicId= visit.ClinicId,
                Clinic = visit.Clinic== null? null: new ViewModels.Clinics.ClinicViewModel
                {
                    Name = visit.Clinic.Name,
                    Id = visit.Clinic.Id,
                    IsActive = visit.Clinic.IsActive,
                },
                 HasVisitRest = hasVisitRest,
                ExitTime = visit.ExitTime,
                Id = visit.Id,
                CreatedAt = visit.CreatedAt,
                Diagnosis = visit.Diagnosis,
                IsMedicinesGiven = visit.IsMedicinesGiven,
                MedicineGivenTime = visit.MedicineGivenTime,
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
                    RegisteredUserName = visit.Patient.RegisteredUser?.FullName??""
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
            ViewBag.AvailableMedicines =(await _visitMedicinesService.GetAvailableMedicinesForVisit(id)).ConvertAll(x=> new MedicineViewModel
            {
                Id=x.Id,
                Quantity =x.Quantity,
                Name = x.Name
            });
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }
        public async Task<IActionResult> Create(int patientId)
        {
            var patient = await _patientsService.GetById(patientId);
            if(patient == null)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = ConstantMessageCodes.PatientNotFound;
                TempData[ConstantMessageCodes.ACTION_MESSAGE_KEY] = nameof(Index);
                TempData[ConstantMessageCodes.CONTROLLER_MESSAGE_KEY] = nameof(VisitsController);

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
            ViewBag.ErrorMessage = TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY];
            TempData.Clear();
            return View(viewModel);
        }
        public async Task<IActionResult> FinishVisitDiagnosis(int id)
        {
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return View("AccessDenied","Account");
            }
            var result = await _visitsService.ChangeStatus(id, userId.Value, ConstantVisitStatus.TAKING_MEDICINE);
            if(!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            return RedirectToAction("Details", new { id });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddVisitViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return View("AccessDenied","Account");
            }
            var result = await _visitsService.Create(userId.Value, viewModel.PatientId, viewModel.VisitTime, viewModel.ClinicId, viewModel.DoctorId, viewModel.Type);
           
           if(!result.Success) 
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
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
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
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
        
        //[HttpPost]
        //public async Task<IActionResult> UploadExcelFile()
        //{

        //    var userId = HttpContext.GetUserId();
        //    if(userId == null)
        //    {
        //        return NotFound();
        //    }
        //    var file = HttpContext.Request.Form.Files.FirstOrDefault();
        //    if(file == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
            
        //     await _uploadService.UploadPatients(file);
           
          
        //    return RedirectToAction("Index");
        //}
        
        //public async Task<IActionResult> ExportExcelFile()
        //{

          
            
        //     var result = await _uploadService.ExportPatients();

        //    if (result.Length == 0)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteVisitImage( int id , [FromForm] int visitId)
        {

            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
           
            var result = await _visitImagesService.Remove(id);
           if(!result.Success) 
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            return RedirectToAction("Details", new { id = visitId });
        }

        [HttpPost]
        public async Task<IActionResult> AddVisitMedicine([FromForm]int VisitId, [FromForm] int MedicineId, [FromForm] int Quantity)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _visitMedicinesService.Add(userId.Value, VisitId, MedicineId, Quantity, "");

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            return RedirectToAction("Details", new { id = VisitId });
        }
        
        [HttpPost]
        public async Task<IActionResult> EditVisitMedicine([FromForm] int id, [FromForm] int visitId, [FromForm] int Quantity)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _visitMedicinesService.Edit(userId.Value, id, Quantity, "");

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            return RedirectToAction("Details", new { id = visitId });
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveVisitMedicine([FromForm] int id, [FromForm] int visitId)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _visitMedicinesService.Remove(userId.Value, id);

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            return RedirectToAction("Details", new { id = visitId });
        }

        public async Task<IActionResult> PatientVisits(int patientId) 
        {

            
            var visits = await _visitsService.GetAll(1, int.MaxValue, null, patientId);
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
                IsMedicinesGiven = x.IsMedicinesGiven,
                VisitNumber = x.VisitNumber,
                VisitTime = x.VisitTime,
                ClinicName = x.Clinic?.Name ?? "",
                DoctorName = x.Doctor?.FullName ?? "",
                PatientName = x.Patient?.Name ?? "",
                PatientGeneralNumber = x.Patient?.GeneralNumber?? "",
                PatientDegree = x.Patient?.Degree?.Name ?? "",
            }).ToList();
            var paginatedViewMode = PaginatedList<VisitsViewModel>.Create(viewModel, 1, int.MaxValue);
            return View("Index", paginatedViewMode);
        }
        public async Task<IActionResult> VisitRest(int id)
        {
            var visit = await _visitsService.Get(id);
            if (visit == null)
            {
                var errorViewModel = new ErrorViewModel {
                ControllerPath = nameof(VisitsController),
                ActionPath = nameof(Index),
                ErrorMessage = ConstantMessageCodes.VisitNotFound,
                };
                return NotFound(errorViewModel);
            }
            var visitRest = await _visitsService.GetVisitRest(id);
            if(visitRest == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    ControllerPath = nameof(VisitsController),
                    ActionPath = nameof(Details),
                    ErrorMessage = ConstantMessageCodes.VisitRestNotFound,
                    Id = id.ToString(),
                };
                return NotFound(errorViewModel);
            }
            ViewBag.RestTypes = (await _visitsService.GetVisitRestTypes()).Select(x=> new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            var viewModel = new VisitRestViewModel
            {
                ClinicId = visit.ClinicId,
                RestType = visitRest.RestType== null? null: visitRest.RestType.Name,
                Clinic = visit.Clinic == null ? null : new ViewModels.Clinics.ClinicViewModel
                {
                    Name = visit.Clinic.Name,
                    Id = visit.Clinic.Id,
                    IsActive = visit.Clinic.IsActive,
                },
                Id = visit.Id,
                CreatedAt = visit.CreatedAt,
                Diagnosis = visit.Diagnosis,
                IsMedicinesGiven = visit.IsMedicinesGiven,
                MedicineGivenTime = visit.MedicineGivenTime,
                DoctorId = visit.DoctorId,
                PatientId = visit.PatientId,
                Notes = visitRest.Notes,
                Type = visit.Type,
                EndDate = visitRest.EndDate,
                RestDaysNumber = visitRest.RestDaysNumber,
                RestTypeId = visitRest.RestTypeId,
                StartDate = visitRest.StartDate,
                VisitId = id,
                VisitNumber = visit.VisitNumber,
                Doctor = visit.Doctor == null ? null : new DoctorViewModel
                {
                    Id = visit.DoctorId.Value,
                    FullName = visit.Doctor.FullName,
                    IsActive = visit.Doctor.IsActive,
                },
                Patient = visit.Patient == null ? null : new PatientViewModel
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
                    Degree = visit.Patient.Degree?.Name ?? "",
                },
                Medicines = visit.Medicines == null ? null : visit.Medicines.Select(x => new VisitMedicineViewModel
                {
                    Id = x.Id,
                    InventoryQuantity = x.Medicine.Quantity,
                    Quantity = x.Quantity,
                    MedicineId = x.Medicine.Id,
                    MedicineName = x.Medicine.Name,

                }).ToList(),
            };
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }
        
        public async Task<IActionResult> CreateVisitRest(int id)
        {
            var visit = await _visitsService.Get(id);
            if (visit == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    ActionPath = nameof(Visit),
                    Id = id.ToString(),
                    ErrorMessage = ConstantMessageCodes.VisitNotFound,
                    ControllerPath = nameof(VisitsController),
                };
                return NotFound(errorViewModel);
            }
            var visitRest = await _visitsService.GetVisitRest(id);
            if(visitRest != null)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = ConstantMessageCodes.VisitRestAlreadyExist;
                return RedirectToAction(nameof(Details), new {id});
            }
            ViewBag.RestTypes = (await _visitsService.GetVisitRestTypes()).Select(x=> new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            var viewModel = new CreateVisitRestViewModel
            {
                ClinicId = visit.ClinicId,
                DegreeId = visit.Patient== null? 0 :  visit.Patient.DegreeId,
                RestType = "",
                Clinic = visit.Clinic == null ? null : new ViewModels.Clinics.ClinicViewModel
                {
                    Name = visit.Clinic.Name,
                    Id = visit.Clinic.Id,
                    IsActive = visit.Clinic.IsActive,
                },
                Id = visit.Id,
                CreatedAt = visit.CreatedAt,
                Diagnosis = visit.Diagnosis,
                IsMedicinesGiven = visit.IsMedicinesGiven,
                MedicineGivenTime = visit.MedicineGivenTime,
                DoctorId = visit.DoctorId,
                PatientId = visit.PatientId,
                Notes = visit.Notes,
                
                VisitId = id,
                VisitNumber = visit.VisitNumber,
                Doctor = visit.Doctor == null ? null : new DoctorViewModel
                {
                    Id = visit.DoctorId.Value,
                    FullName = visit.Doctor.FullName,
                    IsActive = visit.Doctor.IsActive,
                },
                Patient = visit.Patient == null ? null : new PatientViewModel
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
                    Degree = visit.Patient.Degree?.Name ?? "",
                },
                Medicines = visit.Medicines == null ? null : visit.Medicines.Select(x => new VisitMedicineViewModel
                {
                    Id = x.Id,
                    InventoryQuantity = x.Medicine.Quantity,
                    Quantity = x.Quantity,
                    MedicineId = x.Medicine.Id,
                    MedicineName = x.Medicine.Name,

                }).ToList(),
            };
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateVisitRest([FromForm] CreateVisitRestViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result =await _visitRestsService.Add(viewModel.VisitId, userId.Value, viewModel.RestTypeId, viewModel.Notes, viewModel.StartDate, viewModel.RestDaysNumber);
            if(!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = ConstantMessageCodes.VisitRestAlreadyExist;
                return RedirectToAction(nameof(Details), new { id = viewModel.VisitId });
            }
            
            return RedirectToAction(nameof(VisitRest), new { id = viewModel.VisitId});
        }
        private void SendErrorMessageToViewBagAndResetTempData()
        {
            ViewBag.ErrorMessage = TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY];
            TempData.Clear();
        }
    }
}
