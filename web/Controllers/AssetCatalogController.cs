using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using NXYSOFT_RMS.Models;
using System.IO;

namespace NXYSOFT_RMS.Controllers
{
    public class AssetCatalogController : Controller
    {


        public List<DropDown> GetDropDownPageLimit()
        {
            List<DropDown> items = new List<DropDown>();
            try
            {
                items.Add(new DropDown { Name = "10", Id = "10" });
                items.Add(new DropDown { Name = "20", Id = "20" });
                items.Add(new DropDown { Name = "30", Id = "30" });
                items.Add(new DropDown { Name = "40", Id = "40" });
                items.Add(new DropDown { Name = "50", Id = "50" });
                items.Add(new DropDown { Name = "60", Id = "60" });
                items.Add(new DropDown { Name = "70", Id = "70" });
                items.Add(new DropDown { Name = "80", Id = "80" });
                items.Add(new DropDown { Name = "90", Id = "90" });
                items.Add(new DropDown { Name = "100", Id = "100" });



            }
            catch { }

            return items;
        }
      



        bool __isLogIn()
        {
            if (Session["_intCurrentID"] == null)
            {
                return false;
            }
            else
            {
                MyWSContext ws = new MyWSContext();
                try
                {
                    using (DataTable _DTRecord = ws.AMSWebService.GetUserStatus(Session["_intCurrentID"].ToString(), Session["CustomerID"].ToString()))
                    {
                        foreach (DataRow DR in _DTRecord.Rows)
                        {
                            if (DR[1].ToString().ToUpper() != Session["_strCurrentIDCode"].ToString())
                            {
                               
                                Response.Cookies["AMSCookie"]["_intCurrentID"] = null;
                                Session.Clear();
                                return false;
                                
                            }
                        }

                    }
                }
                catch
                {
                    Response.Cookies["AMSCookie"]["_intCurrentID"] = null;
                    Session.Clear();
                    return false;
                }
            }
            return true;
        }

