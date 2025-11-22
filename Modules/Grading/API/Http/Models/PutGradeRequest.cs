using BuildingBlocks.Domain;

namespace GradingModule.API.Http.Models;

public class PutGradeRequest
{
    public Guid ComponentId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid? CourseParticipantId { get; set; }
    public int? Grade { get; set; }
    public LangStr? Feedback { get; set; }
}