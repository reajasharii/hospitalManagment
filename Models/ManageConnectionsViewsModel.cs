using System.Collections.Generic;
using HospitalManagement.Models;

namespace HospitalManagement.ViewModels
{
    public class ManageConnectionsViewModel
    {
        public List<ConnectionViewModel> Connections { get; set; }
         public List<Doctor> AvailableDoctors { get; set; }
        public List<ApplicationUser> CurrentPatients { get; set; }   // List of doctors to add a connection
    }
}
