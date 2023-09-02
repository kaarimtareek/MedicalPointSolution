using MedicalPoint.Common;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Beds;
using MedicalPoint.ViewModels.Departments;
using MedicalPoint.ViewModels.Patients;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalPoint.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentsService _departmentsService;
        private readonly IUnderObservationBedsService _underObservationBedsService;
        private readonly IPatientsService _patientsService;

        public DepartmentsController(IDepartmentsService departmentsService, IUnderObservationBedsService underObservationBedsService, IPatientsService patientsService)
        {
            _departmentsService = departmentsService;
            _underObservationBedsService = underObservationBedsService;
            _patientsService = patientsService;
        }
        public async Task<IActionResult> Index()
        {
            var departments = (await _departmentsService.GetAll()).Select(x=> new DepartmentsViewModel
            {

                AvailableBedsCount = x.AvailableBedsCount,
                BedsCount = x.BedsCount,
                Id = x.Id,
                Name = x.Name,
            }).ToList();
            return View(departments);
        }
        public async Task<IActionResult> Create()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddDepartmentViewModel viewModel)
        {
            var result = await _departmentsService.Create(viewModel.Name, viewModel.BedsCount);
            if(!result.Success)
                return View();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentsService.Get(id);
            if(department == null)
            {
                return NotFound();
            }
            var viewModel = new DepartmentViewModel
            {
                Name = department.Name,
                Id = id,
                BedsCount = department.BedsCount,
                AvailableBedsCount = department.AvailableBedsCount,
                Beds = department.Beds.Select(x => new BedsViewModel
                {
                    BedNumber = x.BedNumber,
                    DepartmentId = x.DepartmentId,
                    DoctorId = x.DoctorId,
                    EnterDate = x.EnterDate,
                    Id = x.Id,
                    IsActive = x.IsActive,
                    Notes = x.Notes,
                    PatientId = x.PatientId,
                    VisitId = x.VisitId,
                    DoctorName = x.Doctor?.FullName ?? "",
                    PatientName = x.Patient?.Name ?? "",
                }).ToList(),
            };
            return View(viewModel);
        }
        public async Task<IActionResult> Bed(int id)
        {
            var bed = await _underObservationBedsService.Get(id);
            if (bed == null)
            {
                return NotFound();
            }
            var viewModel = new BedViewModel
            {
                BedNumber = bed.BedNumber,
                DepartmentId = bed.DepartmentId,
                DepartmentName = bed.Department?.Name??"",
                DoctorDegree = bed.Doctor?.Degree?.Name??"",
                DoctorId = bed.DoctorId,
                DoctorName = bed.Doctor?.FullName??"",
                EnterDate= bed.EnterDate,
                Id= bed.Id,
                IsActive= bed.IsActive,
                Notes= bed.Notes,
                PatientId = bed.PatientId,
                VisitId = bed.VisitId,
                PatientDegree = bed.Patient?.Degree?.Name??"",
                PatientName = bed.Patient?.Name??"",
                History = bed.History?.Select(x=> new BedHistoryViewModel 
                {
                    Id = x.Id,
                    ActionDate = x.ActionDate,
                    ActionType = x.ActionType,
                    BedId = x.BedId,
                    DoctorId= x.DoctorId,
                    Notes = x.Notes??"",
                    PatientId = x.PatientId,
                    VisitId = x.VisitId,
                    DoctorName = x.Doctor?.FullName??"",
                    PatientName = x.Patient?.Name??"",
                    EnterDate = x.EnterDate,

                }).ToList(),
            };
            return View(viewModel);
        }
        public async Task<IActionResult> AddPatientToBed(int id)
        {
            var patient = await _patientsService.GetById(id);
            if(patient == null)
            {
                return NotFound();
            }

            var availableDepartments = await _departmentsService.GetAllAvailable();
            if(availableDepartments.Count == 0)
            {
                return NotFound();
            }
            //var departmentsIds = availableDepartments.Select(x=> x.Id).ToList();
            //var beds = await _underObservationBedsService.GetAllAvailable(departmentsIds);
            //if(beds.Count == 0)
            //{
            //    return BadRequest();
            //}
            
            
            var viewModel =  new AddPatientToBedViewModel{
                
                Departments = availableDepartments.ConvertAll(x=> new DepartmentsViewModel
                {
                    Id = x.Id,
                    AvailableBedsCount = x.AvailableBedsCount,
                    BedsCount = x.BedsCount,
                    Name = x.Name,
                     Beds = x.Beds.Select(x => new BedsViewModel
                     {
                         PatientId= x.PatientId,
                         BedNumber = x.BedNumber,
                         DepartmentId = x.DepartmentId,
                         DoctorId = x.DoctorId,
                         Id = x.Id,
                     }).ToList(),
                }),
                
                PatientId = id,
              Patient = new PatientViewModel
            {
                Id = patient.Id,
                CreatedAt = patient.CreatedAt,
                DegreeId = patient.DegreeId,
                Degree = patient.Degree?.Name?? string.Empty,
                Name = patient.Name,

            }};
          
            return View(viewModel);
        }
       //bed id
        public async Task<IActionResult> AddBedToPatient(int id)
        {
            var patient = await _patientsService.GetAvailableToAddToBed();
            var viewModel = new AddBedToPatientViewModel
            {
                Id = id,
                Patients = patient.ConvertAll(x => new PatientViewModel
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedAt,
                    Name = x.Name,
                    GeneralNumber = x.GeneralNumber,
                    DegreeId = x.DegreeId,
                    Degree = x.Degree?.Name ?? string.Empty,
                    MilitaryNumber = x.MilitaryNumber,
                }),
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> AddBedToPatient(int id, [FromForm] int patientId, [FromForm] string notes)
        {
            var userId = HttpContext.GetUserId();
            if (!userId.HasValue)
            {
                return NotFound();
            }
            var result = await _underObservationBedsService.AddPatientToBed(id,  patientId, userId.Value, notes);
            if(!result.Success)
            {

            }
            return RedirectToAction("Bed", "Departments", new { id });
        }
       
        public async Task<IActionResult> RemovePatient(int id)
        {
            var userId = HttpContext.GetUserId();
            if (!userId.HasValue)
            {
                return NotFound();
            }
            var result = await _underObservationBedsService.RemovePatientFromBed(id, userId.Value);
            if(!result.Success)
            {

            }
            return RedirectToAction(nameof(Bed), new {id });
        }
        [HttpPost]
        public async Task<IActionResult> AddPatientToBed(int id, [FromForm] AddPatientToBedFormViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if(!userId.HasValue)
            {
                return NotFound();
            }
            var result = await _underObservationBedsService.AddPatientToBed(id,  viewModel.PatientId, userId.Value, viewModel.Notes, null);
            if(!result.Success)
            { 
                return BadRequest();
            }
            return RedirectToAction("Bed", "Departments", new {id });
        }
        public async Task<IActionResult> CreateBed(int id)
        {
            var result = await _underObservationBedsService.AddBedToDepartment(id);
            if (!result.Success)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
