using MedicalPoint.Services;
using MedicalPoint.ViewModels.Departments;

using Microsoft.AspNetCore.Mvc;

namespace MedicalPoint.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentsService _departmentsService;
        private readonly IUnderObservationBedsService _underObservationBedsService;

        public DepartmentsController(IDepartmentsService departmentsService, IUnderObservationBedsService underObservationBedsService, IUnderObservationBedsService underObservationBedsService1)
        {
            _departmentsService = departmentsService;
            _underObservationBedsService = underObservationBedsService;
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
            };
            return View(viewModel);
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
