using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class Hospital
    {
        public int Id { get; set; } // Primary Key

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Name of the hospital

        [Required]
        [StringLength(200)]
        public string Address { get; set; } // Hospital's address

        [Required]
        [StringLength(100)]
        public string City { get; set; } // City where the hospital is located

        [Required]
        [Phone]
        public string Phone { get; set; } // Phone number of the hospital

       
    }
}
