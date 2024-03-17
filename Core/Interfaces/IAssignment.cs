using Core.Enums;

namespace Core.Interfaces
{
    public interface IAssignment
    {
        int Id { get; set; }
        string? Title { get; set; }
        string? Description { get; set; }
        int Difficulty { get; set; }
        AssignmentSubject Subject { get; set; }
        string? AuthorName { get; set; }
        string? AuthorDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        IAssignmentFile? File { get; set; }
        IEnumerable<IAssignmentAnswer>? Answers { get; set; }
        IEnumerable<int>? AssignmentThemesIds { get; set; }
        IEnumerable<ISchedule>? Schedules { get; set; }
    }
}
