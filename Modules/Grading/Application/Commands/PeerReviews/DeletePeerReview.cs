using GradingModule.Domain;
using GradingModule.Infrastructure;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace GradingModule.Application.Commands.PeerReviews;

public record DeletePeerReviewCommand(Guid UserId, Guid PeerReviewId, bool IsAdmin) : IRequest;

public class DeletePeerReviewCommandHandler(GradingContext context) : IRequestHandler<DeletePeerReviewCommand>
{
    public async Task Handle(DeletePeerReviewCommand request, CancellationToken cancellationToken)
    {
        var peerReview = await context.PeerReviews.Include(r => r.SourceComponent)
                             .ThenInclude(c => c.Assignment)
                             .ThenInclude(a => a.Course)
                             .ThenInclude(c => c.Teachers)
                             .SingleOrDefaultAsync(r => r.Id.Equals(request.PeerReviewId), cancellationToken)
                         ?? throw Errors.PeerReview.NotFound;

        if (!request.IsAdmin
            && !peerReview.SourceComponent.Assignment.Course.Teachers.Any(t => t.UserId.Equals(request.UserId)))
            throw Errors.PeerReview.NotAllowed;

        context.PeerReviews.Remove(peerReview);

        await context.SaveChangesAsync(cancellationToken);
    }
}