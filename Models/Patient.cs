using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; 

namespace HospitalManagement.Models
{
    public class Patient : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } // Patient's full name

        [Required]
        [StringLength(100)]
        public string Surname { get; set; } // Patient's surname

        [Required]
        public DateTime DateOfBirth { get; set; } // Patient's date of birth

        [StringLength(500)]
        
        public string MedicalHistory { get; set; } // Patient's medical history (optional)
        public List<PatientDoctor> PatientDoctors { get; set; } // This links to the PatientDoctor table  
    }
}
