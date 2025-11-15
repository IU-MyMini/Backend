using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class CourseParticipant : Entity<Guid>
{
    public Guid                        UserId   { get; set; }
    public Guid                        CourseId { get; set; }
    public Course                      Course   { get; set; } = default!;
    public Guid                        GroupId  { get; set; }
    public Group                       Group    { get; set; } = default!;
    public ICollection<ComponentGrade> Grades   { get; set; } = default!;
}