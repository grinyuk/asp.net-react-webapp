using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class AssigmentThemes : IAssigmentTheme
    {
        public int ThemeId { get; set; }
        public int AssignmentId { get; set; }
    }
}
