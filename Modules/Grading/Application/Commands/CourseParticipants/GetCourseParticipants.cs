using ApiClients.OpenApi.Clients.Personal;

using BuildingBlocks.Domain;

using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.CourseParticipants;

public record GetCourseParticipantsQuery(Guid UserId, Guid CourseId, bool IsAdmin)
    : IRequest<IEnumerable<CourseParticipantDto>>;

public class GetCourseParticipantsQueryHandler(GradingContext context, PersonalClient personalClient)
    : IRequestHandler<GetCourseParticipantsQuery, IEnumerable<CourseParticipantDto>>
{
    public async Task<IEnumerable<CourseParticipantDto>> Handle(
        GetCourseParticipantsQuery request,
        CancellationToken cancellationToken
    )
    {
        var course = await context.Courses.Include(c => c.Teachers)
                         .Include(c => c.CourseParticipants)
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin
            && !course.Teachers.Any(t => t.UserId.Equals(request.UserId))
            && !course.CourseParticipants.Any(p => p.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        var userIds = course.CourseParticipants.Select(p => p.UserId).Cast<Guid?>().ToList();
        var personalUsers = await personalClient.Api.Personal.SearchByIds.PostAsync(
                                userIds,
                                cancellationToken: cancellationToken
                            )
                            ?? throw Errors.User.NotFound;

        var userMap = personalUsers.ToDictionary(u => u.UserId!.Value);

        return course.CourseParticipants.Select(
            p => new CourseParticipantDto
            {
                Id = p.Id,
                Email = userMap.GetValueOrDefault(p.UserId)?.Email!,
                FirstName = LangStr.FromKiota(userMap.GetValueOrDefault(p.UserId)?.FirstName?.AdditionalData),
                SecondName = LangStr.FromKiota(userMap.GetValueOrDefault(p.UserId)?.SecondName?.AdditionalData),
                Patronymic = LangStr.FromKiota(userMap.GetValueOrDefault(p.UserId)?.Patronymic?.AdditionalData),
                TelegramAlias = userMap.GetValueOrDefault(p.UserId)?.TelegramAlias,
            }
        );
    }
}