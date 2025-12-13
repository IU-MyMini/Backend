using ApiClients.OpenApi.Clients.FileNamespace;

using BuildingBlocks.Application.File.Download;

using GradingModule.Application.Dtos;
using GradingModule.Domain.Entities;

using MediatR;

namespace GradingModule.Application.Commands.Assignments;

public record MapAssignmentsToShortDtoQuery(ICollection<Assignment> Assignments) : IRequest<IEnumerable<AssignmentShortDto>>;

public class MapAssignmentsToShortDtoQueryHandler(FileClient fileClient) : IRequestHandler<MapAssignmentsToShortDtoQuery, IEnumerable<AssignmentShortDto>>
{
    public async Task<IEnumerable<AssignmentShortDto>> Handle(MapAssignmentsToShortDtoQuery request, CancellationToken cancellationToken)
    {
        var files = (await fileClient.Api.File.FileInfos.GetAsync(rc => { rc.QueryParameters.FileIds = request.Assignments.SelectMany(a => a.FileIds.Concat(a.Components.SelectMany(c => c.FileIds))).Cast<Guid?>().ToArray(); }, cancellationToken))!.ToDictionary(
            f => f.Id!.Value,
            f => new FileInfoDto
            {
                Id = f.Id!.Value,
                FileName = f.FileName!,
                ContentType = f.ContentType!,
                CreatedAt = f.CreatedAt!.Value.DateTime
            }
        );

        return request.Assignments.OrderByDescending(a => a.Deadline)
            .ThenByDescending(a => a.CreatedAt)
            .Select(
                a => new AssignmentShortDto(a)
                {
                    Files = a.FileIds.Select(id => files[id]),
                    Components = a.Components.OrderBy(c => c.Name.Translate() ?? "").Select(c => new ComponentShortDto(c) { Files = c.FileIds.Select(id => files[id]) }).ToList()
                }
            )
            .ToList();
    }
}