using System.Net;
using System.Security.Claims;
using MedicalPoint.Data;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Patients;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;

namespace MedicalPoint.Controllers
{
    [Authorize]
    public class PatientsController : Controller
    {
        private readonly IPatientsService _patientsService;
        private readonly IDegreesService  _degreesService;
        private readonly IVisitsService _visitsService;
        public PatientsController(IPatientsService patientsService, IDegreesService degreesService,IVisitsService visitsService)
        {
            _patientsService = patientsService;
            _degreesService = degreesService;
            _visitsService = visitsService;
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
                RegisteredUserName = x.RegisteredUser?.FullName ?? string.Empty,
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
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(x=> x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            int userId = int.Parse(userIdStr);
            var result = await _patientsService.Add(viewModel.Name, viewModel.DegreeId, viewModel.MilitaryNumber, viewModel.NationalNumber, viewModel.GeneralNumber, viewModel.SaryaNumber, viewModel.Major, userId);
            if(!result.Success)
            { 
                return View();
            }
            return RedirectToAction(nameof(Index), "Patients");
        }

        public async Task<IActionResult>Detials(int id )
        {
            
            var patient = await _patientsService.GetById(id);
            
            //get patient id 
            ViewBag.Id = patient.Id;

            if(patient==null)
            {
                return NotFound();
            }
            //Creating the View model
            var viewModel =  new PatientViewModel
            {
                
                Name                = patient.Name,    
                Id                  = id,
                CreatedAt           =patient.CreatedAt,
                 Major              = patient.Major,
                 SaryaNumber        = patient.SaryaNumber,
                 MilitaryNumber     = patient.MilitaryNumber,
                 LastUpdatedAt      = patient.LastUpdatedAt,
                 LastVisitAt        = patient.LastVisitAt,
                 DegreeId           = patient.DegreeId,
                 NationalNumber     =patient.NationalNumber,
                 GeneralNumber      =patient.GeneralNumber,
                 

            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task< IActionResult> Edit(int id)
    
        {
            var patient = await _patientsService.GetById(id);
            if(patient==null)
            {
                return  NotFound();
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
        public async Task<IActionResult> Edit([FromForm] EditPatientViewModel viewModel)
        {
            var userIdStr = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            int userId = int.Parse(userIdStr);

            
            var result = await _patientsService.Edit(  viewModel.Id, viewModel.Name, viewModel.DegreeId,viewModel.MilitaryNumber, viewModel.NationalNumber, viewModel.GeneralNumber, viewModel.SaryaNumber, viewModel.Major,userId);
           
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(Index));
        }



        
       
    }
}
