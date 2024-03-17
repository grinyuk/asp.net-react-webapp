using Core.Helpers;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Models;
using Core.Models.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class CalculateService : ICalculateService
    {
        private readonly ICalculateRepository _scheduleCalculateRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IGlobalSettingsService _globalSettingsService;

        public CalculateService(ICalculateRepository scheduleCalculateRepository,
                                IAssignmentRepository assignmentRepository,
                                IGlobalSettingsService globalSettingsService)
        {
            _scheduleCalculateRepository = scheduleCalculateRepository;
            _assignmentRepository = assignmentRepository;
            _globalSettingsService = globalSettingsService;
        }


        public async Task<bool> StartScoreCalculateAsync()
        {
            try
            {
                var assignmentOption = _globalSettingsService.GetAssignmentOption();
                var schedules = _scheduleCalculateRepository.GetNoCalculatePeriods();
                var assignmentUserResultModel = _scheduleCalculateRepository.GetNoCalculateAnswerUser(schedules.Select(x => x.AssignmentId).ToList());

                foreach (var schedule in schedules)
                {
                    foreach (var answer in assignmentUserResultModel)
                    {
                        if (answer.AssignmentId == schedule.AssignmentId)
                        {
                            var userAnswers = await _assignmentRepository.GetUserAnswersByAssignmentId(answer.UserId, answer.AssignmentId);

                            if (userAnswers != null && userAnswers.Any())
                            {
                                foreach (var userAnswer in userAnswers)
                                {
                                    if (userAnswer?.Answer != null && !userAnswer.IsProcessed && schedule.AssignmentId == userAnswer.AssignmentId && userAnswer.Period == schedule.Period)
                                    {
                                        if ((userAnswer.Answer - 1 / Math.Pow(10, assignmentOption.AssignmentAnswerAccuracyOption) <= userAnswer?.AssignmentAnswer?.Answer
                                           && userAnswer?.AssignmentAnswer?.Answer <= userAnswer.Answer + 1 / Math.Pow(10, assignmentOption.AssignmentAnswerAccuracyOption)) || userAnswer?.Answer == userAnswer?.AssignmentAnswer?.Answer)
                                        {
                                            userAnswer.Score = (int)(userAnswer.AssignmentAnswer.MaxScore * assignmentOption.AssignmentPeriodsScoreOption[userAnswer.Period - 1] / 100);
                                        }
                                        userAnswer.IsProcessed = true;
                                        await _scheduleCalculateRepository.UpdateUserResultAsync(userAnswer);
                                    }
                                }
                            }
                        }
                    }
                    await _scheduleCalculateRepository.UpdateScheduleIsCalculate(schedule.Id, true);
                }
                //_ = Logger.Instance.LogInfoAsync(nameof(CalculateService) + Constants.Arrow + nameof(StartScoreCalculateAsync) + " Calculate Finish");
                return true;
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(CalculateService) + Constants.Arrow + nameof(StartScoreCalculateAsync), ex);
                return false;
            }
        }

        public async Task<bool> StartArchiveScoreCalculateAsync()
        {
            try
            {
                var assignmentOption = _globalSettingsService.GetAssignmentOption();
                var userResults = await _assignmentRepository.GetArchiveNotProcessedResultAsync();

                foreach (var userAnswer in userResults)
                {
                    if (userAnswer?.Answer != null && userAnswer.AssignmentAnswer != null)
                    {
                        if ((userAnswer.Answer - 1 / Math.Pow(10, assignmentOption.AssignmentAnswerAccuracyOption) <= userAnswer?.AssignmentAnswer?.Answer
                                          && userAnswer?.AssignmentAnswer?.Answer <= userAnswer.Answer + 1 / Math.Pow(10, assignmentOption.AssignmentAnswerAccuracyOption)) || userAnswer?.Answer == userAnswer?.AssignmentAnswer?.Answer)
                        {
                            if (userAnswer.Period <= assignmentOption.AssignmentPeriodsScoreOption.Count)
                            {
                                userAnswer.Score = (int)(userAnswer.AssignmentAnswer.MaxScore * assignmentOption.AssignmentPeriodsScoreOption[userAnswer.Period - 1] / 100);
                            }
                            else
                            {
                                userAnswer.Score = (int)(userAnswer.AssignmentAnswer.MaxScore * assignmentOption.AssignmentArchivePeriodScoreOption / 100);
                            }
                        }
                        
                        userAnswer.IsProcessed = true;
                        await _scheduleCalculateRepository.UpdateUserResultAsync(userAnswer);
                    }
                }
                //_ = Logger.Instance.LogInfoAsync(nameof(CalculateService) + Constants.Arrow + nameof(StartArchiveScoreCalculateAsync) + " Calculate Finish");
                return true;
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(CalculateService) + Constants.Arrow + nameof(StartArchiveScoreCalculateAsync), ex);
                return false;
            }

        }

        public async Task SetRecalculation(int? assignmentId)
        {
            await _scheduleCalculateRepository.SetRecalculationAsync(assignmentId);
            await StartScoreCalculateAsync();
            await StartArchiveScoreCalculateAsync();
        }

    }
}
