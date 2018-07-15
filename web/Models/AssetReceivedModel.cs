using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NXYSOFT_RMS.Models
{
    public class AssetReceivedModel
    {
        [Required]
        [Display(Name = "CurrentID")]
        public string CurrentID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date Encoded")]
        public DateTime fldDateEncoded { get; set; }
        [Display(Name = "Name")]
        public string fldName { get; set; }
        [Required]
        [StringLength(200)]
        [Display(Name = "Trans. Code")]
        public string fldCode { get; set; }

        [StringLength(200)]
        [Display(Name = "Supplier")]
        public string fldSupplierName { get; set; }
        [Display(Name = "From Supplier")]
        public string fldSupplierCode { get; set; }

        public List<DropDown> cbSupplier { get; set; }

        [StringLength(200)]
        [Display(Name = "Other Ref No.")]
        public string fldPONo { get; set; }

        [StringLength(200)]
        [Display(Name = "DR No.")]
        public string fldDRNo { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "DR Date")]
        public DateTime fldDRDate { get; set; }

      
        [Display(Name = "Currency")]
        public string fldCurrency { get; set; }
        public List<DropDown> cbCurrency { get; set; }

        [StringLength(200)]
        [Display(Name = "Warehouse")]
        public string fldWarehouseName { get; set; }
        [Display(Name = "To Warehouse")]
        public string fldWarehouseCode { get; set; }
        public List<DropDown> cbWarehouse { get; set; }

        [StringLength(200)]
        [Display(Name = "Section")]
        public string fldSectionCode { get; set; }
        public List<DropDown> cbSection { get; set; }

        [StringLength(200)]
        [Display(Name = "Rack")]
        public string fldRackCode { get; set; }
        public List<DropDown> cbRack { get; set; }

        [StringLength(200)]
        [Display(Name = "Shelf")]
        public string fldShelfCode { get; set; }
        public List<DropDown> cbShelf { get; set; }

        [Display(Name = "Remarks")]
        public string fldRemarks { get; set; }

        [Display(Name = "Transact By")]
        public string fldTransactionByCode { get; set; }
        public List<DropDown> cbTransactionBy { get; set; }

        [Display(Name = "Recevied By")]
        public string fldReceivedByCode { get; set; }
        public List<DropDown> cbReceviedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Received Date")]
        public DateTime fldReceivedDateTime { get; set; }

        [Display(Name = "Authorized By")]
        public string fldAuthorizedByCode { get; set; }
        public List<DropDown> cbAuthorizedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Authorized Date")]
        public DateTime fldAuthorizedDateTime { get; set; }

        [Display(Name = "Auth. Comment")]
        public string fldAuthorizedComment { get; set; }

        [Display(Name = "Trans. Status")]
        public string fldAuthorizedStatus { get; set; }


        public string fldFromDate { get; set; }
        public string fldToDate { get; set; }
        public string fldSearchText { get; set; }

        public int fldPageNo { get; set; }

        [Display(Name = "View")]
        public int fldPageLimit { get; set; }
        public string fldPageCommand { get; set; }
        public List<DropDown> cbPageLimit { get; set; }

        public string fldSearchTextSupplier { get; set; }
        public string fldSearchTextWarehouse { get; set; }


        //DETAILS

        [Required]
        [Display(Name = "fldID")]
        public string fldAssetID { get; set; }

        [Display(Name = "ReceivedNo")]
        public string fldAssetReceivedNo { get; set; }
        [Display(Name = "Code")]
        public string fldAssetCode { get; set; }
        [Display(Name = "Name")]
        public string fldAssetName{ get; set; }
        [Display(Name = "UOM")]
        public string fldAssetUOMCode { get; set; }
        [Display(Name = "Price")]
        public string fldAssetPricePerPiece { get; set; }
        [Display(Name = "QTY")]
        public string fldAssetQty { get; set; }


      
        [Display(Name = "Batch")]
        public string fldAssetBatchReceived { get; set; }


        [Display(Name = "Cost Center")]
        public string fldCostCenter { get; set; }


        //Supplier
        [Display(Name = "Nature")]
        public string fldNature { get; set; }
        public List<DropDown> cbNature { get; set; }

        public List<DropDown> cbCategory { get; set; }
        public List<DropDown> cbClassification { get; set; }
        public List<DropDown> cbType { get; set; }


        public List<DropDown> cbPurchasingUOM { get; set; }

        public List<DropDown> cbPackageUOM { get; set; }

        public List<DropDown> cbBrand { get; set; }

        public List<DropDown> cbModel { get; set; }

        public List<DropDown> cbCostCenter { get; set; }

        [Display(Name = "Category")]
        public string fldCategoryCode { get; set; }

        [Display(Name = "Classification")]
        public string fldClassificationCode { get; set; }
        [Display(Name = "Type")]
        public string fldTypeCode { get; set; }
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




    }

}