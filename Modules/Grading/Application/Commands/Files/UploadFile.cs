using ApiClients.OpenApi.Clients.FileNamespace;

using BuildingBlocks.Application.File.Upload;

using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

namespace GradingModule.Application.Commands.Files;

public record UploadFileCommand(
    Guid UserId,
    ECourseEntityType CourseEntityType,
    Guid CourseEntityId,
    Guid? ExistingFileId,
    string FileName,
    string ContentType,
    Stream FileStream,
    bool IsAdmin
) : IRequest<Guid>;

public class UploadFileCommandHandler(GradingContext context, FileClient fileClient)
    : IRequestHandler<UploadFileCommand, Guid>
{
    public async Task<Guid> Handle(UploadFileCommand request, CancellationToken cancellationToken)
    {
        var fileIds = await context.GetExistingFileIdsAsync(
            request.CourseEntityType,
            request.CourseEntityId,
            request.IsAdmin ? null : request.UserId,
            false,
            cancellationToken
        );

        var multipartBody = new MultipartBodyBuilder
        {
            ExistingFileId = request.ExistingFileId,
            FileName = request.FileName,
            ContentType = request.ContentType,
            FileStream = request.FileStream
        }.Build();

        var fileId = await fileClient.Api.File.Upload.PostAsync(multipartBody, cancellationToken: cancellationToken)
                     ?? throw Errors.File.NotFound;

        fileIds.Add(fileId);

        await context.SaveChangesAsync(cancellationToken);

        return fileId;
    }
}