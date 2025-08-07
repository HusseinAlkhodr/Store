using Store.Models.Authenitication;
using System.ComponentModel.DataAnnotations;

namespace Store.DTO
{
    public class AccountDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public AccountStatus Status { get; set; }
        public AccountType AccountType { get; set; }
    }
    public class GetAccountDTO : AccountDTO
    {
        public long Id { get; set; }
    }
    public class RegisterDTO
    {
        [Required(ErrorMessage = "الاسم  مطلوب")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "الكنية مطلوبة")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقتين")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginDTO
    {
        [Required(ErrorMessage = "البريد الالكتروني  مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
        public string Email { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
    public class ChangePasswordDTO
    {
        public long Id { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "كلمة السر وتأكيدها غير متطابقتين")]
        public string ConfirmNewPassword { get; set; }
    }

}
