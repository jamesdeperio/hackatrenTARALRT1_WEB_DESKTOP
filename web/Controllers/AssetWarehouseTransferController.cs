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
    public class AssetWarehouseTransferController : Controller
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
                if (bool.Parse(Session["AssetWarehouseTransfer"].ToString()) == false)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch {                
                return RedirectToAction("Index", "Home");
            }

            Session["CurrentMenu"] = "InventoryManagement";
            AssetWarehouseTransferModel Data = new AssetWarehouseTransferModel();

            if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
                Data.fldPageLimit = 10;
            else
                Data.fldPageLimit = int.Parse(Session["fldPageLimit"].ToString());

            ViewBag.From = Session["_strFromDate"];
            ViewBag.To = Session["_strToDate"];
            ViewBag.Search = Session["fldSearchText"];
            ViewBag.fldFromWarehouseCode = Session["fldFromWarehouseCode"];
            ViewBag.fldToWarehouseCode = Session["fldToWarehouseCode"];
        

            Data.fldFromDate = ViewBag.From==""| ViewBag.From ==null? null:String.Format("{0:yyyy-MM-dd}", DateTime.Parse(ViewBag.From));
            Data.fldToDate = ViewBag.To == "" | ViewBag.To == null ? null : String.Format("{0:yyyy-MM-dd}", DateTime.Parse(ViewBag.To));
            Data.fldSearchText = ViewBag.Search;
            Data.fldFromWarehouseCode= ViewBag.fldFromWarehouseCode;
            Data.fldToWarehouseCode = ViewBag.fldToWarehouseCode;
           
            try
            {
                MyWSContext ws = new MyWSContext();                            
                string _strWhereStatement = "`" + ViewBag.From + "`" + ViewBag.To + "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`" + Data.fldFromWarehouseCode + "`" + Data.fldToWarehouseCode;
                using (DataTable _DTRecord = ws.AMSWebService.LoadWarehouseTransfer_Where(_strWhereStatement, Session["CustomerID"].ToString()))
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
                           
                        
                       

                        _DTRecord.DefaultView.Sort = "fldDateEncoded desc";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }

                Data.cbPageLimit = GetDropDownPageLimit();

                Data.cbFromWarehouse = GetDropDownItem(true, "FromWarehouse");
                Data.cbToWarehouse = GetDropDownItem(true, "ToWarehouse");
                

                if (Session["fldFromWarehouseCode"] != null)
                {
                    if (Session["fldFromWarehouseCode"].ToString() != "")
                        Data.fldFromWarehouseCode = Session["fldFromWarehouseCode"].ToString();
                }
                if (Session["fldToWarehouseCode"] != null)
                {
                    if (Session["fldToWarehouseCode"].ToString() != "")
                        Data.fldToWarehouseCode = Session["fldToWarehouseCode"].ToString();
                }
            
               
            }
            catch { }
         
            return View(Data);
        }



        [HttpPost]
        public ActionResult List(AssetWarehouseTransferModel Data)
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
                string _strWhereStatement = "`" + _strFromDate + "`" + _strToDate + "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`" + Data.fldFromWarehouseCode + "`" + Data.fldToWarehouseCode;

                using (DataTable _DTRecord = ws.AMSWebService.LoadWarehouseTransfer_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    Session["_strFromDate"] = _strFromDate;
                    Session["_strToDate"] = _strToDate;
                    Session["fldSearchText"] = Data.fldSearchText;
                    Session["fldPageLimit"] = Data.fldPageLimit;


                    Session["fldFromWarehouseCode"] = Data.fldFromWarehouseCode;
                    Session["fldToWarehouseCode"] = Data.fldToWarehouseCode;                 

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

                        _DTRecord.DefaultView.Sort = "fldDateEncoded desc";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;

                }

                Data.cbPageLimit = GetDropDownPageLimit();

                Data.cbFromWarehouse = GetDropDownItem(true, "FromWarehouse");
                Data.cbToWarehouse = GetDropDownItem(true, "ToWarehouse");
             

                if (Session["fldFromWarehouseCode"] != null)
                {
                    if (Session["fldFromWarehouseCode"].ToString() != "")
                        Data.fldFromWarehouseCode = Session["fldFromWarehouseCode"].ToString();
                }
                if (Session["fldToWarehouseCode"] != null)
                {
                    if (Session["fldToWarehouseCode"].ToString() != "")
                        Data.fldToWarehouseCode = Session["fldToWarehouseCode"].ToString();
                }
               
            }
            catch { }

            return View("List", Data);
        }


        public JsonResult CheckPendingTransaction()
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                int _intTempCount = ws.AMSWebService.LoadWarehouseTransferCount_Where(Session["CustomerID"].ToString());

                if (_intTempCount == 0)
                    _intTempCount = ws.AMSWebService.LoadOutgoingCount_Where(Session["CustomerID"].ToString());

                if (_intTempCount == 0)
                {                  
                    return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { Success = false, msg = "Creating new record might not be allowed at this moment due to pending transactions in (Warehouse Transfer/Outoging)." }, JsonRequestBehavior.AllowGet);

                }

            }
            catch { }
            return Json(new { Success = false, msg = "" }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Add()
        {
            if (Session["_intCurrentID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (bool.Parse(Session["AssetWarehouseTransferCreateAccess"].ToString()) != true)
                {
                    return RedirectToAction("List", "AssetWarehouseTransfer");
                }
            }
            catch
            {
                return RedirectToAction("List", "AssetWarehouseTransfer");
            }


          

            // DELETE TEMP SELETED Details
            MyWSContext ws = new MyWSContext();
            string[] _strOutput = ws.AMSWebService.AssetTagSelected_New_RemoveTemp("AssetWarehouseTransfer","", Session["CustomerID"].ToString()).Split('`');
            //

            int _intTempCount = ws.AMSWebService.LoadWarehouseTransferCount_Where(Session["CustomerID"].ToString());

            if (_intTempCount == 0)
                _intTempCount = ws.AMSWebService.LoadOutgoingCount_Where(Session["CustomerID"].ToString());

            if (_intTempCount > 0)
            {
                return RedirectToAction("List", "AssetWarehouseTransfer");

            }


            AssetWarehouseTransferModel Data = new AssetWarehouseTransferModel();


            try
            {
                
                Data.fldCode = "AT" + DateTime.Now.ToString().Remove(DateTime.Now.ToString().Length - 2).Replace("/", "").Replace(" ", "").Replace(":", "");
                Data.fldDateEncoded = DateTime.Now;                
                Data.cbFromWarehouse = GetDropDownItem(false, "FromWarehouse");
                Data.cbToWarehouse = GetDropDownItem(false, "ToWarehouse");
                Data.cbTransactionBy = GetDropDownItem(false, "Employees");
                Data.cbTransferredBy = GetDropDownItem(false, "Employees");
                Data.fldTransactionByCode = Session["_strCurrentUserCode"].ToString();
                Session["fldAuthorizedStatus"] = "";
            }
            catch
            {

            }

            //List_Asset(Data.fldCode);

            return View(Data);
        }

        public JsonResult Delete(AssetWarehouseTransferModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "ASSETWAREHOUSETRANSFER",Session["CustomerID"].ToString()).Split('`');

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

        public JsonResult Save(AssetWarehouseTransferModel Data)
        {
            try
            {                                                                                                                                                                                                                                                                                                           
                MyWSContext ws = new MyWSContext();
                string _strInput = "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldCode + "`" + Data.fldFromWarehouseCode + "`" + Data.fldToWarehouseCode + "`" + Data.fldRemarks + "`" + Data.fldTransactionByCode + "`" + Data.fldTransferredByCode + "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldAuthorizedByCode + "`" + Data.fldAuthorizedDateTime + "`" + Data.fldAuthorizedComment + "`" + Data.fldAuthorizedStatus;
                string[] _strOutput = ws.AMSWebService.WarehouseTransfer_SaveRecord(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');

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

        public JsonResult GetEditRecord(AssetWarehouseTransferModel Data)
        {
            Session["CurrentID"] = Data.CurrentID;
            return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(AssetWarehouseTransferModel Data)
        {
            try
            {
                if (Session["_intCurrentID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                try
                {
                    if (bool.Parse(Session["AssetWarehouseTransferEditAccess"].ToString()) == false && bool.Parse(Session["AssetWarehouseTransferViewAccess"].ToString()) == false)
                    {
                        return RedirectToAction("List", "AssetWarehouseTransfer");
                    }
                }
                catch
                {
                    return RedirectToAction("List", "AssetWarehouseTransfer");
                }

                
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = Session["CurrentID"] + "`````````````````````";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadWarehouseTransfer_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        foreach (DataRow DR in _DTRecord.Rows)
                        {
                            
                            Data.cbFromWarehouse = GetDropDownItem(false, "FromWarehouse");
                            Data.cbToWarehouse = GetDropDownItem(false, "ToWarehouse");
                            Data.cbTransactionBy = GetDropDownItem(false, "Employees");
                            Data.cbTransferredBy = GetDropDownItem(false, "Employees");

                            Data.CurrentID = DR[0].ToString();
                            Data.fldDateEncoded = DateTime.Parse(DR[1].ToString());
                            Data.fldCode = DR[2].ToString();
                            Data.fldFromWarehouseCode = DR[3].ToString();                                                        
                            Data.fldToWarehouseCode = DR[4].ToString();
                            Data.fldRemarks = DR[5].ToString();
                            Data.fldTransactionByCode = DR[6].ToString();
                            Data.fldTransferredByCode = DR[7].ToString();

                            Session["fldAuthorizedStatus"] = DR[12].ToString();

                    }
                    }
                

                return View(Data);


            }
            catch { }

            return View(Data);

        }

        public JsonResult Update(AssetWarehouseTransferModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();
                string _strInput = Data.CurrentID + "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldCode + "`" + Data.fldFromWarehouseCode + "`" + Data.fldToWarehouseCode + "`" + Data.fldRemarks + "`" + Data.fldTransactionByCode + "`" + Data.fldTransferredByCode + "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldAuthorizedByCode + "`" + Data.fldAuthorizedDateTime + "`" + Data.fldAuthorizedComment + "`" + Data.fldAuthorizedStatus;                
                string[] _strOutput = ws.AMSWebService.WarehouseTransfer_SaveRecord(true, "", _strInput, Session["CustomerID"].ToString()).Split('`');
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


        public JsonResult CheckCode(AssetWarehouseTransferModel Data)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string _strInput = Data.fldCode;
                string[] _strOutput = ws.AMSWebService.WarehouseTransfer_CheckCode(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');

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


        public JsonResult SaveApproval(string _strInput)
        {
            try
            {
                MyWSContext ws = new MyWSContext();               
                string[] _strOutput = ws.AMSWebService.WarehouseTransferApproval_SaveRecord(true, "", _strInput + "`" + Session["_strCurrentUserCode"].ToString(), Session["CustomerID"].ToString()).Split('`');

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



        public List<DropDown> GetDropDownItem(bool _isWithAll, string _strName)
        {
            List<DropDown> items = new List<DropDown>();

          

            if (_strName == "Employees")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();                  
                    using (DataTable _DTRecord = ws.AMSWebService.LoadEmployeesTransSelection_Where("WarehouseTransfer", Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Employees)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[1].ToString(), Id = DR[0].ToString() });
                        }

                    }


                }
                catch { }
            }


            if (_strName == "FromWarehouse")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingWarehouse_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (From Warehouse)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "ToWarehouse")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingWarehouse_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (To Warehouse)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }
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

            if (_strName == "Section")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingSection_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Section)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Rack")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingRack_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Rack)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Shelf")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingShelf_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Shelf)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }
            return items;
        }



    

            //Warehouse
        public PartialViewResult ViewWarehouseSearch(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetWarehouseTransferModel Data = new AssetWarehouseTransferModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchTextWarehouse = _strFilter.Split('`')[1];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = "```" + Data.fldSearchTextWarehouse + "`";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingWarehouse_Where(_strWhereStatement, Session["CustomerID"].ToString()))
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
                    ViewData["AssetWarehouseRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }
            }
            catch
            {
                ViewData["AssetWarehouseRecordData"] = dt;
            }
            return PartialView(Data);
        }
        //


        //ASSETS
        public PartialViewResult ViewAssets(string _strTransferCode)
        {
            try
            {
              
                MyWSContext ws = new MyWSContext();               
                string _strWhereStatement = _strTransferCode + "``````";
                using (DataTable _DTRecord = ws.AMSWebService.LoadWarehouseTransferAsset_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                
                    ViewData["AssetRecordData"] = _DTRecord;

                }
            }
            catch(Exception ex)
            {
                DataTable dt = new DataTable();
                ViewData["AssetRecordData"] = dt;
            }
            return PartialView();
        }
      
        public PartialViewResult ViewAssetsSelection(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetWarehouseTransferModel Data = new AssetWarehouseTransferModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 8;
                try
                {
                    Data.fldSearchText = _strFilter.Split('`')[6];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[5]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }
              
                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = "`" + _strFilter.Split('`')[8] + "`" + _strFilter.Split('`')[9] + "`" + _strFilter.Split('`')[10] + "`" + _strFilter.Split('`')[11] + "`" + _strFilter.Split('`')[0] + "`" + _strFilter.Split('`')[1] + "`" + _strFilter.Split('`')[2] + "`" + _strFilter.Split('`')[3] + "`" + _strFilter.Split('`')[4] + "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`" + _strFilter.Split('`')[12];
                using (DataTable _DTRecord = ws.AMSWebService.LoadWarehouseTransferAssetInventory_Where(_strWhereStatement, Session["CustomerID"].ToString()))                
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldName";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["AssetWOAssetRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    Data.cbPurchasingUOM = GetDropDownItem(false, "UOM");
                    ViewBag.fldPageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;

                }
            }
            catch
            {

                ViewData["AssetWOAssetRecordData"] = dt;
            }

            return PartialView(Data);
        }

        public PartialViewResult ViewAssetsFilter(string _strSearchtext)
        {
            AssetWarehouseTransferModel Data = new AssetWarehouseTransferModel();
            Data.cbClassification = GetDropDownItem(true, "Classification");
            Data.cbCategory = GetDropDownItem(true, "Category");
            Data.cbType = GetDropDownItem(true, "Type");
            Data.cbBrand = GetDropDownItem(true, "Brand");
            Data.cbModel = GetDropDownItem(true, "Model");

            Data.cbFromWarehouse = GetDropDownItem(false, "FromWarehouse");
            Data.cbSection = GetDropDownItem(true, "Section");
            Data.cbRack = GetDropDownItem(true, "Rack");
            Data.cbShelf = GetDropDownItem(true, "Shelf");

            Data.cbPageLimit = GetDropDownPageLimit();
            Data.fldPageLimit = 10;
            ViewBag.PageNo = 1;

            Data.fldSearchText = _strSearchtext;

            return PartialView(Data);
        }



        public JsonResult SaveAsset(string _strTransferCode, string _strAssetCodes)
        {
            try
            {
                if (_strAssetCodes != "")
                {
                    string[] listofCodes = _strAssetCodes.Split('+');

                    MyWSContext ws = new MyWSContext();
                    foreach (string subcode in listofCodes)
                    {
                        if (subcode != "")
                        {
                            string[] subcodewqty = subcode.Split('`');

                            string _strTemp5 = "";
                            string _strTemp6 = "";
                            try
                            {
                                _strTemp5 = subcodewqty[5];
                                _strTemp6 = subcodewqty[6];
                            }
                            catch
                            {
                            }

                            string _strInput = _strTransferCode + "`" + subcodewqty[0] + "`" + subcodewqty[1] + "`" + subcodewqty[2] + "`" + subcodewqty[3] + "`" + subcodewqty[4] + "`" + _strTemp5 + "`" + _strTemp6;
                            string[] _strOutput = ws.AMSWebService.WarehouseTransfer_SaveRecordAssets(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');
                            //if (_strOutput[0] == "true")
                            //{
                            //    ViewBag.SystemError = "";
                            //    ViewBag.SystemSuccess = _strOutput[1];
                            //    //return Json(new { Success = true, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

                            //}
                            //else
                            //{
                            //    //return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);
                            //}
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


        public JsonResult DeleteAsset(AssetWarehouseTransferModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "TRANSFERREDITEMS", Session["CustomerID"].ToString()).Split('`');

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

        public PartialViewResult ViewAssetMainSubInfo(string _strCode,string _isMain)
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

        public PartialViewResult ViewSerialSelected(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetWarehouseTransferModel Data = new AssetWarehouseTransferModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 8;
                try
                {
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[7]);
                    Data.fldSearchTextTag = _strFilter.Split('`')[5];
                }
                catch
                {
                    Data.fldPageNo = 1;

                }
                MyWSContext ws = new MyWSContext();
                using (DataTable _DTRecord = ws.AMSWebService.LoadAssetTagSeleted_Where(_strFilter, Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldSerialno,fldUID";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["TagSelectionRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.fldPageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                   


                }
            }
            catch
            {               
                ViewData["TagSelectionRecordData"] = dt;

            }

            return PartialView(Data);
        }

        public PartialViewResult ViewSerialSelection(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetWarehouseTransferModel Data = new AssetWarehouseTransferModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 8;
                try
                {
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[7]);
                    Data.fldSearchTextTag = _strFilter.Split('`')[5];
                }
                catch
                {
                    Data.fldPageNo = 1;

                }
                MyWSContext ws = new MyWSContext();
                using (DataTable _DTRecord = ws.AMSWebService.LoadAssetTags_Where(_strFilter, Session["CustomerID"].ToString()))
                {
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    try
                    {
                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        _DTRecord.DefaultView.Sort = "fldSerialno,fldUID";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["TagSelectionRecordData"] = dt;
                    Data.cbPageLimit = GetDropDownPageLimit();
                    ViewBag.fldPageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;



                }
            }
            catch
            {
                ViewData["TagSelectionRecordData"] = dt;

            }

            return PartialView(Data);
        }

        public JsonResult SaveTagSelected(string _strUID, string _strTransCode)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string[] _strOutput = ws.AMSWebService.AssetTagSelected_Save(_strUID, _strTransCode, Session["CustomerID"].ToString()).Split('`');
                if (_strOutput[0] == "true")
                {
                    return Json(new { Success = true, msg = "Successful" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

        }

        public JsonResult RemoveTagSelected(string _strUID, string _strTransCode)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string[] _strOutput = ws.AMSWebService.AssetTagSelected_Remove(_strUID, _strTransCode, Session["CustomerID"].ToString()).Split('`');
                return Json(new { Success = true, msg = "Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

        }

        public JsonResult RemoveTag_WarehouseSelected(string _strTransCode)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string[] _strOutput = ws.AMSWebService.AssetTagSelected_New_RemoveTemp("AssetWarehouseTransfer", _strTransCode, Session["CustomerID"].ToString()).Split('`');
                return Json(new { Success = true, msg = "Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

        }
        public JsonResult AssetTagSelected_RemoveTemp()
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string[] _strOutput = ws.AMSWebService.AssetTagSelected_RemoveTemp("AssetOutgoing", Session["CustomerID"].ToString()).Split('`');
                return Json(new { Success = true, msg = "Successful" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

        }

        public JsonResult BarcodeToUID_Select(string _strBarcode, string _strWarehouse, string _strTransCode)
        {
            try
            {
                string _strMsg = "";
                MyWSContext ws = new MyWSContext();
                using (DataTable _DTRecord = ws.AMSWebService.WarehouseTransfer_BarcodeToUID_Select(_strBarcode + "`" + _strWarehouse + "`" + _strTransCode, out _strMsg, Session["CustomerID"].ToString()))
                {
                    foreach (DataRow DR in _DTRecord.Rows)
                    {
                        return Json(new { Success = true, msg = _strMsg, UID = DR[3].ToString(), AssetCode = DR[0].ToString(), UOMCode = DR[1].ToString(), BatchCode = DR[2].ToString(), QTY = DR[5].ToString() }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { Success = false, msg = _strMsg }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }


        }


    }
}