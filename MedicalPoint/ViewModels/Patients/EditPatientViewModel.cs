using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Patients
{
    
        public class EditPatientViewModel
        {
            [Required]
             public int Id { get; set; }
            public string Name { get; set; }
            public string MilitaryNumber { get; set; }
            public string NationalNumber { get; set; }
            public int DegreeId { get; set; }
            public string? SaryaNumber { get; set; }
            public string? GeneralNumber { get; set; }
            public string? Major { get; set; }

        }
    }

