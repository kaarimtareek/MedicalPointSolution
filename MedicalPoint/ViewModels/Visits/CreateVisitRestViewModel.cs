using MedicalPoint.ViewModels.Clinics;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Patients;

namespace MedicalPoint.ViewModels.Visits
{
    public class CreateVisitRestViewModel
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public int DegreeId { get; set; }
        public int RestTypeId { get; set; }
        public string RestType { get; set; }
        public int? ClinicId { get; set; }
        public int PatientId { get; set; }
        public PatientViewModel Patient { get; set; }
        public ClinicViewModel Clinic { get; set; }
        //TODO: add patient view model for more info about the patient
        public int? DoctorId { get; set; }
        public DoctorViewModel Doctor { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RestDaysNumber { get; set; } 
      
        //Rest Type => Normal, Emergency
       
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public string VisitNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<VisitMedicineViewModel> Medicines { get; set; } 
        public bool IsMedicinesGiven { get; set; }
        public DateTime? MedicineGivenTime { get; set; }
    }
    public class VisitRestTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
