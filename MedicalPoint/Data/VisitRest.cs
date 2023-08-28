using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class VisitRest
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public int RestTypeId { get; set; }
        public LookupVisitRestType RestType { get; set; }
        public int VisitId { get; set; }
        public int PatientId { get;set; }
        public int DoctorId { get;set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RestDaysNumber { get; set; }
        [MaxLength(300)]
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
