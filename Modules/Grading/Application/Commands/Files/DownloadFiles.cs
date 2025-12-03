using System.IO.Compression;

using ApiClients.OpenApi.Clients.FileNamespace;

using BuildingBlocks.Application.File.Download;

using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.Kiota.Abstractions;

namespace GradingModule.Application.Commands.Files;

public record DownloadFilesQuery(
    Guid UserId,
    ECourseEntityType CourseEntityType,
    Guid CourseEntityId,
    bool IsAdmin
) : IRequest<FileDto>;

public class DownloadFilesQueryHandler(GradingContext context, FileClient fileClient)
    : IRequestHandler<DownloadFilesQuery, FileDto>
{
    public async Task<FileDto> Handle(DownloadFilesQuery request, CancellationToken cancellationToken)
    {
        var fileIds = await context.GetExistingFileIdsAsync(
            request.CourseEntityType,
            request.CourseEntityId,
            request.IsAdmin ? null : request.UserId,
            true,
            cancellationToken
        );

        if (fileIds.Count == 0)
            throw Errors.File.NotFound;

        var callbacks = fileIds.Select<Guid, Func<Action<RequestConfiguration<DefaultQueryParameters>>, Task<Stream>>>(
            id => async rc => await fileClient.Api.File.Download[id].GetAsync(rc, cancellationToken)
                              ?? throw Errors.File.NotFound
        );

        if (fileIds.Count == 1)
            return await FileDownloader.Download(callbacks.First());

        var zipStream = new MemoryStream();

        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var callback in callbacks)
            {
                var file = await FileDownloader.Download(callback);

                var entry = archive.CreateEntry(file.FileName);
                await using var entryStream = entry.Open();
                await file.Stream.CopyToAsync(entryStream, cancellationToken);
                await file.Stream.DisposeAsync();
            }
        }

        zipStream.Position = 0;

        var fileName = await context.GetComponentNameAsync(
            request.CourseEntityType,
            request.CourseEntityId,
            cancellationToken
        );

        return new FileDto
        {
            Stream = zipStream,
            FileName = fileName,
            ContentType = "application/zip"
        };
    }
}