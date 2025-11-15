using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Group : Entity<Guid>
{
    public Guid                           CourseId    { get; set; }
    public Course                         Course      { get; set; } = default!;
    public ICollection<CourseParticipant> Members     { get; set; } = default!;
    public ICollection<ComponentGrade>    Grades      { get; set; } = default!;
    public ICollection<Submission>        Submissions { get; set; } = default!;
}