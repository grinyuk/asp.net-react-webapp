namespace Core.Interfaces;

public interface IAssignmentAnswer
{
    int Id { get; set; }
    int AssignmentId { get; set; }
    float Answer { get; set; }
    int MaxScore { get; set; }
    string? Description { get; set; }
}