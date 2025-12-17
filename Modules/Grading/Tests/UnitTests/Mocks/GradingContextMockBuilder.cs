using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Tests.UnitTests.Mocks;

public class GradingContextMockBuilder(string dbName = "Test.Grading")
{
    private readonly GradingContext _context = new GradingContextMock(
        new DbContextOptionsBuilder<GradingContext>().UseInMemoryDatabase(dbName).Options
    );

    public GradingContext Build()
    {
        _context.SaveChanges();
        return _context;
    }

    public GradingContextMockBuilder WithSomeGrades(
        out Guid userId,
        out Guid courseId,
        out ICollection<ComponentGrade> grades
    )
    {
        userId = Guid.NewGuid();
        courseId = Guid.NewGuid();
        var user = new User { Id = userId };

        var courseParticipantId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Name = [],
            CourseNumber = 1,
            Semester = 1,
            EducationLevel = "",
            StartsAt = default,
            EndsAt = default,
            Assignments =
            [
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    Name = [],
                    Description = [],
                    FileIds = [],
                    Deadline = null,
                    IsGroupAssignment = false,
                    Components =
                    [
                        new AssignmentComponent
                        {
                            Id = Guid.NewGuid(),
                            Name = [],
                            Description = [],
                            FileIds = [],
                            MaxPoints = 100,
                            Grades = [],
                            Submissions = [],
                        },
                        new AssignmentComponent
                        {
                            Id = Guid.NewGuid(),
                            Name = [],
                            Description = [],
                            FileIds = [],
                            MaxPoints = 100,
                            Grades = [],
                            Submissions = [],
                        },
                        new AssignmentComponent
                        {
                            Id = Guid.NewGuid(),
                            Name = [],
                            Description = [],
                            FileIds = [],
                            MaxPoints = 100,
                            Grades = [],
                            Submissions = [],
                        }
                    ]
                },
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    Name = [],
                    Description = [],
                    FileIds = [],
                    Deadline = null,
                    IsGroupAssignment = false,
                    Components =
                    [
                        new AssignmentComponent
                        {
                            Id = Guid.NewGuid(),
                            Name = [],
                            Description = [],
                            FileIds = [],
                            MaxPoints = 100,
                            Grades = [],
                            Submissions = [],
                        },
                        new AssignmentComponent
                        {
                            Id = Guid.NewGuid(),
                            Name = [],
                            Description = [],
                            FileIds = [],
                            MaxPoints = 100,
                            Grades = [],
                            Submissions = [],
                        }
                    ]
                }
            ],
            Groups = [],
            CourseParticipants =
            [
                new CourseParticipant
                {
                    Id = courseParticipantId,
                    UserId = userId,
                    Grades = [],
                    Submissions = []
                }
            ],
            Teachers = []
        };

        grades = course.Assignments.SelectMany(a => a.Components)
            .Select(
                c => new ComponentGrade
                {
                    Id = Guid.NewGuid(),
                    Grade = c.MaxPoints,
                    Feedback = null,
                    FileIds = [],
                    GradedAt = default,
                    GradedBy = default,
                    ComponentId = c.Id,
                    Component = c,
                    CourseParticipantId = courseParticipantId
                }
            )
            .ToList();

        course.CourseParticipants.First().Grades = grades;

        _context.Users.Add(user);
        _context.Courses.Add(course);
        _context.ComponentGrades.AddRange(grades);
        return this;
    }
}