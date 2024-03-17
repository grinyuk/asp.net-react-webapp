using Core.Models.Assignment;

namespace Core.Interfaces.Service
{
    public interface IAssignmentService
    {
        ICreatedAssignment CreateAssignment(IAssignment assignmentModel);
        Task<ICreatedAssignment> UpdateAssignmentAsync(IAssignment assignmentModel);
        bool SaveUserResult(Guid userId, IUserAnswerModel assignmentUserResult);
        Task<IEnumerable<AssignmentAdminResponse>> GetAssignmentsAdmin();
        Task<IEnumerable<AssignmentResponse>> GetAssignments(Guid userId, bool? isActive);
        Task<IEnumerable<ITheme>> GetThemes();
        Task<IFile?> GetFile(int assignmentId);
        Task<bool> DeleteAssignment(int? assignmentId);
    }
}
