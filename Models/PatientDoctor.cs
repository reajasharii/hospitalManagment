
using System.Collections.Generic; 
namespace HospitalManagement.Models

{
    public class PatientDoctor
    {
        public string PatientId { get; set; } 
        public Patient Patient { get; set; }  
        
        public string DoctorId { get; set; }  
        public Doctor Doctor { get; set; } 
        public string PatientFullName { get; set; } 
    public string DoctorFullName { get; set; }   
    }
}
