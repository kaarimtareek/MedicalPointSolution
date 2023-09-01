using MedicalPoint.Common;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Users;
using MedicalPoint.ViewModels.Visits;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace MedicalPoint.Controllers
{
    public class SuperAdminController : Controller
    {


        private readonly IPatientsService _patientsService;
        private readonly IDegreesService _degreesService;
        private readonly IVisitsService _visitsService;
        private readonly IMedicalPointUsersService _medicalPointUsersService;

        public SuperAdminController(IPatientsService patientsService, IDegreesService degreesService, IVisitsService visitsService , IMedicalPointUsersService medicalPointUsersService)
        {
            _patientsService = patientsService;
            _degreesService = degreesService;
            _visitsService = visitsService;
            _medicalPointUsersService = medicalPointUsersService;
        }

        public async Task<IActionResult> Index(DateTime? date)
        {
            var visits = await _visitsService.GetAll(null, null, date, date.HasValue ? date.Value.AddDays(1) : null);
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
    
            });
            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> UserEdit(int id)

        {
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


            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> UserEdit([FromForm] UsersViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }


            var result = await _medicalPointUsersService.Edit(userId.Value, viewModel.Email, viewModel.Name, viewModel.DegreeId, viewModel.MilitaryNumber, viewModel.PhoneNumber,viewModel.AccountType,true);

            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction("GetUsers","SuperAdmin");
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
                return View();
            }
            return RedirectToAction("GetPatients","SuperAdmin");
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
    }
}
