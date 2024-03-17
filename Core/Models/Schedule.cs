using Core.Interfaces;

namespace Core.Models;

public class Schedule : ISchedule
{
    public int Id { get; set; }
    
    public int AssignmentId { get; set; }
    
    public DateTime PeriodStartDate { get; set; }
    
    public DateTime PeriodEndDate { get; set; }
    
    public int Period { get; set; }
    
    public bool IsScoreCalculated { get; set; }
}