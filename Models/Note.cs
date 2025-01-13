using System;
using HospitalManagement.Models;

public class Note
{
    public int Id { get; set; }
    public string DoctorId { get; set; }
    public string PatientId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public Doctor Doctor { get; set; }
    public ApplicationUser Patient { get; set; }
}
