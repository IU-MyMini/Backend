using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Groups;

public record GetGroupsWithGradesQuery(Guid UserId, Guid AssignmentId, bool IsAdmin)
    : IRequest<IEnumerable<GroupWithGradesDto>>;

public class GetGroupsWithGradesQueryHandler(GradingContext context)
    : IRequestHandler<GetGroupsWithGradesQuery, IEnumerable<GroupWithGradesDto>>
{
    public async Task<IEnumerable<GroupWithGradesDto>> Handle(
        GetGroupsWithGradesQuery request,
        CancellationToken cancellationToken
    )
    {
        var assignment = await context.Assignments.Include(a => a.Course)
                             .ThenInclude(c => c.Groups)
                             .ThenInclude(g => g.Members)
                             .ThenInclude(
                                 m => m.Grades.Where(g => g.Component.AssignmentId.Equals(request.AssignmentId))
                             )
                             .Include(assignment => assignment.Course)
                             .ThenInclude(course => course.Groups)
                             .ThenInclude(
                                 group => group.Grades.Where(g => g.Component.AssignmentId.Equals(request.AssignmentId))
                             )
                             .Include(a => a.Components)
                             .Include(assignment => assignment.Course)
                             .ThenInclude(course => course.Teachers)
                             .SingleOrDefaultAsync(a => a.Id.Equals(request.AssignmentId), cancellationToken)
                         ?? throw Errors.Assignment.NotFound;

        if (!request.IsAdmin && !assignment.Course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Assignment.NotAllowed;

        // todo: load user info (add apiClients first)
        return assignment.Course.Groups.Select(
            g => new GroupWithGradesDto(g)
            {
                Members = g.Members.Select(
                        m => new CourseParticipantWithGradesDto
                        {
                            Id = m.Id,
                            Email = null,
                            FirstName = null,
                            SecondName = null,
                            Patronymic = null,
                            TelegramAlias = null,
                            Grades = m.Grades.ToDictionary(
                                grade => grade.ComponentId,
                                grade => new GradeDto
                                {
                                    AssignedGrade = grade.Grade,
                                    MaxGrade = grade.Component.MaxPoints
                                }
                            ),
                            TotalGrade = FindTotalGrade(assignment.Components, m.Grades)
                        }
                    )
                    .ToList(),
                Grades = assignment.Components.ToDictionary(
                    c => c.Id,
                    c => new GradeDto
                    {
                        AssignedGrade = g.Grades.FirstOrDefault(grade => grade.ComponentId.Equals(c.Id))?.Grade,
                        MaxGrade = c.MaxPoints
                    }
                ),
                TotalGrade = FindTotalGrade(assignment.Components, g.Grades)
            }
        );
    }

    private static GradeDto FindTotalGrade(
        ICollection<AssignmentComponent> components,
        ICollection<ComponentGrade> grades
    )
        => new GradeDto
        {
            AssignedGrade = components.Sum(
                c => grades.FirstOrDefault(grade => grade.ComponentId.Equals(c.Id))?.Grade ?? 0
            ),
            MaxGrade = components.Sum(c => c.MaxPoints)
        };
}