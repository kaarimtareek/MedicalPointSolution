using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class VisitMedicine
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        [MaxLength(200)]
        public string Notes { get; set; }
        public Visit Visit { get; set; }
        public Medicine Medicine { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
