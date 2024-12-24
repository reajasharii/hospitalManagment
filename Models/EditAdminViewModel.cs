using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class EditAdminViewModel
{
    public string Id { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    public string Password { get; set; } // Not marked as [Required]

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
}
