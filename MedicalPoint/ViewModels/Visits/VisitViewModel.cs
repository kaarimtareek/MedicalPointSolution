using System.ComponentModel.DataAnnotations.Schema;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Clinics;

namespace MedicalPoint.ViewModels.Visits
{
    public class VisitViewModel
    {
        public int Id { get; set; }
        public int? ClinicId { get; set; }
        public ClinicViewModel Clinic { get; set; }

        public int PatientId { get; set; }
        public PatientViewModel Patient { get; set; }
        //TODO: add patient view model for more info about the patient
        public int? DoctorId { get; set; }
        public DoctorViewModel Doctor { get; set; }
        //TODO: should be another dto  ( DoctorViewModel for example )
        public string Status { get; set; }
        //Visit Type => Normal, Emergency
        public string Type { get; set; }
        public bool HasFollowingVisit { get; set; }
        public string Diagnosis { get; set; }
        public string Notes { get; set; }
        public string VisitNumber { get; set; }
        public int? PreviousVisitId { get; set; }
        public VisitViewModel PreviousVisit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public List<VisitImageViewModel> Images { get; set; }
        public List<VisitMedicineViewModel> Medicines { get; set; }
        public DateTime? FollowingVisitDate { get; set; }
        public bool IsMedicinesGiven { get; set; }
        public DateTime? MedicineGivenTime { get; set; }
        public bool IsFollowingVisit => PreviousVisitId != null;
    }
    public class VisitImageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
    public class VisitMedicineViewModel
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int InventoryQuantity { get; set; }
        public int Quantity { get; set; }
    }
}
