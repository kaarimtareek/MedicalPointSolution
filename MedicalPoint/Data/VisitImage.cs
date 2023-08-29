using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.Data
{
    public class VisitImage
    {
        public int Id { get; set; }
        public int VisitId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Path { get; set; }

        [MaxLength(100)]
        public string Format { get;set; }
        public byte[] Content { get; set; }
        public bool IsDeleted { get; set; }
        public Visit Visit { get; set; }
    }
}
