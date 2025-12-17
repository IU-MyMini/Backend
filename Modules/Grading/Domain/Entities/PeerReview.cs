using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class PeerReview : Entity<Guid>
{
    public int? AssignedGrade { get; set; }

    public Guid SourceComponentId { get; set; }
    public AssignmentComponent SourceComponent { get; set; } = default!;

    public Guid TargetComponentId { get; set; }
    public AssignmentComponent TargetComponent { get; set; } = default!;

    public Guid SourceGroupId { get; set; }
    public Group SourceGroup { get; set; } = default!;

    public Guid TargetGroupId { get; set; }
    public Group TargetGroup { get; set; } = default!;
}