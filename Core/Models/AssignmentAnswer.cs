using Core.Interfaces;

namespace Core.Models;

public class AssignmentAnswer : IAssignmentAnswer
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public float Answer { get; set; }
    public int MaxScore { get; set; }
    public string? Description { get; set; }
}