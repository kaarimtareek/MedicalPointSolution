namespace MedicalPoint.ViewModels.Patients
{
    public class AddPatientViewModel
    {
        public DateTime? VisitTime { get; set; }
        public int PatientId { get; set;}
        public int? ClinicId { get; set; }
        public int? Degree { get; set; }
        public string? VisitType { get;set; }
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }

    }
}
