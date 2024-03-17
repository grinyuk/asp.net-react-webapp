using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace DataBase.Models;

internal class Assignment
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(256)]
    public string? Title { get; set; }
    
    [Column(TypeName = "nvarchar(24)")]
    public AssignmentSubject Subject { get; set; }
    
    [StringLength(512)]
    public string? Description { get; set; }
    
    public int Difficulty { get; set; }
    
    [StringLength(64)]
    public string? AuthorName { get; set; }
    
    [StringLength(512)]
    public string? AuthorDescription { get; set; }

    public DateTime CreateDate { get; set; }
    
    public DateTime? UpdateDate { get; set; }
    
    public DateTime StarDate { get; set; }
    
    public DateTime EndDate { get; set; }

    public AssignmentFile? File { get; set; }
    
    public ICollection<AssignmentAnswer>? Answers { get; set; }
    public ICollection<Theme>? Themes { get; set; }
    public ICollection<Schedule>? Schedules { get; set; }
}