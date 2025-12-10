using BuildingBlocks.Application.File.Download;
using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class ComponentShortDto(AssignmentComponent c)
{
    public Guid Id { get; set; } = c.Id;

    public LangStr Name { get; set; } = c.Name;
    public LangStr Description { get; set; } = c.Description;
    public IEnumerable<FileInfoDto> Files { get; set; } = [];
    public int MaxPoints { get; set; } = c.MaxPoints;
}