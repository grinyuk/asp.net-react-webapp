using System.ComponentModel.DataAnnotations;
using Core.Enums;
using System.Text.Json.Serialization;
using Web2PnK.Helpers.Attributes;

namespace Web2PnK.Models.Assignment;

public class AssignmentModel 
{
    [Required]
    [StringLength(256)]
    public string? Title { get; set; }
    
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AssignmentSubject Subject { get; set; }
    
    [StringLength(512)]
    public string? Description { get; set; }
    
    [Required]
    [Range(1, 3, ErrorMessage = "Складність повинна бути між 1 і 3")]
    public int Difficulty { get; set; }
    
    [StringLength(64)]
    public string? AuthorName { get; set; }
    
    [StringLength(512)]
    public string? AuthorDescription { get; set; }
    
    [PdfFile]
    public string? File { get; set; }
    
    [StringLength(128)]
    public string? FileName { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    public DateTime EndDate { get; set; }
    
    [Required]
    public List<AssignmentAnswerModel>? Answers { get; set; }
    
    public List<int>? AssignmentThemesIds { get; set; }  
}