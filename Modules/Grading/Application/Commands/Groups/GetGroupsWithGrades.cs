using ApiClients.OpenApi.Clients.Personal;

using BuildingBlocks.Domain;

using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Groups;

public record GetGroupsWithGradesQuery(Guid UserId, Guid AssignmentId, bool IsAdmin)
    : IRequest<IEnumerable<GroupWithGradesDto>>;

public class GetGroupsWithGradesQueryHandler(GradingContext context, PersonalClient personalClient)
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

        var userIds = assignment.Course.Groups.SelectMany(g => g.Members).Select(p => p.UserId).Cast<Guid?>().ToList();
        var personalUsers = await personalClient.Api.Personal.SearchByIds.PostAsync(
                                userIds,
                                cancellationToken: cancellationToken
                            )
                            ?? throw Errors.User.NotFound;

        var userMap = personalUsers.ToDictionary(u => u.UserId!.Value);

        return assignment.Course.Groups.Select(
            g => new GroupWithGradesDto(g)
            {
                Members = g.Members.Select(
                        m => new CourseParticipantWithGradesDto
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
                            Grades = ToGradeDtos(assignment.Components, m.Grades),
                            TotalGrade = FindTotalGrade(assignment.Components, m.Grades)
                        }
                    )
                    .ToList(),
                Grades = ToGradeDtos(assignment.Components, g.Grades),
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

    private static Dictionary<Guid, GradeDto> ToGradeDtos(
        ICollection<AssignmentComponent> components,
        ICollection<ComponentGrade> grades
    )
        => components.ToDictionary(
            c => c.Id,
            c => new GradeDto
            {
                AssignedGrade = grades.FirstOrDefault(grade => grade.ComponentId.Equals(c.Id))?.Grade,
                MaxGrade = c.MaxPoints
            }
        );
}