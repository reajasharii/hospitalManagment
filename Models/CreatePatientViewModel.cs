using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.ViewModels
{
    public class CreatePatientViewModel
    {
           [Required]
        public string FullName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        public string MedicalHistory { get; set; }
     public class EditPatientViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [NotMapped]
        public string MedicalHistory { get; set; }
    }
    }
    }
