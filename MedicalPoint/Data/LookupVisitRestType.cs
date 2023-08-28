using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class LookupVisitRestType
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public  string Name { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
