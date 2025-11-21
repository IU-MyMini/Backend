using BuildingBlocks.Domain;

using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Groups;

public record UpdateGroupCommand(
    Guid UserId,
    Guid GroupId,
    LangStr Name,
    LangStr Description,
    bool IsAdmin
) : IRequest;

public class UpdateGroupCommandHandler(GradingContext context) : IRequestHandler<UpdateGroupCommand>
{
    public async Task Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await context.Groups.Include(g => g.Members)
                        .Include(g => g.Course)
                        .ThenInclude(c => c.Teachers)
                        .SingleOrDefaultAsync(g => g.Id.Equals(request.GroupId), cancellationToken)
                    ?? throw Errors.Group.NotFound;

        if (!request.IsAdmin
            && !group.Members.Any(m => m.UserId.Equals(request.UserId))
            && !group.Course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Group.NotAllowed;

        group.Name = request.Name;
        group.Description = request.Description;

        await context.SaveChangesAsync(cancellationToken);
    }
}