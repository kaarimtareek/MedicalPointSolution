using MedicalPoint.ViewModels.Clinics;
using MedicalPoint.ViewModels.Doctors;
using MedicalPoint.ViewModels.Patients;
using MedicalPoint.ViewModels.Visits;

namespace MedicalPoint.ViewModels.Medicines
{
    public class GiveVisitMedicinesViewModel
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
        public string Diagnosis { get; set; }
        //Visit Type => Normal, Emergency
        public string Type { get; set; }
        public string Notes { get; set; }
        public string VisitNumber { get; set; }
        public bool IsMedicinesGiven { get; set; }
        public DateTime? MedicineGivenTime { get; set; }
        public DateTime? VisitTime { get; set; }
        public List<VisitMedicineViewModel> Medicines { get; set; }

    }
}
