using System.ComponentModel.DataAnnotations;

using MedicalPoint.Constants;

namespace MedicalPoint.ViewModels.Medicines
{
    public class GetAllMediciensViewModel
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Status { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        public DateTime OldestExpirationDate { get; set; }

        public string ExpirationStatus => ConstantMedicineExpirationStatus.GetAppropiateStatus(OldestExpirationDate);

        public decimal Price { get; set; }
    }
}