using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserActionManagment
    {
        uint Id { get; set; }

        Guid UserId { get; set; }

        ActionType ActionType { get; set; }

        DateTime Create { get; set; }

        string? Description { get; set; }

        string? Value { get; set; }

        UserActionValueType? ValueType { get; set; }
    }
}
