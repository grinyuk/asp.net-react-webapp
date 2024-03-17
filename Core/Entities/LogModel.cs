using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    internal class LogModel : ILogModel
    {
        public LogType LogType { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
