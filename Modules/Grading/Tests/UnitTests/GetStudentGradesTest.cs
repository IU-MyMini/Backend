using GradingModule.Application.Commands.Grades;
using GradingModule.Tests.UnitTests.Mocks;

namespace GradingModule.Tests.UnitTests;

public class GetStudentGradesTest
{
    [Fact]
    public async Task StudentWithGradesExists_ReturnsGrades()
    {
        // Arrange
        var context = new GradingContextMockBuilder().WithSomeGrades(out var userId, out var courseId, out var grades)
            .Build();

        var query = new GetStudentGradesQuery(userId, courseId, false);

        var handler = new GetStudentGradesQueryHandler(context);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        foreach (var grade in grades)
        {
            Assert.Contains(
                result.Assignments.SelectMany(a => a.Components),
                c => grade.ComponentId.Equals(c.Id) && grade.Grade.Equals(c.IndividualGrade.AssignedGrade)
            );
        }

        foreach (var component in result.Assignments.SelectMany(a => a.Components))
        {
            Assert.Contains(
                grades,
                g => g.ComponentId.Equals(component.Id) && g.Grade.Equals(component.IndividualGrade.AssignedGrade)
            );
        }
    }
}