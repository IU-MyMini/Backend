using BuildingBlocks.Domain;

namespace GradingModule.Application.Dtos;

public class CourseParticipantWithGradesDto
{
    public string Email { get; set; } = default!;
    public LangStr FirstName { get; set; } = default!;
    public LangStr SecondName { get; set; } = default!;
    public LangStr Patronymic { get; set; } = default!;
    public string? TelegramAlias { get; set; }

    public Dictionary<Guid, GradeDto> Grades { get; set; } = default!; // mapping from component id to grade
}