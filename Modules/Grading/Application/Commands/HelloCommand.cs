using MediatR;

namespace GradingModule.Application.Commands;

public record HelloCommand : IRequest<string>;

public class HelloCommandHandler : IRequestHandler<HelloCommand, string>
{
    public Task<string> Handle(HelloCommand request, CancellationToken cancellationToken)
        => Task.FromResult("Hello!");
}