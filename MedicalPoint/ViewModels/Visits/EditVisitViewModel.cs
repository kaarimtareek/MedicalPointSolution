namespace MedicalPoint.ViewModels.Visits
{
    public class EditVisitViewModel
    {
        public int? ClinicId { get; set; }
        public int? DoctorId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public bool HasFollowingVisit { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public string VisitNumber { get; set; }
        public int? PreviousVisitId { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public DateTime? FollowingVisitDate { get; set; }
    }
}

