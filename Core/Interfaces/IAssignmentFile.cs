namespace Core.Interfaces;

public interface IAssignmentFile
{
    int Id { get; set; }
    int AssignmentId { get; set; }
    string? File { get; set; }
    string? FileName { get; set; }
}