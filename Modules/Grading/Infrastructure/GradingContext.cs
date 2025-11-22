using BuildingBlocks.Infrastructure.Configuration;

using GradingModule.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Infrastructure;

public class GradingContext(DbContextOptions options) : AppDbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<AssignmentComponent> AssignmentComponents { get; set; }
    public DbSet<ComponentGrade> ComponentGrades { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<CourseParticipant> CourseParticipants { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<PeerReview> PeerReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Course>()
                    .HasMany(c => c.Teachers)
                    .WithOne(t => t.Course);

        modelBuilder.Entity<Course>()
                    .HasMany(c => c.CourseParticipants)
                    .WithOne(p => p.Course);

        modelBuilder.Entity<Course>()
                    .HasMany(c => c.Assignments)
                    .WithOne(a => a.Course);

        modelBuilder.Entity<Course>()
                    .HasMany(c => c.Groups)
                    .WithOne(g => g.Course);

        modelBuilder.Entity<Assignment>()
                    .HasMany(a => a.Components)
                    .WithOne(c => c.Assignment);

        modelBuilder.Entity<AssignmentComponent>()
                    .HasMany(c => c.Grades)
                    .WithOne(g => g.Component);

        modelBuilder.Entity<AssignmentComponent>()
                    .HasMany(c => c.Submissions)
                    .WithOne(s => s.Component);

        modelBuilder.Entity<AssignmentComponent>()
                    .HasMany(c => c.PeerReviews)
                    .WithOne(r => r.Component);

        modelBuilder.Entity<Group>()
                    .HasMany(g => g.Members)
                    .WithOne(m => m.Group);

        modelBuilder.Entity<Group>()
                    .HasMany(g => g.Grades)
                    .WithOne(g => g.Group);

        modelBuilder.Entity<Group>()
                    .HasMany(g => g.Submissions)
                    .WithOne();

        modelBuilder.Entity<Group>()
                    .HasMany(g => g.AuthoredPeerReviews)
                    .WithOne(r => r.Group);

        modelBuilder.Entity<CourseParticipant>()
                    .HasMany(p => p.Grades)
                    .WithOne(g => g.CourseParticipant);

        modelBuilder.Entity<CourseParticipant>()
                    .HasMany(p => p.Submissions)
                    .WithOne();

        modelBuilder.Entity<CourseParticipant>()
                    .HasOne<User>()
                    .WithOne();

        modelBuilder.Entity<PeerReview>()
                    .HasOne(r => r.Submission)
                    .WithMany(s => s.ReceivedPeerReviews);

        modelBuilder.Entity<Teacher>()
                    .HasOne<User>()
                    .WithOne();
    }
}