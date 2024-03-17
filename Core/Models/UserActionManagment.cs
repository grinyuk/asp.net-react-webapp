using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.Models
{
    public class UserActionManagment : IUserActionManagment
    {
        public uint Id { get; set; }

        public Guid UserId { get; set; }

        public ActionType ActionType { get; set; }

        public DateTime Create { get; set; }

        public string? Description { get; set; }

        public string? Value { get; set; }

        public UserActionValueType? ValueType { get; set; }
    }
}
