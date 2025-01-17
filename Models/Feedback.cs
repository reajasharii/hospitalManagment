using System;

namespace HospitalManagement.Models
{
   public class Feedback
{
    public int Id { get; set; } 
    public string DoctorId { get; set; }  
    public Doctor Doctor { get; set; }  

    public string PatientId { get; set; }  
    public Patient Patient { get; set; }  

    public string FeedbackText { get; set; }  
    public DateTime SubmittedAt { get; set; }  

    
    public string DoctorFullName { get; set; }
    public string PatientFullName { get; set; }
}

}
