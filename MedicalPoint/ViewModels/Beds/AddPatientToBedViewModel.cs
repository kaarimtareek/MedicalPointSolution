using MedicalPoint.ViewModels.Departments;
using MedicalPoint.ViewModels.Patients;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedicalPoint.ViewModels.Beds
{
    public class AddPatientToBedViewModel
    {
        public int PatientId { get; set; }
        public PatientViewModel Patient { get; set; }
        public List<DepartmentsViewModel> Departments { get; set; }
        public int SelectedDepartmentId { get; set; }
        public int SelectedBedId { get; set;}
        public string? Notes { get; set; }
        public DateTime? EnterDate { get; set; }

    }
}
