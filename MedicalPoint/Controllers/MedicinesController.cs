using MedicalPoint.Common;
using MedicalPoint.Data;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Medicines;
using MedicalPoint.ViewModels.Patients;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedicalPoint.Controllers
{
    [Authorize]
    public class MedicinesController : Controller
    {
        private readonly IMedicinesService _medicinesService;

        public MedicinesController(IMedicinesService mediicinesService)
        {
            _medicinesService = mediicinesService;

        }
        public async Task< IActionResult> Index() 
        {
            var medicines = await _medicinesService.GetAll();
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
            
            var result = await _medicinesService.Add(userId.Value, viewModel.Name, viewModel.Quantity,viewModel.MinimumQuantityThreshold);
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(Index), "Medicines");
        }

        [HttpPost]
        public async Task<IActionResult> AddQuantity([FromForm] int MedicineId, [FromForm] int Quantity)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var result = await _medicinesService.AddQauntity(userId.Value, MedicineId,  Quantity);
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(Index), "Medicines");
        }

        public async Task<IActionResult> Details(int id)
        {

            var medicine = await _medicinesService.Get(id);

            

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
               History = medicine.History.Select(x=> new MedicineHistoryViewModel
               {

                   ActionType = x.ActionType,
                   CreatedAt = x.CreatedAt,
                   Id = x.Id,
                   MedicineId = x.Id,
                   MedicineName = x.MedicineName,
                   MedicineQuantity = x.MedicineQuantity,
                   MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                   UserId = x.UserId,
                   UserName = x.User?.FullName?? string.Empty,
               }).ToList()
              
            };
            return View(viewModel);
        }


        // delete Element Medicines 
        public async Task<IActionResult> Delete(int id)
        {

            var medicine = await _medicinesService.Get(id);
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return NotFound();
            }
            if (medicine == null)
            {
                return NotFound();
            }
            await _medicinesService.Delete(userId.Value, id);
           
            return RedirectToAction("Index","Medicines");
        }



        // edit medicine with Super Admin

        public async Task<IActionResult> Edit(int id)

        {
            var medicines = await _medicinesService.Get(id);
            if (medicines == null)
            {
                return NotFound();
            }

           
            var viewModel = new GetAllMediciensViewModel
            {

               
                Name = medicines.Name,
                Id = id,
                CreatedAt = medicines.CreatedAt,
                LastUpdatedAt = medicines.LastUpdatedAt,
                Quantity = medicines.Quantity,
                MinimumQuantityThreshold = medicines.MinimumQuantityThreshold,
                

            };
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromForm]GetAllMediciensViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _medicinesService.Edit(userId.Value, viewModel.Id,viewModel.Name, viewModel.Quantity, viewModel.MinimumQuantityThreshold);

            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(Index));
        }

    }



}
