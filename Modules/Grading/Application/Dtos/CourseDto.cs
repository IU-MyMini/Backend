using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class CourseDto(Course c, Guid userId)
{
    public Guid Id { get; set; } = c.Id;
    public LangStr Name { get; set; } = c.Name;
    public int CourseNumber { get; set; } = c.CourseNumber;
    public int Semester { get; set; } = c.Semester;
    public string EducationLevel { get; set; } = c.EducationLevel;
    public DateTime StartsAt { get; set; } = c.StartsAt;
    public DateTime EndsAt { get; set; } = c.EndsAt;

    public ETeacherRole? TeacherRole { get; set; } = c.Teachers.SingleOrDefault(t => t.UserId.Equals(userId))?.Role;
    public bool IsParticipant { get; set; } = c.CourseParticipants.Any(p => p.UserId.Equals(userId));
}