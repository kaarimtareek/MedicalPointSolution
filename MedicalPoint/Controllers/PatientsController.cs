﻿using MedicalPoint.Common;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Patients;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MedicalPoint.Controllers
{
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
        public async Task<IActionResult> Details(int id)
        {
            var patients = await _patientsService.Get(id);
            if (patients == null)
            {
                return NotFound();
            }
            return View(patients);
        }
    }
}
