using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NXYSOFT_RMS.Models;
using System.Data;

namespace NXYSOFT_RMS.Controllers
{
    public class UserAccessController : Controller
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
                if (bool.Parse(Session["UserAccess"].ToString()) == false)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }

            Session["CurrentMenu"] = "Settings";

            UserAccess Data = new UserAccess();

            if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
                Data.fldPageLimit = 10;
            else
                Data.fldPageLimit = int.Parse(Session["fldPageLimit"].ToString());

            ViewBag.Search = Session["fldSearchText"];          
            Data.fldSearchText = ViewBag.Search;

            try
            {
                MyWSContext ws = new MyWSContext();


                string _strWhereStatement = "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingUserAccess_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {                 
                    ViewBag.RecordCount = _DTRecord.Rows.Count;
                    DataTable dt = new DataTable();
                    try
                    {


                        ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit;
                        if ((_DTRecord.Rows.Count > (Data.fldPageLimit * ViewBag.Pagging)) && ViewBag.Pagging > 0)
                            ViewBag.Pagging = _DTRecord.Rows.Count / Data.fldPageLimit + 1;

                        if (Session["fldPageNo"] == null || int.Parse(Session["fldPageNo"].ToString()) == 0 || ViewBag.Pagging == 1)
                        {
                            Session["fldPageNo"] = 1;
                            Data.fldPageNo = 1;
                        }
                        else
                            Data.fldPageNo = int.Parse(Session["fldPageNo"].ToString());




                        _DTRecord.DefaultView.Sort = "fldAccessName desc";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;
                }

                Data.cbPageLimit = GetDropDownPageLimit();


            }
            catch { }

            return View(Data);
        }


        [HttpPost]
        public ActionResult List(UserAccess Data)
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
               string _strWhereStatement = "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`";
                using (DataTable _DTRecord = ws.AMSWebService.LoadSettingUserAccess_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {                  
                    Session["fldSearchText"] = Data.fldSearchText;
                    Session["fldPageLimit"] = Data.fldPageLimit;

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

                        _DTRecord.DefaultView.Sort = "fldAccessName desc";
                        dt = _DTRecord.Select().Skip((Data.fldPageLimit * Data.fldPageNo) - Data.fldPageLimit).Take(Data.fldPageLimit).CopyToDataTable();

                    }
                    catch { }
                    ViewData["RecordData"] = dt;

                    ViewBag.PageNo = Data.fldPageNo;
                    Session["fldPageNo"] = Data.fldPageNo;

                }

                Data.cbPageLimit = GetDropDownPageLimit();


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
                if (bool.Parse(Session["UserAccessCreateAccess"].ToString()) != true)
                {
                    return RedirectToAction("List", "UserAccess");
                }
            }
            catch {
                return RedirectToAction("List", "UserAccess");
            }

            UserAccess Data = new UserAccess();
            try
            {
                MyWSContext ws = new MyWSContext();

                List<string> listofModules = new List<string>();
                string _strError = "";
               
                using (DataTable _DTRecord = ws.AMSWebService.LoadSetupModules("COMPLETE", Session["_strAccessName"].ToString(), Session["CustomerID"].ToString(), out _strError))
                {
                    foreach (System.Data.DataRow dr in _DTRecord.Rows)
                    {
                        listofModules.Add(dr[2].ToString() + "-" + dr[3].ToString() + "-" + dr[4].ToString());
                    }
                }

                Session["AddAccessModules"] = listofModules;
            }
            catch
            {

            }
            return View(Data);
        }

        public JsonResult Save(UserAccess Data, string _strModuleButton,bool _isEdit)
        {
            try
            {

                MyWSContext ws = new MyWSContext();
                string _strInput = "`" + Data.fldAccessName + "`" + _strModuleButton;
                string[] _strOutput = ws.AMSWebService.SettingUserAccess_SaveRecord(_isEdit, "", _strInput, Session["CustomerID"].ToString()).Split('`');

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
        public JsonResult Delete(UserAccess Data)
        {
            try
            {

                MyWSContext ws = new MyWSContext();

                string[] _strOutput = ws.AMSWebService.DELETESettingUserAccess_WHERE(Data.fldAccessName,Session["CustomerID"].ToString()).Split('`');

                if (_strOutput[0] == "true")
                {                    
                    return Json(new { Success = true, msg = _strOutput[1] }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    ViewBag.SystemError = _strOutput[1];                    

                }

            }
            catch { }
            return Json(new { Success = false, msg = ViewBag.SystemError }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetEditRecord(UserAccess Data)
        {
            Session["fldAccessName"] = Data.fldAccessName;
            return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(UserAccess Data)
        {
            try
            {
                if (Session["_intCurrentID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                try
                {
                    if (bool.Parse(Session["UserAccessEditAccess"].ToString()) == false && bool.Parse(Session["UserAccessViewAccess"].ToString()) == false)
                    {
                        return RedirectToAction("List", "UserAccess");
                    }
                }
                catch {
                    return RedirectToAction("List", "UserAccess");
                }

               
                MyWSContext ws = new MyWSContext();

                List<string> listofModules = new List<string>();
                string _strError = "";

                Data.fldAccessName = Session["fldAccessName"].ToString();
                using (DataTable _DTRecord = ws.AMSWebService.LoadSetupModules("COMPLETE", Data.fldAccessName, Session["CustomerID"].ToString(), out _strError))
                {
                    foreach (System.Data.DataRow dr in _DTRecord.Rows)
                    {
                        listofModules.Add(dr[2].ToString() + "-" + dr[3].ToString() + "-" + dr[4].ToString());
                    }
                }

                Session["EditAccessModules"] = listofModules;


                return View(Data);


            }
            catch { }

            return View(Data);

        }

    }
}