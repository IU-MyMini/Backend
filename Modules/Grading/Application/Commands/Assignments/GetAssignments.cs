using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Assignments;

public record GetAssignmentsQuery(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<IEnumerable<AssignmentShortDto>>;

public class GetAssignmentsQueryHandler(GradingContext context)
    : IRequestHandler<GetAssignmentsQuery, IEnumerable<AssignmentShortDto>>
{
    public async Task<IEnumerable<AssignmentShortDto>> Handle(
        GetAssignmentsQuery request,
        CancellationToken cancellationToken
    )
    {
        var course = await context.Courses.Include(course => course.Teachers)
                         .Include(course => course.CourseParticipants)
                         .Include(course => course.Assignments)
                         .ThenInclude(a => a.Components)
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin
            && !course.Teachers.Any(t => t.UserId.Equals(request.UserId))
            && !course.CourseParticipants.Any(p => p.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        return course.Assignments.Select(a => new AssignmentShortDto(a));
    }
}