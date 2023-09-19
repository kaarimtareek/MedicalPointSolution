using MedicalPoint.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Medicines
{
    public class AddMedicinesViewModel
    {
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }
        [Required]
        public int Quantity { get; set; }
        public int? MinimumQuantityThreshold { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal Price { get; set; }
    }
}
