using GradingModule.Application.Dtos;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Courses;

// todo: make it a paginated query
public record GetCoursesQuery(Guid UserId) : IRequest<IEnumerable<CourseDto>>;

public class GetCoursesQueryHandler(GradingContext context) : IRequestHandler<GetCoursesQuery, IEnumerable<CourseDto>>
{
    public async Task<IEnumerable<CourseDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await context.Courses
            .Where(
                c => c.CourseParticipants.Any(p => p.UserId.Equals(request.UserId))
                     || c.Teachers.Any(t => t.UserId.Equals(request.UserId))
            )
            .Include(c => c.Teachers)
            .ToListAsync(cancellationToken: cancellationToken);

        return courses.Select(c => new CourseDto(c, request.UserId));
    }
}