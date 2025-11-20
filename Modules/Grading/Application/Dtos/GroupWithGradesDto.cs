using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class GroupWithGradesDto(Group g) : GroupDtoBase(g)
{
    public ICollection<CourseParticipantWithGradesDto> Members { get; set; } = default!;

    public Dictionary<Guid, GradeDto> Grades { get; set; } = default!; // mapping from component id to grade
}