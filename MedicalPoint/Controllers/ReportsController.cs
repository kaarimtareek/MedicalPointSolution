using MedicalPoint.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicalPoint.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }
        public async Task<IActionResult> Patient( int id)
        {

            var report = await _reportsService.GeneratePatientReport(id);

            return View(report);
        }
        public async Task<IActionResult> VisitsToday()
        {

            var report = await _reportsService.GenerateTodayStudentsVisitsReport(DateTime.Now);

            return View(report);
        }
        public async Task<IActionResult> MedicinesToday()
        {

            var report = await _reportsService.GenerateDailyMedicineReport(DateTime.Now);

            return View(report);
        }
    }
}
