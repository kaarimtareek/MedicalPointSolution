using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Beds
{
    public class EditBedViewModel
    {
        [Required]
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
