using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.CourseParticipants;

public record RemoveCourseParticipantCommand(Guid UserId, Guid CourseParticipantId, bool IsAdmin) : IRequest;

public class RemoveCourseParticipantCommandHandler(GradingContext context)
    : IRequestHandler<RemoveCourseParticipantCommand>
{
    public async Task Handle(RemoveCourseParticipantCommand request, CancellationToken cancellationToken)
    {
        var courseParticipant
            = await context.CourseParticipants.Include(p => p.Course)
                  .ThenInclude(c => c.Teachers)
                  .SingleOrDefaultAsync(p => p.Id.Equals(request.CourseParticipantId), cancellationToken)
              ?? throw Errors.CourseParticipant.NotFound;

        if (!request.IsAdmin
            && !courseParticipant.Course.Teachers.Any(
                t => t.UserId.Equals(request.UserId) && t.Role == ETeacherRole.PrimaryInstructor
            ))
            throw Errors.Course.NotAllowed;

        context.Remove(courseParticipant);

        await context.SaveChangesAsync(cancellationToken);
    }
}