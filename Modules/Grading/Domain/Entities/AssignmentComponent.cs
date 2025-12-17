using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class AssignmentComponent : Entity<Guid>
{
    public LangStr Name { get; set; } = default!;
    public LangStr Description { get; set; } = default!;
    public IList<Guid> FileIds { get; set; } = default!;
    public int MaxPoints { get; set; }

    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = default!;
    public ICollection<ComponentGrade> Grades { get; set; } = default!;
    public ICollection<Submission> Submissions { get; set; } = default!;
}