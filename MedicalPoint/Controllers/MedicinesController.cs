using MedicalPoint.Common;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Medicines;
using MedicalPoint.ViewModels.Patients;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalPoint.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly IMedicinesService _mediicinesService;

        public MedicinesController(IMedicinesService mediicinesService)
        {
            _mediicinesService = mediicinesService;

        }
        public async Task< IActionResult> Index() 
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

        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AddMedicinesViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            
            var result = await _mediicinesService.Add(userId.Value, viewModel.Name, viewModel.Quantity,viewModel.MinimumQuantityThreshold);
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(Index), "Medicines");
        }

    }



}
