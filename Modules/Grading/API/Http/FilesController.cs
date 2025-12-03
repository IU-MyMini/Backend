using FileTypeChecker.Web.Attributes;

using GradingModule.Application.Commands.Files;
using GradingModule.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GradingModule.API.Http;

[ApiController]
[Route("api/Grading/[action]")]
public class FilesController(IMediator mediator) : Controller
{
    private const string AdminRole = "admin_grading";

    #region Assignment
    [HttpGet]
    [Route("/api/Grading/Assignment/File")]
    public async Task<FileStreamResult> AssignmentFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid assignmentId,
        [FromQuery] Guid fileId
    )
    {
        var file = await mediator.Send(
            new DownloadFileQuery(
                userId,
                ECourseEntityType.Assignment,
                assignmentId,
                fileId,
                roles.Contains(AdminRole)
            )
        );

        return new FileStreamResult(file.Stream, file.ContentType) { FileDownloadName = file.FileName };
    }

    [HttpPost]
    [Route("/api/Grading/Assignment/File")]
    [RequestSizeLimit(10_485_760)] // 10MB
    public async Task<Guid> AssignmentFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [AllowedTypes("pdf")] IFormFile file,
        [FromForm] Guid assignmentId,
        [FromForm] Guid? existingFileId
    )
    {
        await using var fileStream = file.OpenReadStream();
        return await mediator.Send(
            new UploadFileCommand(
                userId,
                ECourseEntityType.Assignment,
                assignmentId,
                existingFileId,
                file.FileName,
                file.ContentType,
                fileStream,
                roles.Contains(AdminRole)
            )
        );
    }

    [HttpDelete]
    [Route("/api/Grading/Assignment/File")]
    public Task DeleteAssignmentFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid assignmentId,
        [FromQuery] Guid fileId
    )
        => mediator.Send(
            new DeleteFileCommand(
                userId,
                ECourseEntityType.Assignment,
                assignmentId,
                fileId,
                roles.Contains(AdminRole)
            )
        );
    #endregion

    #region AssignmentComponent
    [HttpGet]
    [Route("/api/Grading/AssignmentComponent/File")]
    public async Task<FileStreamResult> AssignmentComponentFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid assignmentComponentId,
        [FromQuery] Guid fileId
    )
    {
        var file = await mediator.Send(
            new DownloadFileQuery(
                userId,
                ECourseEntityType.AssignmentComponent,
                assignmentComponentId,
                fileId,
                roles.Contains(AdminRole)
            )
        );

        return new FileStreamResult(file.Stream, file.ContentType) { FileDownloadName = file.FileName };
    }

    [HttpPost]
    [Route("/api/Grading/AssignmentComponent/File")]
    [RequestSizeLimit(10_485_760)] // 10MB
    public async Task<Guid> AssignmentComponentFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [AllowedTypes("pdf")] IFormFile file,
        [FromForm] Guid assignmentComponentId,
        [FromForm] Guid? existingFileId
    )
    {
        await using var fileStream = file.OpenReadStream();
        return await mediator.Send(
            new UploadFileCommand(
                userId,
                ECourseEntityType.AssignmentComponent,
                assignmentComponentId,
                existingFileId,
                file.FileName,
                file.ContentType,
                fileStream,
                roles.Contains(AdminRole)
            )
        );
    }

    [HttpDelete]
    [Route("/api/Grading/AssignmentComponent/File")]
    public Task DeleteAssignmentComponentFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid assignmentComponentId,
        [FromQuery] Guid fileId
    )
        => mediator.Send(
            new DeleteFileCommand(
                userId,
                ECourseEntityType.AssignmentComponent,
                assignmentComponentId,
                fileId,
                roles.Contains(AdminRole)
            )
        );
    #endregion

    #region ComponentGrade
    [HttpGet]
    [Route("/api/Grading/ComponentGrade/File")]
    public async Task<FileStreamResult> ComponentGradeFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid componentGradeId,
        [FromQuery] Guid fileId
    )
    {
        var file = await mediator.Send(
            new DownloadFileQuery(
                userId,
                ECourseEntityType.ComponentGrade,
                componentGradeId,
                fileId,
                roles.Contains(AdminRole)
            )
        );

        return new FileStreamResult(file.Stream, file.ContentType) { FileDownloadName = file.FileName };
    }

    [HttpPost]
    [Route("/api/Grading/ComponentGrade/File")]
    [RequestSizeLimit(10_485_760)] // 10MB
    public async Task<Guid> ComponentGradeFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [AllowedTypes("pdf")] IFormFile file,
        [FromForm] Guid componentGradeId,
        [FromForm] Guid? existingFileId
    )
    {
        await using var fileStream = file.OpenReadStream();
        return await mediator.Send(
            new UploadFileCommand(
                userId,
                ECourseEntityType.ComponentGrade,
                componentGradeId,
                existingFileId,
                file.FileName,
                file.ContentType,
                fileStream,
                roles.Contains(AdminRole)
            )
        );
    }

    [HttpDelete]
    [Route("/api/Grading/ComponentGrade/File")]
    public Task DeleteComponentGradeFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid componentGradeId,
        [FromQuery] Guid fileId
    )
        => mediator.Send(
            new DeleteFileCommand(
                userId,
                ECourseEntityType.ComponentGrade,
                componentGradeId,
                fileId,
                roles.Contains(AdminRole)
            )
        );
    #endregion

    #region Submission
    [HttpGet]
    [Route("/api/Grading/Submission/File")]
    public async Task<FileStreamResult> SubmissionFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid submissionId,
        [FromQuery] Guid fileId
    )
    {
        var file = await mediator.Send(
            new DownloadFileQuery(
                userId,
                ECourseEntityType.Submission,
                submissionId,
                fileId,
                roles.Contains(AdminRole)
            )
        );

        return new FileStreamResult(file.Stream, file.ContentType) { FileDownloadName = file.FileName };
    }

    [HttpPost]
    [Route("/api/Grading/Submission/File")]
    [RequestSizeLimit(10_485_760)] // 10MB
    public async Task<Guid> SubmissionFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [AllowedTypes("pdf")] IFormFile file,
        [FromForm] Guid submissionId,
        [FromForm] Guid? existingFileId
    )
    {
        await using var fileStream = file.OpenReadStream();
        return await mediator.Send(
            new UploadFileCommand(
                userId,
                ECourseEntityType.Submission,
                submissionId,
                existingFileId,
                file.FileName,
                file.ContentType,
                fileStream,
                roles.Contains(AdminRole)
            )
        );
    }

    [HttpDelete]
    [Route("/api/Grading/Submission/File")]
    public Task DeleteSubmissionFile(
        [FromHeader] Guid userId,
        [FromHeader] string[] roles,
        [FromQuery] Guid submissionId,
        [FromQuery] Guid fileId
    )
        => mediator.Send(
            new DeleteFileCommand(
                userId,
                ECourseEntityType.Submission,
                submissionId,
                fileId,
                roles.Contains(AdminRole)
            )
        );
    #endregion
}