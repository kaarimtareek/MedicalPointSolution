using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MedicalPoint.Constants;

namespace MedicalPoint.Data
{
    public class Medicine
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OldestExpirationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public string Status => Quantity == 0 ? ConstantMedicineStatus.NOT_AVAILABLE : MinimumQuantityThreshold.HasValue && MinimumQuantityThreshold.Value >= Quantity? ConstantMedicineStatus.NEAR_FINISH : ConstantMedicineStatus.AVAILABLE;
        public ICollection<MedicineBatch> Batches { get; set; }
        public ICollection<MedicineHistory> History { get; set; }
    }
}
