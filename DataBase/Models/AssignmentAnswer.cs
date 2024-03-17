using Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models;

internal class AssignmentAnswer : IAssignmentAnswer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey(nameof(Assignment))]
    public int AssignmentId { get; set; }

    [Required]
    public float Answer { get; set; }

    public int MaxScore { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }
}