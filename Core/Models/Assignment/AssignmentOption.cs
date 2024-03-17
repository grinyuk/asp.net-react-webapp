using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models.Assignment
{
    public class AssignmentOption
    {
        public List<int> AssignmentPeriodsOption { get; set; } = new List<int>();
        public List<int> AssignmentPeriodsScoreOption { get; set; } = new List<int>();
        public int AssignmentArchivePeriodScoreOption { get; set; }
        public int AssignmentAnswerAccuracyOption { get; set; }
    }
}
