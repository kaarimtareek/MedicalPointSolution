using System.Net;
using System.Security.Claims;

using DocumentFormat.OpenXml.Math;

using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Data;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Patients;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Plugins;

namespace MedicalPoint.Controllers
{
    [Authorize(Roles = $"{ConstantUserType.Recieptionist},{ConstantUserType.SUPER_ADMIN},{ConstantUserType.Doctor}")]

    public class PatientsController : Controller
    {
        private readonly IPatientsService _patientsService;
        private readonly IDegreesService  _degreesService;
        private readonly IVisitsService _visitsService;
        private readonly IUnderObservationBedsService _underObservationBedsService;

        public PatientsController(IPatientsService patientsService, IDegreesService degreesService,IVisitsService visitsService, IUnderObservationBedsService underObservationBedsService)
        {
            _patientsService = patientsService;
            _degreesService = degreesService;
            _visitsService = visitsService;
            _underObservationBedsService = underObservationBedsService;
        }
      
        public async Task<IActionResult> Index(string searchValue,int? degreeId = null, string? checkHasVisit = null, int pageNumber = 1, int pageSize = 20)
        {
            var hasVisit = !string.IsNullOrEmpty(checkHasVisit);
           
            var patients = await _patientsService.GetPatients( pageNumber, pageSize, searchValue, degreeId, hasVisit);
            ViewBag.Degrees = _degreesService.GetAll().ConvertAll(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = degreeId.HasValue && degreeId.Value == x.Id,
            });
            ViewBag.SelectedDegree = degreeId;
            ViewBag.SearchValue = searchValue;
            ViewBag.CheckHasVisit = hasVisit;
            ViewBag.PageSizeList = new List<int>() { 10, 20, 50, 100 }.Select(x => new SelectListItem
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = pageSize == x
            });
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
            var paginatedViewModel = PaginatedList<PatientViewModel>.Create(viewModel, pageNumber, pageSize);
            return View(paginatedViewModel);
        }
        public IActionResult Create()
        {
            var degrees =  _degreesService.GetAll();
           
            ViewBag.Degrees = degrees.ConvertAll(x=> new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            SendErrorMessageToViewBagAndResetTempData();
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
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
                return RedirectToAction(nameof(Create), "Patients");
            }
            return RedirectToAction(nameof(Index), "Patients");
        }

        public async Task<IActionResult>Details(int id )
        {
            
            var patient = await _patientsService.GetById(id);
            if(patient==null)
            {
                return NotFound();
            }
            var bedId = await _underObservationBedsService.GetBedIdByPatientId(id);
            //Creating the View model
            var viewModel =  new PatientViewModel
            {
                
                Name = patient.Name,    
                Id = id,
                CreatedAt =patient.CreatedAt,
                 Major = patient.Major,
                 SaryaNumber  = patient.SaryaNumber,
                 MilitaryNumber = patient.MilitaryNumber,
                 LastUpdatedAt  = patient.LastUpdatedAt,
                 LastVisitAt    = patient.LastVisitAt,
                 DegreeId  = patient.DegreeId,
                 NationalNumber =patient.NationalNumber,
                 GeneralNumber  =patient.GeneralNumber,
                 Degree = patient.Degree?.Name ??string.Empty,
                 BedId = bedId,
                 RegisteredUserName = patient.RegisteredUser?.FullName??string.Empty,
            };
            SendErrorMessageToViewBagAndResetTempData();
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
            SendErrorMessageToViewBagAndResetTempData();
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
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
                return RedirectToAction(nameof(Edit), new {id = viewModel.Id});
            }
            return RedirectToAction(nameof(Index));
        }
        private void SendErrorMessageToViewBagAndResetTempData()
        {
            ViewBag.ErrorMessage = TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY];
            TempData.Clear();
        }

    }
}
