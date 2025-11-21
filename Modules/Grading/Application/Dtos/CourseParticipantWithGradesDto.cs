namespace GradingModule.Application.Dtos;

public class CourseParticipantWithGradesDto : CourseParticipantDto
{
    public Dictionary<Guid, GradeDto> Grades { get; set; } = default!; // mapping from component id to grade

    public GradeDto TotalGrade { get; set; } = default!;
}