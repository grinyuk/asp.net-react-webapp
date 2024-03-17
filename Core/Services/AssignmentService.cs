using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Models.Assignment;

namespace Core.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IGlobalSettingsService _globalSettingsService;

        public AssignmentService(IAssignmentRepository assignmentRepository,
            IGlobalSettingsService globalSettingsService)
        {
            _assignmentRepository = assignmentRepository;
            _globalSettingsService = globalSettingsService;
        }

        public ICreatedAssignment CreateAssignment(IAssignment assignmentModel)
        {
            assignmentModel.Schedules = createSchedules(assignmentModel);
            return _assignmentRepository.CreateAssignment(assignmentModel);
        }

        public async Task<ICreatedAssignment> UpdateAssignmentAsync(IAssignment assignmentModel)
        {
            assignmentModel.Schedules = createSchedules(assignmentModel);
            return await _assignmentRepository.UpdateAssignmentAsync(assignmentModel);
        }

        public bool SaveUserResult(Guid userId, IUserAnswerModel assignmentUserResult)
        {
            var currentSchedule = _assignmentRepository.GetCurrentScheduleAsync(assignmentUserResult.AssignmentId).Result;
            var periodOption = _globalSettingsService.GetAssignmentOption().AssignmentPeriodsOption!;

            _assignmentRepository.SaveOrUpdateUserResult(userId,
                new UserAnswerModel()
                {
                    AssignmentId = assignmentUserResult.AssignmentId,
                    Answers = assignmentUserResult.Answers?
                        .Where(x => x.Answer.HasValue)
                        .Select(x => new AssignmentUserResultModel()
                        {
                            Answer = x.Answer,
                            AnswerId = x.AnswerId,
                            Period = currentSchedule != null ? currentSchedule.Period : periodOption.Count + 1
                        })
                        .ToList()
                });

            return true;
        }

        public async Task<IEnumerable<AssignmentAdminResponse>> GetAssignmentsAdmin()
        {
            var assignmentsOptions = _globalSettingsService.GetAssignmentOption()!;
            
            var assignments = _assignmentRepository.GetAssignments(null).Select(x =>
            {
                var periodX = (x.EndDate - x.StartDate).TotalMinutes / assignmentsOptions.AssignmentPeriodsOption.Sum();
                var elapsedMinutes = (DateTime.UtcNow - x.StartDate).TotalMinutes;
                var period = 1;

                foreach (var opt in assignmentsOptions.AssignmentPeriodsOption)
                {
                    elapsedMinutes -= opt * periodX;
                    if (elapsedMinutes < 0)
                    {
                        break;
                    }

                    period++;
                }

                return new AssignmentAdminResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Subject = x.Subject,
                    Description = x.Description,
                    Difficulty = x.Difficulty,
                    AuthorName = x.AuthorName,
                    AuthorDescription = x.AuthorDescription,
                    Period = period,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Answers = x.Answers.Select(a => new AssignmentAnswer
                    {
                        Id = a.Id,
                        AssignmentId = a.AssignmentId,
                        Answer = a.Answer,
                        MaxScore = a.MaxScore,
                        Description = a.Description
                    }),
                    AssignmentThemesIds = x.AssignmentThemesIds
                };
            });
            
            return assignments;
        }
        
        public async Task<IEnumerable<AssignmentResponse>> GetAssignments(Guid userId, bool? isActive)
        {
            var assignmentsOptions = _globalSettingsService.GetAssignmentOption()!;

            var assignments = _assignmentRepository.GetAssignments(isActive).Select(x =>
            {
                var periodX = (x.EndDate - x.StartDate).TotalMinutes / assignmentsOptions.AssignmentPeriodsOption.Sum();
                var elapsedMinutes = (DateTime.UtcNow - x.StartDate).TotalMinutes;
                var period = 1;

                var timeToArchive = (int)(x.EndDate - DateTime.UtcNow).TotalMinutes;

                foreach (var opt in assignmentsOptions.AssignmentPeriodsOption)
                {
                    elapsedMinutes -= opt * periodX;
                    if (elapsedMinutes < 0)
                    {
                        break;
                    }

                    period++;
                }

                return new AssignmentResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Difficulty = x.Difficulty,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Subject = x.Subject,
                    Status = AssigmentStatus.NotDone,
                    AuthorName = x.AuthorName,
                    AuthorDescription = x.AuthorDescription,
                    TimeToArchive = timeToArchive,
                    MinutesLeft = period > assignmentsOptions.AssignmentPeriodsOption.Count ? 0 :
                                            (int)(((x.Schedules?.FirstOrDefault(y => y.Period == period)?.PeriodEndDate ?? DateTime.UtcNow) - DateTime.UtcNow).TotalMinutes),
                    Period = period,
                    MaxPeriod = assignmentsOptions.AssignmentPeriodsOption.Count(),
                    Answers = x.Answers?.Select(a => new ActiveAssignmentAnswer
                    {
                        Id = a.Id,
                        AssignmentId = a.AssignmentId,
                        CurrentScore = period <= assignmentsOptions.AssignmentPeriodsOption.Count ?
                                        a.MaxScore * assignmentsOptions.AssignmentPeriodsScoreOption[period - 1] / 100 :
                                        a.MaxScore * assignmentsOptions.AssignmentArchivePeriodScoreOption / 100,
                        Description = a.Description,
                    }),
                    AssignmentThemesIds = x.AssignmentThemesIds?.Select(t => t)?.ToList()
                };
            });

            var results = await _assignmentRepository.GetUserAnswers(userId, assignments.Select(x => x.Id));

            if (results != null && results.Any())
            {
                assignments = assignments.Select(assignment =>
                {
                    if (assignment.Answers != null)
                    {
                        assignment.Answers = assignment.Answers.Select(x =>
                        {
                            foreach (var result in results)
                            {
                                if (result.AnswerId == x.Id)
                                {
                                    x.Value = result.Answer;
                                    x.ResultType = result.IsProcessed ? result.Score != null ? ResultType.Success : ResultType.Fail : ResultType.Input;
                                    x.UserScore = result.Score;
                                }
                            }

                            return x;
                        });

                        if(assignment.Answers.All(x => x.ResultType == ResultType.Success))
                        {
                            assignment.Status = AssigmentStatus.Done;
                        }
                        else if(assignment.Answers.All(x => x.ResultType == ResultType.Fail))
                        {
                            assignment.Status = AssigmentStatus.WronglyDone;
                        }
                        else if (!assignment.Answers.Any(x => x.ResultType == ResultType.Input || x.ResultType == ResultType.None))
                        {
                            assignment.Status = AssigmentStatus.PartlyDone;
                        }
                    }

                    return assignment;
                });
            }

            return assignments;
        }

        public async Task<IEnumerable<ITheme>> GetThemes()
        {
            return await _assignmentRepository.GetThemes();
        }

        private IEnumerable<ISchedule> createSchedules(IAssignment assignmentModel)
        {
            var periodOption = _globalSettingsService.GetAssignmentOption().AssignmentPeriodsOption!;
            var assignmentDuration = (assignmentModel.EndDate - assignmentModel.StartDate).TotalMinutes;
            var coefficientDuration = assignmentDuration / periodOption.Sum();
            var periodEnd = assignmentModel.StartDate;
            var schedules = new List<Schedule>();
            var period = 1;

            foreach (var option in periodOption)
            {
                var periodStart = periodEnd;
                periodEnd = periodStart.AddMinutes(coefficientDuration * option);

                schedules.Add(new Schedule
                {
                    PeriodStartDate = periodStart,
                    PeriodEndDate = periodEnd,
                    Period = period++
                });
            }

            return schedules;
        }

        public async Task<IFile?> GetFile(int assignmentId)
        {
            return await _assignmentRepository.GetFile(assignmentId);
        }

        public async Task<bool> DeleteAssignment(int? assignmentId)
        {
            return await _assignmentRepository.DeleteAssignment(assignmentId);
        }

    }
}
