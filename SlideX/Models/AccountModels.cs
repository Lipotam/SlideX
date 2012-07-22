using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SlideX.Localization;

namespace SlideX.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("CurrentPassword", NameResourceType = typeof(PropertyNames))]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessageResourceName = "PasswordMinLength", ErrorMessageResourceType = typeof(ValidationStrings), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("NewPassword", NameResourceType = typeof(PropertyNames))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [LocalizedDisplayName("ConfirmNewPassword", NameResourceType = typeof(PropertyNames))]
        [Compare("Password", ErrorMessageResourceName = "PasswordsMustMatch", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [LocalizedDisplayName("UserName", NameResourceType = typeof(PropertyNames))]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Password", NameResourceType = typeof(PropertyNames))]
        public string Password { get; set; }

        [LocalizedDisplayName("RememberMe", NameResourceType = typeof(PropertyNames))]
        public bool RememberMe { get; set; }
    }


    public class RegisterModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        [LocalizedDisplayName("UserName", NameResourceType = typeof(PropertyNames))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        [DataType(DataType.EmailAddress)]
        [LocalizedDisplayName("EmailAddress", NameResourceType = typeof(PropertyNames))]
        public string Email { get; set; }
        
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        [StringLength(100,  ErrorMessageResourceName = "PasswordMinLength", ErrorMessageResourceType = typeof(ValidationStrings), MinimumLength = 6)]
        
        [DataType(DataType.Password)]
        [LocalizedDisplayName("Password", NameResourceType = typeof(PropertyNames))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(ValidationStrings))]
        [DataType(DataType.Password)]
        [LocalizedDisplayName("ConfirmPassword", NameResourceType = typeof(PropertyNames))]
        [Compare("Password", ErrorMessageResourceName = "PasswordsMustMatch", ErrorMessageResourceType = typeof(ValidationStrings))]
        public string ConfirmPassword { get; set; }
    }
}
