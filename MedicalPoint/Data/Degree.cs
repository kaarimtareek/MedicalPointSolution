using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class Degree
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
    }
}
