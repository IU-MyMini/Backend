using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Course : Entity<Guid>
{
    public LangStr  Name           { get; set; } = default!;
    public int      CourseNumber   { get; set; }
    public int      Semester       { get; set; }
    public string   EducationLevel { get; set; } = default!;
    public DateTime StartsAt       { get; set; }
    public DateTime EndsAt         { get; set; }

    public ICollection<Assignment>        Assignments        { get; set; } = default!;
    public ICollection<Group>             Groups             { get; set; } = default!;
    public ICollection<CourseParticipant> CourseParticipants { get; set; } = default!;
    public ICollection<Teacher>           Teachers           { get; set; } = default!;
}