using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace DataBase.Models;

internal class GlobalSettings
{
    [Key]
    [Column(TypeName = "nvarchar(64)")]
    public GlobalSettingsType Key { get; set; }
    [Required]
    public string? Value { get; set; }
}