using Core.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models;

internal class Schedule : ISchedule
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Assignment))]
    public int AssignmentId { get; set; }
    
    public DateTime PeriodStartDate { get; set; }
    
    public DateTime PeriodEndDate { get; set; }
    
    public int Period { get; set; }

    public bool IsScoreCalculated { get; set; }

    public Assignment? Assignment { get; set; }
}