﻿using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class UnderObservationBedHistory
    {
        public int Id { get; set; }
        public int BedId { get; set; }
        public int PatientId { get; set; }
        public int? VisitId { get; set; }
        //The doctor id that took that action
        public int DoctorId { get; set; }
        [MaxLength(100)]
        public string ActionType { get; set; }

        [MaxLength(300)]
        public string Notes { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
