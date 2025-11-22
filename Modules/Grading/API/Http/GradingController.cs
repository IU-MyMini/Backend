using GradingModule.API.Http.Models;
using GradingModule.Application.Commands.Assignments;
using GradingModule.Application.Commands.CourseParticipants;
using GradingModule.Application.Commands.Courses;
using GradingModule.Application.Commands.Grades;
using GradingModule.Application.Commands.Groups;
using GradingModule.Application.Dtos;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GradingModule.API.Http;

[ApiController]
[Route("api/[controller]/[action]")]
public class GradingController(IMediator mediator) : Controller
{
    private const string AdminRole = "admin_grading";

    [HttpGet]
    public Task<IEnumerable<CourseDto>> Courses(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles
    )
        => mediator.Send(new GetCoursesQuery(userId, roles.Contains(AdminRole)));

    [HttpGet]
    public Task<CourseDto> Course(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetCourseQuery(userId, courseId, roles.Contains(AdminRole)));

    [HttpGet]
    public Task<IEnumerable<CourseParticipantDto>> CourseParticipants(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetCourseParticipantsQuery(userId, courseId, roles.Contains(AdminRole)));

    [HttpPost]
    public Task<Guid> CourseParticipant(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid targetUserId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(
            new AddCourseParticipantCommand(
                userId,
                targetUserId,
                courseId,
                roles.Contains(AdminRole)
            )
        );

    [HttpDelete]
    public Task CourseParticipant(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid courseParticipantId
    )
        => mediator.Send(new RemoveCourseParticipantCommand(userId, courseParticipantId, roles.Contains(AdminRole)));

    [HttpGet]
    public Task<IEnumerable<AssignmentShortDto>> Assignments(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetAssignmentsQuery(userId, courseId, roles.Contains(AdminRole)));

    [HttpPost]
    public Task<Guid> ItpdAssignment(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new CreateItpdAssignmentCommand(userId, courseId, roles.Contains(AdminRole)));

    [HttpGet]
    public Task<IEnumerable<GroupWithGradesDto>> GroupsWithGrades(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid assignmentId
    )
        => mediator.Send(new GetGroupsWithGradesQuery(userId, assignmentId, roles.Contains(AdminRole)));

    [HttpGet]
    public Task<IEnumerable<GroupWithMembersDto>> Groups(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetGroupsQuery(userId, courseId, roles.Contains(AdminRole)));

    [HttpPost]
    public Task<Guid> Group(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid groupId
    )
        => mediator.Send(new CreateGroupCommand(userId, groupId, roles.Contains(AdminRole)));

    [HttpPut]
    public Task Group(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromBody] PutGroupRequest request
    )
        => mediator.Send(
            new UpdateGroupCommand(
                userId,
                request.GroupId,
                request.Name,
                request.Description,
                roles.Contains(AdminRole)
            )
        );

    [HttpPost]
    public Task GroupMember(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid groupId,
        [FromQuery] Guid courseParticipantId
    )
        => mediator.Send(
            new AddGroupMemberCommand(
                userId,
                groupId,
                courseParticipantId,
                roles.Contains(AdminRole)
            )
        );

    [HttpPut]
    public Task Grade(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromBody] PutGradeRequest request
    )
        => mediator.Send(
            new PutGradeCommand(
                userId,
                request.ComponentId,
                request.GroupId,
                request.CourseParticipantId,
                request.Grade,
                request.Feedback ?? [],
                roles.Contains(AdminRole)
            )
        );

    [HttpGet]
    public Task<StudentGradesDto> StudentGrades(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetStudentGradesQuery(userId, courseId, false));
}