using GradingModule.Application.Commands;
using GradingModule.Application.Commands.Assignments;
using GradingModule.Application.Commands.Courses;
using GradingModule.Application.Commands.Groups;
using GradingModule.Application.Dtos;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GradingModule.API.Http;

[ApiController]
[Route("api/[controller]/[action]")]
public class GradingController(IMediator mediator) : Controller
{
    [HttpGet]
    public Task<string> Hello()
        => mediator.Send(new HelloCommand());

    [HttpGet]
    public Task<IEnumerable<CourseDto>> Courses([FromHeader] Guid userId)
        => mediator.Send(new GetCoursesQuery(userId, true)); // todo: remove admin rights

    [HttpGet]
    public Task<IEnumerable<AssignmentShortDto>> Assignments(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetAssignmentsQuery(userId, courseId, true)); // todo: remove admin rights

    [HttpGet]
    public Task<IEnumerable<GroupWithGradesDto>> GroupsWithGrades(
        [FromHeader] Guid userId,
        [FromQuery] Guid assignmentId
    )
        => mediator.Send(new GetGroupsWithGradesQuery(userId, assignmentId, true)); // todo: remove admin rights
}