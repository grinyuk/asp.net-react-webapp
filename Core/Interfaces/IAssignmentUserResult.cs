namespace Core.Interfaces
{
    public interface IAssignmentUserResult
    {
        int AnswerId { get; set; }
        int AssignmentId { get; set; }
        DateTime CreateDate { get; set; }
        long Id { get; set; }
        bool IsProcessed { get; set; }
        int? Score { get; set; }
        int Period { get; set; }
        Guid UserId { get; set; }
        float Answer { get; set; }
        IAssignmentAnswer? AssignmentAnswer { get; }
    }
}