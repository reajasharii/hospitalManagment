using System;

namespace HospitalManagement.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required DateTime AppointmentDate { get; set; }
        public string? Reason { get; set; } // Nullable Reason for the visit
        public required string Department { get; set; }
    }
}
