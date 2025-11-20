using GradingModule.Application.Commands;
using GradingModule.Application.Commands.Courses;
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
        => mediator.Send(new GetCoursesQuery(userId));
}