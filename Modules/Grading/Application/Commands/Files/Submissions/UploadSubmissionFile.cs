using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.Files.Submissions;

public record UploadSubmissionFileCommand(
    Guid UserId,
    Guid ComponentId,
    Guid? ExistingFileId,
    string FileName,
    string ContentType,
    Stream FileStream,
    bool IsAdmin
) : IRequest<Guid>;

public class UploadSubmissionFileCommandHandler(GradingContext context, IMediator mediator)
    : IRequestHandler<UploadSubmissionFileCommand, Guid>
{
    public async Task<Guid> Handle(UploadSubmissionFileCommand request, CancellationToken cancellationToken)
    {
        var participant = await context.CourseParticipants.SingleOrDefaultAsync(
                              p => p.UserId.Equals(request.UserId),
                              cancellationToken
                          )
                          ?? throw Errors.CourseParticipant.NotFound;

        var component = await context.AssignmentComponents.Include(
                                c => c.Submissions.Where(
                                    s => (s.SubmittedByParticipant != null
                                          && s.SubmittedByParticipant.UserId.Equals(request.UserId))
                                         || (s.SubmittedByGroup != null
                                             && s.SubmittedByGroup.Members.Any(m => m.UserId.Equals(request.UserId)))
                                )
                            )
                            .SingleOrDefaultAsync(c => c.Id.Equals(request.ComponentId), cancellationToken)
                        ?? throw Errors.AssignmentComponent.NotFound;

        var submission = component.Submissions.SingleOrDefault();
        if (submission is null)
        {
            submission = new Submission
            {
                Id = Guid.NewGuid(),
                ComponentId = component.Id,
                FileIds = []
            };

            if (participant.GroupId is not null)
                submission.SubmittedByGroupId = participant.GroupId;
            else
                submission.SubmittedByParticipantId = participant.Id;

            await context.AddAsync(submission, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        var fileId = await mediator.Send(
            new UploadFileCommand(
                request.UserId,
                ECourseEntityType.Submission,
                submission.Id,
                request.ExistingFileId,
                request.FileName,
                request.ContentType,
                request.FileStream,
                request.IsAdmin
            ),
            cancellationToken
        );

        submission.SubmittedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        return fileId;
    }
}