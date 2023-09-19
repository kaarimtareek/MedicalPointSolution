using MedicalPoint.Common;
using MedicalPoint.Constants;
using MedicalPoint.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        
        public async Task<IActionResult> VisitsToday(DateTime fromDate, DateTime toDate, string patientType)
        {
            ViewBag.PatientTypes = new List<SelectListItem>()
            {
                new SelectListItem
                {
                    Value = "",
                     Text = "الطلبة فقط",
                     Selected = string.IsNullOrEmpty(patientType)
                },
                new SelectListItem
                {
                    Value = "1",
                    Text = "كل المرضى",
                    Selected = !string.IsNullOrEmpty(patientType)
                }
            };
            if(fromDate == DateTime.MinValue || fromDate == DateTime.MaxValue) 
            {
                SendErrorMessageToViewBagAndResetTempData();
                return View(null);
            }
            if(!DateTimeHelper.IsValidFromToDate(fromDate, toDate))
            {
                TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY] = ConstantMessageCodes.TO_DATE_MUST_BE_GREATER_FROM_DATE;
            }
            ViewBag.FromDate = fromDate.ToString(DateTimeHelper.DefaultFormat);
            ViewBag.ToDate = toDate.ToString(DateTimeHelper.DefaultFormat);
            ViewBag.SelectedPatientType = patientType;
            bool includeAll = !string.IsNullOrEmpty(patientType);
            var report = await _reportsService.GenerateVisitsReport(fromDate, toDate, includeAll );
            SendErrorMessageToViewBagAndResetTempData();
            return View(report);
        }
        public async Task<IActionResult> MedicinesToday()
        {

            var report = await _reportsService.GenerateDailyMedicineReport(DateTime.Now);

            return View(report);
        }
        public async Task<IActionResult> ExportedMedicines(DateTime? date)
        {
            if(date == null)
            {
                return View(null);
            }
            ViewBag.Date = date.Value.ToString(DateTimeHelper.DefaultFormat);
            var report = await _reportsService.GenerateDailyExportedVisitMedicines(date.Value);
            return View(report);
        }
        private void SendErrorMessageToViewBagAndResetTempData()
        {
            ViewBag.ErrorMessage = TempData[ConstantMessageCodes.ERROR_MESSAGE_KEY];
            TempData.Clear();
        }
    }
}