        public ActionResult List()
        {
           
            if (!__isLogIn()) return RedirectToAction("Login", "Account");

            try
            {
                if (bool.Parse(Session["AssetCatalog"].ToString()) == false)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch {                
                return RedirectToAction("Index", "Home");
            }

            Session["CurrentMenu"] = "AssetProfile";
            AssetCatalogModel Data = new AssetCatalogModel();

            if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
                Data.fldPageLimit = 10;
            else
                Data.fldPageLimit = int.Parse(Session["fldPageLimit"].ToString());

            ViewBag.From = Session["_strFromDate"];
            ViewBag.To = Session["_strToDate"];
            ViewBag.Search = Session["fldSearchText"];
            ViewBag.fldClassificationCode = Session["fldClassificationCode"];
            ViewBag.fldCategoryCode = Session["fldCategoryCode"];
            ViewBag.fldTypeCode = Session["fldTypeCode"];
            ViewBag.fldBrandCode = Session["fldBrandCode"];
            ViewBag.fldModelCode = Session["fldModelCode"];

            ViewBag.Thumbnailview = Session["Thumbnailview"];

            Data.fldFromDate = ViewBag.From==""| ViewBag.From ==null? null:String.Format("{0:yyyy-MM-dd}", DateTime.Parse(ViewBag.From));
            Data.fldToDate = ViewBag.To == "" | ViewBag.To == null ? null : String.Format("{0:yyyy-MM-dd}", DateTime.Parse(ViewBag.To));
            Data.fldSearchText = ViewBag.Search;
            Data.fldClassificationCode = ViewBag.fldClassificationCode;
            Data.fldCategoryCode = ViewBag.fldCategoryCode;
            Data.fldTypeCode = ViewBag.fldTypeCode;
            Data.fldBrandCode = ViewBag.fldBrandCode;
            Data.fldModelCode = ViewBag.fldModelCode;
            if(ViewBag.Thumbnailview == null)
                Data.Thumbnailview = true;
            else
                Data.Thumbnailview = ViewBag.Thumbnailview;
            try
            {
                MyWSContext ws = new MyWSContext();

                            
                string _strWhereStatement = "```" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`" + Data.fldClassificationCode + "`" + Data.fldCategoryCode + "`" + Data.fldTypeCode + "`" + Data.fldBrandCode + "`" + Data.fldModelCode;
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetCatalog_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {

                    ViewBag.SystemError = "";
                    ViewBag.SystemSuccess = "";
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    DataTable dt = new DataTable();
                    try
                    {


                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;
                       
                            if (Session["fldPageNo"] == null || int.Parse(Session["fldPageNo"].ToString()) == 0 || ViewBag.Pagging ==1)
                            {
                                Session["fldPageNo"] = 1;
                                Data.fldPageNo = 1;
                            }
                            else
                                Data.fldPageNo = int.Parse(Session["fldPageNo"].ToString());
                           
                        
                       

                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }

                Data.cbPageLimit = GetDropDownPageLimit();

                Data.cbClassification = GetDropDownItem(true, "Classification");
                Data.cbCategory = GetDropDownItem(true, "Category");
                Data.cbType = GetDropDownItem(true, "Type");
                Data.cbBrand = GetDropDownItem(true, "Brand");
                Data.cbModel = GetDropDownItem(true, "Model");

                if (Session["fldClassificationCode"] != null)
                {
                    if (Session["fldClassificationCode"].ToString() != "")
                        Data.fldClassificationCode = Session["fldClassificationCode"].ToString();
                }
                if (Session["fldCategoryCode"] != null)
                {
                    if (Session["fldCategoryCode"].ToString() != "")
                        Data.fldCategoryCode = Session["fldCategoryCode"].ToString();
                }
                if (Session["fldTypeCode"] != null)
                {
                    if (Session["fldTypeCode"].ToString() != "")
                        Data.fldTypeCode = Session["fldTypeCode"].ToString();
                }
                if (Session["fldBrandCode"] != null)
                {
                    if (Session["fldBrandCode"].ToString() != "")
                        Data.fldBrandCode = Session["fldBrandCode"].ToString();
                }
                if (Session["fldModelCode"] != null)
                {
                    if (Session["fldModelCode"].ToString() != "")
                        Data.fldModelCode = Session["fldModelCode"].ToString();
                }

               
            }
            catch { }

            clearTempSession();

          
            return View(Data);
        }
       
      

        [HttpPost]
        public ActionResult List(AssetCatalogModel Data)
        {           
            try
            {
                if (Session["_intCurrentID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
                    Data.fldPageLimit = 10;
                MyWSContext ws = new MyWSContext();
                string _strFromDate = Data.fldFromDate == null ? "" : String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Parse(Data.fldFromDate.ToString()));
                string _strToDate = Data.fldToDate == null ? "" : String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Parse(Data.fldToDate.ToString()));
                string _strWhereStatement = "```" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`" + Data.fldClassificationCode + "`" + Data.fldCategoryCode + "`" + Data.fldTypeCode + "`" + Data.fldBrandCode + "`" + Data.fldModelCode;

                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetCatalog_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    Session["_strFromDate"] = _strFromDate;
                    Session["_strToDate"] = _strToDate;
                    Session["fldSearchText"] = Data.fldSearchText;
                    Session["fldPageLimit"] = Data.fldPageLimit;
  

                    Session["fldClassificationCode"] = Data.fldClassificationCode;
                    Session["fldCategoryCode"] = Data.fldCategoryCode;
                    Session["fldTypeCode"] = Data.fldTypeCode;
                    Session["fldBrandCode"] = Data.fldBrandCode;
                    Session["fldModelCode"] = Data.fldModelCode;

                    Session["Thumbnailview"] = Data.Thumbnailview;
                    

                    ViewBag.RecordCount = _DTRecord.Rows.Count;

                    DataTable dt = new DataTable();
                    try
                    {


                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        if (Data.fldPageCommand != null)
                        {
                            if (Session["fldPageNo"] == null)
                                Session["fldPageNo"] = 1;
                            else
                                Data.fldPageNo = int.Parse(Session["fldPageNo"].ToString());


                            if (Data.fldPageCommand.Contains(">>"))
                            {
                                if (ViewBag.Pagging > Data.fldPageNo)
                                    Data.fldPageNo++;
                            }
                            else
                                Data.fldPageNo--;

                            if (Data.fldPageNo < 1)
                                Data.fldPageNo = 1;
                        }
                        else
                        { Data.fldPageNo = 1; }

                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;

                }

                Data.cbPageLimit = GetDropDownPageLimit();

                Data.cbClassification = GetDropDownItem(true, "Classification");
                Data.cbCategory = GetDropDownItem(true, "Category");
                Data.cbType = GetDropDownItem(true, "Type");
                Data.cbBrand = GetDropDownItem(true, "Brand");
                Data.cbModel = GetDropDownItem(true, "Model");

                if (Session["fldClassificationCode"] != null)
                {
                    if (Session["fldClassificationCode"].ToString() != "")
                        Data.fldClassificationCode = Session["fldClassificationCode"].ToString();
                }
                if (Session["fldCategoryCode"] != null)
                {
                    if (Session["fldCategoryCode"].ToString() != "")
                        Data.fldCategoryCode = Session["fldCategoryCode"].ToString();
                }
                if (Session["fldTypeCode"] != null)
                {
                    if (Session["fldTypeCode"].ToString() != "")
                        Data.fldTypeCode = Session["fldTypeCode"].ToString();
                }
                if (Session["fldBrandCode"] != null)
                {
                    if (Session["fldBrandCode"].ToString() != "")
                        Data.fldBrandCode = Session["fldBrandCode"].ToString();
                }
                if (Session["fldModelCode"] != null)
                {
                    if (Session["fldModelCode"].ToString() != "")
                        Data.fldModelCode = Session["fldModelCode"].ToString();
                }

            }
            catch { }

            return View("List", Data);
        }


        public ActionResult Add()
        {
            if (Session["_intCurrentID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (bool.Parse(Session["AssetCatalogCreateAccess"].ToString()) != true)
                {
                    return RedirectToAction("List", "AssetCatalog");
                }
            }
            catch {
                return RedirectToAction("List", "AssetCatalog");
            }
            
            AssetCatalogModel Data = new AssetCatalogModel();


            //Data.cbClassification = GetDropDownItem(false, "Classification");
            //Data.cbCategory = GetDropDownItem(false, "Category");
            //Data.cbType = GetDropDownItem(false, "Type");
            //Data.cbBrand = GetDropDownItem(false, "Brand");
            //Data.cbModel = GetDropDownItem(false, "Model");
            //Data.cbPackageUOM = GetDropDownItem(false, "UOM");
            //Data.cbPurchasingUOM = GetDropDownItem(false, "UOM");
            //Data.cbDepMethod = GetDropDownItem(false, "Method");

            
            try
            {
                if (Session["fldCode"].ToString() != "")
                {                   
                    Data.fldCode = Session["fldCode"].ToString();
                    Data.fldName = Session["fldName"].ToString();
                    Data.fldRemarks = Session["fldRemarks"].ToString();
                    Data.fldCategoryCode = Session["fldCategoryCode"].ToString();
                    Data.fldClassificationCode = Session["fldClassificationCode"].ToString();
                    Data.fldTypeCode = Session["fldTypeCode"].ToString();
                    Data.fldMinimumInv = Session["fldMinimumInv"].ToString();
                    Data.fldMaximumInv = Session["fldMaximumInv"].ToString();
                    Data.fldPurchasingUOMCode = Session["fldPurchasingUOMCode"].ToString();
                    Data.fldPackageUOMCode = Session["fldPackageUOMCode"].ToString();
                    Data.fldQtyPerUnit = Session["fldQtyPerUnit"].ToString();
                    Data.fldBrandCode = Session["fldBrandCode"].ToString();
                    Data.fldModelCode = Session["fldModelCode"].ToString();
                    Data.fldImgLocation = Session["fldImgLocation"].ToString();
                    Data.fldDepSalvageValue = Session["fldDepSalvageValue"].ToString();
                    Data.fldDepLifeSpan = Session["fldDepLifeSpan"].ToString();
                    Data.fldDepMethod = Session["fldDepMethod"].ToString();
                    Data.fldIsActive = bool.Parse(Session["fldIsActive"].ToString());
                    Data.fldIsPicUploaded = bool.Parse(Session["fldIsPicUploaded"].ToString());
                    clearTempSession();

                }
                else
                {
                    Data.fldCode = "A" + DateTime.Now.ToString().Remove(DateTime.Now.ToString().Length - 2).Replace("/", "").Replace(" ", "").Replace(":", "");
                    Data.fldIsActive = true;
                    Data.fldQtyPerUnit = "1";
                    Data.fldMinimumInv = "0";
                    Data.fldMaximumInv = "0";
                    Data.fldDepSalvageValue = "0.00";
                }

               
            }
            catch {
               
            }

            //List_SubPart(Data.fldCode);

            return View(Data);
        }

      
        public JsonResult Save(AssetCatalogModel Data)
        {
            try
            {
                    MyWSContext ws = new MyWSContext();
                    string _strInput = "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldCode + "`" + Data.fldName + "`" + Data.fldRemarks + "`" + Data.fldCategoryCode + "`" + Data.fldClassificationCode + "`" + Data.fldTypeCode + "`" + Data.fldMinimumInv + "`" + Data.fldMaximumInv + "`" + Data.fldPurchasingUOMCode + "`" + Data.fldPackageUOMCode + "`" + Data.fldQtyPerUnit + "`" + Data.fldBrandCode + "`" + Data.fldModelCode + "`" + Data.fldImgLocation + "`" + Data.fldDepSalvageValue + "`" + Data.fldDepLifeSpan + "`" + Data.fldDepMethod + "`" + Data.fldIsActive;
                    string[] _strOutput = ws.AMSWebService.SettingAssetCatalog_SaveRecord(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');

                    if (_strOutput[0] == "true")
                    {                       
                        ModelState.AddModelError("", _strOutput[1]);
                        return Json(new { Success = true, msg = _strOutput[1]}, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        ModelState.AddModelError("", _strOutput[1]);
                        return Json(new { Success = false, msg = _strOutput[1]}, JsonRequestBehavior.AllowGet);
                    }
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }
           

        }


        public JsonResult Delete(AssetCatalogModel Data)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "ASSETCATALOGS", Session["CustomerID"].ToString()).Split('`');
                if (_strOutput[0] == "true")
                {
                    ViewBag.SystemError = "";
                    ViewBag.SystemSuccess = _strOutput[1];
                    return Json(new { Success = true, msg = ViewBag.SystemSuccess }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    ViewBag.SystemError = _strOutput[1];
                    ViewBag.SystemSuccess = "";
                    return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);
                }
            }
            catch { return Json(new { Success = false, msg = "" }, JsonRequestBehavior.AllowGet); }
         

        }
        public JsonResult GetEditRecord(AssetCatalogModel Data)
        {            
                Session["CurrentID"] = Data.CurrentID;
                return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);            
        }
        public ActionResult Edit(AssetCatalogModel Data)
        {
            try
            {
                if (Session["_intCurrentID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                try
                {
                    if (bool.Parse(Session["AssetCatalogEditAccess"].ToString()) == false && bool.Parse(Session["AssetCatalogViewAccess"].ToString()) == false)
                    {
                        return RedirectToAction("List", "AssetCatalog");
                    }
                }
                catch {
                    return RedirectToAction("List", "AssetCatalog");
                }

                if (Session["fldCode"].ToString() != "")
                {

                    Data.CurrentID = Session["CurrentID"].ToString();
                    Data.fldCode = Session["fldCode"].ToString();
                    Data.fldName = Session["fldName"].ToString();
                    Data.fldRemarks = Session["fldRemarks"].ToString();
                    Data.fldCategoryCode = Session["fldCategoryCode"].ToString();
                    Data.fldClassificationCode = Session["fldClassificationCode"].ToString();
                    Data.fldTypeCode = Session["fldTypeCode"].ToString();
                    Data.fldMinimumInv = Session["fldMinimumInv"].ToString();
                    Data.fldMaximumInv = Session["fldMaximumInv"].ToString();
                    Data.fldPurchasingUOMCode = Session["fldPurchasingUOMCode"].ToString();
                    Data.fldPackageUOMCode = Session["fldPackageUOMCode"].ToString();
                    Data.fldQtyPerUnit = Session["fldQtyPerUnit"].ToString();
                    Data.fldBrandCode = Session["fldBrandCode"].ToString();
                    Data.fldModelCode = Session["fldModelCode"].ToString();
                    Data.fldImgLocation = Session["fldImgLocation"].ToString();
                    Data.fldDepSalvageValue = Session["fldDepSalvageValue"].ToString();
                    Data.fldDepLifeSpan = Session["fldImgLocation"].ToString();
                    Data.fldDepMethod = Session["fldDepSalvageValue"].ToString();
                    Data.fldIsActive = bool.Parse(Session["fldIsActive"].ToString());
                    Data.fldIsPicUploaded = bool.Parse(Session["fldIsPicUploaded"].ToString());
                    clearTempSession();
                }
                else
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = Session["CurrentID"] + "`````````````````";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetCatalog_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        foreach (DataRow DR in _DTRecord.Rows)
                        {                           
                            Data.CurrentID = DR[0].ToString();
                            Data.fldCode = DR[2].ToString();
                            Data.fldName = DR[3].ToString();
                            Data.fldRemarks = DR[4].ToString();
                            Data.fldCategoryCode = DR[5].ToString();
                            Data.fldClassificationCode = DR[6].ToString();
                            Data.fldTypeCode = DR[7].ToString();
                            Data.fldMinimumInv = DR[8].ToString();
                            Data.fldMaximumInv = DR[9].ToString();
                            Data.fldPurchasingUOMCode = DR[10].ToString();
                            Data.fldPackageUOMCode = DR[11].ToString();
                            Data.fldQtyPerUnit = DR[12].ToString();
                            Data.fldBrandCode = DR[13].ToString();
                            Data.fldModelCode = DR[14].ToString();
                            Data.fldImgLocation = DR[15].ToString();
                            Data.fldDepSalvageValue = DR[16].ToString();
                            Data.fldDepLifeSpan = DR[17].ToString();
                            Data.fldDepMethod = DR[18].ToString();                            
                            Data.fldIsActive = bool.Parse(DR[19].ToString());

                        }
                    }
                }
              

                
                return View(Data);


            }
            catch { }

            return View(Data);

        }

        public JsonResult Update(AssetCatalogModel Data)
        {
            try
            {
             
                MyWSContext ws = new MyWSContext();
                string _strInput = Data.CurrentID + "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldCode + "`" + Data.fldName + "`" + Data.fldRemarks + "`" + Data.fldCategoryCode + "`" + Data.fldClassificationCode + "`" + Data.fldTypeCode + "`" + Data.fldMinimumInv + "`" + Data.fldMaximumInv + "`" + Data.fldPurchasingUOMCode + "`" + Data.fldPackageUOMCode + "`" + Data.fldQtyPerUnit + "`" + Data.fldBrandCode + "`" + Data.fldModelCode + "`" + Data.fldImgLocation + "`" + Data.fldDepSalvageValue + "`" + Data.fldDepLifeSpan + "`" + Data.fldDepMethod + "`" + Data.fldIsActive;
                string[] _strOutput = ws.AMSWebService.SettingAssetCatalog_SaveRecord(true, "", _strInput, Session["CustomerID"].ToString()).Split('`');
                if (_strOutput[0] == "true")
                {
                    ModelState.AddModelError("", _strOutput[1]);
                    return Json(new { Success = true, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", _strOutput[1]);
                    return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }        

        }

        public PartialViewResult ViewClassificationSelection(AssetCatalogModel Data)
        {           
            Data.cbClassification = GetDropDownItem(false, "Classification");
            return PartialView(Data);
        }

        public PartialViewResult ViewCategorySelection(AssetCatalogModel Data)
        {
            Data.cbCategory = GetDropDownItem(false, "Category");
            return PartialView(Data);
        }

        public PartialViewResult ViewTypeSelection(AssetCatalogModel Data)
        {
            Data.cbType = GetDropDownItem(false, "Type");
            return PartialView(Data);
        }
        public PartialViewResult ViewBrandSelection(AssetCatalogModel Data)
        {
             Data.cbBrand = GetDropDownItem(false, "Brand");
            return PartialView(Data);
        }
        public PartialViewResult ViewModelSelection(AssetCatalogModel Data)
        {
            Data.cbModel = GetDropDownItem(false, "Model");
            return PartialView(Data);
        }
        //Data.cbClassification = GetDropDownItem(false, "Classification");
        //Data.cbCategory = GetDropDownItem(false, "Category");
        //Data.cbType = GetDropDownItem(false, "Type");
        //Data.cbBrand = GetDropDownItem(false, "Brand");
        //Data.cbModel = GetDropDownItem(false, "Model");
        //Data.cbPackageUOM = GetDropDownItem(false, "UOM");
        //Data.cbPurchasingUOM = GetDropDownItem(false, "UOM");
        //Data.cbDepMethod = GetDropDownItem(false, "Method");

        public List<DropDown> GetDropDownItem(bool _isWithAll, string _strName)
        {
            List<DropDown> items = new List<DropDown>();

            if (_strName== "Classification")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingClassification_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Classification)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Category")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCategory_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Category)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Type")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingType_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Type)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Brand")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingBrand_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Brand)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Model")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingModel_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Model)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "UOM")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingUOM_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (UOM)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Method")
            {
                try
                {
                    items.Add(new DropDown { Name = "Straight", Id = "Straight" });
                   
                }
                catch { }
            }


