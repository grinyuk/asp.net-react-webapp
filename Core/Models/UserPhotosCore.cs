using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class UserPhotosCore
    {
        public Guid UserId { get; set; }
        public string? Photo { get; set; }

    }
}
