using GradingModule.Application.Commands.Users;
using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.CourseParticipants;

public record AddCourseParticipantCommand(
    Guid RequestingUserId,
    Guid TargetUserId,
    Guid CourseId,
    bool IsAdmin
) : IRequest<Guid>;

public class AddCourseParticipantCommandHandler(GradingContext context, IMediator mediator)
    : IRequestHandler<AddCourseParticipantCommand, Guid>
{
    public async Task<Guid> Handle(AddCourseParticipantCommand request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.Include(c => c.Teachers)
                         .Include(c => c.CourseParticipants)
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin && !course.Teachers.Any(t => t.UserId.Equals(request.RequestingUserId)))
            throw Errors.Course.NotAllowed;

        if (course.CourseParticipants.Any(p => p.UserId.Equals(request.TargetUserId)))
            throw Errors.CourseParticipant.AlreadyAdded;

        await mediator.Send(new MustFindUserCommand(request.TargetUserId), cancellationToken);

        var courseParticipant = new CourseParticipant
        {
            Id = Guid.NewGuid(),
            UserId = request.TargetUserId,
            CourseId = request.CourseId,
        };

        await context.AddAsync(courseParticipant, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return courseParticipant.Id;
    }
}