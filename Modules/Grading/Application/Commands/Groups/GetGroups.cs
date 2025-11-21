using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Groups;

public record GetGroupsQuery(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<IEnumerable<GroupWithMembersDto>>;

public class GetGroupsQueryHandler(GradingContext context)
    : IRequestHandler<GetGroupsQuery, IEnumerable<GroupWithMembersDto>>
{
    public async Task<IEnumerable<GroupWithMembersDto>> Handle(
        GetGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var course = await context.Courses.Include(c => c.Groups)
                         .ThenInclude(g => g.Members)
                         .Include(c => c.Teachers)
                         .Include(c => c.CourseParticipants)
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin
            && !course.Teachers.Any(t => t.UserId.Equals(request.UserId))
            && !course.CourseParticipants.Any(p => p.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        // todo: get personal data from personal
        return course.Groups.Select(
            g => new GroupWithMembersDto(g)
            {
                Members = g.Members.Select(
                        m => new CourseParticipantDto
                        {
                            Id = m.Id,
                            Email = null,
                            FirstName = null,
                            SecondName = null,
                            Patronymic = null,
                            TelegramAlias = null
                        }
                    )
                    .ToList()
            }
        );
    }
}