using BuildingBlocks.Domain;

namespace GradingModule.Application.Dtos;

public class CourseParticipantDto
{
    public Guid Id { get; set; }

    public string Email { get; set; } = default!;
    public LangStr FirstName { get; set; } = default!;
    public LangStr SecondName { get; set; } = default!;
    public LangStr Patronymic { get; set; } = default!;
    public string? TelegramAlias { get; set; }
}