using Core.Interfaces;

namespace Core.Models;

public class AssignmentUserResultModel : IAssignmentUserResultModel
{
    public int AnswerId { get; set; }
    public float? Answer { get; set; }
    public int AssignmentId { get; set; }
    public int Period { get; set; }
}