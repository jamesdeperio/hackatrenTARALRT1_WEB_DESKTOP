using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NXYSOFT_RMS.Models
{
    public class AssetInventoryModel
    {
        public string generateBarcode()
        {
            try
            {
                string[] charPool = "1-2-3-4-5-6-7-8-9-0".Split('-');
                StringBuilder rs = new StringBuilder();
                int length = 8;
                Random rnd = new Random();
                while (length-- > 0)
                {
                    int index = (int)(rnd.NextDouble() * charPool.Length);
                    if (charPool[index] != "-")
                    {
                        rs.Append(charPool[index]);
                        charPool[index] = "-";
                    }
                    else
                        length++;
                }
                return rs.ToString();
            }
            catch (Exception ex)
            {
                //ErrorLog.WriteErrorLog("Barcode", ex.ToString(), ex.Message);
            }
            return "";
        }
        public string CurrentID { get; set; }
        public string fldReceivedNo { get; set; }
        public string fldCode { get; set; }
        public string fldAssetCode { get; set; }
        public string fldUOMCode { get; set; }
        public string fldValueOnReceived { get; set; }
        public string fldQtyOnReceived { get; set; }
        public string fldValueOnHand { get; set; }
        public string fldQtyOnHand { get; set; }
        public string fldBatchReceived { get; set; }
        public string fldName { get; set; }
        public string fldRemarks { get; set; }
        public string fldCategoryCode { get; set; }
        public string fldClassificationCode { get; set; }
        public string fldTypeCode { get; set; }
        public string fldBrandCode { get; set; }
        public string fldModelCode { get; set; }
        public string fldMinimumInv { get; set; }
        public string fldMaximumInv { get; set; }
        public string fldSupplierCode { get; set; }
        public string fldWarehouseCode { get; set; }
        public string fldUOMName { get; set; }
        public string fldSectionCode { get; set; }
        public string fldShelfCode { get; set; }
        public string fldRackCode { get; set; }

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

        public bool fldWithBatchReceived { get; set; }


        public string fldSearchText { get; set; }
        public string fldSearchTextTag { get; set; }

        public int fldPageNo { get; set; }

        [Display(Name = "View")]
        public int fldPageLimit { get; set; }
        public string fldPageCommand { get; set; }
        public List<DropDown> cbPageLimit { get; set; }

        public List<DropDown> cbCategory { get; set; }
        public List<DropDown> cbClassification { get; set; }
        public List<DropDown> cbType { get; set; }        

        public List<DropDown> cbBrand { get; set; }

        public List<DropDown> cbModel { get; set; }


        public List<DropDown> cbSupplier { get; set; }
        public List<DropDown> cbWarehouse { get; set; }
        public List<DropDown> cbSection { get; set; }

        public List<DropDown> cbRack { get; set; }

        public List<DropDown> cbShelf { get; set; }


        //SUPPLIER


        [Display(Name = "Nature")]
        public string fldNature { get; set; }

        public List<DropDown> cbNature { get; set; }


        public string fldSearchTextClassification { get; set; }
        public string fldSearchTextCategory { get; set; }
        public string fldSearchTextType { get; set; }
        public string fldSearchTextBrand { get; set; }
        public string fldSearchTextModel { get; set; }

        public string fldSearchTextWarehouse { get; set; }


        public List<DropDown> cbReceivingCode { get; set; }

        [Display(Name = "Trans. Code")]
        public string fldReceivingCode { get; set; }
    }

}