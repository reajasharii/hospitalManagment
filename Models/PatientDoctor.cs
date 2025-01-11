
using System.Collections.Generic; 
namespace HospitalManagement.Models

{
    public class PatientDoctor
    {
        public string PatientId { get; set; } // Foreign key to Patient (IdentityUser)
        public Patient Patient { get; set; }  // Navigation property
        
        public string DoctorId { get; set; }  // Foreign key to Doctor (IdentityUser)
        public Doctor Doctor { get; set; } 
        public string PatientFullName { get; set; } // To store patient's full name for convenience
    public string DoctorFullName { get; set; }   // Navigation property
    }
}
