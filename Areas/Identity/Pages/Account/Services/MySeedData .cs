using System.Threading.Tasks;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Services
{
    public class MySeedData
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MySeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            // Create roles if they do not exist
            string[] roleNames = { "Admin", "Doctor", "Patient" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    await _roleManager.CreateAsync(role);
                }
            }

            // Seed Admin User
            var adminUser = await _userManager.FindByEmailAsync("admin@example.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FullName = "Admin User", // Provide a value for FullName
                    Surname = "Admin",       // Provide a value for Surname
                    // Set other properties as needed
                };
                await _userManager.CreateAsync(adminUser, "AdminPassword123!");
            }

            // Add admin role
            if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Doctor User
            var doctorUser = await _userManager.FindByEmailAsync("doctor@example.com");
            if (doctorUser == null)
            {
                doctorUser = new ApplicationUser
                {
                    UserName = "doctor@example.com",
                    Email = "doctor@example.com",
                    FullName = "Dr. John Doe", // Ensure FullName is set
                    Surname = "Doe",          // Ensure Surname is set
                    // Set other properties as needed
                };
                await _userManager.CreateAsync(doctorUser, "DoctorPassword123!");
            }

            // Add doctor role
            if (!await _userManager.IsInRoleAsync(doctorUser, "Doctor"))
            {
                await _userManager.AddToRoleAsync(doctorUser, "Doctor");
            }

            // Seed Patient User
            var patientUser = await _userManager.FindByEmailAsync("patient@example.com");
            if (patientUser == null)
            {
                patientUser = new ApplicationUser
                {
                    UserName = "patient@example.com",
                    Email = "patient@example.com",
                    FullName = "Patient Name", // Ensure FullName is set
                    Surname = "Name",         // Ensure Surname is set
                    // Set other properties as needed
                };
                await _userManager.CreateAsync(patientUser, "PatientPassword123!");
            }

            // Add patient role
            if (!await _userManager.IsInRoleAsync(patientUser, "Patient"))
            {
                await _userManager.AddToRoleAsync(patientUser, "Patient");
            }
        }
    }
}
