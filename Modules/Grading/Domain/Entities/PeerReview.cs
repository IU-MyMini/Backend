using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class PeerReview : Entity<Guid>
{
    public int? AssignedGrade { get; set; }

    public Guid ComponentId { get; set; }
    public AssignmentComponent Component { get; set; } = default!;

    // Submission to be reviewed
    public Guid SubmissionId { get; set; }
    public Submission Submission { get; set; } = default!;

    // Reviewing group
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; } = default!;

    // Reviewing student
    public Guid? CourseParticipantId { get; set; }
    public CourseParticipant? CourseParticipant { get; set; } = default!;
}