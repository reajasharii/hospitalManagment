using Microsoft.EntityFrameworkCore;
using HospitalManagement.Models; 

namespace HospitalManagement.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

  
        public DbSet<Hospital> Hospitals { get; set; }
    }
}
