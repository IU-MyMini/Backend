using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class StudentGradesDto
{
    public int Total { get; set; }

    public ICollection<AssignmentWithGradesDto> Assignments { get; set; } = default!;
}

public class AssignmentWithGradesDto(Assignment a)
{
    public Guid Id { get; set; } = a.Id;

    public LangStr Name { get; set; } = a.Name;
    public LangStr Description { get; set; } = a.Description;
    public IEnumerable<Guid> FileIds { get; set; } = a.FileIds;

    public DateTime? Deadline { get; set; } = a.Deadline;
    public bool IsGroupAssignment { get; set; } = a.IsGroupAssignment;

    public GradeDto TotalGrade { get; set; } = default!;

    public ICollection<ComponentWithGradeDto> Components { get; set; } = default!;
}

public class ComponentWithGradeDto(AssignmentComponent c) : ComponentShortDto(c)
{
    public GradeDto? GroupGrade { get; set; }
    public GradeDto IndividualGrade { get; set; } = default!;
}