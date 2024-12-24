using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.ViewModels
{
    public class CreateDoctorViewModel
    {
        [Required]
    public string FullName { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } // Ensure this property exists

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string Specialty  { get; set; }   
     [Required]
         public string Surname { get; set; }  
           [Required]
           public string LicenseNumber { get; set; }
     }
     
     
}
