using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class ComponentGrade : Entity<Guid>
{
    public int Grade { get; set; }
    public LangStr? Feedback { get; set; }
    public List<Guid> FileIds { get; set; } = default!;
    public DateTime GradedAt { get; set; }
    public Guid GradedBy { get; set; }

    public Guid ComponentId { get; set; }
    public AssignmentComponent Component { get; set; } = default!;

    public Guid? CourseParticipantId { get; set; }
    public CourseParticipant? CourseParticipant { get; set; }

    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
}