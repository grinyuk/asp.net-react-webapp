using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface ILogService
    {
        Task LogErrorAsync(string from, Exception ex);
        Task LogInfoAsync(string description);
        Task LogInfoAsync(string from, object description);
    }
}
