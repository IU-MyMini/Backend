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

    public static class Assignment
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }

    public static class Course
    {
        public static readonly AppError NotFound = new(ErrorCode.EntityNotFound);
        public static readonly AppError NotAllowed = new(ErrorCode.Forbidden);
    }
}