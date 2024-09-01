using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolProject1640.Models;
using System.Data;

namespace SchoolProject1640.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<Contribution> Contribution { get; set; }
        public DbSet<FileEntry> Files { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Article> Article { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<TermAndCon> TermAndCon { get; set; }
        // public DbSet<Submission> Submission { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = "1", Name = "Guest", Descriptions = "Guest role description", ConcurrencyStamp = "1", NormalizedName = "GUEST" },
                new ApplicationRole { Id = "2", Name = "Student", Descriptions = "Student role description", ConcurrencyStamp = "2", NormalizedName = "STUDENT" },
                new ApplicationRole { Id = "3", Name = "Manager", Descriptions = "Manager role description", ConcurrencyStamp = "3", NormalizedName = "MANAGER" },
                new ApplicationRole { Id = "4", Name = "Administrator", Descriptions = "Admin role description", ConcurrencyStamp = "4", NormalizedName = "ADMINISTRATOR" },
                new ApplicationRole { Id = "5", Name = "Coordinator", Descriptions = "Coordinator role description", ConcurrencyStamp = "5", NormalizedName = "COORDINATOR" }
            );

            modelBuilder.Entity<Faculty>().HasData(
                 new Faculty { Id = "1", Name = "Arts", Description = "Faculty of Arts", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                 new Faculty { Id = "2", Name = "Business", Description = "Faculty of Business", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                 new Faculty { Id = "3", Name = "Engineering", Description = "Faculty of Engineering", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                 new Faculty { Id = "4", Name = "Information Technology", Description = "Faculty of Information Technology", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                 new Faculty { Id = "5", Name = "Law", Description = "Faculty of Law", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                 new Faculty { Id = "6", Name = "Medicine", Description = "Faculty of Medicine", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now },
                 new Faculty { Id = "7", Name = "Admin", Description = "Faculty of Admin", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }, // Added Admin faculty
                 new Faculty { Id = "8", Name = "Manager", Description = "Faculty of Manager", CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now } // Added Manager faculty
             );
        }

    }
}
