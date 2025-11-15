using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Assignment : Entity<Guid>
{
    public LangStr           Name { get; set; } = default!;
    public LangStr           Description { get; set; } = default!;
    public IEnumerable<Guid> FileIds     { get; set; } = default!;

    public Guid                             CourseId   { get; set; }
    public Course                           Course     { get; set; } = default!;
    public ICollection<AssignmentComponent> Components { get; set; } = default!;
}