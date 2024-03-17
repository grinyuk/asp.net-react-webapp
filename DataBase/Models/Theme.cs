using Core.Enums;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    internal class Theme : ITheme
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public AssignmentSubject Subject { get; set; }
        
        [Required]
        [StringLength(124)]
        public string? Value { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Assignment>? Assignments { get; set; }
    }
}
