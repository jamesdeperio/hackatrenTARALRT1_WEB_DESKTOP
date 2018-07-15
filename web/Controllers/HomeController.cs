using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NXYSOFT_RMS.Models;
using Microsoft.AspNet.Identity;

namespace NXYSOFT_RMS.Controllers
{
    public class HomeController : Controller
    {
     
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

                            if (DateTime.Parse(DR[1].ToString()).Date != DateTime.Now.Date )
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
        public ActionResult Index()
        {
            if (!__isLogIn()) return RedirectToAction("Login", "Account");

            if (Session["_intCurrentID"] != null)
            {
                MyWSContext ws = new MyWSContext();
                try
                {
                    ViewBag.StaffRegistered = 0;
                    ViewBag.StaffPresenttoday = 0;                   
                    Session["CurrentMenu"] = "Dashboard";
                }
                catch { }

                List<string> listofModules = new List<string>();
                try
                {                    
                    
                    string _strError = "";
                    using (DataTable _DTRecord = ws.AMSWebService.LoadSetupModules("COMPLETE", Session["_strAccessName"].ToString(), Session["CustomerID"].ToString(),out _strError))
                    {
                        foreach (System.Data.DataRow dr in _DTRecord.Rows)
                        {
                            listofModules.Add(dr[2].ToString() + " - " + dr[4].ToString());                         
                        }
                    }
                    
                    Session["listofModules"] = listofModules;
                }
                catch { Session["listofModules"] = listofModules; }
              
                ViewBag.AccountRegistered = ws.AMSWebService.CountCommuterAccounts_Where(Session["CustomerID"].ToString());
                ViewBag.TodayActiveCommuters = ws.AMSWebService.CountCommuterAccounts_Today(Session["CustomerID"].ToString());
                ViewBag.RewardsTotal = ws.AMSWebService.RewardsTotal(Session["CustomerID"].ToString());
                ViewBag.ScannedAdsCount = ws.AMSWebService.ScannedAdsCount(Session["CustomerID"].ToString());
                ViewBag.PassengerCountsToday = ws.AMSWebService.PassengerCounts_Today(Session["CustomerID"].ToString());
                //ViewBag.DaysLeft = ws.AMSWebService.LoadDaysLeft_Where(Session["CustomerID"].ToString());
                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult About()
        {
            if (Session["_intCurrentID"] != null)
            {
                Session["CurrentMenu"] = "About";
                ViewBag.Message = "TARA! LRT1 - Communter Experience Management System";

                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public JsonResult GetTime()
        {
            try
            {
                if (Session["_intCurrentID"] == null)
                {
                    return Json(new { Success = false, msg = "" }, JsonRequestBehavior.AllowGet);
                }

                Session["_intCurrentID"] = Request.Cookies["AMSCookie"]["_intCurrentID"];
                Session["_intCurrentUserName"] = Request.Cookies["AMSCookie"]["_intCurrentUserName"];
                Session["_strCurrentFullName"] = Request.Cookies["AMSCookie"]["_strCurrentFullName"];
                Session["_strAccessName"] = Request.Cookies["AMSCookie"]["_strAccessName"];
                Session["_strCurrentIDCode"] = Request.Cookies["AMSCookie"]["CurrentIDCode"];
                Session["CustomerID"] = Request.Cookies["AMSCookieCustomerID"]["CustomerID"];
                Session["_strCurrentUserCode"] = Request.Cookies["AMSCookie"]["_strCurrentUserCode"];
                return Json(new { Success = true, msg = DateTime.Now.ToLongTimeString() }, JsonRequestBehavior.AllowGet);
                
            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }


        }


        public JsonResult GetDashBoardValue()
        {
            try
            {
                MyWSContext ws = new MyWSContext();
                ViewBag.AccountRegistered = ws.AMSWebService.CountCommuterAccounts_Where(Session["CustomerID"].ToString());
                ViewBag.TodayActiveCommuters = ws.AMSWebService.CountCommuterAccounts_Today(Session["CustomerID"].ToString());
                ViewBag.RewardsTotal = ws.AMSWebService.RewardsTotal(Session["CustomerID"].ToString());
                ViewBag.ScannedAdsCount = ws.AMSWebService.ScannedAdsCount(Session["CustomerID"].ToString());
                ViewBag.PassengerCountsToday = ws.AMSWebService.PassengerCounts_Today(Session["CustomerID"].ToString());
                return Json(new { Success = true,
                    AccountRegistered = ViewBag.AccountRegistered,
                    TodayActiveCommuters=ViewBag.TodayActiveCommuters,
                    RewardsTotal=ViewBag.RewardsTotal,
                    ScannedAdsCount=ViewBag.ScannedAdsCount,
                    PassengerCountsToday=ViewBag.PassengerCountsToday
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex) { return Json(new { Success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet); }


        }

    }
}