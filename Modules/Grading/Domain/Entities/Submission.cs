using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Submission : Entity<Guid>
{
    public DateTime SubmittedAt { get; set; }
    public LangStr? Text { get; set; }

    public Guid ComponentId { get; set; }
    public AssignmentComponent Component { get; set; } = default!;
    public List<Guid> FileIds { get; set; } = default!;

    public Guid? SubmittedByUserId { get; set; }
    public Guid? SubmittedByGroupId { get; set; }

    public ICollection<PeerReview> ReceivedPeerReviews { get; set; } = default!;
}