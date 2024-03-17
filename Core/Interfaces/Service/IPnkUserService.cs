using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface IPnkUserService
    {
        bool IsEmailUsed(string email);
        bool IsNicknameUsed(string nickname);
        ResultActionResponce IsAllowAction(Guid userId, ActionType actionType);
        PnKUser? GetUserById(Guid id);
        bool CreateActionManager(IUserActionManagment actionManager);
        Task<IEnumerable<IUserRating>> GetUsersRatingAsync(bool isCurrentPeriod);
    }
}
