using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models.Assignment;

public class AssignmentAnswerModel
{   
    public int Id { get; set; }
    [Required]
    public float Answer { get; set; }
    [Required]
    public int MaxScore { get; set; }
    [StringLength(512)]
    public string? Description { get; set; }
}