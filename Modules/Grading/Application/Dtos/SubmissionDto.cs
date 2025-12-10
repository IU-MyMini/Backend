using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class SubmissionDto(Submission s)
{
    public Guid Id { get; set; } = s.Id;

    public DateTime SubmittedAt { get; set; } = s.SubmittedAt;
    public LangStr? Text { get; set; } = s.Text;

    public List<Guid> FileIds { get; set; } = s.FileIds;
}