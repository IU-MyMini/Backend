using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Groups;

public record AddGroupMemberCommand(
    Guid UserId,
    Guid GroupId,
    Guid CourseParticipantId,
    bool IsAdmin
) : IRequest;

public class AddGroupMemberCommandHandler(GradingContext context) : IRequestHandler<AddGroupMemberCommand>
{
    public async Task Handle(AddGroupMemberCommand request, CancellationToken cancellationToken)
    {
        var group = await context.Groups.Include(g => g.Members)
                        .Include(g => g.Course)
                        .ThenInclude(c => c.Teachers)
                        .SingleOrDefaultAsync(g => g.Id.Equals(request.GroupId), cancellationToken)
                    ?? throw Errors.Group.NotFound;

        var courseParticipant = await context.CourseParticipants.SingleOrDefaultAsync(
                                    p => p.Id.Equals(request.CourseParticipantId),
                                    cancellationToken
                                )
                                ?? throw Errors.CourseParticipant.NotFound;

        if (!request.IsAdmin
            && !group.Members.Any(m => m.UserId.Equals(request.UserId))
            && !group.Course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Group.NotAllowed;

        if (group.Members.Any(m => m.Id.Equals(request.CourseParticipantId)))
            throw Errors.CourseParticipant.AlreadyAdded;

        group.Members.Add(courseParticipant);

        await context.SaveChangesAsync(cancellationToken);
    }
}