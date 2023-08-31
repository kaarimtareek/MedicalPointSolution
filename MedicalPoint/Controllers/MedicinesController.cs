using MedicalPoint.Common;
using MedicalPoint.Data;
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



        public async Task<IActionResult> AddQuantity([FromForm] AddMedicineQuantityViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var result = await _mediicinesService.AddQauntity(userId.Value, viewModel.MedicineId,  viewModel.Quantity);
            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(Index), "Medicines");
        }








        public async Task<IActionResult> Details(int id)
        {

            var medicine = await _mediicinesService.Get(id);

            //get medicine id 
            ViewBag.Id = medicine.Id;

            if (medicine == null)
            {
                return NotFound();
            }
            //Creating the View model
            var viewModel = new GetAllMediciensViewModel
            {

                Name = medicine.Name,
                Id = id,
                CreatedAt = medicine.CreatedAt,
                LastUpdatedAt = medicine.LastUpdatedAt,
            Quantity = medicine.Quantity,
               MinimumQuantityThreshold = medicine.MinimumQuantityThreshold,
              
            };
            return View(viewModel);
        }





        // delete Element Medicines 
        public async Task<IActionResult> Delete(int id)
        {

            var medicine = await _mediicinesService.Get(id);
            var userId = HttpContext.GetUserId();
            if(userId == null)
            {
                return NotFound();
            }
            if (medicine == null)
            {
                return NotFound();
            }
            await _mediicinesService.Delete(userId.Value, id);
           
            return RedirectToAction("Index","Medicines");
        }



        // edit medicine with Super Admin

        public async Task<IActionResult> Edit(int id)

        {
            var medicines = await _mediicinesService.Get(id);
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
            var result = await _mediicinesService.Edit(userId.Value, viewModel.Id,viewModel.Name, viewModel.Quantity, viewModel.MinimumQuantityThreshold);

            if (!result.Success)
            {
                return View();
            }
            return RedirectToAction(nameof(Index));
        }











    }



}
