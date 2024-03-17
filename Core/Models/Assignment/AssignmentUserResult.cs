using Core.Interfaces;

namespace Core.Models.Assignment;

public class AssignmentUserResult : IAssignmentUserResult
{
    public int AssignmentId { get; set; }
    
    public int AnswerId { get; set; }
        
    public DateTime CreateDate { get; set; }
        
    public float Answer { get; set; }
        
    public bool IsProcessed { get; set; }
        
    public int? Score { get; set; }
    public int Period { get; set; }
    public Guid UserId { get; set; }
    public long Id { get; set; }
    public IAssignmentAnswer? AssignmentAnswer { get; }
}