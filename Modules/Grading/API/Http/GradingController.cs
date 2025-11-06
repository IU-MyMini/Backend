using GradingModule.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GradingModule.API.Http;

[ApiController]
[Route("api/[controller]/[action]")]
public class GradingController(IMediator mediator) : Controller
{
    [HttpGet]
    public Task<string> Hello() => mediator.Send(new HelloCommand());
}