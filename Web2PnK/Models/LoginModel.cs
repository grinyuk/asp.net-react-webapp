using System.ComponentModel.DataAnnotations;
using Core.ResourcesFiles;

namespace Web2PnK.Models
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.UsernameIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceName = nameof(DefaultLanguage.UsernameLength), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? UserNameOrEmail { get; set; }

        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.PasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,16}$", ErrorMessageResourceName = nameof(DefaultLanguage.PasswordValidation), ErrorMessageResourceType = typeof(DefaultLanguage))]
		public string? Password { get; set; }
    }
}
