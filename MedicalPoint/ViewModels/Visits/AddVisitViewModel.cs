using MedicalPoint.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicalPoint.ViewModels.Patients;

namespace MedicalPoint.ViewModels.Visits
{
    public class AddVisitViewModel
    {
        public int? ClinicId { get; set; }
       
        public int PatientId { get; set; }
        public PatientViewModel Patient { get; set; }
        public int? DoctorId { get; set; }
        public string Type { get; set; }
        public DateTime? VisitTime { get; set; }
    }
}
