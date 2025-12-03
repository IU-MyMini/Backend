using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Submission : Entity<Guid>
{
    public DateTime SubmittedAt { get; set; }
    public LangStr? Text { get; set; }

    public Guid ComponentId { get; set; }
    public AssignmentComponent Component { get; set; } = default!;
    public List<Guid> FileIds { get; set; } = default!;

    public Guid? SubmittedByParticipantId { get; set; }
    public CourseParticipant? SubmittedByParticipant { get; set; }
    public Guid? SubmittedByGroupId { get; set; }
    public Group? SubmittedByGroup { get; set; }

    public ICollection<PeerReview> ReceivedPeerReviews { get; set; } = default!;
}