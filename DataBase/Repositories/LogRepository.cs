using Core.Interfaces;
using Core.Interfaces.Repository;

namespace DataBase.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly PnkDbContext _context;

        public LogRepository(PnkDbContext context)
        {
            _context = context;
        }

        public async Task LogInfoAsync(ILogModel log)
        {
            _context.Logs.Add(new Models.Log 
            {
                CreateDate = log.CreateDate,
                Description = log.Description,
                LogType = log.LogType
            });

            _ = await _context.SaveChangesAsync();
        }
    }
}
