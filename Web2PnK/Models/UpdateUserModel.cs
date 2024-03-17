using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models
{
    public class UpdateUserModel
    {
        public Guid UserId { get; set; }
        [StringLength(50)]
        public string? Login { get; set; }
        
        [StringLength(64)]
        public string? FullName { get; set; }
        
        [StringLength(512)]
        public string? Description { get; set; }
    }
}
