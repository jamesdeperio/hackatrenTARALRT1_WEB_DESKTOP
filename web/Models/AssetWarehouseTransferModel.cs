using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NXYSOFT_RMS.Models
{
    public class AssetWarehouseTransferModel
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
        [Display(Name = "From Warehouse")]
        public string fldFromWarehouseName { get; set; }
        [Display(Name = "From Warehouse")]
        public string fldFromWarehouseCode { get; set; }

        public List<DropDown> cbFromWarehouse { get; set; }

     
        [StringLength(200)]
        [Display(Name = "To Warehouse")]
        public string fldToWarehouseName { get; set; }
        [Display(Name = "To Warehouse")]
        public string fldToWarehouseCode { get; set; }
        public List<DropDown> cbToWarehouse { get; set; }

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

        [Display(Name = "Transffered By")]
        public string fldTransferredByCode { get; set; }
        public List<DropDown> cbTransferredBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Transffered Date")]
        public DateTime fldTransfferedDateTime { get; set; }

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

        public string fldSearchTextWarehouse { get; set; }

        public string fldSearchTextTag { get; set; }

        public string fldUID { get; set; }

        [Display(Name = "Serial No")]
        public string fldSerialNo { get; set; }

        [Display(Name = "RFID")]
        public string fldRFID { get; set; }

        [Display(Name = "Other No")]
        public string fldOtherNo { get; set; }

        [Display(Name = "Barcode")]
        public string fldBarcode { get; set; }

        [Display(Name = "Status")]
        public string fldStatusTag { get; set; }

        [Display(Name = "Other Info")]
        public string fldOtherInfo { get; set; }

        //DETAILS

        [Required]
        [Display(Name = "fldID")]
        public string fldAssetID { get; set; }

        [Display(Name = "TransfferedNo")]
        public string fldAsseTransfferedNo { get; set; }
        [Display(Name = "Code")]
        public string fldAssetCode { get; set; }
        [Display(Name = "Name")]
        public string fldAssetName{ get; set; }
        [Display(Name = "UOM")]
        public string fldAssetUOMCode { get; set; }      
        [Display(Name = "QTY")]
        public string fldAssetQty { get; set; }


      
        [Display(Name = "Batch")]
        public string fldAssetBatchReceived { get; set; }




        public List<DropDown> cbCategory { get; set; }
        public List<DropDown> cbClassification { get; set; }
        public List<DropDown> cbType { get; set; }


        public List<DropDown> cbPurchasingUOM { get; set; }

        public List<DropDown> cbPackageUOM { get; set; }

        public List<DropDown> cbBrand { get; set; }

        public List<DropDown> cbModel { get; set; }

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