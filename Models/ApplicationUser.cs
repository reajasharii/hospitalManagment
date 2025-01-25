using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
       
         [Required]
        public string FullName { get; set; }
         [Required]
        public string Surname { get; set; }
         [Required]
        public string MedicalHistory { get; set; } = "No medical history available";
    }
}
