using BuildingBlocks.Domain;

namespace GradingModule.API.Http.Models;

public class PutGroupRequest
{
    public Guid GroupId { get; set; }
    public LangStr Name { get; set; } = [];
    public LangStr Description { get; set; } = [];
}