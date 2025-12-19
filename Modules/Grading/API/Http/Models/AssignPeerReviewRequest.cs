namespace GradingModule.API.Http.Models;

public class AssignPeerReviewRequest
{
    public Guid SourceComponentId { get; set; }
    public Guid TargetComponentId { get; set; }
    public Guid SourceGroupId { get; set; }
    public Guid TargetGroupId { get; set; }
}