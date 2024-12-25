
using System;
using System.ComponentModel.DataAnnotations;



namespace HospitalManagement.ViewModels
{
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

       

        public string MedicalHistory { get; set; }
    }
    }