using Core.Enums;

namespace Core.Interfaces;

public interface ICreatedAssignment
{
    int Id { get; set; }
    string? Title { get; set; }
    AssignmentSubject Subject { get; set; }
    string? Description { get; set; }
    int Difficulty { get; set; }
    string? AuthorName { get; set; }
    string? AuthorDescription { get; set; }
    DateTime StartDate { get; set; }
    DateTime EndDate { get; set; }
    IEnumerable<IAssignmentAnswer>? Answers { get; set; }
    IEnumerable<ITheme>? AssignmentThemesIds { get; set; }
    IEnumerable<ISchedule>? Schedules { get; set; }
}