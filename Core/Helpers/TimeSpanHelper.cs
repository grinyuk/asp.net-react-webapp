using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helpers
{
    public static class TimeSpanHelper
    {
        public static string ToStringTime(this TimeSpan value)
        {
            if (value.Minutes >= 5)
            {
                return $"{value.Minutes} хвилин";
            }
            else if (value.Minutes == 0) 
            {
                return $"{value.Seconds} секунд";
			}
            else
            {
                return $"{value.Minutes} хвилин {value.Seconds} секунд"; 
            }
        }
    }
}
