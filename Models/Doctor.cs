using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Doctor : IdentityUser
    {
       
        public string FullName { get; set; } // Doctor's full name

       
        public string Surname { get; set; } // Doctor's surname

        public DateTime DateOfBirth { get; set; } // Doctor's date of birth

    
        public string Specialty { get; set; } // Doctor's specialty (e.g., cardiology)

               public string LicenseNumber { get; set; } // Doctor's license number

         public List<PatientDoctor> PatientDoctors { get; set; } // This links to the PatientDoctor table
    //    public string ApplicationUserId { get; set; }
    // public ApplicationUser ApplicationUser { get; set; }
}
}
