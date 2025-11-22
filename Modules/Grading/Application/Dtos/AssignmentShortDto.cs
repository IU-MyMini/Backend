using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class AssignmentShortDto(Assignment a)
{
    public Guid Id { get; set; } = a.Id;

    public LangStr Name { get; set; } = a.Name;
    public LangStr Description { get; set; } = a.Description;
    public IEnumerable<Guid> FileIds { get; set; } = a.FileIds;

    public DateTime? Deadline { get; set; } = a.Deadline;
    public bool IsGroupAssignment { get; set; } = a.IsGroupAssignment;

    public ICollection<ComponentShortDto> Components { get; set; } = a.Components.Select(c => new ComponentShortDto(c)).OrderBy(c => c.Name.Translate() ?? "").ToList();
}

public class ComponentShortDto(AssignmentComponent c)
{
    public Guid Id { get; set; } = c.Id;

    public LangStr Name { get; set; } = c.Name;
    public LangStr Description { get; set; } = c.Description;
    public IEnumerable<Guid> FileIds { get; set; } = c.FileIds;
    public int MaxPoints { get; set; } = c.MaxPoints;
}