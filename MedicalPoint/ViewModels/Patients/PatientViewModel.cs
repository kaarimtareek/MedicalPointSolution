using MedicalPoint.Data;
using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Patients
{
    public class PatientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MilitaryNumber { get; set; }
        public string NationalNumber { get; set; }
        public int DegreeId { get; set; }
        public string? SaryaNumber { get; set; }
        public string? GeneralNumber { get; set; }
        public string? Major { get; set; }
        public string Degree { get; set; }
        public string RegisteredUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime? LastVisitAt { get; set; }
    }
}
