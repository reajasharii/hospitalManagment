using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>  // Change to ApplicationUser
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientDoctor> PatientDoctors { get; set; }
     

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many relationship between Patient and Doctor
            modelBuilder.Entity<PatientDoctor>()
                .HasKey(pd => new { pd.PatientId, pd.DoctorId });

            modelBuilder.Entity<PatientDoctor>()
                .HasOne(pd => pd.Patient)
                .WithMany(p => p.PatientDoctors)
                .HasForeignKey(pd => pd.PatientId);

            modelBuilder.Entity<PatientDoctor>()
                .HasOne(pd => pd.Doctor)
                .WithMany(d => d.PatientDoctors)
                .HasForeignKey(pd => pd.DoctorId);
        }
    }
}
