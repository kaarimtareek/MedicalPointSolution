using MedicalPoint.Services;
using MedicalPoint.ViewModels.Visits;
using Microsoft.AspNetCore.Mvc;

namespace MedicalPoint.Controllers
{
    public class VisitsController : Controller
    {
        private readonly IVisitsService _iVisitsService;
        public VisitsController(IVisitsService visitsservice)
        {
            _iVisitsService = visitsservice;
        }
        public async Task<IActionResult> ListAllVisits()
        {
            var listVisits = await _iVisitsService.GetAll();
        var viewModel = listVisits.ConvertAll(x => new VisitsViewModel
        {
            ClinicId = x.ClinicId,
            DoctorId = x.DoctorId,
            Diagnosis = x.Diagnosis,
            Id = x.Id,
            PatientId = x.PatientId,
            Status = x.Status,
            Type = x.Type,
            VisitNumber = x.VisitNumber,
            PreviousVisitId = x.PreviousVisitId,
            VisitTime = x.VisitTime,
            ExitTime = x.ExitTime,
            FollowingVisitDate=x.FollowingVisitDate,
            IsDeleted=x.IsDeleted,
           
        });;
            return View(viewModel);
    }
}
}
