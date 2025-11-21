using GradingModule.API.Http.Models;
using GradingModule.Application.Commands.Assignments;
using GradingModule.Application.Commands.CourseParticipants;
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
    public Task<IEnumerable<CourseDto>> Courses([FromHeader] Guid userId)
        => mediator.Send(new GetCoursesQuery(userId, true)); // todo: remove admin rights

    [HttpGet]
    public Task<CourseDto> Course(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetCourseQuery(userId, courseId, true)); // todo: remove admin rights

    [HttpGet]
    public Task<IEnumerable<CourseParticipantDto>> CourseParticipants(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetCourseParticipantsQuery(userId, courseId, true)); // todo: remove admin rights

    [HttpPost]
    public Task<Guid> CourseParticipant(
        [FromHeader] Guid userId,
        [FromQuery] Guid targetUserId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(
            new AddCourseParticipantCommand(
                userId,
                targetUserId,
                courseId,
                true
            )
        ); // todo: remove admin rights

    [HttpDelete]
    public Task CourseParticipant(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseParticipantId
    )
        => mediator.Send(
            new RemoveCourseParticipantCommand(userId, courseParticipantId, true)
        ); // todo: remove admin rights

    [HttpGet]
    public Task<IEnumerable<AssignmentShortDto>> Assignments(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetAssignmentsQuery(userId, courseId, true)); // todo: remove admin rights

    [HttpPost]
    public Task<Guid> ItpdAssignment(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new CreateItpdAssignmentCommand(userId, courseId, true)); // todo: remove admin rights

    [HttpGet]
    public Task<IEnumerable<GroupWithGradesDto>> GroupsWithGrades(
        [FromHeader] Guid userId,
        [FromQuery] Guid assignmentId
    )
        => mediator.Send(new GetGroupsWithGradesQuery(userId, assignmentId, true)); // todo: remove admin rights

    [HttpGet]
    public Task<IEnumerable<GroupWithMembersDto>> Groups(
        [FromHeader] Guid userId,
        [FromQuery] Guid courseId
    )
        => mediator.Send(new GetGroupsQuery(userId, courseId, true)); // todo: remove admin rights

    [HttpPost]
    public Task<Guid> Group(
        [FromHeader] Guid userId,
        [FromQuery] Guid groupId
    )
        => mediator.Send(new CreateGroupCommand(userId, groupId, true)); // todo: remove admin rights

    [HttpPut]
    public Task Group(
        [FromHeader] Guid userId,
        [FromBody] PutGroupRequest request
    )
        => mediator.Send(
            new UpdateGroupCommand(
                userId,
                request.GroupId,
                request.Name,
                request.Description,
                true
            )
        ); // todo: remove admin rights

    [HttpPost]
    public Task GroupMember(
        [FromHeader] Guid userId,
        [FromQuery] Guid groupId,
        [FromQuery] Guid courseParticipantId
    )
        => mediator.Send(
            new AddGroupMemberCommand(
                userId,
                groupId,
                courseParticipantId,
                true
            )
        ); // todo: remove admin rights
}