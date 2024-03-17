using Core.Enums;
using Core.Interfaces;

namespace Core.Models.Assignment;

public class AssignmentResponse
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public AssignmentSubject? Subject { get; set; }
    public AssigmentStatus? Status { get; set; }
    public string? Description { get; set; }
    public int Difficulty { get; set; }
    public string? AuthorName { get; set; }
    public string? AuthorDescription { get; set; }
    public int MinutesLeft { get; set; }
    public int TimeToArchive { get; set; }
    public int Period { get; set; }
    public int MaxPeriod { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<ActiveAssignmentAnswer>? Answers { get; set; }
    public IEnumerable<int>? AssignmentThemesIds { get; set; }
}