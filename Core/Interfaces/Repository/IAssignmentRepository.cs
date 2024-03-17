using Core.Models.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    public interface IAssignmentRepository
    {
        IEnumerable<IAssignment> GetAssignments(bool? isActive);
        ICreatedAssignment CreateAssignment(IAssignment assignmentModel);
        Task<ICreatedAssignment> UpdateAssignmentAsync(IAssignment assignmentModel);
        bool SaveOrUpdateUserResult(Guid userId, IUserAnswerModel assignmentUserResult);
        Task<IEnumerable<ITheme>> GetThemes();
        Task<IFile?> GetFile(int assignmentId);
        Task<ISchedule?> GetCurrentScheduleAsync(int assignmentId);
        Task<IEnumerable<IAssignmentUserResult>> GetUserAnswers(Guid userId, IEnumerable<int> assignmentIds);
        Task<IEnumerable<IAssignmentUserResult>> GetUserAnswersByAssignmentId(Guid userId, int assignmentId);
        Task<IEnumerable<IAssignmentAnswer>> GetCorrectAnswers(int assignmentId);
        Task<IEnumerable<IAssignmentUserResult>> GetArchiveNotProcessedResultAsync();
        Task<bool> DeleteAssignment(int? assignmentId);
    }
}
