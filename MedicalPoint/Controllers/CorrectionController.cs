using MedicalPoint.Common;
using MedicalPoint.Services;

using Microsoft.AspNetCore.Mvc;

namespace MedicalPoint.Controllers
{
    public class CorrectionController : Controller
    {
        private readonly ICorrectionService _correctionService;

        public CorrectionController(ICorrectionService correctionService)
        {
            _correctionService = correctionService;
        }
        public async Task<IActionResult> CorrectMedicineBatches()
        {
            var userId = this.HttpContext.GetUserId();
            if(userId == null)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            await _correctionService.CorrectMedicinesBatch(userId.Value);
            
            return RedirectToAction("Index","Home");
        }
    }
}
