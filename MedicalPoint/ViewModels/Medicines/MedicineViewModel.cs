using System.ComponentModel.DataAnnotations;

using MedicalPoint.Data;

namespace MedicalPoint.ViewModels.Medicines
{
    public class MedicineViewModel
    {
        public int Id { get; set; }
        [MaxLength(100)]

        public string Name { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        public List<MedicineHistoryViewModel> History { get; set; }

    }
    public class MedicineHistoryViewModel
    {
        public int Id { get; set; }
        public int MedicineId { get; set; }
        public int UserId { get; set; }
        [MaxLength(100)]
        public string? MedicineName { get; set; }
        public int? MedicineQuantity { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        [MaxLength(100)]
        public string ActionType { get; set; }
        public string UserName { get; set; }
        public int? VisitId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
