using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NXYSOFT_RMS.Models
{
    public class RfidSettingModel
    {
        [Display(Name = "Activate")]
        public bool fldIsActive { get; set; }
        public string CurrentID { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Serial No")]
        public string fldPCSerialNo { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "IP Address")]
        public string fldRFIDIPaddress { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Port No")]
        public string fldRFIDPortNo { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Status")]
        public string fldCurrentStatus { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Command")]
        public string fldLastCommand { get; set; }


        public string fldSearchText { get; set; }

        public int fldPageNo { get; set; }

        [Display(Name = "View")]
        public int fldPageLimit { get; set; }
        public string fldPageCommand { get; set; }
        public List<DropDown> cbPageLimit { get; set; }

    }

}