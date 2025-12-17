using ApiClients.OpenApi.Clients.FileNamespace;

using BuildingBlocks.Application.File.Download;

using GradingModule.Application.Dtos;
using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Grades;

public record GetStudentGradesQuery(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<StudentGradesDto>;

public class GetStudentGradesQueryHandler(GradingContext context, FileClient fileClient)
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

        var peerReviews = await context.PeerReviews
            .Where(r => r.SourceGroupId.Equals(participant.GroupId) || r.TargetGroupId.Equals(participant.GroupId))
            .Include(peerReview => peerReview.SourceComponent)
            .ThenInclude(assignmentComponent => assignmentComponent.Submissions)
            .Include(peerReview => peerReview.TargetComponent)
            .ThenInclude(assignmentComponent => assignmentComponent.Submissions)
            .Include(peerReview => peerReview.TargetGroup)
            .Include(peerReview => peerReview.SourceGroup)
            .ToListAsync(cancellationToken);

        var outgoingPeerReviews = peerReviews.Where(r => r.SourceGroupId.Equals(participant.GroupId)).ToList();
        var incomingPeerReviews = peerReviews.Where(r => r.TargetGroupId.Equals(participant.GroupId)).ToList();

        var fileIds = course.Assignments.SelectMany(
            a => a.FileIds.Concat(
                a.Components.SelectMany(c => c.FileIds.Concat(c.Submissions.SelectMany(s => s.FileIds)))
            )
        );

        foreach (var r in outgoingPeerReviews)
        {
            r.TargetComponent.Submissions = r.TargetComponent.Submissions
                .Where(s => s.SubmittedByGroupId.Equals(r.TargetGroupId))
                .ToList();

            r.SourceComponent.Submissions = [];
        }

        foreach (var r in incomingPeerReviews)
        {
            r.SourceComponent.Submissions = r.SourceComponent.Submissions
                .Where(s => s.SubmittedByGroupId.Equals(r.SourceGroupId))
                .ToList();

            r.TargetComponent.Submissions = [];
        }

        fileIds = fileIds.Concat(
            outgoingPeerReviews.SelectMany(r => r.TargetComponent.Submissions.SelectMany(s => s.FileIds))
        );

        fileIds = fileIds.Concat(
            incomingPeerReviews.SelectMany(r => r.SourceComponent.Submissions.SelectMany(s => s.FileIds))
        );

        var files = (await fileClient.Api.File.FileInfos.GetAsync(
            rc => { rc.QueryParameters.FileIds = fileIds.Cast<Guid?>().ToArray(); },
            cancellationToken
        ))!.ToDictionary(
            f => f.Id!.Value,
            f => new FileInfoDto
            {
                Id = f.Id!.Value,
                FileName = f.FileName!,
                ContentType = f.ContentType!,
                CreatedAt = f.CreatedAt!.Value.DateTime
            }
        );

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
                                        IndividualGrade
                                            = new GradeDto
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
                                        Files = c.FileIds.Select(id => files[id]),
                                        Submission
                                            = c.Submissions.Count > 0
                                                ? new SubmissionDto(c.Submissions.First())
                                                {
                                                    Files = c.Submissions.First()
                                                        .FileIds.Select(id => files[id])
                                                        .ToList()
                                                }
                                                : null,
                                        OutgoingPeerReviews
                                            = outgoingPeerReviews.Select(
                                                r => new PeerReviewStudentDto
                                                {
                                                    Group = new GroupShortDto(r.TargetGroup),
                                                    Submission
                                                        = r.TargetComponent.Submissions.Select(
                                                                s => new SubmissionDto(s)
                                                                {
                                                                    Files = s.FileIds.Select(id => files[id])
                                                                        .ToList()
                                                                }
                                                            )
                                                            .FirstOrDefault()
                                                }
                                            ),
                                        IncomingPeerReviews = incomingPeerReviews.Select(
                                            r => new PeerReviewStudentDto
                                            {
                                                Group = new GroupShortDto(r.SourceGroup),
                                                Submission = r.SourceComponent.Submissions.Select(
                                                        s => new SubmissionDto(s)
                                                        {
                                                            Files = s.FileIds.Select(id => files[id]).ToList()
                                                        }
                                                    )
                                                    .FirstOrDefault()
                                            }
                                        )
                                    }
                                )
                                .ToList(),
                            Files = a.FileIds.Select(id => files[id])
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