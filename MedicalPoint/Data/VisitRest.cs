using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class VisitRest
    {
        public int Id { get; set; }
        [MaxLength(50)]
        //Visit rest => rest in bedroom, rest in playground,  etc...
        public string? Type { get; set; }
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
