using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Core.Models;
using Core.Models.Assignment;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Repositories
{
    public class CalculateRepository : ICalculateRepository
    {
        private readonly PnkDbContext _context;
        public CalculateRepository(PnkDbContext context)
        {
            _context = context;
        }
        public IEnumerable<ISchedule> GetNoCalculatePeriods()
        {
            var arr = _context.Schedule
                .Where(x => x.PeriodEndDate <= DateTime.UtcNow && x.IsScoreCalculated == false)
                .Select(x => new Core.Models.Schedule
                {
                    Id = x.Id,
                    AssignmentId = x.AssignmentId,
                    PeriodStartDate = x.PeriodStartDate,
                    PeriodEndDate = x.PeriodEndDate,
                    Period = x.Period,
                    IsScoreCalculated = x.IsScoreCalculated,
                });

            return arr.ToList();
        }
        public IEnumerable<IAssignmentUserResult> GetNoCalculateAnswerUser(IEnumerable<int> assignmrntIds)
        {
            var arr = _context.AssignmentUserResults
                .Where(x => x.IsProcessed == false && assignmrntIds.Contains(x.AssignmentId))
                .Select(x => new Core.Models.Assignment.AssignmentUserResult
                {
                    Answer = x.Answer,
                    AnswerId = x.AnswerId,
                    AssignmentId = x.AssignmentId,
                    CreateDate = x.CreateDate,
                    Id = x.Id,
                    IsProcessed = x.IsProcessed,
                    Score = x.Score,
                    UserId = x.UserId
                });
            return arr.ToList();
        }

        public async Task UpdateUserResultAsync(IAssignmentUserResult userAnswer)
        {
            if (userAnswer != null)
            {
                var dbUserAnswer = await _context.AssignmentUserResults.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.AnswerId == userAnswer.AnswerId && x.UserId == userAnswer.UserId);
                if (dbUserAnswer != null && !dbUserAnswer.IsProcessed)
                {
                    dbUserAnswer.IsProcessed = userAnswer.IsProcessed;
                    dbUserAnswer.Score = userAnswer.Score;

                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task UpdateScheduleIsCalculate(int scheduleId, bool isCalculate)
        {
            var dbSchedule = await _context.Schedule
                    .FirstOrDefaultAsync(x => x.Id.Equals(scheduleId));
            if (dbSchedule != null)
            {
                dbSchedule.IsScoreCalculated = isCalculate;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetRecalculationAsync(int? assignmentId)
        {
            var assignmentAnswers = _context.AssignmentUserResults.Where(x => x.AssignmentId == assignmentId).ToList();
            var assignment = _context.Assignments.Include(x => x.Schedules).FirstOrDefault(x => x.Id == assignmentId);

            if(assignment?.EndDate > DateTime.UtcNow)
            {
                foreach (var schedule in assignment?.Schedules)
                {
                    schedule.IsScoreCalculated = false;
                }
            }

            foreach (var assignmentAnswer in assignmentAnswers)
            {
                assignmentAnswer.IsProcessed = false;
                assignmentAnswer.Score = null;
            }
            await _context.SaveChangesAsync();
        }
    }
}
