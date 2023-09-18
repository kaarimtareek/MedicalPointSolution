using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalPoint.Data
{
    public class MedicineBatch
    {
        public int Id { get;set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get;set; }
        [MaxLength(200)]
        public string Notes { get; set; }
        public decimal Price { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsFinished { get; private set; }
        public DateTime CreatedAt { get; set; }
        public Medicine Medicine { get; set; }

    }
}
