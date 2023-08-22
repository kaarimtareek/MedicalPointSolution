using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class Clinic
    {
        public int Id { get;set; }
        [MaxLength(100)]
        public string Name { get;set; }
        public bool IsActive { get; set; }
    }
}
