using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserRating
    {
        Guid Id { get; set; }
        string? Description { get; set; }
        string? FullName { get; set; }
        string? UserName { get; set; }
        public int ScoreMath { get; set; }
        public int ScorePhysics { get; set; }
    }
}
