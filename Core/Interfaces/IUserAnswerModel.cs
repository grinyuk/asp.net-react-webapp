namespace Core.Interfaces;

public interface IUserAnswerModel
{
    int AssignmentId { get; set; }
    IEnumerable<IAssignmentUserResultModel>? Answers { get; }
}