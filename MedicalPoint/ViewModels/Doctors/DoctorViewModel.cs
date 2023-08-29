using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Doctors
{
    public class DoctorViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Degree { get; set; }
        public string MilitaryNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
