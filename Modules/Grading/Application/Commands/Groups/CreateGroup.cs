using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Groups;

public record CreateGroupCommand(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<Guid>;

public class CreateGroupCommandHandler(GradingContext context) : IRequestHandler<CreateGroupCommand, Guid>
{
    public async Task<Guid> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.Include(c => c.Teachers)
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin && !course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        var group = new Group
        {
            Id = Guid.NewGuid(),
            Name = [],
            Description = null,
            IsActive = true,
            CourseId = request.CourseId,
        };

        await context.AddAsync(group, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return group.Id;
    }
}