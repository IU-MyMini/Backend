namespace GradingModule.Application.Dtos;

public class PeerReviewStudentDto
{
    public GroupShortDto Group { get; set; } = default!;
    public SubmissionDto? Submission { get; set; }
}