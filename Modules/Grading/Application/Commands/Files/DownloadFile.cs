using ApiClients.OpenApi.Clients.FileNamespace;

using BuildingBlocks.Application.File.Download;

using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.Kiota.Abstractions;

namespace GradingModule.Application.Commands.Files;

public record DownloadFileQuery(
    Guid UserId,
    ECourseEntityType CourseEntityType,
    Guid CourseEntityId,
    Guid FileId,
    bool IsAdmin
) : IRequest<FileDto>;

public class DownloadFileQueryHandler(GradingContext context, FileClient fileClient)
    : IRequestHandler<DownloadFileQuery, FileDto>
{
    public async Task<FileDto> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        var fileIds = await context.GetExistingFileIdsAsync(
            request.CourseEntityType,
            request.CourseEntityId,
            request.IsAdmin ? null : request.UserId,
            true,
            cancellationToken
        );

        if (!fileIds.Contains(request.FileId))
            throw Errors.File.NotFound;

        return await FileDownloader.Download(Callback);

        async Task<Stream> Callback(Action<RequestConfiguration<DefaultQueryParameters>> rc)
            => await fileClient.Api.File.Download[request.FileId].GetAsync(rc, cancellationToken)
               ?? throw Errors.File.NotFound;
    }
}