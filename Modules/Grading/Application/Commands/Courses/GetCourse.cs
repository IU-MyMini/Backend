using GradingModule.Application.Dtos;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Errors = GradingModule.Domain.Errors;

namespace GradingModule.Application.Commands.Courses;

public record GetCourseQuery(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<CourseDto>;

public class GetCourseQueryHandler(GradingContext context) : IRequestHandler<GetCourseQuery, CourseDto>
{
    public async Task<CourseDto> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.Include(c => c.Teachers)
                         .Include(c => c.CourseParticipants)
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin
            && !course.CourseParticipants.Any(p => p.UserId.Equals(request.UserId))
            && !course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        return new CourseDto(course, request.UserId);
    }
}