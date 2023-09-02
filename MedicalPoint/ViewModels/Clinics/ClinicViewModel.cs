namespace MedicalPoint.ViewModels.Clinics
{
    public class ClinicViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
