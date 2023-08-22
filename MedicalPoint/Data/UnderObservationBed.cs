using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class UnderObservationBed
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? VisitId { get; set; }
        public int DepartmentId { get;set; }
        //The doctor id that put that patient in that bed
        public int? DoctorId { get; set; }
        [MaxLength(300)]
        public string Notes { get; set; }
        public DateTime? EnterDate { get; set; }
        public UnderObservationDepartment Department { get; set; }
    }
}
