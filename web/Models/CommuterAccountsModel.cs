using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NXYSOFT_RMS.Models
{
    public class CommuterAccountsModel
    {
       
        public string CurrentID { get; set; }

        [Display(Name = "Date Registered")]
        public DateTime fldDateRegistered { get; set; }

        [Display(Name = "Account Code")]
        public string fldAccountCode { get; set; }
        [Display(Name = "Name")]
        public string fldName { get; set; }
        [Display(Name = "Email Address")]        
        public string fldEmailAddress { get; set; }
        [Display(Name = "Phone No")]
        public string fldPhone { get; set; }
        [Display(Name = "Birthday")]
        public DateTime fldBirthday { get; set; }
        [Display(Name = "Gender")]
        public string fldGender { get; set; }
        [Display(Name = "Beep card")]
        public string fldNFC { get; set; }
        [Display(Name = "RFID/QR Code")]
        public string fldRFIDQRCode { get; set; }
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string fldPassword { get; set; }
        [Display(Name = "Last seen")]
        public DateTime fldLastSeen { get; set; }

        

        public string fldSearchText { get; set; }
 
        public int fldPageNo { get; set; }

        [Display(Name = "View")]
        public int fldPageLimit { get; set; }
        public string fldPageCommand { get; set; }
        //public List<DropDown> cbPageLimit { get; set; }

   




    

  
    }

    public class DropDown
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}