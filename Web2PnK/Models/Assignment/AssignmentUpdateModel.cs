using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models.Assignment
{
    public class AssignmentUpdateModel : AssignmentModel
    {
        [Required]
        public int? Id { get; set; }
    }
}
