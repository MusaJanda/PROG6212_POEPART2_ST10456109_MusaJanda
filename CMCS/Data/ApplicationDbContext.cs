using CMCS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //DbSet properties for new models
        public DbSet<Lecturer> Lecturer { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ProgrammeCoordinator> ProgrammeCoordinator { get; set; }
        public DbSet<AcademicManager> AcademicManager { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Lecturer>()
                .Property(l => l.HourlyRate)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Claim>()
                .Property(c => c.HourlyRate)
                .HasColumnType("decimal(18, 2)");
        }
    }
}
