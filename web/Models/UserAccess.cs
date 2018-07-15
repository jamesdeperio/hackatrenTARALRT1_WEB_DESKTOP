using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NXYSOFT_RMS.Models
{
    public class UserAccess
    {
        [Required]
        [Display(Name = "CurrentID")]
        public string CurrentID { get; set; }

        [Required]
        [DefaultValue("")]
        [StringLength(100)]
        [Display(Name = "Access Name")]
        public string fldAccessName { get; set; }

        public string fldModuleName { get; set; }
        public string fldButtons { get; set; }

        public string fldSearchText { get; set; }

        public int fldPageNo { get; set; }

        [Display(Name = "View")]
        public int fldPageLimit { get; set; }
        public string fldPageCommand { get; set; }
        public List<DropDown> cbPageLimit { get; set; }


     
    }
}