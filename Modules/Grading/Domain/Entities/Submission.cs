namespace GradingModule.Domain.Entities;

public class Submission
{
    public DateTime SubmittedAt { get; set; }

    public Guid                ComponentId { get; set; }
    public AssignmentComponent Component   { get; set; } = default!;
    public Guid                FileId      { get; set; }
}