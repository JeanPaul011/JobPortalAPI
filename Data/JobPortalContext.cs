using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobPortalAPI.Models
{
    public class JobPortalContext : IdentityDbContext<User>
    {
        public JobPortalContext(DbContextOptions<JobPortalContext> options) : base(options) { }

        // Tables
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure Identity relationships are properly configured
            modelBuilder.Entity<User>()
                .HasMany(u => u.Applications)
                .WithOne(ja => ja.JobSeeker)  // ✅ Use 'JobSeeker' (not 'User')
                .HasForeignKey(ja => ja.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<User>()
                .HasMany(u => u.PostedJobs)
                .WithOne(j => j.Recruiter)
                .HasForeignKey(j => j.RecruiterId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent accidental deletion of jobs

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enable Cascade Delete for Jobs when a Company is deleted
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Enable Cascade Delete for Reviews when a Company is deleted
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Company)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            // If users are linked as recruiters to companies, configure Many-to-Many relationship properly
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Recruiters)
                .WithMany(u => u.Companies)
                .UsingEntity(j => j.ToTable("CompanyRecruiters"));  // Creates a junction table
        }
    }
}
