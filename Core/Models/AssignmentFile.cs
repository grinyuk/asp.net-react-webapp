using Core.Interfaces;

namespace Core.Models;

public class AssignmentFile : IAssignmentFile
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public string? File { get; set; }
    public string? FileName { get; set; }
}