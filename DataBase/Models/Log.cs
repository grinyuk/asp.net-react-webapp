using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    internal class Log
    {
        [Key]
        public long Id { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public LogType LogType { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
