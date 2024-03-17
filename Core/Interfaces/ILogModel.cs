using Core.Enums;

namespace Core.Interfaces
{
    public interface ILogModel
    {
        LogType LogType { get; set; }
        string? Description { get; set; }
        DateTime CreateDate { get; set; }
    }
}