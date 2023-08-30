using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalPoint.Data
{
    public class UnderObservationBed
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? VisitId { get; set; }
        public int DepartmentId { get;set; }
        public int BedNumber { get; set; }
        //The doctor id that put that patient in that bed
        public int? DoctorId { get; set; }
        [MaxLength(300)]
        public string Notes { get; set; }
        public DateTime? EnterDate { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public bool IsAvailable => PatientId == null;
        [NotMapped]
        public bool CanAddPatient => IsAvailable && IsActive;
        public bool CanRemovePatient => !IsAvailable && IsActive;
        public UnderObservationDepartment Department { get; set; }
        public Patient Patient { get; set; }
        public ICollection<UnderObservationBedHistory> History { get; set; }
    }
}
