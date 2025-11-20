using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public abstract class GroupDtoBase(Group g)
{
    public LangStr Name { get; set; } = g.Name;
    public LangStr? Description { get; set; } = g.Description;
}