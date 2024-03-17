using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Service
{
    public interface ICalculateService
    {
        Task<bool> StartScoreCalculateAsync();
        Task<bool> StartArchiveScoreCalculateAsync();
        Task SetRecalculation(int? assignmentId);
    }
}
