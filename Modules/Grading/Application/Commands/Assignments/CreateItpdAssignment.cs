using BuildingBlocks.Domain;

using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Assignments;

// todo: for future, make different assignment templates (not only for ITPD)
public record CreateItpdAssignmentCommand(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<Guid>;

public class CreateItpdAssignmentCommandHandler(GradingContext context)
    : IRequestHandler<CreateItpdAssignmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateItpdAssignmentCommand request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.Include(c => c.Assignments)
                         .Include(course => course.Teachers)
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin && !course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        var assignmentNum = course.Assignments.Count + 1;
        var assignmentId = Guid.NewGuid();

        var assignment = new Assignment
        {
            Id = assignmentId,
            Name = new LangStr($"Assignment {assignmentNum}"),
            Description = [],
            FileIds = [],
            CreatedAt = DateTime.UtcNow,
            Deadline = null,
            IsGroupAssignment = true,
            CourseId = request.CourseId,
            Components =
            [
                new AssignmentComponent
                {
                    Id = Guid.NewGuid(),
                    Name = new LangStr($"A{assignmentNum}"),
                    Description = [],
                    FileIds = [],
                    MaxPoints = 100,
                    AssignmentId = assignmentId,
                },
                new AssignmentComponent
                {
                    Id = Guid.NewGuid(),
                    Name = new LangStr($"R{assignmentNum}"),
                    Description = [],
                    FileIds = [],
                    MaxPoints = 100,
                    AssignmentId = assignmentId,
                },
                new AssignmentComponent
                {
                    Id = Guid.NewGuid(),
                    Name = new LangStr($"I{assignmentNum}"),
                    Description = [],
                    FileIds = [],
                    MaxPoints = 100,
                    AssignmentId = assignmentId,
                },
                new AssignmentComponent
                {
                    Id = Guid.NewGuid(),
                    Name = new LangStr($"C{assignmentNum}"),
                    Description = [],
                    FileIds = [],
                    MaxPoints = 100,
                    AssignmentId = assignmentId,
                },
                new AssignmentComponent
                {
                    Id = Guid.NewGuid(),
                    Name = new LangStr("~Extra"),
                    Description = [],
                    FileIds = [],
                    MaxPoints = 0,
                    AssignmentId = assignmentId,
                },
            ]
        };

        await context.AddAsync(assignment, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return assignmentId;
    }
}