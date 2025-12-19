using BuildingBlocks.Domain;

namespace GradingModule.Application.Dtos;

public class PeerReviewStudentDto
{
    public LangStr TargetComponentName { get; set; } = default!;
    public GroupShortDto Group { get; set; } = default!;
    public SubmissionDto? Submission { get; set; }
}