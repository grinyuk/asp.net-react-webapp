using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    [Keyless]
    public class UserRating : IUserRating
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public int ScoreMath { get; set; }
        public int ScorePhysics { get; set; }
    }
}
