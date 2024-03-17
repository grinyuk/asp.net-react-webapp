namespace Core.Interfaces;

public interface IAssignmentUserResultModel
{
    int AnswerId { get; set; }
    float? Answer { get; set; }
    int AssignmentId { get; set; }
    int Period { get; set; }
}