using MedicalPoint.Data;
using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Patients
{
    public class AddPatientViewModel
    {
        [Required]
        public string Name { get; set; }
        public string MilitaryNumber { get; set; }
        public string NationalNumber { get; set; }
        public int DegreeId { get; set; }
        public string? SaryaNumber { get; set; }
        public string? GeneralNumber { get; set; }
        public string? Major { get; set; }

    }
}
