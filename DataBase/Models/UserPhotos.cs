using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Models
{
    internal class UserPhotos
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        
        public string? Photo { get; set; }

        [StringLength(512)]
        public string? Description { get; set; }
        public DateTime UploadDate { get; set; }

        public PnKUser? User { get; set; }
    }
}
