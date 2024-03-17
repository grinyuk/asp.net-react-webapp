using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Core.Interfaces
{
    public interface ITheme
    {
        public int Id { get; set; }

        public AssignmentSubject Subject { get; set; }

        public string? Value { get; set; }
    }
}
