using MedicalPoint.ViewModels.Clinics;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Patients;

namespace MedicalPoint.ViewModels.Visits
{
    public class VisitRestViewModel
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public int RestTypeId { get; set; }
        public string RestType { get; set; }
        public int? ClinicId { get; set; }
        public ClinicViewModel Clinic { get; set; }
        public int PatientId { get; set; }
        public PatientViewModel Patient { get; set; }
        //TODO: add patient view model for more info about the patient
        public int? DoctorId { get; set; }
        public DoctorViewModel Doctor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RestDaysNumber { get; set; } 
      
        //Rest Type => Normal, Emergency
        public string Type { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public string VisitNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<VisitMedicineViewModel> Medicines { get; set; } 
        public bool IsMedicinesGiven { get; set; }
        public DateTime? MedicineGivenTime { get; set; }
    }
  
}
