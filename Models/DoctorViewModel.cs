using HospitalManagement.Models;
namespace HospitalManagement.ViewModels
{
public class DoctorViewModel
{
    public Doctor Doctor { get; set; }
    public bool IsConnected { get; set; }
    public string PatientFullName { get; set; }
}
}