using Core.Enums;

namespace Core.Models.Assignment;

public class ActiveAssignmentAnswer
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public int CurrentScore { get; set; }
    public string? Description { get; set; }
    public float? Value { get; set; }
    public ResultType ResultType { get; set; }
    public int? UserScore { get; set; }
    
}