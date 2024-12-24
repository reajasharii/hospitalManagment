


using System.ComponentModel.DataAnnotations;
namespace HospitalManagement.Models{


public class EditDoctorViewModel
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Surname { get; set; }
     public string Email { get; set; }
    public string LicenseNumber { get; set; }
public string Specialty { get; set; }   

    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }
}
}