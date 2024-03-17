using Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models.Assignment;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using AssignmentAnswer = Core.Models.AssignmentAnswer;
using Schedule = Core.Models.Schedule;
using Microsoft.Extensions.Caching.Memory;
using Core.Entities;

namespace DataBase.Repositories
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly PnkDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly AppConfig _appConfig;

        public AssignmentRepository(PnkDbContext context, IMemoryCache memoryCache, AppConfig appConfig)
        {
            _context = context;
            _memoryCache = memoryCache;
            _appConfig = appConfig;
        }

        public IEnumerable<IAssignment> GetAssignments(bool? isActive)
        {
            var assignments = _context.Assignments
                .Include(x => x.Themes)
                .Include(x => x.Schedules)
                .Include(x => x.Answers)
                .Where(x => !isActive.HasValue || (isActive.Value
                    ? DateTime.UtcNow < x.EndDate && DateTime.UtcNow > x.StarDate
                    : DateTime.UtcNow >= x.EndDate)).ToList();

            return assignments.Select(x => new Core.Models.Assignment.Assignment
            {
                Id = x.Id,
                Title = x.Title,
                Subject = x.Subject,
                Description = x.Description,
                Difficulty = x.Difficulty,
                AuthorName = x.AuthorName,
                AuthorDescription = x.AuthorDescription,
                StartDate = x.StarDate,
                EndDate = x.EndDate,
                Schedules = x.Schedules?.Select(s => new Schedule
                {
                    Period = s.Period,
                    Id = s.Id,
                    AssignmentId = s.AssignmentId,
                    IsScoreCalculated = s.IsScoreCalculated,
                    PeriodStartDate = s.PeriodStartDate,
                    PeriodEndDate = s.PeriodEndDate,
                }),
                Answers = x.Answers?.Select(a => new AssignmentAnswer
                {
                    Id = a.Id,
                    AssignmentId = a.AssignmentId,
                    Answer = a.Answer,
                    MaxScore = a.MaxScore,
                    Description = a.Description
                }),
                AssignmentThemesIds = x.Themes?.Select(t => t.Id),
            }).ToList();
        }

        public ICreatedAssignment CreateAssignment(IAssignment assignmentModel)
        {
            var assignment = new Models.Assignment
            {
                Title = assignmentModel.Title,
                Subject = assignmentModel.Subject,
                Description = assignmentModel.Description,
                Difficulty = assignmentModel.Difficulty,
                AuthorName = assignmentModel.AuthorName,
                AuthorDescription = assignmentModel.AuthorDescription,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null,
                StarDate = assignmentModel.StartDate,
                EndDate = assignmentModel.EndDate,
                File = new AssignmentFile()
                {
                    File = assignmentModel.File?.File,
                    FileName = assignmentModel.File?.FileName,
                    UploadDate = DateTime.UtcNow
                },
                Answers = assignmentModel.Answers?.Select(x => new Models.AssignmentAnswer()
                {
                    Description = x.Description,
                    Answer = x.Answer,
                    MaxScore = x.MaxScore
                }).ToList(),
                Themes = assignmentModel.AssignmentThemesIds != null
                    ? _context.Themes.Where(x => assignmentModel.AssignmentThemesIds.Any(y => y == x.Id) && x.Subject == assignmentModel.Subject).ToList()
                    : null,
                Schedules = assignmentModel.Schedules?.Select(x => new Models.Schedule()
                {
                    Period = x.Period,
                    PeriodStartDate = x.PeriodStartDate,
                    PeriodEndDate = x.PeriodEndDate
                }).ToList()
            };

            _context.Assignments.Add(assignment);
            _context.SaveChanges();

            return toAssignment(assignment);
        }

        public async Task<ICreatedAssignment> UpdateAssignmentAsync(IAssignment assignmentModel)
        {
            var assignmentDb = await _context.Assignments
                                 .Include(x => x.Themes)
                                 .Include(x => x.Schedules)
                                 .Include(x => x.Answers)
                                 .FirstOrDefaultAsync(x => x.Id == assignmentModel.Id);

            if (assignmentDb != null)
            {
                assignmentDb.Title = assignmentModel.Title;
                assignmentDb.Description = assignmentModel.Description;
                assignmentDb.Subject = assignmentModel.Subject;
                assignmentDb.Difficulty = assignmentModel.Difficulty;
                assignmentDb.AuthorName = assignmentModel.AuthorName;
                assignmentDb.AuthorDescription = assignmentModel.AuthorDescription;
                assignmentDb.UpdateDate = DateTime.UtcNow;
                assignmentDb.StarDate = assignmentModel.StartDate;
                assignmentDb.EndDate = assignmentModel.EndDate;

                if (assignmentModel.File?.File != null)
                {
                    var file = await _context.AssignmentFile.FirstOrDefaultAsync(x => x.AssignmentId == assignmentModel.Id);
                    if (file != null)
                    {
                        var ceo = new MemoryCacheEntryOptions()
                                        .SetSlidingExpiration(_appConfig.CacheLifeTime);
                        _memoryCache.Set(nameof(AssignmentFile) + assignmentModel.Id, file, ceo);

                        file.UploadDate = DateTime.UtcNow;
                        file.FileName = assignmentModel.File.FileName;
                        file.File = assignmentModel.File?.File;
                    }
                    else
                    {
                        _context.AssignmentFile.Add(new Models.AssignmentFile()
                        {
                            AssignmentId = assignmentModel.Id,
                            File = assignmentModel.File?.File,
                            FileName = assignmentModel.File?.FileName,
                            UploadDate = DateTime.UtcNow
                        });
                    }
                }

                if (assignmentDb.Answers != null && assignmentModel.Answers != null)
                {
                    assignmentDb.Answers = assignmentDb.Answers.Where(x => assignmentModel.Answers.Any(y => y.Id == x.Id)).ToList();
                    foreach (var answer in assignmentModel.Answers)
                    {
                        var answerDb = assignmentDb.Answers.FirstOrDefault(x => x.Id == answer.Id);
                        if (answerDb != null)
                        {
                            answerDb.Answer = answer.Answer;
                            answerDb.Description = answer.Description;
                            answerDb.MaxScore = answer.MaxScore;
                        }
                        else
                        {
                            assignmentDb.Answers.Add(new Models.AssignmentAnswer()
                            {
                                Answer = answer.Answer,
                                Description = answer.Description,
                                MaxScore = answer.MaxScore
                            });
                        }
                    }
                }

                assignmentDb.Themes = assignmentModel.AssignmentThemesIds != null
                    ? _context.Themes.Where(x => assignmentModel.AssignmentThemesIds.Any(y => y == x.Id) && x.Subject == assignmentModel.Subject).ToList()
                    : null;

                if (assignmentDb.Schedules != null && assignmentModel.Schedules != null)
                {
                    foreach (var scheduleDb in assignmentDb.Schedules)
                    {
                        var schedule = assignmentModel.Schedules.FirstOrDefault(x => x.Period == scheduleDb.Period);
                        if (schedule != null)
                        {
                            scheduleDb.PeriodStartDate = schedule.PeriodStartDate;
                            scheduleDb.PeriodEndDate = schedule.PeriodEndDate;
                        }
                    }
                }
            }

            _context.SaveChanges();

            return toAssignment(assignmentDb);
        }

        private ICreatedAssignment toAssignment(Models.Assignment? assignment)
        {
            return new CreatedAssignment()
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Subject = assignment.Subject,
                Description = assignment.Description,
                Difficulty = assignment.Difficulty,
                AuthorName = assignment.AuthorName,
                AuthorDescription = assignment.AuthorDescription,
                StartDate = assignment.StarDate,
                EndDate = assignment.EndDate,
                Answers = assignment.Answers?.Select(x => new AssignmentAnswer
                {
                    Id = x.Id,
                    AssignmentId = x.AssignmentId,
                    Answer = x.Answer,
                    MaxScore = x.MaxScore,
                    Description = x.Description
                }),
                AssignmentThemesIds = assignment.Themes?.Select(x => new Theme
                {
                    Id = x.Id,
                    Subject = x.Subject,
                    Value = x.Value,
                    IsActive = x.IsActive,
                    Assignments = x.Assignments
                }),
                Schedules = assignment.Schedules?.Select(x => new Core.Models.Schedule
                {
                    Id = x.Id,
                    AssignmentId = x.AssignmentId,
                    PeriodStartDate = x.PeriodStartDate,
                    PeriodEndDate = x.PeriodEndDate,
                    Period = x.Period,
                    IsScoreCalculated = x.IsScoreCalculated
                })
            };
        }

        public async Task<ISchedule?> GetCurrentScheduleAsync(int assignmentId)
        {
            var currentTime = DateTime.UtcNow;
            var schedule = await _context.Schedule.FirstOrDefaultAsync(x => x.AssignmentId == assignmentId && x.PeriodStartDate <= currentTime && x.PeriodEndDate > currentTime);
            return schedule;
        }

        public bool SaveOrUpdateUserResult(Guid userId, IUserAnswerModel assignmentUserResult)
        {
            if (assignmentUserResult?.Answers != null && assignmentUserResult.Answers.Any())
            {
                foreach (var answer in assignmentUserResult.Answers)
                {
                    var dbAnswer = _context.AssignmentUserResults
                        .FirstOrDefault(x => x.AssignmentId.Equals(assignmentUserResult.AssignmentId) &&
                                             x.UserId.Equals(userId) &&
                                             x.AnswerId.Equals(answer.AnswerId));

                    if (answer.Answer.HasValue)
                    {
                        if (dbAnswer != null)
                        {
                            if (dbAnswer.Score == null)
                            {
                                dbAnswer.Answer = answer.Answer.Value;
                                dbAnswer.Period = answer.Period;
                                dbAnswer.IsProcessed = false;
                                dbAnswer.CreateDate = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            var userResult = new Models.AssignmentUserResult
                            {
                                UserId = userId,
                                AnswerId = answer.AnswerId,
                                Period = answer.Period,
                                Answer = answer.Answer.Value,
                                AssignmentId = assignmentUserResult.AssignmentId,
                                CreateDate = DateTime.UtcNow
                            };

                            _context.AssignmentUserResults.Add(userResult);
                        }
                    }
                }
                _context.SaveChanges();
            }
            return true;
        }

        public async Task<IEnumerable<ITheme>> GetThemes()
        {
            return await _context.Themes.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<IFile?> GetFile(int assignmentId)
        {
            if (_memoryCache.TryGetValue(nameof(AssignmentFile) + assignmentId, out IFile? file))
            {
                return file!;
            }

            file = await _context.AssignmentFile.FirstOrDefaultAsync(x => x.AssignmentId == assignmentId);
            var ceo = new MemoryCacheEntryOptions()
               .SetSlidingExpiration(_appConfig.CacheLifeTime);
            _memoryCache.Set(nameof(AssignmentFile) + assignmentId, file, ceo);
            return file;
        }

        public async Task<IEnumerable<IAssignmentUserResult>> GetUserAnswers(Guid userId, IEnumerable<int> assignmentIds)
        {
            return await _context.AssignmentUserResults.Where(x => x.UserId == userId && assignmentIds.Contains(x.AssignmentId)).ToListAsync();
        }

        public async Task<IEnumerable<IAssignmentUserResult>> GetUserAnswersByAssignmentId(Guid userId, int assignmentId)
        {
            return await _context.AssignmentUserResults
                .Where(y => y.UserId == userId && y.AssignmentId == assignmentId)
                .Include(x => x.AssignmentAnswer)
                .ToListAsync();
        }

        public async Task<IEnumerable<IAssignmentAnswer>> GetCorrectAnswers(int assignmentId)
        {
            var arr = _context.AssignmentAnswer
                .Where(y => y.AssignmentId == assignmentId)
                .Select(x => new AssignmentAnswer
                {
                    Id = x.Id,
                    AssignmentId = x.AssignmentId,
                    Answer = x.Answer,
                    MaxScore = x.MaxScore,
                    Description = x.Description
                });

            return await arr.ToListAsync();
        }

        public async Task<IEnumerable<IAssignmentUserResult>> GetArchiveNotProcessedResultAsync()
        {
            var archiveIds = _context.Assignments.Where(x => x.EndDate < DateTime.UtcNow).Select(x => x.Id);
            var userAnswers = await _context.AssignmentUserResults
                                            .Where(x => !x.IsProcessed && archiveIds.Any(y => y == x.AssignmentId))
                                            .Include(x => x.AssignmentAnswer).ToListAsync();
            return userAnswers;
        }

        public async Task<bool> DeleteAssignment(int? assignmentId)
        {
            var assignment = await _context.Assignments.FirstOrDefaultAsync(x => x.Id == assignmentId);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

    }
}