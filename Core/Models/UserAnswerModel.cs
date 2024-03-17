using Core.Interfaces;

namespace Core.Models;

public class UserAnswerModel : IUserAnswerModel
{
    public int AssignmentId { get; set; }
    public List<AssignmentUserResultModel>? Answers { get; set; }
    IEnumerable<IAssignmentUserResultModel>? IUserAnswerModel.Answers => Answers;
}