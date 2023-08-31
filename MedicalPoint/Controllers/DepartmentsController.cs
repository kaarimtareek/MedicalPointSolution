using MedicalPoint.Services;
using MedicalPoint.ViewModels.Departments;

using Microsoft.AspNetCore.Mvc;

namespace MedicalPoint.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IDepartmentsService _departmentsService;
        private readonly IUnderObservationBedsService _underObservationBedsService;

        public DepartmentsController(IDepartmentsService departmentsService, IUnderObservationBedsService underObservationBedsService)
        {
            _departmentsService = departmentsService;
            _underObservationBedsService = underObservationBedsService;
        }
        public async Task<IActionResult> Index()
        {
            var departments = (await _departmentsService.GetAll()).Select(x=> new DepartmentsViewModel
            {

            });
            return View(departments);
        } public async Task<IActionResult> Details(int id)
        {
            var department = await _departmentsService.Get(id);
            return View(department);
        }
    }
}
