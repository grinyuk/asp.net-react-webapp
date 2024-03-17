using Core.Enums;
using Core.Interfaces;
using Core.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Core.Helpers;

namespace DataBase.Repositories
{
    public class PnkUserRepository : IPnkUserRepository
    {
        protected readonly PnkDbContext _context;

        public PnkUserRepository(PnkDbContext context)
        {
            _context = context;
        }

        public bool IsEmailUsed(string email)
        {
            return _context.PnKUsers.Any(x => x.Email != null && email != null && x.Email.ToLower() == email.ToLower());
        }
        public bool IsNicknameUsed(string nickname)
        {
            return _context.PnKUsers.Any(x => x.UserName != null && nickname != null && x.UserName.ToLower() == nickname.ToLower());
        }

        public Core.Models.PnKUser? GetUserById(Guid id)
        {
            return _context.PnKUsers.FirstOrDefault(x => id != Guid.Empty && x.Id == id);
        }

        public IEnumerable<IUserActionManagment> GetLastActions(Guid userId, ActionType actionType, DateTime afterTime)
        {
            return _context.UserActionManagments.Where(x => x.UserId.Equals(userId) && x.ActionType == actionType && x.Create >= afterTime).Take(10).ToList();
        }

        public bool CreateActionManager(IUserActionManagment actionManager)
        {
            try
            {
                _context.UserActionManagments.Add(new DataBase.Models.UserActionManagment
                {
                    UserId = actionManager.UserId,
                    ActionType = actionManager.ActionType,
                    Description = actionManager.Description,
                    Value = actionManager.Value,
                    ValueType = actionManager.ValueType,
                    Create = actionManager.Create
                });
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _ = Logger.Instance.LogErrorAsync(nameof(PnkUserRepository) + Constants.Arrow + nameof(CreateActionManager), ex);
                return false;
            }

            return true;
        }

        public async Task<IEnumerable<IUserRating>> GetActiveUsersWithScore(bool isCurrentPeriod)
        {
            return await _context.UserRating.FromSqlInterpolated($"exec [dbo].[GetActiveUsersWithScore] @semester={isCurrentPeriod}").ToListAsync();
        }
    }
}
