using BuildingBlocks.Domain;

using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Grades;

/// <summary>
/// If Grade is null, remove grade
/// </summary>
public record PutGradeCommand(
    Guid UserId,
    Guid ComponentId,
    Guid? GroupId,
    Guid? CourseParticipantId,
    int? Grade,
    LangStr Feedback,
    bool IsAdmin
) : IRequest;

public class PutGradeCommandHandler(GradingContext context) : IRequestHandler<PutGradeCommand>
{
    public async Task Handle(PutGradeCommand request, CancellationToken cancellationToken)
    {
        var component = await context.AssignmentComponents
                            .Include(assignmentComponent => assignmentComponent.Assignment)
                            .ThenInclude(assignment => assignment.Course)
                            .ThenInclude(course => course.Teachers)
                            .Include(assignmentComponent => assignmentComponent.Grades)
                            .SingleOrDefaultAsync(c => c.Id.Equals(request.ComponentId), cancellationToken)
                        ?? throw Errors.AssignmentComponent.NotFound;

        if (request.GroupId is null && request.CourseParticipantId is null)
            throw Errors.Grade.GroupAndParticipantIdsBothNull;

        if (request.GroupId is not null && request.CourseParticipantId is not null)
            throw Errors.Grade.GroupAndParticipantIdsBothNotNull;

        if (!request.IsAdmin && !component.Assignment.Course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        var grade = component.Grades.SingleOrDefault(
            g => g.CourseParticipantId.Equals(request.CourseParticipantId) && g.GroupId.Equals(request.GroupId)
        );

        if (grade is null)
        {
            if (request.Grade is null) // No need to do anything
                return;

            grade = new ComponentGrade
            {
                Id = Guid.NewGuid(),
                Grade = request.Grade.Value,
                Feedback = request.Feedback,
                GradedAt = DateTime.UtcNow,
                GradedBy = request.UserId,
                ComponentId = component.Id,
                CourseParticipantId = request.CourseParticipantId,
                GroupId = request.GroupId,
            };

            await context.AddAsync(grade, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return;
        }

        if (request.Grade is not null)
        {
            grade.Grade = request.Grade.Value;
            grade.Feedback = request.Feedback;
        }
        else
            context.Remove(grade);

        await context.SaveChangesAsync(cancellationToken);
    }
}