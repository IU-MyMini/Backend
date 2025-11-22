using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class GroupWithMembersDto(Group g) : GroupShortDto(g)
{
    public ICollection<CourseParticipantDto> Members { get; set; } = default!;
}