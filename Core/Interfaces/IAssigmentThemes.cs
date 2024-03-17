using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAssigmentTheme
    {
        public int ThemeId { get; set; }
        public int AssignmentId { get; set; }
    }
}
