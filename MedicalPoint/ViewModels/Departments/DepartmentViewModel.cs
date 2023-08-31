using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Departments
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        public int BedsCount { get; set; }
        public int AvailableBedsCount { get; set; }
        public List<BedsViewModel> Beds { get; set; }
    }
}
