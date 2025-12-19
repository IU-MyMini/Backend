using BuildingBlocks.Application.File.Download;
using BuildingBlocks.Domain;

using GradingModule.Domain.Entities;

namespace GradingModule.Application.Dtos;

public class StudentGradesDto
{
    public GradeDto Total { get; set; } = default!;

    public ICollection<AssignmentWithGradesDto> Assignments { get; set; } = default!;
}

public class AssignmentWithGradesDto(Assignment a)
{
    public Guid Id { get; set; } = a.Id;

    public LangStr Name { get; set; } = a.Name;
    public LangStr Description { get; set; } = a.Description;
    public IEnumerable<FileInfoDto> Files { get; set; } = [];

    public DateTime? Deadline { get; set; } = a.Deadline;
    public bool IsGroupAssignment { get; set; } = a.IsGroupAssignment;

    public GradeDto TotalGrade { get; set; } = default!;

    public ICollection<ComponentWithGradeDto> Components { get; set; } = default!;
}

public class ComponentWithGradeDto(AssignmentComponent c) : ComponentShortDto(c)
{
    public GradeDto? GroupGrade { get; set; }
    public GradeDto IndividualGrade { get; set; } = default!;

    public SubmissionDto? Submission { get; set; }

    public IEnumerable<PeerReviewStudentDto> OutgoingPeerReviews { get; set; } = default!;
    public IEnumerable<PeerReviewStudentDto> IncomingPeerReviews { get; set; } = default!;
}