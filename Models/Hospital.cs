using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Hospital
    {
        [Key]
        public int Id { get; set; } // Primary Key

        [Required]
        public string Name { get; set; } = string.Empty; // Initialized to avoid null issues

        [Required]
        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}
