using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class VisitHistory
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int VisitId { get; set; }
        public int? ClinicId { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        [MaxLength(100)]
        public string Status { get; set; }
        //Visit Type => Normal, Emergency
        [MaxLength(100)]
        public string Type { get; set; }
        public bool? HasFollowingVisit { get; set; }
        [MaxLength(1000)]
        public string Diagnosis { get; set; }
        [MaxLength(1000)]
        public string Notes { get; set; }
        [MaxLength(50)]
        public string? VisitNumber { get; set; }
        public int? PreviousVisitId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? VisitTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public bool? IsMedicinesGiven { get; set; }
        public DateTime? MedicineGivenTime { get; set; }
        public DateTime? FollowingVisitDate { get; set; }
        [NotMapped]
        public bool IsFollowingVisit => PreviousVisitId != null;
        public bool IsDeleted { get; set; }

    }
}
