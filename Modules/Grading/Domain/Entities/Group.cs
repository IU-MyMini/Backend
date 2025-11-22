using BuildingBlocks.Domain;

namespace GradingModule.Domain.Entities;

public class Group : Entity<Guid>
{
    public LangStr Name { get; set; } = default!;
    public LangStr? Description { get; set; }
    public bool IsActive { get; set; } = true; // todo: probably remove and make it soft-deletable

    public Guid CourseId { get; set; }
    public Course Course { get; set; } = default!;
    public ICollection<CourseParticipant> Members { get; set; } = default!;
    public ICollection<ComponentGrade> Grades { get; set; } = default!;
    public ICollection<Submission> Submissions { get; set; } = default!;
    public ICollection<PeerReview> AuthoredPeerReviews { get; set; } = default!;
}