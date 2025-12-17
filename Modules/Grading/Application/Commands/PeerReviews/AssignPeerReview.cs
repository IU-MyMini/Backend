using GradingModule.Domain;
using GradingModule.Domain.Entities;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.PeerReviews;

public record AssignPeerReviewCommand(
    Guid UserId,
    Guid SourceComponentId,
    Guid TargetComponentId,
    Guid SourceGroupId,
    Guid TargetGroupId,
    bool IsAdmin
) : IRequest<Guid>;

public class AssignPeerReviewCommandHandler(GradingContext context) : IRequestHandler<AssignPeerReviewCommand, Guid>
{
    public async Task<Guid> Handle(AssignPeerReviewCommand request, CancellationToken cancellationToken)
    {
        var sourceComponent = await context.AssignmentComponents
                                  .Include(assignmentComponent => assignmentComponent.Assignment)
                                  .ThenInclude(assignment => assignment.Course)
                                  .ThenInclude(course => course.Teachers)
                                  .SingleOrDefaultAsync(c => c.Id.Equals(request.SourceComponentId), cancellationToken)
                              ?? throw Errors.AssignmentComponent.NotFound;

        var targetComponent
            = await context.AssignmentComponents.Include(assignmentComponent => assignmentComponent.Assignment)
                  .SingleOrDefaultAsync(c => c.Id.Equals(request.TargetComponentId), cancellationToken)
              ?? throw Errors.AssignmentComponent.NotFound;

        var sourceGroup
            = await context.Groups.SingleOrDefaultAsync(g => g.Id.Equals(request.SourceGroupId), cancellationToken)
              ?? throw Errors.Group.NotFound;

        var targetGroup
            = await context.Groups.SingleOrDefaultAsync(g => g.Id.Equals(request.TargetGroupId), cancellationToken)
              ?? throw Errors.Group.NotFound;

        if (sourceComponent.Assignment.CourseId != targetComponent.Assignment.CourseId
            || targetComponent.Assignment.CourseId != sourceGroup.CourseId
            || sourceGroup.CourseId != targetGroup.CourseId)
            throw Errors.PeerReview.CoursesMismatch;

        if (!request.IsAdmin && !sourceComponent.Assignment.Course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.Course.NotAllowed;

        var peerReview = new PeerReview
        {
            Id = Guid.NewGuid(),
            SourceComponentId = request.SourceComponentId,
            TargetComponentId = request.TargetComponentId,
            SourceGroupId = request.SourceGroupId,
            TargetGroupId = request.TargetGroupId,
        };

        await context.AddAsync(peerReview, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        return peerReview.Id;
    }
}