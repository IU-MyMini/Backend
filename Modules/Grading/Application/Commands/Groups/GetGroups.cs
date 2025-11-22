using ApiClients.OpenApi.Clients.Personal;

using BuildingBlocks.Domain;

using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Groups;

public record GetGroupsQuery(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<IEnumerable<GroupWithMembersDto>>;

public class GetGroupsQueryHandler(GradingContext context, PersonalClient personalClient)
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

        var userIds = course.Groups.SelectMany(g => g.Members).Select(p => p.UserId).Cast<Guid?>().ToList();
        var personalUsers = await personalClient.Api.Personal.SearchByIds.PostAsync(
                                userIds,
                                cancellationToken: cancellationToken
                            )
                            ?? throw Errors.User.NotFound;

        var userMap = personalUsers.ToDictionary(u => u.UserId!.Value);

        return course.Groups.Select(
            g => new GroupWithMembersDto(g)
            {
                Members = g.Members.Select(
                        m => new CourseParticipantDto
                        {
                            Id = m.Id,
                            Email = userMap.GetValueOrDefault(m.UserId)?.Email!,
                            FirstName = LangStr.FromKiota(
                                userMap.GetValueOrDefault(m.UserId)?.FirstName?.AdditionalData
                            ),
                            SecondName = LangStr.FromKiota(
                                userMap.GetValueOrDefault(m.UserId)?.SecondName?.AdditionalData
                            ),
                            Patronymic = LangStr.FromKiota(
                                userMap.GetValueOrDefault(m.UserId)?.Patronymic?.AdditionalData
                            ),
                            TelegramAlias = userMap.GetValueOrDefault(m.UserId)?.TelegramAlias,
                        }
                    )
                    .ToList()
            }
        );
    }
}