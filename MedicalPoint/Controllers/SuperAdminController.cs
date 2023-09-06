using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Clinics;
using MedicalPoint.ViewModels.Degrees;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Medicines;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Users;
using MedicalPoint.ViewModels.Visits;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace MedicalPoint.Controllers
{
    [Authorize(Roles = $"{ConstantUserType.SUPER_ADMIN}")]
    public class SuperAdminController : Controller
    {


        private readonly IPatientsService _patientsService;
        private readonly IDegreesService _degreesService;
        private readonly IVisitsService _visitsService;
        private readonly IMedicalPointUsersService _medicalPointUsersService;
        private readonly IMedicinesService _mediicinesService;
        private readonly IClinicsServices _clinicsServices;



        public SuperAdminController(IPatientsService patientsService, IDegreesService degreesService, IVisitsService visitsService , IMedicalPointUsersService medicalPointUsersService, IMedicinesService mediicinesService, IClinicsServices clinicsServices)
        {
            _patientsService = patientsService;
            _degreesService = degreesService;
            _visitsService = visitsService;
            _medicalPointUsersService = medicalPointUsersService;
            _mediicinesService = mediicinesService;
            _clinicsServices = clinicsServices;
        }

        public async Task<IActionResult> Index(DateTime? date)
        {
            var visits = await _visitsService.GetAll(1, 20, null, null, date, date.HasValue ? date.Value.AddDays(1) : null);
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
            return View(viewModel);
        }





        public async Task<IActionResult> GetUsers()
        {
            var userId = HttpContext.GetUserId();
            ViewBag.UserId = userId;
            var users = await _medicalPointUsersService.GetUsers();
            var viewModel = users.ConvertAll(x => new UsersViewModel
            {
                Id       = x.Id, 
                Name     = x.FullName,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                DegreeName =x.Degree.Name,
                AccountType=x.AccoutType.ToString(),
                MilitaryNumber = x.MilitaryNumber,
                IsActive = x.IsActive,
                DegreeId = x.DegreeId
    
            });
            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> UserEdit(int id)

        {
            ViewBag.PageName = "تعديل بيانات الحساب";
            var user = await _medicalPointUsersService.Get(id);
            if (user == null)
            {
                return NotFound();
            }

            var degrees = _degreesService.GetAll();
            ViewBag.Degrees = degrees.ConvertAll(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = x.Id == user.DegreeId
            });
            ViewBag.UserTypes = ConstantUserType.Types.Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
                Selected = x== user.AccoutType
            });
            var viewModel = new UsersViewModel
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DegreeId = user.Degree.Id,
                DegreeName = user.Degree?.Name??"",
                AccountType = user.AccoutType.ToString(),
                MilitaryNumber = user.MilitaryNumber,
                IsActive = user.IsActive


            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit(int id, [FromForm] UsersViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            var result = await _medicalPointUsersService.Edit(id, viewModel.Email, viewModel.Name, viewModel.DegreeId, viewModel.MilitaryNumber, viewModel.PhoneNumber,viewModel.AccountType);

            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetUsers","SuperAdmin");
        }

         
        public async Task<IActionResult> ChangeAtiveStatus(int id)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if(id == userId.Value)
                return RedirectToAction("GetUsers", "SuperAdmin");


            var result = await _medicalPointUsersService.ChangeUserActiveStatus(id);

            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetUsers","SuperAdmin");
        }





        [HttpGet]
        public async Task<IActionResult> ChangeUserPassword(int id)
        {
            var user = await _medicalPointUsersService.Get(id);
            if(user == null)
            {
                return NotFound();
            }
            var viewModel = new UserViewModel
            {
                AccountType = user.AccoutType,
                DegreeName = user.Degree?.Name ?? "",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.FullName,
                Id = user.Id,
                MilitaryNumber = user.MilitaryNumber,
                
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeUserPassword([FromForm] int id, [FromForm] string password)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var result = await _medicalPointUsersService.ChangePassword(id, password);
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(GetUsers), "SuperAdmin");
        }
        [HttpGet]
        public IActionResult MedicineCreate()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> MedicineCreate([FromForm] AddMedicinesViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var result = await _mediicinesService.Add(userId.Value, viewModel.Name, viewModel.Quantity, viewModel.MinimumQuantityThreshold);
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetMedicines", "SuperAdmin");
        }

     
        public async Task<IActionResult> GetMedicines()
        {
            var medicines = await _mediicinesService.GetAll();
            var viewModel = medicines.ConvertAll(x => new GetAllMediciensViewModel
            {
                CreatedAt = x.CreatedAt,
                Id = x.Id,
                Name = x.Name,
                LastUpdatedAt = x.LastUpdatedAt,
                Quantity = x.Quantity,
                MinimumQuantityThreshold = x.MinimumQuantityThreshold,

            });
            return View(viewModel);
        }


        public async Task<IActionResult> MedicinesDetails(int id)
        {

            var medicine = await _mediicinesService.Get(id);



            if (medicine == null)
            {
                return NotFound();
            }
            //Creating the View model
            var viewModel = new MedicineViewModel
            {

                Name = medicine.Name,
                Id = id,
                CreatedAt = medicine.CreatedAt,
                LastUpdatedAt = medicine.LastUpdatedAt,
                Quantity = medicine.Quantity,
                MinimumQuantityThreshold = medicine.MinimumQuantityThreshold,
                History = medicine.History.Select(x => new MedicineHistoryViewModel
                {

                    ActionType = x.ActionType,
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    MedicineId = x.Id,
                    MedicineName = x.MedicineName,
                    MedicineQuantity = x.MedicineQuantity,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    UserId = x.UserId,
                    UserName = x.User?.FullName ?? string.Empty,
                }).ToList()

            };
            return View(viewModel);
        }


        // Get  data patients

        public async Task<IActionResult> PatientDetials(int id)
        {

            var patient = await _patientsService.GetById(id);

            //get patient id 
            ViewBag.Id = patient.Id;

            if (patient == null)
            {
                return NotFound();
            }
            //Creating the View model
            var viewModel = new PatientViewModel
            {

                Name = patient.Name,
                Id = id,
                CreatedAt = patient.CreatedAt,
                Major = patient.Major,
                SaryaNumber = patient.SaryaNumber,
                MilitaryNumber = patient.MilitaryNumber,
                LastUpdatedAt = patient.LastUpdatedAt,
                LastVisitAt = patient.LastVisitAt,
                DegreeId = patient.DegreeId,
                NationalNumber = patient.NationalNumber,
                GeneralNumber = patient.GeneralNumber,


            };
            return View(viewModel);
        }

        public async Task<IActionResult> GetPatients()
        {
            var patients = await _patientsService.GetPatients();
            var viewModel = patients.ConvertAll(x => new PatientViewModel
            {
                CreatedAt = x.CreatedAt,
                DegreeId = x.DegreeId,
                GeneralNumber = x.GeneralNumber,
                Id = x.Id,
                Name = x.Name,
                LastUpdatedAt = x.LastUpdatedAt,
                LastVisitAt = x.LastVisitAt,
                Major = x.Major,
                MilitaryNumber = x.MilitaryNumber,
                NationalNumber = x.NationalNumber,
                SaryaNumber = x.SaryaNumber,
                Degree = x.Degree?.Name ?? string.Empty,
                RegisteredUserName = x.RegisteredUser?.FullName ?? string.Empty,
            });
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> PatientEdit(int id)

        {
            var patient = await _patientsService.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            var degrees = _degreesService.GetAll();
            ViewBag.Degrees = degrees.ConvertAll(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            var viewModel = new PatientViewModel
            {

                Name = patient.Name,
                Id = id,
                Major = patient.Major,
                SaryaNumber = patient.SaryaNumber,
                MilitaryNumber = patient.MilitaryNumber,
                LastUpdatedAt = patient.LastUpdatedAt,
                LastVisitAt = patient.LastVisitAt,
                DegreeId = patient.DegreeId,
                NationalNumber = patient.NationalNumber,
                GeneralNumber = patient.GeneralNumber,


            };
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> PatientEdit([FromForm] EditPatientViewModel viewModel)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            int userId = int.Parse(userIdStr);


            var result = await _patientsService.Edit(viewModel.Id, viewModel.Name, viewModel.DegreeId, viewModel.MilitaryNumber, viewModel.NationalNumber, viewModel.GeneralNumber, viewModel.SaryaNumber, viewModel.Major, userId);

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            SendErrorMessageToViewBagAndResetTempData();
            return RedirectToAction("GetPatients","SuperAdmin");
        }



        public async Task<IActionResult> GetClinics()
        {
            var clinic = _clinicsServices.GetAll();
            var viewModel = clinic.ConvertAll(x => new ClinicViewModel
            {
                Id = x.Id,
                Name = x.Name,
            });
            return View(viewModel);
        }
        //EditClinics


        public async Task<IActionResult> EditClinics(int id)

        {
            var clinic = await _clinicsServices.GetById(id);
            if (clinic == null)
            {
                return NotFound();
            }

            var viewModel = new ClinicViewModel
            {

                Name = clinic.Name,
                Id = id,
               

            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditClinics([FromForm] ClinicViewModel viewModel)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            int userId = int.Parse(userIdStr);


            var result = await _clinicsServices.Edit(viewModel.Id, viewModel.Name,true);

            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetClinics", "SuperAdmin");
        }


        [HttpGet]
        public IActionResult ClinicCreate()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ClinicCreate([FromForm] AddClinicViewModel viewModel)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            int userId = int.Parse(userIdStr);
            var result = await _clinicsServices.Add(viewModel.Name );
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetClinics", "SuperAdmin");
        }











        public async Task<IActionResult> GetDegrees()
        {
            var degrees = _degreesService.GetAll();
            var viewModel = degrees.ConvertAll(x => new DegreesViewModel
            {
                Id = x.Id,
                Name = x.Name,
            });
            return View(viewModel);
        }
        //EditClinics


        public async Task<IActionResult> EditDegree(int id)

        {
            var clinic = await _degreesService.GetById(id);
            if (clinic == null)
            {
                return NotFound();
            }

            var viewModel = new DegreesViewModel
            {

                Name = clinic.Name,
                Id = id,


            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> EditDegree([FromForm] EditDegreeViewModel viewModel)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            int userId = int.Parse(userIdStr);


            var result = await _degreesService.Edit(viewModel.Id, viewModel.Name );

            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetDegrees", "SuperAdmin");
        }


        [HttpGet]
        public IActionResult DegreeCreate()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DegreeCreate([FromForm] AddDegreeViewModel viewModel)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            int userId = int.Parse(userIdStr);
            var result = await _degreesService.Add(viewModel.Name);
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetDegrees", "SuperAdmin");
        }



        // Get Visit data
        public async Task<IActionResult> Details(int id)
        {
            var visit = await _visitsService.Get(id);
            if (visit == null)
                return NotFound();
            var viewModel = new VisitViewModel
            {
                ClinicId = visit.ClinicId,
                Clinic = visit.Clinic == null ? null : new ViewModels.Clinics.ClinicViewModel
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
                Images = visit.Images == null ? null : visit.Images.Select(x => new VisitImageViewModel
                {
                    Name = x.Name,
                    Content = x.Content,
                    Id = x.Id,
                    ContentType = x.Format
                }).ToList(),
                Medicines = visit.Medicines == null ? null : visit.Medicines.Select(x => new VisitMedicineViewModel
                {
                    Id = x.Id,
                    InventoryQuantity = x.Medicine.Quantity,
                    Quantity = x.Quantity,
                    MedicineId = x.Medicine.Id,
                    MedicineName = x.Medicine.Name,

                }).ToList()
            };
            return View(viewModel);
        }
        private void SendErrorMessageToViewBagAndResetTempData()
        {
            ViewBag.ErrorMessage = TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY];
            TempData.Clear();
        }
    }

}
