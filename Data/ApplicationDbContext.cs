using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;


namespace HospitalManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientDoctor> PatientDoctors { get; set; }
        public DbSet<Contact> Contact { get; set; }
        public DbSet<MedicalService> MedicalServices { get; set; }
        public DbSet<HospitalStaff> HospitalStaff { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
            public DbSet<AboutUsContent> AboutUsContent { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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


            // Optional: Add indices for FullName columns to optimize searching by name
            modelBuilder.Entity<PatientDoctor>()
                .HasIndex(pd => pd.PatientFullName);
            modelBuilder.Entity<PatientDoctor>()
                .HasIndex(pd => pd.DoctorFullName);
        }

    }


}
