using BuildingBlocks.Domain.Exceptions;

namespace GradingModule.Domain;

[AutoMapErrors]
public static class Errors
{
    public static class User
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }

    public static class File
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
    }

    public static class Grade
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError GroupAndParticipantIdsBothNull = new(ErrorCode.BadRequest);
        public static readonly AppError GroupAndParticipantIdsBothNotNull = new(ErrorCode.BadRequest);
    }

    public static class Assignment
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }

    public static class AssignmentComponent
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }

    public static class Course
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }

    public static class Group
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }

    public static class CourseParticipant
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
        public static readonly AppError AlreadyAdded = new(ErrorCode.BadRequest);
    }

    public static class Submission
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }
}