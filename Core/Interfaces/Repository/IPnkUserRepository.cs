using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    public interface IPnkUserRepository
    {
        bool IsEmailUsed(string email);
        bool IsNicknameUsed(string nickname);
        public PnKUser? GetUserById(Guid id);
        IEnumerable<IUserActionManagment> GetLastActions(Guid userId, ActionType action, DateTime afterTime);
        bool CreateActionManager(IUserActionManagment actionManager);
        Task<IEnumerable<IUserRating>> GetActiveUsersWithScore(bool isCurrentPeriod);
    }
}
