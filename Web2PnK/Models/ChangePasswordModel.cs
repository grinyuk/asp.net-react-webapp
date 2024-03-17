using Core.ResourcesFiles;
using System.ComponentModel.DataAnnotations;

namespace Web2PnK.Models
{
    public class ChangePasswordModel
    {
        public Guid UserId { get; set; }
        
        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.PasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,16}$", ErrorMessageResourceName = nameof(DefaultLanguage.PasswordValidation), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? OldPassword { get; set; }

        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.PasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,16}$", ErrorMessageResourceName = nameof(DefaultLanguage.PasswordValidation), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? NewPassword { get; set; }

        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.ConfirmPasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [Compare("NewPassword", ErrorMessageResourceName = nameof(DefaultLanguage.ComparePassword), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? ConfirmPassword { get; set; }
       
    }
}
