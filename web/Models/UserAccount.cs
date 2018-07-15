using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NXYSOFT_RMS.Models
{
    public class UserAccount
    {
        [Required]
        [Display(Name = "CurrentID")]
        public string CurrentID { get; set; }

        [Required]
        [Display(Name = "Account ID")]
        public string AccountID { get; set; }

        [Required]
        [DefaultValue("")]
        [StringLength(100)]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DefaultValue("")]
        [StringLength(100)]
        [Display(Name = "Current Username")]
        public string CurrentUserName { get; set; }

        [Required]
        [DefaultValue("")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DefaultValue("")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DefaultValue("")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

       
       
    }
}