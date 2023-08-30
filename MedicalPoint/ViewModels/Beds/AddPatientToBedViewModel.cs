namespace MedicalPoint.ViewModels.Beds
{
    public class AddPatientToBedViewModel
    {
        public int PatientId { get; set; }
        public string Notes { get; set; }
        public int VisitId { get; set; }
        public DateTime? EnterDate { get; set; }
    }
}
