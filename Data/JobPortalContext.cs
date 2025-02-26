using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobPortalAPI.Models
{
    public class JobPortalContext : IdentityDbContext<User>
    {
        public JobPortalContext(DbContextOptions<JobPortalContext> options) : base(options) { }

        
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
