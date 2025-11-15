using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Teacher : Entity<Guid>
{
    public ETeacherRole Role { get; set; }

    public Guid   UserId   { get; set; }
    public Guid   CourseId { get; set; }
    public Course Course   { get; set; } = default!;
}