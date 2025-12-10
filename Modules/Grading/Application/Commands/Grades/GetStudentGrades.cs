using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Grades;

public record GetStudentGradesQuery(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<StudentGradesDto>;

public class GetStudentGradesQueryHandler(GradingContext context)
    : IRequestHandler<GetStudentGradesQuery, StudentGradesDto>
{
    public async Task<StudentGradesDto> Handle(GetStudentGradesQuery request, CancellationToken cancellationToken)
    {
        var participant
            = await context.CourseParticipants.SingleOrDefaultAsync(
                  p => p.UserId.Equals(request.UserId),
                  cancellationToken
              )
              ?? throw Errors.CourseParticipant.NotFound;

        var course = await context.Courses.Include(c => c.Assignments)
                         .ThenInclude(a => a.Components)
                         .ThenInclude(
                             c => c.Grades.Where(
                                 g => g.CourseParticipantId.Equals(participant.Id)
                                      || g.GroupId.Equals(participant.GroupId)
                             )
                         )
                         .Include(course => course.CourseParticipants)
                         .Include(c => c.Assignments)
                         .ThenInclude(a => a.Components)
                         .ThenInclude(
                             c => c.Submissions.Where(
                                 s => s.SubmittedByParticipantId.Equals(participant.Id)
                                      || s.SubmittedByGroupId.Equals(participant.GroupId)
                             )
                         )
                         .SingleOrDefaultAsync(c => c.Id.Equals(request.CourseId), cancellationToken)
                     ?? throw Errors.Course.NotFound;

        if (!request.IsAdmin && !course.CourseParticipants.Any(p => p.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        var dto = new StudentGradesDto
        {
            Assignments = course.Assignments.OrderByDescending(a => a.Deadline)
                .ThenByDescending(a => a.CreatedAt)
                .Select(
                    a =>
                    {
                        var dto = new AssignmentWithGradesDto(a)
                        {
                            Components = a.Components.OrderBy(c => c.Name.Translate() ?? "")
                                .Select(
                                    c => new ComponentWithGradeDto(c)
                                    {
                                        GroupGrade
                                            = new GradeDto
                                            {
                                                AssignedGrade = c.Grades
                                                    .FirstOrDefault(g => g.GroupId.Equals(participant.GroupId))
                                                    ?.Grade,
                                                MaxGrade = c.MaxPoints
                                            },
                                        IndividualGrade = new GradeDto
                                        {
                                            AssignedGrade
                                                = c.Grades.FirstOrDefault(
                                                          g => g.CourseParticipantId.Equals(participant.Id)
                                                      )
                                                      ?.Grade
                                                  ?? c.Grades.FirstOrDefault(
                                                          g => g.GroupId.Equals(participant.GroupId)
                                                      )
                                                      ?.Grade,
                                            MaxGrade = c.MaxPoints
                                        },
                                        Submission = c.Submissions.Count > 0
                                            ? new SubmissionDto(c.Submissions.First())
                                            : null
                                    }
                                )
                                .ToList()
                        };

                        dto.TotalGrade = new GradeDto
                        {
                            AssignedGrade
                                = dto.Components.All(c => c.IndividualGrade.AssignedGrade is null)
                                    ? null
                                    : dto.Components.Sum(c => c.IndividualGrade.AssignedGrade ?? 0),
                            MaxGrade = a.Components.Sum(c => c.MaxPoints)
                        };

                        return dto;
                    }
                )
                .ToList()
        };

        dto.Total = new GradeDto
        {
            AssignedGrade
                = dto.Assignments.All(a => a.TotalGrade.AssignedGrade is null)
                    ? null
                    : dto.Assignments.Sum(a => a.TotalGrade.AssignedGrade ?? 0),
            MaxGrade = dto.Assignments.Sum(a => a.TotalGrade.MaxGrade)
        };

        return dto;
    }
}