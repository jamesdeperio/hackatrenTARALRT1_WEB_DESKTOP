using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebNyxSoftAMS.Models;
using System.Text;

namespace NXYSOFT_RMS.Models
{
   
    public class AssetCatalogModel
    {
        [Required]
        [Display(Name = "CurrentID")]
        public string CurrentID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date Encoded")]
        public DateTime fldDateEncoded { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Code")]
        public string fldCode { get; set; }

        [StringLength(200)]
        [Display(Name = "Name")]
        public string fldName { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string fldRemarks { get; set; }

    
        [Display(Name = "Category")]
        public string fldCategoryCode { get; set; }

        [Display(Name = "Classification")]
        public string fldClassificationCode { get; set; }
        [Display(Name = "Type")]
        public string fldTypeCode { get; set; }

      
        [Display(Name = "Min Stock")]
        public string fldMinimumInv { get; set; }

        [Display(Name = "Max Stock")]
        public string fldMaximumInv { get; set; }

        [Display(Name = "Purc. UOM")]
        public string fldPurchasingUOMCode { get; set; }

        [Display(Name = "Pack. UOM")]
        public string fldPackageUOMCode { get; set; }

        [Display(Name = "Unit Qty")]
        public string fldQtyPerUnit { get; set; }

        [Display(Name = "Brand")]
        public string fldBrandCode { get; set; }

        [Display(Name = "Model")]
        public string fldModelCode { get; set; }

       

        [Display(Name = "Image")]
        public string fldImgLocation { get; set; }

        [Display(Name = "Salvage Value")]
        public string fldDepSalvageValue { get; set; }

        [Display(Name = "Life Span")]
        public string fldDepLifeSpan { get; set; }

        [Display(Name = "Method")]
        public string fldDepMethod { get; set; }




        [Display(Name = "Active")]
        public bool fldIsActive { get; set; }

       

        public string fldFromDate { get; set; }
        public string fldToDate { get; set; }
        public string fldSearchText { get; set; }

        public int fldPageNo { get; set; }

        [Display(Name = "View")]
        public int fldPageLimit { get; set; }
        public string fldPageCommand { get; set; }
        public List<DropDown> cbPageLimit { get; set; }

        public List<DropDown> cbCategory { get; set; }
        public List<DropDown> cbClassification { get; set; }
        public List<DropDown> cbType { get; set; }


        public List<DropDown> cbPurchasingUOM { get; set; }

        public List<DropDown> cbPackageUOM { get; set; }

        public List<DropDown> cbBrand { get; set; }

        public List<DropDown> cbModel { get; set; }

        public List<DropDown> cbDepMethod { get; set; }

        public bool fldIsPicUploaded { get; set; }





        // SUB PART

        [Required]
        [Display(Name = "CurrentID")]
        public string SubPartID { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Sub Part Code")]
        public string fldSubPartCode { get; set; }

        [Display(Name = "Asset Name")]
        public string fldSubPartName { get; set; }

        [Display(Name = "Qty")]
        public string fldSubPartQty { get; set; }


        //SUPPLIER

        [Required]
        [Display(Name = "CurrentID")]
        public string SupplierID { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Supplier Code")]
        public string fldSupplierCode { get; set; }

        [Display(Name = "Supplier Name")]
        public string fldSupplierName { get; set; }

        [Display(Name = "Nature")]
        public string fldNature { get; set; }

        public List<DropDown> cbNature { get; set; }


        public string fldSearchTextClassification { get; set; }
        public string fldSearchTextCategory { get; set; }
        public string fldSearchTextType { get; set; }
        public string fldSearchTextBrand { get; set; }
        public string fldSearchTextModel { get; set; }



       public bool Thumbnailview { get; set; }
    }

}