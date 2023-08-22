using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace MedicalPoint.Data
{
    public class MedicalPointUser 
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string AccoutType { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(100)]
        public string FullName { get; set; }
        [MaxLength(500)]
        public string Password { get; set; }
        [MaxLength(50)]
        public string Degree { get; set; }
        [MaxLength(50)]
        public string MilitaryNumber { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        

    }
}
