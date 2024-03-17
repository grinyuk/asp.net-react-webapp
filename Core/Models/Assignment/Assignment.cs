using Core.Enums;
using Core.Interfaces;

namespace Core.Models.Assignment
{
    public class Assignment : IAssignment
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public AssignmentSubject Subject { get; set; }
        public string? Description { get; set; }
        public int Difficulty { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IAssignmentFile? File { get; set; }
        public IEnumerable<IAssignmentAnswer>? Answers { get; set; }
        public IEnumerable<int>? AssignmentThemesIds { get; set; }
        public IEnumerable<ISchedule>? Schedules { get; set; }
    }
}
