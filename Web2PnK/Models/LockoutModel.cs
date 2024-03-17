using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models
{
    public class LockoutModel
    {
        public Guid UserId { get; set; }
        public bool LockoutStatus { get; set; }
    }
}
