using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class GroupShortDto(Group g)
{
    public Guid Id { get; set; } = g.Id;
    public LangStr Name { get; set; } = g.Name;
    public LangStr? Description { get; set; } = g.Description;
}