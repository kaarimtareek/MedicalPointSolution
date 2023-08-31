using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Departments
{
    public class BedsViewModel
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? VisitId { get; set; }
        public int DepartmentId { get; set; }
        public int BedNumber { get; set; }
        //The doctor id that put that patient in that bed
        public int? DoctorId { get; set; }
        public string Notes { get; set; }
        public DateTime? EnterDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable => PatientId == null;
        public bool CanAddPatient => IsAvailable && IsActive;
        public bool CanRemovePatient => !IsAvailable && IsActive;
    }
}
