using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.CourseParticipants;

public record GetCourseParticipantsQuery(Guid UserId, Guid CourseId, bool IsAdmin)
    : IRequest<IEnumerable<CourseParticipantDto>>;

public class GetCourseParticipantsQueryHandler(GradingContext context)
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

        // todo: get personal data from personal
        return course.CourseParticipants.Select(
            p => new CourseParticipantDto
            {
                Id = p.Id,
                Email = null,
                FirstName = null,
                SecondName = null,
                Patronymic = null,
                TelegramAlias = null
            }
        );
    }
}