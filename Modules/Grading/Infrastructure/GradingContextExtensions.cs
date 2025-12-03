using GradingModule.Domain;
using GradingModule.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Infrastructure;

public static class GradingContextExtensions
{
    /// <summary>
    /// Gets the list of file ids for specific course component.
    /// If userId is not null, additionally check the permission for this user.
    /// If forViewing is true, there is no difference between CourseParticipants and Teachers
    /// when checking permission
    /// </summary>
    public static async Task<IList<Guid>> GetExistingFileIdsAsync(
        this GradingContext context,
        ECourseEntityType courseEntityType,
        Guid courseEntityId,
        Guid? userId,
        bool forViewing,
        CancellationToken cancellationToken = default
    )
        => courseEntityType switch
        {
            ECourseEntityType.Assignment => (await context.Assignments.Where(a => a.Id.Equals(courseEntityId))
                                                .Where(
                                                    a => userId == null
                                                         || a.Course.Teachers.Any(t => t.UserId.Equals(userId))
                                                         || forViewing
                                                         && a.Course.CourseParticipants.Any(
                                                             p => p.UserId.Equals(userId)
                                                         )
                                                )
                                                .SingleOrDefaultAsync(cancellationToken))?.FileIds
                                            ?? throw Errors.Assignment.NotFound,
            ECourseEntityType.AssignmentComponent => (await context.AssignmentComponents
                                                         .Where(c => c.Id.Equals(courseEntityId))
                                                         .Where(
                                                             c => userId == null
                                                                  || c.Assignment.Course.Teachers.Any(
                                                                      t => t.UserId.Equals(userId)
                                                                  )
                                                                  || forViewing
                                                                  && c.Assignment.Course.CourseParticipants.Any(
                                                                      p => p.UserId.Equals(userId)
                                                                  )
                                                         )
                                                         .SingleOrDefaultAsync(cancellationToken))?.FileIds
                                                     ?? throw Errors.AssignmentComponent.NotFound,
            ECourseEntityType.ComponentGrade => (await context.ComponentGrades.Where(g => g.Id.Equals(courseEntityId))
                                                    .Where(
                                                        g => userId == null
                                                             || g.Component.Assignment.Course.Teachers.Any(
                                                                 t => t.UserId.Equals(userId)
                                                             )
                                                             || forViewing
                                                             && g.Component.Assignment.Course.CourseParticipants.Any(
                                                                 p => p.UserId.Equals(userId)
                                                             )
                                                    )
                                                    .SingleOrDefaultAsync(cancellationToken))?.FileIds
                                                ?? throw Errors.Grade.NotFound,
            ECourseEntityType.Submission => (await context.Submissions.Where(s => s.Id.Equals(courseEntityId))
                                                .Where(
                                                    s => userId == null
                                                         || s.SubmittedByParticipant != null
                                                         && s.SubmittedByParticipant.UserId.Equals(userId)
                                                         || s.SubmittedByGroup != null
                                                         && s.SubmittedByGroup.Members.Any(m => m.UserId.Equals(userId))
                                                         || forViewing
                                                         && s.Component.Assignment.Course.Teachers.Any(
                                                             t => t.UserId.Equals(userId)
                                                         )
                                                )
                                                .SingleOrDefaultAsync(cancellationToken))?.FileIds
                                            ?? throw Errors.Submission.NotFound,
            _ => throw new ArgumentOutOfRangeException(nameof(courseEntityType))
        };

    public static async Task<string> GetComponentNameAsync(
        this GradingContext context,
        ECourseEntityType courseEntityType,
        Guid courseEntityId,
        CancellationToken cancellationToken = default
    )
    {
        var entityName = courseEntityType switch
        {
            ECourseEntityType.Assignment => await context.Assignments.Where(a => a.Id.Equals(courseEntityId))
                                                .Select(a => a.Name)
                                                .SingleOrDefaultAsync(cancellationToken)
                                            ?? throw Errors.Assignment.NotFound,
            ECourseEntityType.AssignmentComponent => await context.AssignmentComponents
                                                         .Where(c => c.Id.Equals(courseEntityId))
                                                         .Select(c => c.Name)
                                                         .SingleOrDefaultAsync(cancellationToken)
                                                     ?? throw Errors.AssignmentComponent.NotFound,
            ECourseEntityType.ComponentGrade => await context.ComponentGrades.Where(g => g.Id.Equals(courseEntityId))
                                                    .Select(g => g.Component.Name)
                                                    .SingleOrDefaultAsync(cancellationToken)
                                                ?? throw Errors.Grade.NotFound,
            ECourseEntityType.Submission => await context.Submissions.Where(s => s.Id.Equals(courseEntityId))
                                                .Select(s => s.Component.Name)
                                                .SingleOrDefaultAsync(cancellationToken)
                                            ?? throw Errors.Submission.NotFound,
            _ => throw new ArgumentOutOfRangeException(nameof(courseEntityType), courseEntityType, null)
        };

        var entityNameEn = entityName.Translate();

        return courseEntityType switch
        {
            ECourseEntityType.Assignment => entityNameEn ?? nameof(ECourseEntityType.Assignment),
            ECourseEntityType.AssignmentComponent => entityNameEn ?? nameof(ECourseEntityType.AssignmentComponent),
            ECourseEntityType.ComponentGrade => entityNameEn + "Feedback",
            ECourseEntityType.Submission => entityNameEn + "Submission",
            _ => throw new ArgumentOutOfRangeException(nameof(courseEntityType), courseEntityType, null)
        };
    }
}