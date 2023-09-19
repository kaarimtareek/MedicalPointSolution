﻿using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Models;
using MedicalPoint.Services;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Medicines;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Visits;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalPoint.Controllers
{
    [Authorize(Roles = $"{ConstantUserType.Pharmacist},{ConstantUserType.SUPER_ADMIN}")]
    public class MedicinesController : Controller
    {
        private readonly IMedicinesService _medicinesService;
        private readonly IVisitsService _visitsService;
        private readonly IVisitMedicinesService _visitMedicinesService;

        public MedicinesController(IMedicinesService mediicinesService, IVisitsService visitsService, IVisitMedicinesService visitMedicinesService)
        {
            _medicinesService = mediicinesService;
            _visitsService = visitsService;
            _visitMedicinesService = visitMedicinesService;
        }

        public async Task<IActionResult> Index()
        {
            var medicines = await _medicinesService.GetAll();
            var viewModel = medicines.ConvertAll(x => new GetAllMediciensViewModel
            {
                CreatedAt = x.CreatedAt,
                Id = x.Id,
                Name = x.Name,
                LastUpdatedAt = x.LastUpdatedAt,
                Quantity = x.Quantity,
                Status = x.Status,
                MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                OldestExpirationDate = x.OldestExpirationDate,
                Price = x.Price,
            });
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }

        public async Task<IActionResult> VisitsGivenMedicines(DateTime date)
        {
            var result = await _visitsService.GetVisitsThatGivenMedicines(date);
            var viewModel = result.ConvertAll(x => new VisitsViewModel
            {
                //TODO: add view for this
                ClinicId = x.Id,
                Diagnosis = x.Diagnosis,
                DoctorId = x.DoctorId,
                ExitTime = x.ExitTime,
                FollowingVisitDate = x.FollowingVisitDate,
                Id = x.Id,
                IsDeleted = x.IsDeleted,
                PatientId = x.PatientId,
                PreviousVisitId = x.PreviousVisitId,
                Status = x.Status,
                Type = x.Type,
                VisitNumber = x.VisitNumber,
                VisitTime = x.VisitTime,
                IsMedicinesGiven = x.IsMedicinesGiven,
                ClinicName = x.Clinic?.Name ?? "",
                DoctorName = x.Doctor?.FullName ?? "",
                PatientName = x.Patient?.Name ?? "",
                PatientDegree = x.Patient?.Degree?.Name ?? "",
                PatientGeneralNumber = x.Patient?.GeneralNumber ?? "",
                PatientSaryaNumber = x.Patient?.SaryaNumber ?? ""
            });
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            SendErrorMessageToViewBagAndResetTempData();
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

            var result = await _medicinesService.Add(userId.Value, viewModel.Name, viewModel.Quantity, viewModel.ExpirationDate,  viewModel.MinimumQuantityThreshold, viewModel.Price );
            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
                return View();
            }
            return RedirectToAction(nameof(Index), "Medicines");
        }

        [HttpPost]
        public async Task<IActionResult> AddQuantity([FromForm] int MedicineId, [FromForm] int Quantity, [FromForm] DateTime expirationDate)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var result = await _medicinesService.AddBatch(userId.Value, MedicineId, Quantity, expirationDate);
            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            return RedirectToAction(nameof(Details), new {id = MedicineId});
        }

        public async Task<IActionResult> Details(int id)
        {
            var medicine = await _medicinesService.Get(id);
            if (medicine == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    ActionPath = nameof(Index),
                    ErrorMessage = ConstantMessageCodes.MedicineNotFound,
                    ControllerPath = nameof(MedicinesController),
                };
                return NotFound(errorViewModel);
            }
            //Creating the View model
            var viewModel = new MedicineViewModel
            {
                Name = medicine.Name,
                Id = id,
                CreatedAt = medicine.CreatedAt,
                LastUpdatedAt = medicine.LastUpdatedAt,
                Quantity = medicine.Quantity,
                Price = medicine.Price,
                OldestExpirationDate = medicine.OldestExpirationDate,
                
                MinimumQuantityThreshold = medicine.MinimumQuantityThreshold,
                History = medicine.History.Select(x => new MedicineHistoryViewModel
                {
                    ActionType = x.ActionType,
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    MedicineId = x.Id,
                    MedicineName = x.MedicineName,
                    MedicineQuantity = x.MedicineQuantity,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    UserId = x.UserId,
                    Price = x.Price,
                    UserName = x.User?.FullName ?? string.Empty,
                    VisitId = x.VisitId,
                }).ToList(),
                Batches = medicine.Batches.Select(x=> new MedicineBatchViewModel
                {
                    CreatedAt = x.CreatedAt,
                    ExpirationDate = x.ExpirationDate,
                    IsFinished = x.IsFinished,
                    MedicineId= x.MedicineId,
                    Id = x.Id,
                    Notes = x.Notes,
                    Quantity = x.Quantity,
                    UserId = x.UserId,

                }).ToList()
            };
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }

        public async Task<IActionResult> DetailsHistory(int id)
        {
            var medicine = await _medicinesService.Get(id, true);
            if (medicine == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    ActionPath = nameof(Index),
                    ErrorMessage = ConstantMessageCodes.MedicineNotFound,
                    ControllerPath = nameof(MedicinesController),
                };
                return NotFound(errorViewModel);
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
                OldestExpirationDate = medicine.OldestExpirationDate,
                Price = medicine.Price,
                History = medicine.History.Select(x => new MedicineHistoryViewModel
                {
                    ActionType = x.ActionType,
                    CreatedAt = x.CreatedAt,
                    Id = x.Id,
                    MedicineId = x.Id,
                    MedicineName = x.MedicineName,
                    MedicineQuantity = x.MedicineQuantity,
                    MinimumQuantityThreshold = x.MinimumQuantityThreshold,
                    UserId = x.UserId,
                    UserName = x.User?.FullName ?? string.Empty,
                    VisitId = x.VisitId,
                }).ToList(),
                Batches = medicine.Batches.Select(x => new MedicineBatchViewModel
                {
                    CreatedAt = x.CreatedAt,
                    ExpirationDate = x.ExpirationDate,
                    IsFinished = x.IsFinished,
                    MedicineId = x.MedicineId,
                    Id = x.Id,
                    Notes = x.Notes,
                    Quantity = x.Quantity,
                    UserId = x.UserId,

                }).ToList()
            };
            SendErrorMessageToViewBagAndResetTempData();
            return View(nameof(Details), viewModel);
        }

        // delete Element Medicines
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            var result = await _medicinesService.Delete(userId.Value, id);
            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
            }
            return RedirectToAction("Index", "Medicines");
        }

        // edit medicine with Super Admin

        public async Task<IActionResult> Edit(int id)

        {
            var medicines = await _medicinesService.Get(id);
            if (medicines == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    ActionPath = nameof(Index),
                    ErrorMessage = ConstantMessageCodes.MedicineNotFound,
                    ControllerPath = nameof(MedicinesController),
                };
                return NotFound(errorViewModel);
            }

            var viewModel = new GetAllMediciensViewModel
            {
                Name = medicines.Name,
                Id = id,
                CreatedAt = medicines.CreatedAt,
                LastUpdatedAt = medicines.LastUpdatedAt,
                Quantity = medicines.Quantity,
                Price = medicines.Price,
                MinimumQuantityThreshold = medicines.MinimumQuantityThreshold,
            };
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] GetAllMediciensViewModel viewModel)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _medicinesService.Edit(userId.Value, viewModel.Id, viewModel.Name,  viewModel.MinimumQuantityThreshold, viewModel.Price);

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
                return View();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> VisitsMedicines(string searchValue, DateTime? date, string givenMedicineType, int pageNumber = 1, int pageSize = 20)
        {
            var notGivenOnly = string.IsNullOrEmpty(givenMedicineType);
            date ??= DateTime.Today;
            ViewBag.SearchValue = searchValue;
            ViewBag.Date = date?.ToShortDateString();
            ViewBag.GivenMedicineType = givenMedicineType;
            ViewBag.GivenMedicinesTypes = new List<SelectListItem>()
            { 
                new SelectListItem
                {
                    Text = "لم يتم صرفها",
                    Value = "",
                    Selected = notGivenOnly,
                },
                new SelectListItem
                {
                    Text = "الكل",
                    Value = "1",
                    Selected = !notGivenOnly,
                },
            };
            ViewBag.PageSizeList = new List<int>() { 10, 20, 50, 100 }.Select(x => new SelectListItem
            {
                Text = x.ToString(),
                Value = x.ToString(),
                Selected = pageSize == x
            });

            var visits = await _visitsService.GetVisitsMedicines( searchValue, date, notGivenOnly, pageNumber, pageSize);

            var viewModel = visits.ConvertAll(x => new VisitsViewModel
            {
                ClinicId = x.Id,
                Diagnosis = x.Diagnosis,
                DoctorId = x.DoctorId,
                ExitTime = x.ExitTime,
                FollowingVisitDate = x.FollowingVisitDate,
                Id = x.Id,
                IsDeleted = x.IsDeleted,
                PatientId = x.PatientId,
                PreviousVisitId = x.PreviousVisitId,
                Status = x.Status,
                Type = x.Type,
                VisitNumber = x.VisitNumber,
                VisitTime = x.VisitTime,
                IsMedicinesGiven = x.IsMedicinesGiven,
                ClinicName = x.Clinic?.Name ?? "",
                DoctorName = x.Doctor?.FullName ?? "",
                PatientName = x.Patient?.Name ?? "",
                PatientDegree = x.Patient?.Degree?.Name ?? "",
                PatientGeneralNumber = x.Patient?.GeneralNumber ?? "",
                PatientSaryaNumber = x.Patient?.SaryaNumber ?? "",
            });
            var paginatedViewModel = PaginatedList<VisitsViewModel>.Create(viewModel, visits.PageNumber, visits.PageSize, visits.TotalPages);
            return View(paginatedViewModel);
        }

        public async Task<IActionResult> GiveMedicines(int id, CancellationToken cancellationToken)

        {
            var visit = await _visitsService.Get(id);
            if (visit == null)
            {
                var errorViewModel = new ErrorViewModel
                {
                    ActionPath = nameof(Index),
                    ErrorMessage = ConstantMessageCodes.VisitNotFound,
                    ControllerPath = nameof(MedicinesController),
                };
                return NotFound(errorViewModel);
            }
            var visitMedicines = await _visitMedicinesService.GetMedicinesForVisit(id);

            var viewModel = new GiveVisitMedicinesViewModel
            {
                ClinicId = visit.ClinicId,
                Clinic = visit.Clinic == null ? null : new ViewModels.Clinics.ClinicViewModel
                {
                    Name = visit.Clinic.Name,
                    Id = visit.Clinic.Id,
                    IsActive = visit.Clinic.IsActive,
                },
                Diagnosis = visit.Diagnosis,
                Id = visit.Id,

                IsMedicinesGiven = visit.IsMedicinesGiven,
                MedicineGivenTime = visit.MedicineGivenTime,
                DoctorId = visit.DoctorId,
                PatientId = visit.PatientId,

                Notes = visit.Notes,
                VisitTime = visit.VisitTime,
                Type = visit.Type,
                Status = visit.Status,

                VisitNumber = visit.VisitNumber,
                Doctor = visit.Doctor == null ? null : new DoctorViewModel
                {
                    Id = visit.DoctorId.Value,
                    FullName = visit.Doctor.FullName,
                    IsActive = visit.Doctor.IsActive,
                },
                Patient = visit.Patient == null ? null : new PatientViewModel
                {
                    CreatedAt = visit.Patient.CreatedAt,
                    DegreeId = visit.Patient.DegreeId,
                    GeneralNumber = visit.Patient.GeneralNumber,
                    Id = visit.Patient.Id,
                    LastUpdatedAt = visit.Patient.LastUpdatedAt,
                    Name = visit.Patient.Name,
                    SaryaNumber = visit.Patient.SaryaNumber,
                    MilitaryNumber = visit.Patient.MilitaryNumber,
                    Major = visit.Patient.Major,
                    NationalNumber = visit.Patient.NationalNumber,
                    LastVisitAt = visit.Patient.LastVisitAt,
                    Degree = visit.Patient.Degree?.Name ?? "",
                },

                Medicines = visit.Medicines == null ? null : visit.Medicines.Select(x => new VisitMedicineViewModel
                {
                    Id = x.Id,
                    InventoryQuantity = x.Medicine.Quantity,
                    Quantity = x.Quantity,
                    MedicineId = x.Medicine.Id,
                    MedicineName = x.Medicine.Name,
                }).ToList()
            };
            SendErrorMessageToViewBagAndResetTempData();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> GiveVisitMedicines(int id)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _visitMedicinesService.GiveMedicines(id, userId.Value);

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
                RedirectToAction(nameof(GiveMedicines), new { id });
            }
            return RedirectToAction(nameof(VisitsMedicines));
        }
        
        public async Task<IActionResult> RemoveBatch(int id)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _medicinesService.DeleteBatch(userId.Value, id);

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Details), new {result.Data.Id});
        }
        [HttpPost]
        public async Task<IActionResult> EditBatch(int id, [FromForm] DateTime expirationDate, [FromForm] int quantity)
        {
            var userId = HttpContext.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var result = await _medicinesService.EditBatch(userId.Value, id, expirationDate, quantity);

            if (!result.Success)
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Details), new {result.Data.Id});
        }

        private void SendErrorMessageToViewBagAndResetTempData()
        {
            ViewBag.ErrorMessage = TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY];
            TempData.Clear();
        }
    }
}