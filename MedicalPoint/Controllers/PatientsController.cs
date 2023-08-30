using MedicalPoint.Services;
using MedicalPoint.ViewModels.Patients;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalPoint.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly IPatientsService _patientsService;
        private readonly IDegreesService _degreesService;

        public PatientsController(IPatientsService patientsService, IDegreesService degreesService)
        {
            _patientsService = patientsService;
            _degreesService = degreesService;
        }
      
        public async Task<IActionResult> Index()
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
            });
            return View(viewModel);
        }
        public IActionResult Create()
        {
            var degrees =  _degreesService.GetAll();
            ViewBag.Degrees = degrees.ConvertAll(x=> new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddPatientViewModel viewModel)
        {
            var result = await _patientsService.Add(viewModel.Name, viewModel.DegreeId, viewModel.MilitaryNumber, viewModel.NationalNumber, viewModel.GeneralNumber, viewModel.SaryaNumber, viewModel.Major);
            if(!result.Success)
            { 
                return View();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
