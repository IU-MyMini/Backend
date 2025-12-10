using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class GroupWithGradesDto(Group g) : GroupShortDto(g)
{
    public ICollection<CourseParticipantWithGradesDto> Members { get; set; } = default!;

    public Dictionary<Guid, GradeDto> Grades { get; set; } = default!; // mapping from component id to grade

    public GradeDto TotalGrade { get; set; } = default!;

    public Dictionary<Guid, SubmissionDto> Submissions { get; set; } = default!; // mapping from component id to submission
}