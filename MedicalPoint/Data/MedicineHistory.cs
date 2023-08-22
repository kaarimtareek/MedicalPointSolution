﻿using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class MedicineHistory
    {
        public int Id { get; set; }
        public int MedicineId { get; set;}
        public int UserId { get; set; }
        [MaxLength(100)]
        public string? MedicineName { get; set; }
        public int? MedicineQuantity { get; set;}
        public int? MinimumQuantityThreshold { get; set; }
        [MaxLength(100)]
        public string ActionType { get; set; }
        public MedicalPointUser User { get; set; }
        public Medicine Medicine { get; set; }
    }
}
