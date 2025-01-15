using System;

namespace HospitalManagement.Models
{
    public class MedicalService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public TimeSpan Duration { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