            if (_strName == "NATURES")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingSupplierNature_Where(Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (NATURE)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[0].ToString(), Id = DR[0].ToString() });
                        }

                    }


                }
                catch { }
            }


            return items;
        }

 


        void clearTempSession()
        {
            Session["fldCode"] = "";
            Session["fldName"] = "";
            Session["fldRemarks"] = "";
            Session["fldCategoryCode"] = "";
            Session["fldClassificationCode"] = "";
            Session["fldTypeCode"] = "";
            Session["fldMinimumInv"] = "";
            Session["fldMaximumInv"] = "";
            Session["fldPurchasingUOMCode"] = "";
            Session["fldPackageUOMCode"] = "";
            Session["fldQtyPerUnit"] = "";
            Session["fldBrandCode"] = "";
            Session["fldModelCode"] = "";
            Session["fldImgLocation"] = "";
            Session["fldDepSalvageValue"] = "";
            Session["fldDepLifeSpan"] = "";
            Session["fldDepMethod"] = "";
            Session["fldIsActive"] = "";
            Session["fldIsPicUploaded"] = "";


    }


        private bool RemoveFileFromServer(string path, string _strfilename)
        {
            var fullPath = Request.MapPath(path);
            // if (!System.IO.File.Exists(fullPath)) return false;

            try //Maybe error could happen like Access denied or Presses Already User used
            {
                string[] fileNames = Directory.GetFiles(fullPath);
                foreach (string fileName in fileNames)
                {
                    if (fileName.Contains(_strfilename))
                        System.IO.File.Delete(fileName);
                }

                return true;
            }
            catch (Exception e)
            {
                //Debug.WriteLine(e.Message);
            }
            return false;
        }
        public ActionResult FileUpload(HttpPostedFileBase file, string _strfilename, string _strFields)
        {
            string path = "";
            string pic = "";
            byte[] array = { };
            string extension = "";
            if (file != null && _strfilename != "")
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                extension = Path.GetExtension(file.FileName);


                RemoveFileFromServer("~/images/Asset/" + Session["CustomerID"].ToString(), _strfilename);


                Directory.CreateDirectory(Server.MapPath("~/images/Asset/" + Session["CustomerID"].ToString()));
                path = System.IO.Path.Combine(Server.MapPath("~/images/Asset/" + Session["CustomerID"].ToString()), _strfilename + ".jpg");
                // file is uploaded
                file.SaveAs(path);

                // save the image path path to the database or you can send image 
                // directly to database
                // in-case if you want to store byte[] ie. for DB
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();

                }

                Session["fldCode"] = _strFields.Split('`')[0];
                Session["fldName"] = _strFields.Split('`')[1];
                Session["fldRemarks"] = _strFields.Split('`')[2];
                Session["fldCategoryCode"] = _strFields.Split('`')[3];
                Session["fldClassificationCode"] = _strFields.Split('`')[4];
                Session["fldTypeCode"] = _strFields.Split('`')[5];
                Session["fldMinimumInv"] = _strFields.Split('`')[6];
                Session["fldMaximumInv"] = _strFields.Split('`')[7];
                Session["fldPurchasingUOMCode"] = _strFields.Split('`')[8];
                Session["fldPackageUOMCode"] = _strFields.Split('`')[9];
                Session["fldQtyPerUnit"] = _strFields.Split('`')[10];
                Session["fldBrandCode"] = _strFields.Split('`')[11];
                Session["fldModelCode"] = _strFields.Split('`')[12];
                Session["fldImgLocation"] = _strFields.Split('`')[13];
                Session["fldDepSalvageValue"] = _strFields.Split('`')[14];
                Session["fldDepLifeSpan"] = _strFields.Split('`')[15];
                Session["fldDepMethod"] = _strFields.Split('`')[16];
                Session["fldIsActive"] = _strFields.Split('`')[17];
                Session["fldIsPicUploaded"] = _strFields.Split('`')[18];

            }
            // after successfully uploading redirect the user
            return Content("../Images/Asset/" + Session["CustomerID"].ToString() + "/" + _strfilename + extension);
        }

        public ActionResult GetServerPath(string _strFilename)
        {
          
            try
            {
                string _path = Server.MapPath("~/images/Asset/" + Session["CustomerID"].ToString() + "/");
                string[] fileNames = Directory.GetFiles(_path);
                foreach (string fileName in fileNames)
                {
                    if (fileName.Contains(_strFilename))
                        return Content("../images/Asset/" + Session["CustomerID"].ToString() + "/" + System.IO.Path.GetFileName(fileName));

                }
            }
            catch { }
            return Content("../images/no-img.jpg");
        }



        //SUB PART
        public PartialViewResult ViewSubParts(string _strAssetCode)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = _strAssetCode + "````";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetSubPart_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    ViewData["SubPartRecordData"] = _DTRecord;

                }
            }             
            catch {
                DataTable dt = new DataTable();
                ViewData["SubPartRecordData"] = dt;
            }
            return PartialView();
}
     
        public PartialViewResult ViewSubPartsSelection(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetCatalogModel Data = new AssetCatalogModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try {
                    Data.fldSearchText = _strFilter.Split('`')[6];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[5]);
                   
                } catch {
                    Data.fldPageNo = 1;
                   
                }
                
                MyWSContext ws = new MyWSContext();
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetWOSubPart_Where(_strFilter,Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        //    if (Session["fldPageNo"] == null)
                        //        Session["fldPageNo"] = 1;
                        //    else
                        //        Data.fldPageNo = int.Parse(Session["fldPageNo"].ToString());

                        //    if (Data.fldPageCommand.Contains(">>"))
                        //    {
                        //        if (ViewBag.Pagging > Data.fldPageNo)
                        //            Data.fldPageNo++;
                        //    }
                        //    else
                        //        Data.fldPageNo--;

                        //    if (Data.fldPageNo < 1)
                        //        Data.fldPageNo = 1;
                        //}
                        //else
                        //{ Data.fldPageNo = 1; }

                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetWOSubPartRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.fldPageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                
                }               
            }
            catch {
              
                ViewData["AssetWOSubPartRecordData"] = dt;
            }
    
            return PartialView(Data);
        }

        public PartialViewResult ViewPic()
        {
            return PartialView();
        }
        public PartialViewResult ViewSubPartsFilter()
        {           
            AssetCatalogModel Data = new AssetCatalogModel();
            Data.cbClassification = GetDropDownItem(true, "Classification");
            Data.cbCategory = GetDropDownItem(true, "Category");
            Data.cbType = GetDropDownItem(true, "Type");
            Data.cbBrand = GetDropDownItem(true, "Brand");
            Data.cbModel = GetDropDownItem(true, "Model");
            Data.cbPageLimit = GetDropDownPageLimit();
            Data.fldPageLimit = 10;
            ViewBag.PageNo = 1;
            return PartialView(Data);
        }


        //public JsonResult GetSubPartsSelection(string _strFilter)
        //{
        //    try
        //    {
        //        MyWSContext ws = new MyWSContext();
        //        using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetWOSubPart_Where(Session["CustomerID"].ToString()))
        //        {
        //            ViewData["AssetWOSubPartSource"] = _DTRecord;                 
        //            return Json(new { Success = true, msg = "Sucessful" }, JsonRequestBehavior.AllowGet);

        //        }
        //    }
        //    catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }


        //}

        public JsonResult SaveSubPart(string _strAssetCode,string _strSubPartCodes)
        {
            try
            {
                if (_strSubPartCodes != "")
                {
                    string[] listofCodes = _strSubPartCodes.Split('+');

                    MyWSContext ws = new MyWSContext();
                    foreach (string subcode in listofCodes)
                    {
                        if (subcode != "")
                        {
                            string[] subcodewqty = subcode.Split('`');
                            string _strInput = String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + _strAssetCode + "`" + subcodewqty[0] + "`" + subcodewqty[1];
                            string[] _strOutput = ws.AMSWebService.SettingAssetCatalog_SaveRecordSubParts(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');
                        }
                    }
                    return Json(new { Success = true, msg = "Successful" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, msg = "Please select at least one item." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

        }

        public JsonResult DeleteSubPart(AssetCatalogModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "ASSETSUBPART", Session["CustomerID"].ToString()).Split('`');

                if (_strOutput[0] == "true")
                {
                    ViewBag.SystemError = "";
                    ViewBag.SystemSuccess = _strOutput[1];
                    return Json(new { Success = true, msg = ViewBag.SystemSuccess }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    ViewBag.SystemError = _strOutput[1];
                    ViewBag.SystemSuccess = "";

                }

            }
            catch { }
            return Json(new { Success = false, msg = "" }, JsonRequestBehavior.AllowGet);

        }
        //

        //SUPPLIER
        public PartialViewResult ViewSuppliers(string _strAssetCode)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = _strAssetCode + "````";
                using (DataTable _DTRecord = ws.AMSWebService.LoadAssetSupplier_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    ViewData["SupplierRecordData"] = _DTRecord;

                }
            }
            catch
            {
                DataTable dt = new DataTable();
                ViewData["SupplierRecordData"] = dt;
            }
            return PartialView();
        }

        public PartialViewResult ViewSuppliersSelection(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetCatalogModel Data = new AssetCatalogModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchText = _strFilter.Split('`')[2];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[1]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                using (DataTable _DTRecord = ws.AMSWebService.LoadSuppliers_Where(_strFilter,Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;
                     
                        _DTRecord.DefaultView.Sort = "fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetSupplierRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;                  
                }
            }
            catch
            {             
                ViewData["AssetSupplierRecordData"] = dt;
            }
            return PartialView(Data);
        }

        public PartialViewResult ViewSuppliersFilter()
        {
            AssetCatalogModel Data = new AssetCatalogModel();
            Data.cbNature = GetDropDownItem(true, "NATURES");
            Data.cbPageLimit = GetDropDownPageLimit();
            Data.fldPageLimit = 10;
            ViewBag.PageNo = 1;
            return PartialView(Data);
        }

        public JsonResult SaveSupplier(string _strAssetCode, string _strSupplierCodes)
        {
            try
            {
                if (_strSupplierCodes != "")
                {
                    string[] listofCodes = _strSupplierCodes.Split('+');

                    MyWSContext ws = new MyWSContext();
                    foreach (string subcode in listofCodes)
                    {
                        if (subcode != "")
                        {                            
                            string _strInput = String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + _strAssetCode + "`" + subcode;
                            string[] _strOutput = ws.AMSWebService.SettingAssetCatalog_SaveRecordSupplier(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');
                        }
                    }
                    return Json(new { Success = true, msg = "Successful" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, msg = "Please select at least one item." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

        }

        public JsonResult DeleteSupplier(AssetCatalogModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "ASSETSUPPLIER", Session["CustomerID"].ToString()).Split('`');

                if (_strOutput[0] == "true")
                {
                    ViewBag.SystemError = "";
                    ViewBag.SystemSuccess = _strOutput[1];
                    return Json(new { Success = true, msg = ViewBag.SystemSuccess }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    ViewBag.SystemError = _strOutput[1];
                    ViewBag.SystemSuccess = "";

                }

            }
            catch { }
            return Json(new { Success = false, msg = "" }, JsonRequestBehavior.AllowGet);

        }
        //
        //

        //Search Combo Box
        public PartialViewResult ViewClassificationSearch(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetCatalogModel Data = new AssetCatalogModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchTextClassification = _strFilter.Split('`')[1];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = "```" + Data.fldSearchTextClassification + "`1";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingClassification_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetClassificationRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }
            }
            catch
            {
                ViewData["AssetClassificationRecordData"] = dt;
            }
            return PartialView(Data);
        }


        public PartialViewResult ViewCategorySearch(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetCatalogModel Data = new AssetCatalogModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchTextCategory = _strFilter.Split('`')[1];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = "```" + Data.fldSearchTextCategory + "`1";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCategory_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetCategoryRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }
            }
            catch
            {
                ViewData["AssetCategoryRecordData"] = dt;
            }
            return PartialView(Data);
        }

        public PartialViewResult ViewTypeSearch(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetCatalogModel Data = new AssetCatalogModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchTextType = _strFilter.Split('`')[1];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = "```" + Data.fldSearchTextType + "`1";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingType_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetTypeRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }
            }
            catch
            {
                ViewData["AssetTypeRecordData"] = dt;
            }
            return PartialView(Data);
        }

        public PartialViewResult ViewModelSearch(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetCatalogModel Data = new AssetCatalogModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchTextModel = _strFilter.Split('`')[1];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = "```" + Data.fldSearchTextModel + "`1";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingModel_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetModelRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }
            }
            catch
            {
                ViewData["AssetModelRecordData"] = dt;
            }
            return PartialView(Data);
        }

        public PartialViewResult ViewBrandSearch(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetCatalogModel Data = new AssetCatalogModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchTextBrand = _strFilter.Split('`')[1];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = "```" + Data.fldSearchTextBrand + "`1";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingBrand_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetBrandRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }
            }
            catch
            {
                ViewData["AssetBrandRecordData"] = dt;
            }
            return PartialView(Data);
        }

        //
        public PartialViewResult ViewAssetMainSubInfo(string _strCode, string _isMain)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                using (DataTable _DTRecord = ws.AMSWebService.LoadAssetMainSubInfo_Where(_strCode, bool.Parse(_isMain), Session["CustomerID"].ToString()))
                {
                    ViewData["AssetMainSubRecordData"] = _DTRecord;
                }
            }
            catch
            {
                DataTable dt = new DataTable();
                ViewData["AssetMainSubRecordData"] = dt;

            }

            return PartialView();
        }


    }
}