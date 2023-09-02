using MedicalPoint.ViewModels.Patients;

namespace MedicalPoint.ViewModels.Beds
{
    public class AddBedToPatientViewModel
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public List<PatientViewModel> Patients { get; set; }
    }
}
