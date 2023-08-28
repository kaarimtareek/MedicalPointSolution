using MedicalPoint.Services;

using Microsoft.AspNetCore.Mvc;

namespace MedicalPoint.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IPatientsService _patientsService;

        public PatientsController(IPatientsService patientsService)
        {
            _patientsService = patientsService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> IndexPatients()
        {
            var patients = await _patientsService.GetPatients();
            return View(patients);
        }
    }
}
