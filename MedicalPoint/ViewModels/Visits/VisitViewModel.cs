using MedicalPoint.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Visits
{
    public class VisitViewModel
    {
        public int Id { get; set; }
        public int? ClinicId { get; set; }
        public string ClinicName { get; set; }
        public int PatientId { get; set; }
        //TODO: add patient view model for more info about the patient
        public string PatientName { get; set; }
        public int? DoctorId { get; set; }
        //TODO: should be another dto  ( DoctorViewModel for example )
        public MedicalPointUser Doctor { get; set; }
        public string Status { get; set; }
        //Visit Type => Normal, Emergency
        public string Type { get; set; }
        public bool HasFollowingVisit { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public string VisitNumber { get; set; }
        public int? PreviousVisitId { get; set; }
        //TODO: should be another dto  ( DoctorViewModel for example )
        public Visit PreviousVisit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public DateTime? FollowingVisitDate { get; set; }
        [NotMapped]
        public bool IsFollowingVisit => PreviousVisitId != null;
    }
}
