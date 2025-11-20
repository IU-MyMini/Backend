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

        // todo: config relationships
    }
}