using Core.Entities;
using Core.Helpers;
using Core.Interfaces.Repository;
using Core.Interfaces.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Core.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepository;

        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task LogErrorAsync(string from, Exception ex)
        {
            await _logRepository.LogInfoAsync(new LogModel
            {
                CreateDate = DateTime.UtcNow,
                Description = from + GetErrorInfo(ex),
                LogType = Enums.LogType.Error
            });
        }

        private string GetErrorInfo(Exception ex)
        {
            var error = Constants.ExceptionPlusArrow;
            error += ex.Message + Constants.Trace + ex.StackTrace;
            
            if (ex.InnerException != null)
            {
                error += " InnerException ";
                error += GetErrorInfo(ex.InnerException);
            }
            return error;
        }

        public async Task LogInfoAsync(string description)
        {
            await _logRepository.LogInfoAsync(new LogModel
            {
                CreateDate = DateTime.UtcNow,
                Description = description,
                LogType = Enums.LogType.Information
            });
        }

        public async Task LogInfoAsync(string from, object description)
        {
            try
            {
                await _logRepository.LogInfoAsync(new LogModel
                {
                    CreateDate = DateTime.UtcNow,
                    Description = from + Constants.Info + JsonSerializer.Serialize(description),
                    LogType = Enums.LogType.Information
                });
            }
            catch (Exception ex)
            {
                await LogErrorAsync(nameof(LogService) + Constants.Arrow + nameof(LogInfoAsync), ex);
            }
        }
    }
}
