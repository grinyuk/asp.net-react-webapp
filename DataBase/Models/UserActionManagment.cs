using Core.Enums;
using Core.Interfaces;
using Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    internal class UserActionManagment : IUserActionManagment
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public ActionType ActionType { get; set; }

        [Required]
        public DateTime Create { get; set; }

        [StringLength(512)]
        public string? Description { get; set; }

        [StringLength(64)]
        public string? Value { get; set; }

        public UserActionValueType? ValueType { get; set; }

        public PnKUser? User { get; set; }
    }
}
