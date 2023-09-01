using System.ComponentModel.DataAnnotations;

namespace MedicalPoint.ViewModels.Users
{
    public class EditUserViewModelcs
    {
        public int Id { get; set; }
        [Required]

        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string MilitaryNumber { get; set; }
        [Required]
        public int DegreeId { get; set; }
        public string AccountType { get; set; }
    }
}
