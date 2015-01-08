﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HxJumper.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "工号")]
        public string JobNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage="{0}必须填写！")]
        [Display(Name = "工号")]
        public string JobNumber { get; set; }
        [Required]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0}最少{2}位!", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码与确认密码不一致！")]
        public string ConfirmPassword { get; set; }

        [DisplayName("角色")]
        public int JumperRoleId { get; set; }
        public virtual JumperRole JumperRole { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [DisplayName("工号")]
        public string JobNumber { get; set; }
        [Required]
        [DisplayName("用户名")]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "{0}最少{2}位！", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [DisplayName("密码")]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("确认密码")]
        [Compare("NewPassword", ErrorMessage = "确认密码与密码不一致！")]
        public string ConfirmPassword { get; set; }
    }
}
