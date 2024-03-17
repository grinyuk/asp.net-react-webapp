using Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models;

internal class AssignmentFile : IFile
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Assignment))]
    public int AssignmentId { get; set; }
    
    [Required]
    public string? File { get; set; }
    
    public DateTime UploadDate { get; set; }
    
    [StringLength(128)]
    public string? FileName { get; set; }
}