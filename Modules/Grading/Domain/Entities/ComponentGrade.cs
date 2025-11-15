namespace GradingModule.Domain.Entities;

public class ComponentGrade
{
    public int? Grade { get; set; }

    public Guid                ComponentId { get; set; }
    public AssignmentComponent Component   { get; set; } = default!;
}