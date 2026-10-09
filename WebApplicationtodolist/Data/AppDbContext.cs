namespace WebApplicationtodolist.Data
{
    
        using Microsoft.EntityFrameworkCore;
        using WebApplicationtodolist.Models;
        using Microsoft.Extensions.Configuration;
        using Microsoft.AspNetCore.Identity;
        using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

        public class AppDbContext : IdentityDbContext<IdentityUser>
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
            public DbSet<TodoItem> TodoItems { get; set; }
            public DbSet<CourseMaterial> CourseMaterials { get; set; }
            public DbSet<Assignment> Assignments { get; set; }
            public DbSet<Submission> Submissions { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                // Ensure one submission per student per assignment
                modelBuilder.Entity<Submission>()
                    .HasIndex(s => new { s.AssignmentId, s.StudentId })
                    .IsUnique();
            }
        }
    
}
