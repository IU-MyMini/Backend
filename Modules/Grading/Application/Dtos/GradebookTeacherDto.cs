namespace GradingModule.Application.Dtos;

public class GradebookTeacherDto
{
    public ICollection<GroupWithGradesDto> Groups { get; set; } = default!;
}