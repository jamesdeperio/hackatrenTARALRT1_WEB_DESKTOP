using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NXYSOFT_RMS.Models;
using System.Data;
using System.IO;

namespace NXYSOFT_RMS.Controllers
{
    public class AccountController : Controller
    {
        public string GetCurrentUserWindowsLogin()
        {

            string windowsLogin = User.Identity.Name;
            //Normally if the domain is present you get a string like DOMAINNAME\username, remove the domain
            int hasDomain = windowsLogin.IndexOf(@"\");
            if (hasDomain > 0)
            {
                windowsLogin = windowsLogin.Remove(0, hasDomain + 1);
            }
            else
            {
                windowsLogin = "No Domain";
            }
            //end if 
            return windowsLogin;
        } //end GetCurrentUserWindowsLogin
        public ActionResult Login()
        {
            try
            {               
                Session["ADUsername"] = GetCurrentUserWindowsLogin();

                ViewBag.SystemError = "";
                Session["_intCurrentID"] = Request.Cookies["AMSCookie"]["_intCurrentID"];
           
                if (Session["_intCurrentID"].ToString() == "")
                    Session["_intCurrentID"] = null;

                Session["_intCurrentUserName"] = Request.Cookies["AMSCookie"]["_intCurrentUserName"];
                Session["_strCurrentFullName"] = Request.Cookies["AMSCookie"]["_strCurrentFullName"];
                Session["_strAccessName"] = Request.Cookies["AMSCookie"]["_strAccessName"];
                Session["_strCurrentIDCode"] = Request.Cookies["AMSCookie"]["CurrentIDCode"];

                Session["CustomerID"] = Request.Cookies["AMSCookieCustomerID"]["CustomerID"];
                Session["_strCurrentUserCode"] = Request.Cookies["AMSCookie"]["_strCurrentUserCode"];
                

           
               
                if (Session["_intCurrentID"] != null)
                {
                    Session["ProfilePic"] = GetProfilePicServerPath(Session["_strCurrentUserCode"].ToString());
                    return RedirectToAction("Index", "Home");
                }
            }
            catch { Session["CustomerID"] = "NYXSOFT2018"; }
            UserAccount usr = new UserAccount();
            try
            {
                usr.AccountID = Session["CustomerID"].ToString();
                usr.UserName = "";
                usr.Password = "";
            }
            catch {  }


            return View(usr);
        }

        //public ActionResult CheckStillAlive()
        //{
        //    try
        //    {
        //        Session["ADUsername"] = GetCurrentUserWindowsLogin();

        //        ViewBag.SystemError = "";
        //        Session["_intCurrentID"] = Request.Cookies["AMSCookie"]["_intCurrentID"];

        //        if (Session["_intCurrentID"].ToString() == "")
        //            Session["_intCurrentID"] = null;

        //        Session["_intCurrentUserName"] = Request.Cookies["AMSCookie"]["_intCurrentUserName"];
        //        Session["_strCurrentFullName"] = Request.Cookies["AMSCookie"]["_strCurrentFullName"];
        //        Session["_strAccessName"] = Request.Cookies["AMSCookie"]["_strAccessName"];
        //        Session["_strCurrentIDCode"] = Request.Cookies["AMSCookie"]["CurrentIDCode"];

        //        Session["CustomerID"] = Request.Cookies["AMSCookieCustomerID"]["CustomerID"];
        //        Session["_strCurrentUserCode"] = Request.Cookies["AMSCookie"]["_strCurrentUserCode"];




        //        //if (Session["_intCurrentID"] != null)
        //        //{
        //        //    //Session["ProfilePic"] = GetProfilePicServerPath(Session["_strCurrentUserCode"].ToString());
        //        //    return RedirectToAction("Index", "Home");
        //        //}
        //    }
        //    catch { Session["CustomerID"] = "NYXSOFT2018"; }
        //    UserAccount usr = new UserAccount();
        //    try
        //    {
        //        usr.AccountID = Session["CustomerID"].ToString();
        //        usr.UserName = "";
        //        usr.Password = "";
        //    }
        //    catch { }


        //    return View(usr);
        //}

        [HttpPost]
        public ActionResult Login(UserAccount Data)
        {
            MyWSContext ws = new MyWSContext();
            string _strOutput ="";
            using (DataTable _tblusers = ws.AMSWebService.ValidateUser(Data.UserName, Data.Password, Data.AccountID, out _strOutput))
            {

                if (_tblusers == null)
                {
                    if (_strOutput != "")
                    {
                        ViewBag.SystemError = _strOutput;
                        ModelState.AddModelError("", _strOutput);
                    }
                    else
                    {
                        ViewBag.SystemError = "(Invalid Account ID) Please contact your system administrator";
                        ModelState.AddModelError("", "(Invalid Account ID) Please contact your system administrator");
                    }
                }
                else
                {
                    foreach (DataRow DR in _tblusers.Rows)
                    {
                        //TimeSpan timespan = (DateTime.Parse(DR[1].ToString()).AddMonths(int.Parse(DR[16].ToString())) - DateTime.Now);

                        Response.Cookies["AMSCookie"]["_intCurrentID"] = DR[0].ToString();
                        Response.Cookies["AMSCookie"]["_strCurrentUserCode"] = DR[2].ToString();
                        Response.Cookies["AMSCookie"]["_strCurrentFullName"] = DR[3].ToString().ToUpper();
                        Response.Cookies["AMSCookie"]["_intCurrentUserName"] = DR[14].ToString();//ws.Decrypt(DR[14].ToString(), true);
                        Response.Cookies["AMSCookie"]["_strAccessName"] = DR[13].ToString().ToUpper();
                        Response.Cookies["AMSCookieCustomerID"]["CustomerID"] = Data.AccountID;



                        Response.Cookies["AMSCookie"].Expires = DateTime.Now.AddDays(30);
                        Response.Cookies["AMSCookieCustomerID"].Expires = DateTime.Now.AddDays(30);
                        //Response.Cookies["CustomerID"].Expires = DateTime.Now.AddDays(30);

                        Session["_intCurrentID"] = DR[0].ToString();
                        Session["_strCurrentUserCode"] = DR[2].ToString();
                        Session["_strCurrentFullName"] = DR[3].ToString().ToUpper();
                        Session["_intCurrentUserName"] = DR[14].ToString();//ws.Decrypt(DR[14].ToString(), true);
                        Session["_strAccessName"] = DR[13].ToString();
                        Session["CustomerID"] = Data.AccountID;
                      

                        try
                        {
                            string _strCurrentIDCode = String.Format("{0:MM/dd/yyyy hh:mm:ss}", DateTime.Now);
                            Session["_strCurrentIDCode"] = _strCurrentIDCode;
                            Response.Cookies["AMSCookie"]["CurrentIDCode"] = Session["_strCurrentIDCode"].ToString();
                            bool _isSet = ws.AMSWebService.SetUserStatus(Data.AccountID, _strCurrentIDCode, Session["_intCurrentID"].ToString(), Data.AccountID);

                        }
                        catch { }

                    }
                    
                    if (_strOutput != "")
                    {
                      
                        ViewBag.SystemError = _strOutput;
                        ModelState.AddModelError("", _strOutput);

                        if (_strOutput.Contains("You only have"))
                        {
                            Session["ProfilePic"] = GetProfilePicServerPath(Session["_strCurrentUserCode"].ToString());
                            Session["SystemMessage"] = _strOutput;
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            Session["_intCurrentID"] = null;
                            Session["SystemMessage"] = null;
                        }

                    }
                    else
                    {
                        Session["ProfilePic"] = GetProfilePicServerPath(Session["_strCurrentUserCode"].ToString());
                        return RedirectToAction("Index","Home");
                    }
                }
                return View();
 
            }
        }

        public ActionResult LoggedIn()
        {

           

            if (Session["_intCurrentID"] != null)
            {
                return View();
            }
            else {
                return RedirectToAction("Login");
            }
        }

        public ActionResult LogOut()
        {
            Response.Cookies["AMSCookie"]["_intCurrentID"] = null;
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UserUpdateProfile()
        {
            if (Session["_intCurrentID"] != null)
            {
                UserAccount usr = new UserAccount();
                 MyWSContext ws = new MyWSContext();
                using (DataTable _tblusers = ws.AMSWebService.LoadUserInfo_PerUserID(int.Parse(Session["_intCurrentID"].ToString()), Session["CustomerID"].ToString()))
                {
                    foreach (DataRow DR in _tblusers.Rows)
                    {
                        usr.CurrentID = DR[0].ToString();

                        usr.CurrentUserName = DR[14].ToString(); //ws.Decrypt(DR[17].ToString(), true);
                        usr.Password = DR[15].ToString(); //ws.Decrypt(DR[18].ToString(), true);
                       

                        ViewBag.SystemError = "";
                        ViewBag.SystemSuccess = "";
                    }
                }
                return View(usr);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        public ActionResult UserUpdateProfile(UserAccount Data)
        {
            if (Session["_intCurrentID"] != null)
            {
                    
                    MyWSContext ws = new MyWSContext();

                    try
                    {
                        if (Data.Password == Data.ConfirmPassword && Data.Password != null)
                        {
                            string _strInputValidation = Data.CurrentUserName + "`" + Data.CurrentPassword;

                            string _strInput = Data.CurrentID + "`" + "`" + Data.UserName + "`" + Data.Password;

                            byte[] tempPic = {1,2};
                            string[] _strOutput = ws.AMSWebService.UserChangePassword_SaveRecord(true, _strInputValidation, _strInput, tempPic, Session["CustomerID"].ToString()).Split('`');

                            if (_strOutput[0] == "true")
                            {
                                ViewBag.SystemError = "";
                                ViewBag.SystemSuccess = _strOutput[1];
                                ModelState.AddModelError("", _strOutput[1]);
                                Response.Cookies["AMSCookie"]["_intCurrentID"] = null;
                                Session.Clear();
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                ViewBag.SystemSuccess = "";
                                ViewBag.SystemError = _strOutput[1];
                                ModelState.AddModelError("", _strOutput[1]);
                            }
                        }
                        else
                        {
                            ViewBag.SystemSuccess = "";
                            ViewBag.SystemError = "Please correct these errors:";
                            ModelState.AddModelError("", "Please correct these errors:");
                        }

                    }
                    catch (Exception ex) { ViewBag.SystemError = ex.Message; ModelState.AddModelError("", ex.Message); }
                    return View();
          
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public string GetProfilePicServerPath(string _strFilename)
        {
            try
            {
                string _path = Server.MapPath("~/images/Employee/" + Session["CustomerID"].ToString() + "/");

                string[] fileNames = Directory.GetFiles(_path);
                foreach (string fileName in fileNames)
                {
                    if (fileName.Contains(_strFilename))
                        return "../images/Employee/" + Session["CustomerID"].ToString() + "/" + System.IO.Path.GetFileName(fileName);

                }
            }
            catch { }
            return "../images/PicLogo.png";
        }



    }
}