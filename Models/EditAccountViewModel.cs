using System;

namespace HospitalManagement.ViewModels
{
    public class EditAccountViewModel
    {
        public string FullName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

    public DateTime? DateOfBirth { get; set; } 
        public string? Specialty { get; set; }
        public string? LicenseNumber { get; set; }   
         public string? MedicalHistory { get; set; } 



    }
}