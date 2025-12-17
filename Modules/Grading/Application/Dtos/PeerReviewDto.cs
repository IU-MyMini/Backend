namespace GradingModule.Application.Dtos;

public class PeerReviewDto
{
    public Guid SourceComponentId { get; set; }
    public Guid TargetComponentId { get; set; }
    public Guid SourceGroupId { get; set; }
    public Guid TargetGroupId { get; set; }
}