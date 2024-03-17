using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ConfigActionManager
    {
        public int NumberOfAttempt { get; set; }

        public TimeSpan TimeBeetwenAttempts { get; set; }

        public TimeSpan TimeAfterAllAttemps { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
