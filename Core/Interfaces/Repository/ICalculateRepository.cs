using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    public interface ICalculateRepository
    {
        IEnumerable<ISchedule> GetNoCalculatePeriods();
        IEnumerable<IAssignmentUserResult> GetNoCalculateAnswerUser(IEnumerable<int> assignmrntIds );
        Task UpdateUserResultAsync(IAssignmentUserResult userAnswer);
        Task UpdateScheduleIsCalculate(int scheduleId, bool IsProcessed);
        Task SetRecalculationAsync(int? assignmentId);
    }
}
