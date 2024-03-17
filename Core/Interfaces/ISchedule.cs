namespace Core.Interfaces;

public interface ISchedule
{
    int Id { get; set; }
    
    int AssignmentId { get; set; }
    
    DateTime PeriodStartDate { get; set; }
    
    DateTime PeriodEndDate { get; set; }
    
    int Period { get; set; }
    
    bool IsScoreCalculated { get; set; }
}