using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalPoint.Data
{
    public class Visit
    {
        public int Id { get; set; }
        public int? ClinicId { get; set; }
        public Clinic Clinic { get;set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int? DoctorId { get; set; }
        public MedicalPointUser Doctor { get;set; }
        [MaxLength(100)]
        public string Status { get; set;}
        //Visit Type => Normal, Emergency
        [MaxLength(100)]
        public string Type { get; set; }
        public bool HasFollowingVisit { get; set; }
        [MaxLength(1000)]
        public string Diagnosis { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
        [MaxLength(50)]
        public string VisitNumber { get; set; }
        public int? PreviousVisitId { get; set; }
        public Visit PreviousVisit { get; set; }
        [MaxLength(50)]
        public DateTime CreatedAt { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public DateTime? FollowingVisitDate { get; set; }
        [NotMapped]
        public  bool IsFollowingVisit => PreviousVisitId != null;
        public bool IsDeleted { get; set; }
        public ICollection<Visit> FollowingVisits { get; set; }
        public ICollection<VisitMedicine> Medicines { get; set; }
        public ICollection<VisitImage> Images { get; set; }

    }
}
