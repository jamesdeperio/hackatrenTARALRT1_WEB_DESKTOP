using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using NXYSOFT_RMS.Models;
using System.IO;
using System.Web.Helpers;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace NXYSOFT_RMS.Controllers
{
    public class CommuterAccountsController : Controller
    {


      
    //    bool __isLogIn()
    //    {
    //        if (Session["_intCurrentID"] == null)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            try
    //            {
    //                using (DataTable _DTRecord = ws.AMSWebService.GetUserStatus(Session["_intCurrentID"].ToString(), Session["CustomerID"].ToString()))
    //                {
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {
    //                        if (DR[1].ToString().ToUpper() != Session["_strCurrentIDCode"].ToString())
    //                        {

    //                            Response.Cookies["AMSCookie"]["_intCurrentID"] = null;
    //                            Session.Clear();
    //                            return false;

    //                        }
    //                    }

    //                }
    //            }
    //            catch
    //            {
    //                Response.Cookies["AMSCookie"]["_intCurrentID"] = null;
    //                Session.Clear();
    //                return false;
    //            }
    //        }
    //        return true;
    //    }

    //    public ActionResult List()
    //    {

    //        if (!__isLogIn()) return RedirectToAction("Login", "Account");

    //        try
    //        {
    //            if (bool.Parse(Session["ItemInventory"].ToString()) == false)
    //            {
    //                return RedirectToAction("Index", "Home");
    //            }
    //        }
    //        catch
    //        {
    //            return RedirectToAction("Index", "Home");
    //        }

    //        Session["CurrentMenu"] = "InventoryManagement";
    //        CommuterAccountsModel Data = new CommuterAccountsModel();

    //        if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
    //            Data.fldPageLimit = 10;
    //        else
    //            Data.fldPageLimit = int.Parse(Session["fldPageLimit"].ToString());

    //        Data.cbClassification = GetDropDownItem(true, "Classification");
    //        Data.cbCategory = GetDropDownItem(true, "Category");
    //        Data.cbType = GetDropDownItem(true, "Type");
    //        Data.cbBrand = GetDropDownItem(true, "Brand");
    //        Data.cbModel = GetDropDownItem(true, "Model");

    //        Data.cbSupplier = GetDropDownItem(true, "Supplier");
    //        Data.cbWarehouse = GetDropDownItem(false, "Warehouse");
    //        Data.cbSection = GetDropDownItem(true, "Section");
    //        Data.cbRack = GetDropDownItem(true, "Rack");
    //        Data.cbShelf = GetDropDownItem(true, "Shelf");


    //        ViewBag.Search = Session["fldSearchText"];
    //        ViewBag.fldClassificationCode = Session["fldClassificationCode"];
    //        ViewBag.fldCategoryCode = Session["fldCategoryCode"];
    //        ViewBag.fldTypeCode = Session["fldTypeCode"];
    //        ViewBag.fldBrandCode = Session["fldBrandCode"];
    //        ViewBag.fldModelCode = Session["fldModelCode"];

    //        ViewBag.fldSupplierCode = Session["fldSupplierCode"];
    //        ViewBag.fldWarehouseCode = Session["fldWarehouseCode"];
    //        ViewBag.fldSectionCode = Session["fldSectionCode"];
    //        ViewBag.fldRackCode = Session["fldRackCode"];
    //        ViewBag.fldShelfCode = Session["fldShelfCode"];


    //        ViewBag.fldWithBatchReceived = Session["fldWithBatchReceived"];




    //        Data.fldSearchText = ViewBag.Search;
    //        Data.fldClassificationCode = ViewBag.fldClassificationCode;
    //        Data.fldCategoryCode = ViewBag.fldCategoryCode;
    //        Data.fldTypeCode = ViewBag.fldTypeCode;
    //        Data.fldBrandCode = ViewBag.fldBrandCode;
    //        Data.fldModelCode = ViewBag.fldModelCode;

    //        Data.fldSupplierCode = ViewBag.fldSupplierCode;
    //        Data.fldWarehouseCode = ViewBag.fldWarehouseCode;
    //        Data.fldSectionCode = ViewBag.fldSectionCode;
    //        Data.fldRackCode = ViewBag.fldRackCode;
    //        Data.fldShelfCode = ViewBag.fldShelfCode;
    //        Data.fldWithBatchReceived = (ViewBag.fldWithBatchReceived == null ? false : ViewBag.fldWithBatchReceived);

    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();

    //            string _strInventoryTotalValue = "0";
    //            string _strWhereStatement = Data.fldSupplierCode + "`" + Data.fldWarehouseCode + "`" + Data.fldSectionCode + "`" + Data.fldRackCode + "`" + Data.fldShelfCode + "`" + Data.fldClassificationCode + "`" + Data.fldCategoryCode + "`" + Data.fldTypeCode + "`" + Data.fldBrandCode + "`" + Data.fldModelCode + "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemInventory_Where(_strWhereStatement, Data.fldWithBatchReceived, out _strInventoryTotalValue, Session["CustomerID"].ToString()))
    //            {

    //                ViewBag.SystemError = "";
    //                ViewBag.SystemSuccess = "";
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                ViewBag.InventoryTotalValue = _strInventoryTotalValue;
    //                DataTable dt = new DataTable();
    //                try
    //                {


    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    if (Session["fldPageNo"] == null || int.Parse(Session["fldPageNo"].ToString()) == 0 || ViewBag.Pagging == 1)
    //                    {
    //                        Session["fldPageNo"] = 1;
    //                        Data.fldPageNo = 1;
    //                    }
    //                    else
    //                        Data.fldPageNo = int.Parse(Session["fldPageNo"].ToString());



    //                    if (Data.fldWithBatchReceived == false)
    //                    {
    //                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldAssetCode,fldUOMName,fldBatchReceived asc";
    //                    }
    //                    else
    //                    {
    //                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldAssetCode,fldUOMName, fldAssetLevel asc";
    //                    }

    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["RecordData"] = dt;

    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }

    //            Data.cbPageLimit = GetDropDownPageLimit();



    //            if (Session["fldClassificationCode"] != null)
    //            {
    //                if (Session["fldClassificationCode"].ToString() != "")
    //                    Data.fldClassificationCode = Session["fldClassificationCode"].ToString();
    //            }
    //            if (Session["fldCategoryCode"] != null)
    //            {
    //                if (Session["fldCategoryCode"].ToString() != "")
    //                    Data.fldCategoryCode = Session["fldCategoryCode"].ToString();
    //            }
    //            if (Session["fldTypeCode"] != null)
    //            {
    //                if (Session["fldTypeCode"].ToString() != "")
    //                    Data.fldTypeCode = Session["fldTypeCode"].ToString();
    //            }
    //            if (Session["fldBrandCode"] != null)
    //            {
    //                if (Session["fldBrandCode"].ToString() != "")
    //                    Data.fldBrandCode = Session["fldBrandCode"].ToString();
    //            }
    //            if (Session["fldModelCode"] != null)
    //            {
    //                if (Session["fldModelCode"].ToString() != "")
    //                    Data.fldModelCode = Session["fldModelCode"].ToString();
    //            }

    //            if (Session["fldSupplierCode"] != null)
    //            {
    //                if (Session["fldSupplierCode"].ToString() != "")
    //                    Data.fldSupplierCode = Session["fldSupplierCode"].ToString();
    //            }
    //            if (Session["fldWarehouseCode"] != null)
    //            {
    //                if (Session["fldWarehouseCode"].ToString() != "")
    //                    Data.fldWarehouseCode = Session["fldWarehouseCode"].ToString();
    //            }
    //            if (Session["fldSectionCode"] != null)
    //            {
    //                if (Session["fldSectionCode"].ToString() != "")
    //                    Data.fldSectionCode = Session["fldSectionCode"].ToString();
    //            }
    //            if (Session["fldRackCode"] != null)
    //            {
    //                if (Session["fldRackCode"].ToString() != "")
    //                    Data.fldRackCode = Session["fldRackCode"].ToString();
    //            }
    //            if (Session["fldShelfCode"] != null)
    //            {
    //                if (Session["fldShelfCode"].ToString() != "")
    //                    Data.fldShelfCode = Session["fldShelfCode"].ToString();
    //            }


    //        }
    //        catch { }


    //        return View(Data);
    //    }



    //    public JsonResult FindMe(string _strItemCode)
    //    {
    //        MyWSContext ws = new MyWSContext();
    //        try
    //        {
    //            CommuterAccountsModel Data = new CommuterAccountsModel();
    //            Session["fldSearchText"] = _strItemCode;
    //            string _strWhereStatement = _strItemCode;
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemInventoryFindMe_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                foreach (DataRow DR in _DTRecord.Rows)
    //                {
    //                    Data.fldAssetCode = DR["fldAssetCode"].ToString();
    //                    Data.fldWarehouseCode = DR["fldWarehouseCode"].ToString();
    //                    Data.fldSectionCode = DR["fldSectionCode"].ToString();
    //                    Data.fldRackCode = DR["fldRackCode"].ToString();
    //                    Data.fldShelfCode = DR["fldShelfCode"].ToString();
    //                    Data.fldClassificationCode = DR["fldClassificationCode"].ToString();
    //                    Data.fldCategoryCode = DR["fldCategoryCode"].ToString();
    //                    Data.fldTypeCode = DR["fldTypeCode"].ToString();
    //                    Data.fldBrandCode = DR["fldBrandCode"].ToString();
    //                    Data.fldModelCode = DR["fldModelCode"].ToString();


    //                    Session["fldSearchText"] = Data.fldAssetCode;
    //                    Session["fldClassificationCode"] = Data.fldClassificationCode;
    //                    Session["fldCategoryCode"] = Data.fldCategoryCode;
    //                    Session["fldTypeCode"] = Data.fldTypeCode;
    //                    Session["fldBrandCode"] = Data.fldBrandCode;
    //                    Session["fldModelCode"] = Data.fldModelCode;

    //                    Session["fldSupplierCode"] = Data.fldSupplierCode;
    //                    Session["fldWarehouseCode"] = Data.fldWarehouseCode;
    //                    Session["fldSectionCode"] = Data.fldSectionCode;
    //                    Session["fldRackCode"] = Data.fldRackCode;
    //                    Session["fldShelfCode"] = Data.fldShelfCode;
    //                }

    //                return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //        return null;
    //    }


    //    [HttpPost]
    //    public ActionResult List(CommuterAccountsModel Data)
    //    {
    //        try
    //        {
    //            if (Session["_intCurrentID"] == null)
    //            {
    //                return RedirectToAction("Login", "Account");
    //            }


    //            if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
    //                Data.fldPageLimit = 10;

    //            string _strInventoryTotalValue = "0";
    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = Data.fldSupplierCode + "`" + Data.fldWarehouseCode + "`" + Data.fldSectionCode + "`" + Data.fldRackCode + "`" + Data.fldShelfCode + "`" + Data.fldClassificationCode + "`" + Data.fldCategoryCode + "`" + Data.fldTypeCode + "`" + Data.fldBrandCode + "`" + Data.fldModelCode + "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemInventory_Where(_strWhereStatement, Data.fldWithBatchReceived, out _strInventoryTotalValue, Session["CustomerID"].ToString()))
    //            {

    //                Session["fldSearchText"] = Data.fldSearchText;
    //                Session["fldPageLimit"] = Data.fldPageLimit;


    //                Session["fldClassificationCode"] = Data.fldClassificationCode;
    //                Session["fldCategoryCode"] = Data.fldCategoryCode;
    //                Session["fldTypeCode"] = Data.fldTypeCode;
    //                Session["fldBrandCode"] = Data.fldBrandCode;
    //                Session["fldModelCode"] = Data.fldModelCode;

    //                Session["fldSupplierCode"] = Data.fldSupplierCode;
    //                Session["fldWarehouseCode"] = Data.fldWarehouseCode;
    //                Session["fldSectionCode"] = Data.fldSectionCode;
    //                Session["fldRackCode"] = Data.fldRackCode;
    //                Session["fldShelfCode"] = Data.fldShelfCode;
    //                Session["fldWithBatchReceived"] = Data.fldWithBatchReceived;

    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                ViewBag.InventoryTotalValue = _strInventoryTotalValue;

    //                DataTable dt = new DataTable();
    //                try
    //                {


    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    if (Data.fldPageCommand != null)
    //                    {
    //                        if (Session["fldPageNo"] == null)
    //                            Session["fldPageNo"] = 1;
    //                        else
    //                            Data.fldPageNo = int.Parse(Session["fldPageNo"].ToString());


    //                        if (Data.fldPageCommand.Contains(">>"))
    //                        {
    //                            if (ViewBag.Pagging > Data.fldPageNo)
    //                                Data.fldPageNo++;
    //                        }
    //                        else
    //                            Data.fldPageNo--;

    //                        if (Data.fldPageNo < 1)
    //                            Data.fldPageNo = 1;
    //                    }
    //                    else
    //                    { Data.fldPageNo = 1; }


    //                    if (Data.fldWithBatchReceived == false)
    //                    {
    //                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldAssetCode,fldUOMName,fldBatchReceived asc";
    //                    }
    //                    else
    //                    {
    //                        _DTRecord.DefaultView.Sort = "fldAssetLevel,fldAssetCode,fldUOMName, fldAssetLevel asc";
    //                    }

    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["RecordData"] = dt;

    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;

    //            }

    //            Data.cbPageLimit = GetDropDownPageLimit();

    //            Data.cbClassification = GetDropDownItem(true, "Classification");
    //            Data.cbCategory = GetDropDownItem(true, "Category");
    //            Data.cbType = GetDropDownItem(true, "Type");
    //            Data.cbBrand = GetDropDownItem(true, "Brand");
    //            Data.cbModel = GetDropDownItem(true, "Model");

    //            Data.cbSupplier = GetDropDownItem(true, "Supplier");
    //            Data.cbWarehouse = GetDropDownItem(false, "Warehouse");
    //            Data.cbSection = GetDropDownItem(true, "Section");
    //            Data.cbRack = GetDropDownItem(true, "Rack");
    //            Data.cbShelf = GetDropDownItem(true, "Shelf");

    //        }
    //        catch { }

    //        return View("List", Data);
    //    }




    //    public PartialViewResult ViewClassificationSelection(CommuterAccountsModel Data)
    //    {
    //        Data.cbClassification = GetDropDownItem(false, "Classification");
    //        return PartialView(Data);
    //    }

    //    public PartialViewResult ViewCategorySelection(CommuterAccountsModel Data)
    //    {
    //        Data.cbCategory = GetDropDownItem(false, "Category");
    //        return PartialView(Data);
    //    }

    //    public PartialViewResult ViewTypeSelection(CommuterAccountsModel Data)
    //    {
    //        Data.cbType = GetDropDownItem(false, "Type");
    //        return PartialView(Data);
    //    }
    //    public PartialViewResult ViewBrandSelection(CommuterAccountsModel Data)
    //    {
    //        Data.cbBrand = GetDropDownItem(false, "Brand");
    //        return PartialView(Data);
    //    }
    //    public PartialViewResult ViewModelSelection(CommuterAccountsModel Data)
    //    {
    //        Data.cbModel = GetDropDownItem(false, "Model");
    //        return PartialView(Data);
    //    }


    //    public List<DropDown> GetDropDownItem(bool _isWithAll, string _strName)
    //    {
    //        List<DropDown> items = new List<DropDown>();

    //        if (_strName == "Classification")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingClassification_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Classification)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });

    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Category")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCategory_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Category)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Type")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingType_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Type)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Brand")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingBrand_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Brand)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Model")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingModel_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Model)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Condition")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCondition_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Condition)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "UOM")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingUOM_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (UOM)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }



    //        if (_strName == "NATURES")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingSupplierNature_Where(Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (NATURE)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[0].ToString(), Id = DR[0].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }


    //        if (_strName == "Supplier")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1``";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingSupplier_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Supplier)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Warehouse")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingWarehouse_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Warehouse)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                        try
    //                        {
    //                            if (Session["fldWarehouseCode"].ToString() == "")
    //                            {
    //                                Session["fldWarehouseCode"] = DR[2].ToString();
    //                            }
    //                        }
    //                        catch
    //                        {
    //                            Session["fldWarehouseCode"] = DR[2].ToString();
    //                        }
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Section")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingSection_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Section)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Rack")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingRack_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Rack)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }

    //        if (_strName == "Shelf")
    //        {
    //            try
    //            {
    //                MyWSContext ws = new MyWSContext();

    //                string _strWhereStatement = "````1";
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingShelf_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //                {
    //                    if (_isWithAll)
    //                        items.Add(new DropDown { Name = "All (Shelf)", Id = "-1" });
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {

    //                        items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                    }

    //                }


    //            }
    //            catch { }
    //        }



    //        return items;
    //    }




    //    public ActionResult GetServerPath(string _strFilename)
    //    {

    //        try
    //        {
    //            string _path = Server.MapPath("~/images/Item/" + Session["CustomerID"].ToString() + "/");
    //            string[] fileNames = Directory.GetFiles(_path);
    //            foreach (string fileName in fileNames)
    //            {
    //                if (fileName.Contains(_strFilename))
    //                    return Content("../images/Item/" + Session["CustomerID"].ToString() + "/" + System.IO.Path.GetFileName(fileName));

    //            }
    //        }
    //        catch { }
    //        return Content("../images/no-img.jpg");
    //    }



    //    //SUB PART
    //    public PartialViewResult ViewSubParts(string _strItemCode)
    //    {
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = _strItemCode + "````";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingItemSubPart_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewData["SubPartRecordData"] = _DTRecord;

    //            }
    //        }
    //        catch
    //        {
    //            DataTable dt = new DataTable();
    //            ViewData["SubPartRecordData"] = dt;
    //        }
    //        return PartialView();
    //    }

    //    //Warehouse
    //    public PartialViewResult ViewWarehouseSearch(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 10;
    //            try
    //            {
    //                Data.fldSearchTextWarehouse = _strFilter.Split('`')[1];
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = "```" + Data.fldSearchTextWarehouse + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingWarehouse_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldName";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemWarehouseRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }
    //        }
    //        catch
    //        {
    //            ViewData["ItemWarehouseRecordData"] = dt;
    //        }
    //        return PartialView(Data);
    //    }
    //    //


    //    //SUPPLIER
    //    public PartialViewResult ViewSuppliers(string _strItemCode)
    //    {
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = _strItemCode + "````";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemSupplier_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewData["SupplierRecordData"] = _DTRecord;

    //            }
    //        }
    //        catch
    //        {
    //            DataTable dt = new DataTable();
    //            ViewData["SupplierRecordData"] = dt;
    //        }
    //        return PartialView();
    //    }

    //    public PartialViewResult ViewSuppliersSelection(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 10;
    //            try
    //            {
    //                Data.fldSearchText = _strFilter.Split('`')[2];
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[1]);

    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSuppliers_Where(_strFilter, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldName";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemSupplierRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }
    //        }
    //        catch
    //        {
    //            ViewData["ItemSupplierRecordData"] = dt;
    //        }
    //        return PartialView(Data);
    //    }

    //    public PartialViewResult ViewSuppliersFilter()
    //    {
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        Data.cbNature = GetDropDownItem(true, "NATURES");
    //        Data.cbPageLimit = GetDropDownPageLimit();
    //        Data.fldPageLimit = 10;
    //        ViewBag.PageNo = 1;
    //        return PartialView(Data);
    //    }

    //    //
    //    //

    //    //Search Combo Box
    //    public PartialViewResult ViewClassificationSearch(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 10;
    //            try
    //            {
    //                Data.fldSearchTextClassification = _strFilter.Split('`')[1];
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = "```" + Data.fldSearchTextClassification + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingClassification_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldName";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemClassificationRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }
    //        }
    //        catch
    //        {
    //            ViewData["ItemClassificationRecordData"] = dt;
    //        }
    //        return PartialView(Data);
    //    }


    //    public PartialViewResult ViewCategorySearch(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 10;
    //            try
    //            {
    //                Data.fldSearchTextCategory = _strFilter.Split('`')[1];
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = "```" + Data.fldSearchTextCategory + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCategory_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldName";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemCategoryRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }
    //        }
    //        catch
    //        {
    //            ViewData["ItemCategoryRecordData"] = dt;
    //        }
    //        return PartialView(Data);
    //    }

    //    public PartialViewResult ViewTypeSearch(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 10;
    //            try
    //            {
    //                Data.fldSearchTextType = _strFilter.Split('`')[1];
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = "```" + Data.fldSearchTextType + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingType_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldName";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemTypeRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }
    //        }
    //        catch
    //        {
    //            ViewData["ItemTypeRecordData"] = dt;
    //        }
    //        return PartialView(Data);
    //    }

    //    public PartialViewResult ViewModelSearch(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 10;
    //            try
    //            {
    //                Data.fldSearchTextModel = _strFilter.Split('`')[1];
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = "```" + Data.fldSearchTextModel + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingModel_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldName";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemModelRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }
    //        }
    //        catch
    //        {
    //            ViewData["ItemModelRecordData"] = dt;
    //        }
    //        return PartialView(Data);
    //    }

    //    public PartialViewResult ViewBrandSearch(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 10;
    //            try
    //            {
    //                Data.fldSearchTextBrand = _strFilter.Split('`')[1];
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[0]);

    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = "```" + Data.fldSearchTextBrand + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingBrand_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldName";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemBrandRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.PageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }
    //        }
    //        catch
    //        {
    //            ViewData["ItemBrandRecordData"] = dt;
    //        }
    //        return PartialView(Data);
    //    }

    //    //



    //    public JsonResult UpdateStorage(CommuterAccountsModel Data)
    //    {
    //        try
    //        {

    //            MyWSContext ws = new MyWSContext();
    //            string _strInput = Data.fldAssetCode + "`" + Data.fldUOMCode + "`" + Data.fldBatchReceived + "`" + Data.fldSectionCode + "`" + Data.fldShelfCode + "`" + Data.fldRackCode + "`" + Data.fldWarehouseCode;
    //            string[] _strOutput = ws.AMSWebService.AssignStorage_UpdateRecord(_strInput, Session["CustomerID"].ToString()).Split('`');
    //            if (_strOutput[0] == "true")
    //            {
    //                ModelState.AddModelError("", _strOutput[1]);
    //                return Json(new { Success = true, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);
    //            }
    //            else
    //            {
    //                ModelState.AddModelError("", _strOutput[1]);
    //                return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);
    //            }
    //        }
    //        catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

    //    }



    //    public PartialViewResult ViewItemSerialization(string _strFilter)
    //    {
    //        DataTable dt = new DataTable();
    //        CommuterAccountsModel Data = new CommuterAccountsModel();
    //        try
    //        {
    //            _strFilter = _strFilter.Replace("undefined", "");

    //            Data.fldPageLimit = 8;
    //            try
    //            {
    //                Data.fldPageNo = int.Parse(_strFilter.Split('`')[4]);
    //                Data.fldSearchTextTag = _strFilter.Split('`')[5];
    //                Data.fldConditionCode = _strFilter.Split('`')[7];
    //            }
    //            catch
    //            {
    //                Data.fldPageNo = 1;

    //            }

    //            MyWSContext ws = new MyWSContext();
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemSerialization_Where(_strFilter, Session["CustomerID"].ToString()))
    //            {
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //                try
    //                {
    //                    ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
    //                    if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
    //                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

    //                    _DTRecord.DefaultView.Sort = "fldSerialno,fldUID";
    //                    dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

    //                }
    //                catch { }
    //                ViewData["ItemSerializationRecordData"] = dt;
    //                Data.cbPageLimit = GetDropDownPageLimit();
    //                ViewBag.fldPageNo = Data.fldPageNo;
    //                Session["fldPageNo"] = Data.fldPageNo;
    //            }

    //            string _strfiltertemp = _strFilter.Split('`')[0] + "`" + _strFilter.Split('`')[1] + "`" + _strFilter.Split('`')[2] + "````" + _strFilter.Split('`')[6] + "`" + _strFilter.Split('`')[7];
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemSerialization_Where(_strfiltertemp, Session["CustomerID"].ToString()))
    //            {
    //                if (_DTRecord.Rows.Count > 0)
    //                {
    //                    ViewBag.RecordCountTag = _DTRecord.Rows.Count;
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {
    //                        ViewBag.RecordCountItemtoTag = DR[10];
    //                        break;
    //                    }
    //                }
    //                else { ViewBag.RecordCountTag = 0; ViewBag.RecordCountItemtoTag = 0; }
    //            }
    //        }
    //        catch
    //        {

    //            ViewData["ItemSerializationRecordData"] = dt;
    //            ViewBag.RecordCountItemtoTag = 0;
    //        }
    //        //temp
    //        Data.cbReceivingCode = GetDropDownPageLimit();
    //        Data.cbCondition = Data.cbBrand = GetDropDownItem(true, "Condition");
    //        //
    //        return PartialView(Data);
    //    }

    //    public JsonResult EditTag(string _strUID)
    //    {
    //        try
    //        {

    //            MyWSContext ws = new MyWSContext();

    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemEditSerialization_Where(_strUID, Session["CustomerID"].ToString()))
    //            {
    //                foreach (DataRow DR in _DTRecord.Rows)
    //                {
    //                    return Json(new { Success = true, uid = DR[0], serialno = DR[1], barcode = DR[2], rfid = DR[3], otherno = DR[4], TransCode = DR[5], OtherInfo = DR[6], ConditionCode = DR[7] }, JsonRequestBehavior.AllowGet);
    //                }
    //            }
    //        }
    //        catch { }
    //        return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);

    //    }

    //    public JsonResult GenerateBarcode()
    //    {
    //        try
    //        {
    //            CommuterAccountsModel Data = new CommuterAccountsModel();
    //            return Json(new { Success = true, barcode = Data.generateBarcode(), msg = "" }, JsonRequestBehavior.AllowGet);
    //        }
    //        catch { }
    //        return Json(new { Success = true, barcode = "", msg = "" }, JsonRequestBehavior.AllowGet);

    //    }


    //    public JsonResult Delete(CommuterAccountsModel Data)
    //    {
    //        try
    //        {

    //            MyWSContext ws = new MyWSContext();

    //            string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "ItemSerialization", Session["CustomerID"].ToString()).Split('`');

    //            if (_strOutput[0] == "true")
    //            {
    //                ViewBag.SystemError = "";
    //                ViewBag.SystemSuccess = _strOutput[1];
    //                return Json(new { Success = true, msg = ViewBag.SystemSuccess }, JsonRequestBehavior.AllowGet);

    //            }
    //            else
    //            {
    //                ViewBag.SystemError = _strOutput[1];
    //                ViewBag.SystemSuccess = "";
    //                return Json(new { Success = false, msg = ViewBag.SystemError }, JsonRequestBehavior.AllowGet);
    //            }

    //        }
    //        catch { }
    //        return Json(new { Success = false, msg = "" }, JsonRequestBehavior.AllowGet);

    //    }

    //    public JsonResult SaveItemTag(string _strInputs)
    //    {
    //        try
    //        {
    //            string _strUID = "UID" + DateTime.Now.ToString().Remove(DateTime.Now.ToString().Length - 2).Replace("/", "").Replace(" ", "").Replace(":", "");
    //            _strInputs = _strUID + "`" + _strInputs;

    //            MyWSContext ws = new MyWSContext();

    //            string[] _strInput = _strInputs.Split('`');

    //            if (_strInput[4].Trim() + _strInput[5].Trim() + _strInput[6].Trim() + _strInput[7].Trim() == "")
    //                return Json(new { Success = false, msg = "Please input unique tag at least in one of the boxes." }, JsonRequestBehavior.AllowGet);

    //            string[] _strOutput = ws.AMSWebService.ItemSerialization_SaveRecord(false, "", _strInputs, Session["CustomerID"].ToString()).Split('`');
    //            if (_strOutput[0] == "true")
    //            {

    //                return Json(new { Success = true, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

    //            }
    //            else
    //            {
    //                return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

    //            }


    //        }
    //        catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

    //    }

    //    public JsonResult UpdateItemTag(string _strInputs)
    //    {
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();

    //            string[] _strInput = _strInputs.Split('`');

    //            if (_strInput[4].Trim() + _strInput[5].Trim() + _strInput[6].Trim() + _strInput[7].Trim() == "")
    //                return Json(new { Success = false, msg = "Please input unique tag at least in one of the boxes." }, JsonRequestBehavior.AllowGet);

    //            string[] _strOutput = ws.AMSWebService.ItemSerialization_SaveRecord(true, "", _strInputs, Session["CustomerID"].ToString()).Split('`');
    //            if (_strOutput[0] == "true")
    //            {

    //                return Json(new { Success = true, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

    //            }
    //            else
    //            {
    //                return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

    //            }


    //        }
    //        catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

    //    }

    //    public PartialViewResult ViewItemMainSubInfo(string _strCode, string _isMain)
    //    {
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemMainSubInfo_Where(_strCode, bool.Parse(_isMain), Session["CustomerID"].ToString()))
    //            {
    //                ViewData["ItemMainSubRecordData"] = _DTRecord;
    //            }
    //        }
    //        catch
    //        {
    //            DataTable dt = new DataTable();
    //            ViewData["ItemMainSubRecordData"] = dt;

    //        }

    //        return PartialView();
    //    }

    //    public JsonResult GetCurrentStorage(string _strFilter)
    //    {
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            using (DataTable _DTRecord = ws.AMSWebService.GetCurrentStorage_Where(_strFilter, Session["CustomerID"].ToString()))
    //            {
    //                foreach (DataRow DR in _DTRecord.Rows)
    //                {
    //                    return Json(new { Success = true, _strData = DR[0] + "`" + DR[1] + "`" + DR[2] + "`" + DR[3] + "`" + DR[4] + "`" + DR[5] + "`" + DR[6] + "`" + DR[7] + "`" + DR[8] + "`" + DR[9] }, JsonRequestBehavior.AllowGet);
    //                }
    //                return Json(new { Success = false, _strData = _strFilter + "```````````" }, JsonRequestBehavior.AllowGet);

    //            }

    //        }
    //        catch { return Json(new { Success = false, msg = "" }, JsonRequestBehavior.AllowGet); }


    //    }

    //    public JsonResult GetDropDownConditionCode()
    //    {
    //        List<DropDown> items = new List<DropDown>();
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = "`````";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCondition_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                foreach (DataRow DR in _DTRecord.Rows)
    //                {

    //                    items.Add(new DropDown { Name = DR[3].ToString(), Id = DR[2].ToString() });
    //                }

    //            }

    //        }
    //        catch { }
    //        return Json(new SelectList(items, "Id", "Name"));
    //    }

    //    public JsonResult GetDropDownReceivingCode(string _strItemCode, string _strU0MCode, string _strBatchReceived)
    //    {
    //        List<DropDown> items = new List<DropDown>();
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            string _strWhereStatement = _strItemCode + "`" + _strU0MCode + "`" + _strBatchReceived + "`";
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadSerialization_ReceivingCode_Where(_strWhereStatement, Session["CustomerID"].ToString()))
    //            {
    //                foreach (DataRow DR in _DTRecord.Rows)
    //                {

    //                    items.Add(new DropDown { Name = DR[0].ToString(), Id = DR[0].ToString() });
    //                }

    //            }

    //        }
    //        catch { }
    //        return Json(new SelectList(items, "Id", "Name"));
    //    }

    //    public PartialViewResult ViewItemHistory(string _strUID)
    //    {
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemTagHistory_Where(_strUID, Session["CustomerID"].ToString()))
    //            {
    //                ViewData["ItemTagHistoryRecordData"] = _DTRecord;
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //            }
    //        }
    //        catch
    //        {
    //            DataTable dt = new DataTable();
    //            ViewData["ItemTagHistoryRecordData"] = dt;

    //        }

    //        return PartialView();
    //    }

    //    public PartialViewResult ViewItemDepreciation(string _strUID)
    //    {
    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            using (DataTable _DTRecord = ws.AMSWebService.LoadItemTagDepreciation_Where(_strUID, Session["CustomerID"].ToString()))
    //            {
    //                ViewData["ItemTagDepreciationRecordData"] = _DTRecord;
    //                ViewBag.RecordCount = _DTRecord.Rows.Count;
    //            }
    //        }
    //        catch
    //        {
    //            DataTable dt = new DataTable();
    //            ViewData["ItemTagDepreciationRecordData"] = dt;

    //        }

    //        return PartialView();
    //    }

    //    public JsonResult SetItemDepreciation(string _strInputs)
    //    {
    //        try
    //        {

    //            MyWSContext ws = new MyWSContext();

    //            string[] _strInput = _strInputs.Split('`');

    //            string[] _strOutput = ws.AMSWebService.ItemDepreciation_SaveRecord(false, "", _strInputs, Session["CustomerID"].ToString()).Split('`');
    //            if (_strOutput[0] == "true")
    //            {

    //                return Json(new { Success = true, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

    //            }
    //            else
    //            {
    //                return Json(new { Success = false, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

    //            }


    //        }
    //        catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }

    //    }


    //    public PartialViewResult ViewPic()
    //    {
    //        return PartialView();
    //    }
    //    private bool RemoveFileFromServer(string path, string _strfilename)
    //    {
    //        var fullPath = Request.MapPath(path);
    //        // if (!System.IO.File.Exists(fullPath)) return false;

    //        try //Maybe error could happen like Access denied or Presses Already User used
    //        {
    //            string[] fileNames = Directory.GetFiles(fullPath);
    //            foreach (string fileName in fileNames)
    //            {
    //                if (fileName.Contains(_strfilename))
    //                    System.IO.File.Delete(fileName);
    //            }

    //            return true;
    //        }
    //        catch (Exception e)
    //        {
    //            //Debug.WriteLine(e.Message);
    //        }
    //        return false;
    //    }
    //    public ActionResult FileUpload(HttpPostedFileBase file, string _strfilename)
    //    {
    //        string path = "";
    //        string pic = "";
    //        byte[] array = { };
    //        string extension = "";
    //        if (file != null && _strfilename != "")
    //        {
    //            pic = System.IO.Path.GetFileName(file.FileName);
    //            extension = Path.GetExtension(file.FileName);

    //            string _strPath = "~/images/Item/" + Session["CustomerID"].ToString() + "/Individual/";

    //            RemoveFileFromServer(_strPath, _strfilename);

    //            string _strUnique = DateTime.Now.ToString().Remove(DateTime.Now.ToString().Length - 2).Replace("/", "").Replace(" ", "").Replace(":", "");

    //            Directory.CreateDirectory(Server.MapPath(_strPath));
    //            path = System.IO.Path.Combine(Server.MapPath(_strPath), _strfilename + _strUnique + ".jpg");
    //            // file is uploaded

    //            WebImage img = new WebImage(file.InputStream);
    //            if (img.Width > 500)
    //            {
    //                img.Resize(500, 500);
    //            }

    //            img.Save(path);

    //            // save the image path path to the database or you can send image 
    //            // directly to database
    //            // in-case if you want to store byte[] ie. for DB
    //            //using (MemoryStream ms = new MemoryStream())
    //            //{
    //            //    file.InputStream.CopyTo(ms);
    //            //    array = ms.GetBuffer();

    //            //}


    //        }
    //        // after successfully uploading redirect the user
    //        return Content("");
    //    }

    //    public ActionResult GetPicSerial(string _strFilename)
    //    {

    //        try
    //        {
    //            string _path = Server.MapPath("~/images/Item/" + Session["CustomerID"].ToString() + "/Individual/");
    //            string[] fileNames = Directory.GetFiles(_path);
    //            foreach (string fileName in fileNames)
    //            {
    //                if (fileName.Contains(_strFilename))
    //                    return Content("../images/Item/" + Session["CustomerID"].ToString() + "/Individual/" + System.IO.Path.GetFileName(fileName));

    //            }
    //        }
    //        catch { }
    //        return Content("../images/no-img.jpg");
    //    }


    //    //@(dr["fldAssetCode"] + "`" + dr["fldUOMCode"] + "`" + dr["fldBatchReceived"] + "`" + dr["fldName"] + "`" + dr["fldSectionCode"] + "`" + dr["fldShelfCode"] + "`" + dr["fldRackCode"] + "`" + dr["fldWarehouseCode"] + "`" + dr["fldUOMName"] )'




    //    [HttpGet]
    //    public ActionResult ViewReport(string _strFilter)
    //    {

    //        string jsonErrorCode = "0";
    //        string msg = "";




    //        try
    //        {
    //            MyWSContext ws = new MyWSContext();
    //            string _strTempWarehouse = "";
    //            if (_strFilter != null)
    //            {

    //                string _strInventoryTotalValue = "0";
    //                string _strWhereStatement = _strFilter;
    //                using (DataTable _DTRecord = ws.AMSWebService.LoadItemInventory_Where(_strWhereStatement, bool.Parse(_strFilter.Split('`')[11]), out _strInventoryTotalValue, Session["CustomerID"].ToString()))
    //                {

    //                    Session["Filter"] = _strFilter;
    //                    Session["RecordDB"] = _DTRecord;
                       
    //                }
    //            }
    //            else
    //            {
    //                ReportDocument vReportSource = new ReportDocument();
    //                string strRptPath = System.Web.HttpContext.Current.Server.MapPath("~/") + "Reports//Rpt_Inventory.rpt";
    //                vReportSource.Load(strRptPath);
    //                vReportSource.SetDataSource((DataTable)Session["RecordDB"]);

    //                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingCompany_Where("`````", Session["CustomerID"].ToString()))
    //                {
    //                    foreach (DataRow DR in _DTRecord.Rows)
    //                    {
    //                        vReportSource.SetParameterValue("lblCompanyName", DR[3]);
    //                        vReportSource.SetParameterValue("lblLine1", DR[4]);
    //                        vReportSource.SetParameterValue("lblLine2", DR[5]);
    //                        vReportSource.SetParameterValue("lblLine3", DR[6]);
    //                        vReportSource.SetParameterValue("lblLine4", DR[7]);
    //                        vReportSource.SetParameterValue("lblLine5", DR[8]);
    //                        vReportSource.DataDefinition.FormulaFields["imglocation"].Text = "'" + GetReportLogo(DR[2].ToString()) + "'";

    //                    }

    //                }
                   
    //                    vReportSource.SetParameterValue("lblRangeCutoff", "List of Inventory from " + Session["Filter"].ToString().Split('`')[12].ToString());
                   
    //                //vReportSource.SetParameterValue("lblRangeCutoff", "List of Outgoing Transactoins from the warehouse " + _strFilter.Split('`')[3] + "for the Period of " + String.Format("{0:MM/dd/yyyy}", DateTime.Parse(_strFilter.Split('`')[0].ToString())) + " to " + String.Format("{0:MM/dd/yyyy}", DateTime.Parse(_strFilter.Split('`')[1].ToString())));


    //                vReportSource.ExportToHttpResponse(ExportFormatType.PortableDocFormat, System.Web.HttpContext.Current.Response, false, Session["CustomerID"].ToString());
    //                vReportSource.Dispose();


    //            }





    //        }
    //        catch (Exception ex) { return Json(new { result = "-2", err = ex }, JsonRequestBehavior.AllowGet); }


    //        return Json(new { result = jsonErrorCode, err = msg }, JsonRequestBehavior.AllowGet);

    //    }
    //    public ActionResult RefreshReport()
    //    {
    //        return PartialView("ReportDisplay");
    //    }

    //    public ActionResult ReportView()
    //    {
    //        return View();
    //    }
    //    public string GetReportLogo(string _strFilename)
    //    {
    //        try
    //        {
    //            string _path = Server.MapPath("~/images/Logo/" + Session["CustomerID"].ToString() + "/");

    //            string[] fileNames = Directory.GetFiles(_path);
    //            foreach (string fileName in fileNames)
    //            {
    //                if (fileName.Contains(_strFilename))
    //                    return _path + System.IO.Path.GetFileName(fileName);

    //            }
    //        }
    //        catch { }

    //        return Server.MapPath("~/images/Logo/logo2.png");
    //    }
    }
}