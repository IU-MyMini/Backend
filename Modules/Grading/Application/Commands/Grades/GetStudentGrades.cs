using GradingModule.Application.Dtos;
using GradingModule.Infrastructure;

using MediatR;

namespace GradingModule.Application.Commands.Grades;

public record GetStudentGradesQuery(Guid UserId, Guid CourseId, bool IsAdmin) : IRequest<StudentGradesDto>;

public class GetStudentGradesQueryHandler(GradingContext context)
    : IRequestHandler<GetStudentGradesQuery, StudentGradesDto>
{
    public Task<StudentGradesDto> Handle(GetStudentGradesQuery request, CancellationToken cancellationToken)
        => throw new NotImplementedException();
}