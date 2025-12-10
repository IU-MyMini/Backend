using BuildingBlocks.Application.File.Download;
using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class AssignmentShortDto(Assignment a)
{
    public Guid Id { get; set; } = a.Id;

    public LangStr Name { get; set; } = a.Name;
    public LangStr Description { get; set; } = a.Description;
    public IEnumerable<FileInfoDto> Files { get; set; } = [];

    public DateTime? Deadline { get; set; } = a.Deadline;
    public bool IsGroupAssignment { get; set; } = a.IsGroupAssignment;

    public ICollection<ComponentShortDto> Components { get; set; } = [];
}