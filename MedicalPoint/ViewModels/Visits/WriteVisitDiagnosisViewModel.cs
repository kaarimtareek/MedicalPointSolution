using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Visits
{
    public class WriteVisitDiagnosisViewModel
    {
        [Required]
        public int VisitId { get; set; }
        [Required]
        public string Diagnosis { get; set; }
    }
}
