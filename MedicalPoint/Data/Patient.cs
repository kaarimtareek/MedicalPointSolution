using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class Patient
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string MilitaryNumber { get; set; }
        [MaxLength(100)]
        public string NationalNumber { get; set; }
        [Required]
        public int DegreeId { get; set; }
        [MaxLength(50)]
        public string? SaryaNumber { get; set; }
        [MaxLength(50)]
        public string? GeneralNumber { get; set; }
        [MaxLength(50)]
        public string? Major { get; set; }
        public Degree Degree { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set;}
        public DateTime? LastVisitAt { get; set; }
        public ICollection<Visit> Visits { get; set; }
    }
}
