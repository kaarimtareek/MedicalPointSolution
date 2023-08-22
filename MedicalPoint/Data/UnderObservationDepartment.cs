using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class UnderObservationDepartment
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        public int BedsCount { get; set; }
        public int AvailableBedsCount { get; set; }
        public ICollection<UnderObservationBed> Beds { get; set; }

    }
}
