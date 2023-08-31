using MedicalPoint.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Medicines
{
    public class GetAllMediciensViewModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        
        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public string Status => Quantity == 0 ? "غير متاح" : MinimumQuantityThreshold.HasValue && MinimumQuantityThreshold.Value >= Quantity ? "قارب على الانتهاء" : "متاح";
        public ICollection<MedicineHistory> History { get; set; }
    }
}
