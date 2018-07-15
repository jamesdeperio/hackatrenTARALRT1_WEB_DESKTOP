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
    public class AssetReceivedController : Controller
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
                if (bool.Parse(Session["AssetReceived"].ToString()) == false)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch {                
                return RedirectToAction("Index", "Home");
            }

            Session["CurrentMenu"] = "InventoryManagement";
            AssetReceivedModel Data = new AssetReceivedModel();

            if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
                Data.fldPageLimit = 10;
            else
                Data.fldPageLimit = int.Parse(Session["fldPageLimit"].ToString());

            ViewBag.From = Session["_strFromDate"];
            ViewBag.To = Session["_strToDate"];
            ViewBag.Search = Session["fldSearchText"];
            ViewBag.fldSupplierCode = Session["fldSupplierCode"];
            ViewBag.fldWarehouseCode = Session["fldWarehouseCode"];
        

            Data.fldFromDate = ViewBag.From==""| ViewBag.From ==null? null:String.Format("{0:yyyy-MM-dd}", DateTime.Parse(ViewBag.From));
            Data.fldToDate = ViewBag.To == "" | ViewBag.To == null ? null : String.Format("{0:yyyy-MM-dd}", DateTime.Parse(ViewBag.To));
            Data.fldSearchText = ViewBag.Search;
            Data.fldSupplierCode = ViewBag.fldSupplierCode;
            Data.fldWarehouseCode = ViewBag.fldWarehouseCode;
           
            try
            {
                MyWSContext ws = new MyWSContext();

                            
                string _strWhereStatement = "`" + ViewBag.From + "`" + ViewBag.To + "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`" + Data.fldSupplierCode + "`" + Data.fldWarehouseCode;
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingReceivingfromSupplier_Where(_strWhereStatement, Session["CustomerID"].ToString()))
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
                           
                        
                       

                        _DTRecord.DefaultView.Sort = "fldDRDate desc";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }

                Data.cbPageLimit = GetDropDownPageLimit();

                Data.cbSupplier = GetDropDownItem(true, "Supplier");
                Data.cbWarehouse = GetDropDownItem(true, "Warehouse");
                

                if (Session["fldSupplierCode"] != null)
                {
                    if (Session["fldSupplierCode"].ToString() != "")
                        Data.fldSupplierCode = Session["fldSupplierCode"].ToString();
                }
                if (Session["fldWarehouseCode"] != null)
                {
                    if (Session["fldWarehouseCode"].ToString() != "")
                        Data.fldWarehouseCode = Session["fldWarehouseCode"].ToString();
                }
            
               
            }
            catch { }
         
            return View(Data);
        }



        [HttpPost]
        public ActionResult List(AssetReceivedModel Data)
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
                string _strWhereStatement = "`" + _strFromDate + "`" + _strToDate + "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`" + Data.fldSupplierCode + "`" + Data.fldWarehouseCode;

                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingReceivingfromSupplier_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    Session["_strFromDate"] = _strFromDate;
                    Session["_strToDate"] = _strToDate;
                    Session["fldSearchText"] = Data.fldSearchText;
                    Session["fldPageLimit"] = Data.fldPageLimit;


                    Session["fldSupplierCode"] = Data.fldSupplierCode;
                    Session["fldWarehouseCode"] = Data.fldWarehouseCode;                 

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

                        _DTRecord.DefaultView.Sort = "fldDRDate desc";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;

                }

                Data.cbPageLimit = GetDropDownPageLimit();

                Data.cbSupplier = GetDropDownItem(true, "Supplier");
                Data.cbWarehouse = GetDropDownItem(true, "Warehouse");
             

                if (Session["fldSupplierCode"] != null)
                {
                    if (Session["fldSupplierCode"].ToString() != "")
                        Data.fldSupplierCode = Session["fldSupplierCode"].ToString();
                }
                if (Session["fldWarehouseCode"] != null)
                {
                    if (Session["fldWarehouseCode"].ToString() != "")
                        Data.fldWarehouseCode = Session["fldWarehouseCode"].ToString();
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
                if (bool.Parse(Session["AssetReceivedCreateAccess"].ToString()) != true)
                {
                    return RedirectToAction("List", "AssetReceived");
                }
            }
            catch
            {
                return RedirectToAction("List", "AssetReceived");
            }

            AssetReceivedModel Data = new AssetReceivedModel();


            try
            {
                
                Data.fldCode = "AR" + DateTime.Now.ToString().Remove(DateTime.Now.ToString().Length - 2).Replace("/", "").Replace(" ", "").Replace(":", "");
                Data.fldDateEncoded = DateTime.Now;
                Data.cbCurrency = GetDropDownItem(false,"Currency");
                Data.cbSupplier = GetDropDownItem(false, "Supplier");
                Data.cbWarehouse = GetDropDownItem(false, "Warehouse");
                Data.cbTransactionBy = GetDropDownItem(false, "Employees");
                Data.cbReceviedBy = GetDropDownItem(false, "Employees");
                Data.fldTransactionByCode = Session["_strCurrentUserCode"].ToString();
                Session["fldAuthorizedStatus"] = "";
            }
            catch
            {

            }

            //List_Asset(Data.fldCode);

            return View(Data);
        }

        public JsonResult Delete(AssetReceivedModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "ASSETRECEIVEDS",Session["CustomerID"].ToString()).Split('`');

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

        public JsonResult Save(AssetReceivedModel Data)
        {
            try
            {                                                                                                                                                                                                                                                                                                           
                MyWSContext ws = new MyWSContext();
                string _strInput = "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldCode + "`" + Data.fldSupplierCode + "`" + Data.fldPONo + "`" + Data.fldDRNo + "`" + Data.fldDRDate + "`" + Data.fldCurrency + "`" + Data.fldWarehouseCode + "`" + Data.fldRemarks + "`" + Data.fldTransactionByCode + "`" + Data.fldReceivedByCode + "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldAuthorizedByCode + "`" + Data.fldAuthorizedDateTime + "`" + Data.fldAuthorizedComment + "`" + Data.fldAuthorizedStatus;
                string[] _strOutput = ws.AMSWebService.SettingReceivingfromSupplier_SaveRecord(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');

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

        public JsonResult GetEditRecord(AssetReceivedModel Data)
        {
            Session["CurrentID"] = Data.CurrentID;
            return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(AssetReceivedModel Data)
        {
            try
            {
                if (Session["_intCurrentID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                try
                {
                    if (bool.Parse(Session["AssetReceivedEditAccess"].ToString()) == false && bool.Parse(Session["AssetReceivedViewAccess"].ToString()) == false)
                    {
                        return RedirectToAction("List", "AssetReceived");
                    }
                }
                catch
                {
                    return RedirectToAction("List", "AssetReceived");
                }

                
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = Session["CurrentID"] + "`````````````````````";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingReceivingfromSupplier_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        foreach (DataRow DR in _DTRecord.Rows)
                        {
                            Data.cbCurrency = GetDropDownItem(false, "Currency");
                            Data.cbSupplier = GetDropDownItem(false, "Supplier");
                            Data.cbWarehouse = GetDropDownItem(false, "Warehouse");
                            Data.cbTransactionBy = GetDropDownItem(false, "Employees");
                            Data.cbReceviedBy = GetDropDownItem(false, "Employees");

                            Data.CurrentID = DR[0].ToString();
                            Data.fldDateEncoded = DateTime.Parse(DR[1].ToString());
                            Data.fldCode = DR[2].ToString();
                            Data.fldSupplierCode = DR[3].ToString();
                            Data.fldPONo = DR[4].ToString();
                            Data.fldDRNo = DR[5].ToString();
                            Data.fldDRDate = DateTime.Parse(DR[6].ToString());
                            Data.fldCurrency = DR[7].ToString();
                            Data.fldWarehouseCode = DR[8].ToString();
                            Data.fldRemarks = DR[9].ToString();
                            Data.fldTransactionByCode = DR[10].ToString();
                            Data.fldReceivedByCode = DR[11].ToString();

                            Session["fldAuthorizedStatus"] = DR[16].ToString();

                    }
                    }
                

                return View(Data);


            }
            catch { }

            return View(Data);

        }

        public JsonResult Update(AssetReceivedModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();
                string _strInput = Data.CurrentID + "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldCode + "`" + Data.fldSupplierCode + "`" + Data.fldPONo + "`" + Data.fldDRNo + "`" + Data.fldDRDate + "`" + Data.fldCurrency + "`" + Data.fldWarehouseCode + "`" + Data.fldRemarks + "`" + Data.fldTransactionByCode + "`" + Data.fldReceivedByCode + "`" + String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now) + "`" + Data.fldAuthorizedByCode + "`" + Data.fldAuthorizedDateTime + "`" + Data.fldAuthorizedComment + "`" + Data.fldAuthorizedStatus;                
                string[] _strOutput = ws.AMSWebService.SettingReceivingfromSupplier_SaveRecord(true, "", _strInput, Session["CustomerID"].ToString()).Split('`');
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


        public JsonResult CheckCode(AssetReceivedModel Data)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string _strInput = Data.fldCode;
                string[] _strOutput = ws.AMSWebService.ReceivingfromSupplier_CheckCode(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');

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


        public JsonResult SaveLock(string _strInput)
        {
            try
            {
                MyWSContext ws = new MyWSContext();               
                string[] _strOutput = ws.AMSWebService.SettingReceivingfromSupplierLOCK_SaveRecord(true, "", _strInput + "`" + Session["_strCurrentUserCode"].ToString(), Session["CustomerID"].ToString()).Split('`');

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


            if (_strName == "Currency")
            {
                try
                {
                    if (_isWithAll)
                        items.Add(new DropDown { Name = "All (Currencies)", Id = "-1" });

                    items.Add(new DropDown { Name = "PHP Philippines Piso", Id = "PHP" });
                    items.Add(new DropDown { Name = "USD United States Dollar", Id = "USD" });
                    items.Add(new DropDown { Name = "SGD Singapore Dollar", Id = "SGD" });
                    items.Add(new DropDown { Name = "MYR Malaysia Ringgit", Id = "MYR" });

                }
                catch { }
            }

            if (_strName == "Employees")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();                  
                    using (DataTable _DTRecord = ws.AMSWebService.LoadEmployeesTransSelection_Where("ReceivingfromSupplier", Session["CustomerID"].ToString()))
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


            if (_strName == "Supplier")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1``";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingSupplier_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Supplier)", Id = "-1" });
                        foreach (DataRow DR in _DTRecord.Rows)
                        {

                            items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
                        }

                    }


                }
                catch { }
            }

            if (_strName == "Warehouse")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();

                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingWarehouse_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "All (Warehouse)", Id = "-1" });
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

            if (_strName == "CostCenter")
            {
                try
                {
                    MyWSContext ws = new MyWSContext();
                    string _strWhereStatement = "````1";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCostCenter_Where(_strWhereStatement,Session["CustomerID"].ToString()))
                    {
                        if (_isWithAll)
                            items.Add(new DropDown { Name = "", Id = "-1" });
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



        //SUPPLIER
        
        public PartialViewResult ViewSuppliersSelection(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetReceivedModel Data = new AssetReceivedModel();
            try
            {
                _strFilter = _strFilter.Replace("undefined", "");

                Data.fldPageLimit = 10;
                try
                {
                    Data.fldSearchTextSupplier = _strFilter.Split('`')[2];
                    Data.fldPageNo = int.Parse(_strFilter.Split('`')[1]);

                }
                catch
                {
                    Data.fldPageNo = 1;

                }

                MyWSContext ws = new MyWSContext();
                using (DataTable _DTRecord = ws.AMSWebService.LoadSuppliers_Where(_strFilter, Session["CustomerID"].ToString()))
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
            AssetReceivedModel Data = new AssetReceivedModel();
            Data.cbNature = GetDropDownItem(true, "NATURES");
            Data.cbPageLimit = GetDropDownPageLimit();
            Data.fldPageLimit = 10;
            ViewBag.PageNo = 1;
            return PartialView(Data);
        }

        ////

            //Warehouse
        public PartialViewResult ViewWarehouseSearch(string _strFilter)
        {
            DataTable dt = new DataTable();
            AssetReceivedModel Data = new AssetReceivedModel();
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
        public PartialViewResult ViewAssets(string _strReceivedCode)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string _strWhereStatement = _strReceivedCode + "``````";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetAsset_Where(_strWhereStatement, Session["CustomerID"].ToString()))
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
            AssetReceivedModel Data = new AssetReceivedModel();
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
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingAssetWOAsset_Where(_strFilter, Session["CustomerID"].ToString()))
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
                    Data.cbCostCenter = GetDropDownItem(true, "CostCenter");
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
            AssetReceivedModel Data = new AssetReceivedModel();
            Data.cbClassification = GetDropDownItem(true, "Classification");
            Data.cbCategory = GetDropDownItem(true, "Category");
            Data.cbType = GetDropDownItem(true, "Type");
            Data.cbBrand = GetDropDownItem(true, "Brand");
            Data.cbModel = GetDropDownItem(true, "Model");
            Data.cbPageLimit = GetDropDownPageLimit();
            Data.fldPageLimit = 10;
            ViewBag.PageNo = 1;

            Data.fldSearchText = _strSearchtext;

            return PartialView(Data);
        }



        public JsonResult SaveAsset(string _strReceivedCode, string _strAssetCodes)
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
                            string _strInput = _strReceivedCode + "`" + subcodewqty[0] + "`" + subcodewqty[1] + "`" + subcodewqty[2] + "`" + subcodewqty[3] + "`" + subcodewqty[4] + "`" + subcodewqty[5];
                            string[] _strOutput = ws.AMSWebService.SettingAssetReceiving_SaveRecordAssets(false, "", _strInput, Session["CustomerID"].ToString()).Split('`');
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


        public JsonResult DeleteAsset(AssetReceivedModel Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "RECEIVEDITEMS", Session["CustomerID"].ToString()).Split('`');

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


    }
}