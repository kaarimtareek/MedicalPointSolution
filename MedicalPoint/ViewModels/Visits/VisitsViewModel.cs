using MedicalPoint.Constants;
using MedicalPoint.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MedicalPoint.ViewModels.Patients;

namespace MedicalPoint.ViewModels.Visits
{
    public class VisitsViewModel
    {
        public int Id { get; set; }
        public int? ClinicId { get; set; }
        public string ClinicName { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string PatientGeneralNumber { get; set; }
        public string PatientSaryaNumber { get; set; }
        public string PatientDegree { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Status { get; set; }
        //Visit Type => Normal, Emergency
        public string Type { get; set; }
        public string Diagnosis { get; set; }
        public string VisitNumber { get; set; }
        public int? PreviousVisitId { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public DateTime? FollowingVisitDate { get; set; }
        public bool IsFollowingVisit => PreviousVisitId != null;
        public bool IsDeleted { get; set; }
        public bool IsMedicinesGiven { get; set; }

    }
    
}
