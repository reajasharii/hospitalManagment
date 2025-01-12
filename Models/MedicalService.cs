using System;

namespace HospitalManagement.Models
{
    public class MedicalService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }  // Çmimi i shërbimit
        public TimeSpan Duration { get; set; }  // Kohëzgjatja e shërbimit
        public string Category { get; set; }  // Kategoria e shërbimit
        public DateTime CreatedDate { get; set; }  // Data e krijimit
    }
}
