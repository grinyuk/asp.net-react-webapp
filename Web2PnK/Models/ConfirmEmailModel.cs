using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models
{
    public class ConfirmEmailModel
    {
        [Required]
        public string? Token { get; set; }

        [Required]
        public string? Email { get; set; }
    }
}
