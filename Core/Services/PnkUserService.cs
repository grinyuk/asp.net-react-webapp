using Core.Entities;
using Core.Enums;
using Core.Helpers;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using Core.Models;
using Core.ResourcesFiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class PnkUserService : IPnkUserService
    {
        protected readonly IPnkUserRepository _pnkUserRepository;
        protected readonly AppConfig _appConfig;
        protected readonly UserManager<PnKUser> _userManager;

        public PnkUserService(IPnkUserRepository pnkUserRepository, AppConfig appConfig, UserManager<PnKUser> userManager)
        {
            _pnkUserRepository = pnkUserRepository;
            _appConfig = appConfig;
            _userManager = userManager;
        }

        public bool IsEmailUsed(string email)
        {
            return _pnkUserRepository.IsEmailUsed(email);
        }

        public bool IsNicknameUsed(string nickname)
        {
            return _pnkUserRepository.IsNicknameUsed(nickname);
        }

        public PnKUser? GetUserById(Guid id)
        {
            return _pnkUserRepository.GetUserById(id);
        }

        public ResultActionResponce IsAllowAction(Guid userId, ActionType actionType)
        {
            ConfigActionManager config = null!;

            switch (actionType)
            {
                case ActionType.ConfirmEmail:
                    config = _appConfig?.ConfigEmailActionManager ?? throw new NullReferenceException();
                    break;
                case ActionType.ResetPassword:
                    config = _appConfig?.ConfigPasswordActionManager ?? throw new NullReferenceException();
                    break;
                case ActionType.Login:
                    config = _appConfig?.LoginActionManager ?? throw new NullReferenceException();
                    break;
                case ActionType.TaskAnswerAmount:
                    config = _appConfig?.ConfigTaskAnswerAmountActionManager ?? throw new NullReferenceException();
                    break;
                default:
                    return ResultActionResponce.Error;
            }

            var actions = _pnkUserRepository.GetLastActions(userId, actionType, DateTime.UtcNow - config.TimeAfterAllAttemps);

            if (actions.Count() >= config.NumberOfAttempt)
            {
                var lastAction = actions.OrderBy(x => x.Create).First();
                return new ResultActionResponce(false, string.Format((DefaultLanguage.ResourceManager.GetString(config.ErrorMessage!)!), (config.TimeAfterAllAttemps - (DateTime.UtcNow - lastAction.Create)).ToStringTime()));
            }
            else if (actions.Any())
            {
                var lastAction = actions.OrderBy(x => x.Create).Last();
                if (lastAction.Create >= DateTime.UtcNow - config.TimeBeetwenAttempts)
                {
                    return new ResultActionResponce(false, string.Format((DefaultLanguage.ResourceManager.GetString(config.ErrorMessage!)!), (config.TimeBeetwenAttempts - (DateTime.UtcNow - lastAction.Create)).ToStringTime()));
                }
            }

            CreateActionManager(new UserActionManagment()
            {
                ActionType = actionType,
                UserId = userId,
                Create = DateTime.UtcNow,
            });

            return ResultActionResponce.Success;
        }

        public bool CreateActionManager(IUserActionManagment actionManager)
        {
            return _pnkUserRepository.CreateActionManager(actionManager);
        }

        public async Task<IEnumerable<IUserRating>> GetUsersRatingAsync(bool isCurrentPeriod)
        {
            var users = (await _pnkUserRepository.GetActiveUsersWithScore(isCurrentPeriod))
                        .Select(x =>
                        {
                            return new UserRating
                            {
                                Id = x.Id,
                                Description = x.Description,
                                FullName = string.IsNullOrEmpty(x.FullName) ? x.UserName : x.FullName,
                                ScoreMath = x.ScoreMath,
                                ScorePhysics = x.ScorePhysics
                            };
                        });

            return users;

        }
    }
}
