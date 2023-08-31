using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Departments
{
    public class BedHistoryViewModel
    {
        public int Id { get; set; }
        public int BedId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int? VisitId { get; set; }
        //The doctor id that took that action
        public int DoctorId { get; set; }
        public string DoctorName { get; set;}
        [MaxLength(100)]
        public string ActionType { get; set; }

        [MaxLength(300)]
        public string Notes { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? EnterDate { get; set; }
    }
}
