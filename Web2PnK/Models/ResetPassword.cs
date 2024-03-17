using Core.ResourcesFiles;
using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.PasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,16}$", ErrorMessageResourceName = nameof(DefaultLanguage.PasswordValidation), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? Password { get; set; }

        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.ConfirmPasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [Compare("Password", ErrorMessageResourceName = nameof(DefaultLanguage.ComparePassword), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? ConfirmPassword { get; set; }
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
