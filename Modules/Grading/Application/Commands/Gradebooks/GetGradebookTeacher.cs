using GradingModule.Application.Dtos;

using MediatR;

namespace GradingModule.Application.Commands.Gradebooks;

public record GetGradebookTeacherQuery : IRequest<IEnumerable<GradebookTeacherDto>>;

public class
    GetGradebookTeacherQueryHandler : IRequestHandler<GetGradebookTeacherQuery, IEnumerable<GradebookTeacherDto>>
{
    public async Task<IEnumerable<GradebookTeacherDto>> Handle(
        GetGradebookTeacherQuery request,
        CancellationToken cancellationToken
    )
    {
        // todo
        return [];
    }
}