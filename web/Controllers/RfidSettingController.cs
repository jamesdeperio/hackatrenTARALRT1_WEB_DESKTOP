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
    public class RfidSettingController : Controller
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
      



      

        public ActionResult List()
        {
           
          

            Session["CurrentMenu"] = "Setting";
            RfidSettingModel Data = new RfidSettingModel();

            if (Session["fldPageLimit"] == null || Data.fldPageLimit == 0)
                Data.fldPageLimit = 10;
            else
                Data.fldPageLimit = int.Parse(Session["fldPageLimit"].ToString());
            
            Data.fldSearchText = ViewBag.Search;

            try
            {
                MyWSContext ws = new MyWSContext();

                            
                string _strWhereStatement = "`" + (Data.fldSearchText == null ? "" : Data.fldSearchText) + "`";
                using (DataTable _DTRecord = ws.AMSWebService.LoadRFIDSetting_Where(_strWhereStatement, Session["CustomerID"].ToString()))
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
                           
                        
                       

                        _DTRecord.DefaultView.Sort = "fldPCSerialNo";
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
        public ActionResult List(RfidSettingModel Data)
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
              
                string _strWhereStatement = "`" + (Data.fldSearchText==null?"":Data.fldSearchText);
                using (DataTable _DTRecord = ws.AMSWebService.LoadRFIDSetting_Where(_strWhereStatement, Session["CustomerID"].ToString()))
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

                        _DTRecord.DefaultView.Sort = "fldPCSerialNo";
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


        public JsonResult Save(RfidSettingModel Data)
        {
            try
            {
                    MyWSContext ws = new MyWSContext();
                    string _strInput = "``" + Data.fldPCSerialNo+ "`" + Data.fldRFIDIPaddress + "`" + Data.fldRFIDPortNo + "``````";
                    string[] _strOutput = ws.AMSWebService.RFIDSetting_SaveRecord( _strInput, Session["CustomerID"].ToString()).Split('`');

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


        public JsonResult Delete(RfidSettingModel Data)
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                string[] _strOutput = ws.AMSWebService.DeleteRecord_RecordID(Data.CurrentID, "TYPES", Session["CustomerID"].ToString()).Split('`');
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
        public JsonResult GetEditRecord(RfidSettingModel Data)
        {            
                Session["CurrentID"] = Data.CurrentID;
                return Json(new { Success = true, msg = "" }, JsonRequestBehavior.AllowGet);            
        }
        public ActionResult Edit(RfidSettingModel Data)
        {
            try
            {
              
                MyWSContext ws = new MyWSContext();

                string _strWhereStatement = Session["CurrentID"] + "````";
                using (DataTable _DTRecord = ws.AMSWebService.LoadRFIDSetting_Where(_strWhereStatement, Session["CustomerID"].ToString()))
                {
                    foreach (DataRow DR in _DTRecord.Rows)
                    {
                        Data.CurrentID = DR[0].ToString();
                        Data.fldPCSerialNo = DR[1].ToString();
                        Data.fldRFIDIPaddress = DR[2].ToString();
                        Data.fldRFIDPortNo = DR[3].ToString();

                        if (DR[5].ToString()=="true")
                        {
                            Data.fldIsActive = true;
                        }
                    }
                }
              

                
                return View(Data);


            }
            catch { }

            return View(Data);

        }

        public JsonResult Update(RfidSettingModel Data)
        {
            try
            {
             
                MyWSContext ws = new MyWSContext();
                string _strInput = "`" + Data.fldPCSerialNo + "`" + Data.fldRFIDIPaddress + "`" + Data.fldRFIDPortNo + "`" + Data.fldCurrentStatus + "`" + Data.fldLastCommand + "`````";
                string[] _strOutput = ws.AMSWebService.RFIDSetting_SaveRecord(_strInput, Session["CustomerID"].ToString()).Split('`');
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
     
    }
}