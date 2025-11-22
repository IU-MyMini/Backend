using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Assignment : Entity<Guid>
{
    public LangStr Name { get; set; } = default!;
    public LangStr Description { get; set; } = default!;
    public IList<Guid> FileIds { get; set; } = default!;

    public DateTime? Deadline { get; set; }
    public bool IsGroupAssignment { get; set; }

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = default!;
    public ICollection<AssignmentComponent> Components { get; set; } = default!;
}