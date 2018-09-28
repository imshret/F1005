using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace F1005.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
    }
       
    public partial class LoginViewModel
    {
        [Required]
        [Display(Name = "帳號")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "電子郵件")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)] 
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "登入碼")]
        public string CheckCode { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "帳號")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        //[DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "驗證碼")]
        public string Code { get; set; }
    }

    public class ResetPasswordViewModel
    {

        [Display(Name = "帳號")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} 的長度至少必須為 {2} 個字元。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密碼")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符。")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "帳號")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
   
        public string Password { get; set; }
    }

    public class ChangePasswordViewModel
    {
     
        [Display(Name = "原密碼")]
        [DataType(DataType.Password)]
        public string OPassword { get; set; }

        [Display(Name = "新密碼")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "確認密碼")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
