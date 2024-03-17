using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFile
    {
        int Id { get; set; }
        int AssignmentId { get; set; }
        public string? File { get; set; }
        public string? FileName { get; set; }
    }
}
