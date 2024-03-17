using Core.Interfaces.Service;
using Core.ResourcesFiles;
using System.ComponentModel.DataAnnotations;
using Web2PnK.Helpers.Attributes;

namespace Web2PnK.Models
{
    public class RegisterModel
    {

        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.UsernameIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [StringLength(64, MinimumLength = 2, ErrorMessageResourceName = nameof(DefaultLanguage.UsernameLength), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [UniqueValue(typeof(IPnkUserService), nameof(IPnkUserService.IsNicknameUsed))]
        public string? Nickname { get; set; }


        [StringLength(64, MinimumLength = 2, ErrorMessageResourceName = nameof(DefaultLanguage.UsernameLength), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? FullName { get; set; }


        [StringLength(512, ErrorMessageResourceName = nameof(DefaultLanguage.UserDescriptionLength), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? Description { get; set; }


        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.EmailIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [EmailAddress(ErrorMessageResourceName = nameof(DefaultLanguage.EmailIsNotValid), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [UniqueValue(typeof(IPnkUserService), nameof(IPnkUserService.IsEmailUsed))]
        [RegularExpression("^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$", ErrorMessageResourceName = nameof(DefaultLanguage.EmailIsNotValid), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? Email { get; set; }

        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.PasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).{8,16}$", ErrorMessageResourceName = nameof(DefaultLanguage.PasswordValidation), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? Password { get; set; }

        [Required(ErrorMessageResourceName = nameof(DefaultLanguage.ConfirmPasswordIsRequired), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [Compare("Password", ErrorMessageResourceName = nameof(DefaultLanguage.ComparePassword), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? ConfirmPassword { get; set; }

        [StringLength(250*1024, ErrorMessageResourceName = nameof(DefaultLanguage.UserPhotoSizeTooLarge), ErrorMessageResourceType = typeof(DefaultLanguage))]
        [MinLength(100, ErrorMessageResourceName = nameof(DefaultLanguage.UserPhotoInvalidFormat), ErrorMessageResourceType = typeof(DefaultLanguage))]
        public string? Photo {  get; set; }
    }
}
