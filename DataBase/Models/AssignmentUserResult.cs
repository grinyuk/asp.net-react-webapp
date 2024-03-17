using Core.Interfaces;
using Core.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models
{
    internal class AssignmentUserResult : IAssignmentUserResult
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey(nameof(PnKUser))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Assignment))]
        public int AssignmentId { get; set; }

        [ForeignKey(nameof(AssignmentAnswer))]
        public int AnswerId { get; set; }

        public DateTime CreateDate { get; set; }

        public float Answer { get; set; }

        public bool IsProcessed { get; set; }

        public int? Score { get; set; }

        public int Period { get; set; }

        public AssignmentAnswer? AssignmentAnswer { get; set; }

        public PnKUser? PnKUser { get; set; }

        public Assignment? Assignment { get; set; }
        IAssignmentAnswer? IAssignmentUserResult.AssignmentAnswer { get => AssignmentAnswer; }
    }
}
