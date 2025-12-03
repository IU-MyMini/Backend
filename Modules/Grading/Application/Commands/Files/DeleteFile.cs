using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

namespace GradingModule.Application.Commands.Files;

public record DeleteFileCommand(
    Guid UserId,
    ECourseEntityType CourseEntityType,
    Guid CourseEntityId,
    Guid FileId,
    bool IsAdmin
) : IRequest;

public class DeleteFileCommandHandler(GradingContext context) : IRequestHandler<DeleteFileCommand>
{
    public async Task Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var fileIds = await context.GetExistingFileIdsAsync(
            request.CourseEntityType,
            request.CourseEntityId,
            request.IsAdmin ? null : request.UserId,
            false,
            cancellationToken
        );

        if (!fileIds.Contains(request.FileId))
            throw Errors.File.NotFound;

        fileIds.Remove(request.FileId);

        await context.SaveChangesAsync(cancellationToken);
    }
}