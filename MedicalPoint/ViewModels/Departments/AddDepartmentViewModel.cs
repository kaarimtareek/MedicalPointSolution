using Microsoft.Build.Framework;

namespace MedicalPoint.ViewModels.Departments
{
    public class AddDepartmentViewModel
    {
        [Required]
        public string Name { get; set; }
        public int BedsCount { get; set; }
    }
}
