using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace NXYSOFT_RMS
{
    /// <summary>
    /// Summary description for NyxSoft_AMS_Services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class NyxSoft_AMS_Services : System.Web.Services.WebService
    {

        #region SetupModules

        [WebMethod]
        public DataTable LoadSetupModules(string _strSetupName, string _strAccessName, string _strLicenseKey,out string _strOutput)
        {
            try
            {
     
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM (SELECT DISTINCT  dbo.tblUserAccess.fldAccessName,dbo.tblSetupModules.fldSetupName,dbo.tblSetupModules.fldModuleName,dbo.tblSetupModules.fldButtons as buttons, dbo.tblUserAccess.fldButtons,fldArrangement FROM         dbo.tblSetupModules INNER JOIN dbo.tblUserAccess ON REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(dbo.tblSetupModules.fldModuleName, '/', ''), '(', ''), ')', ''), ' ', ''), 'ItemProfile-', ''), 'InventoryManagement-', ''), 'TrackingManagement-', ''), 'TransportManagement-', ''), 'Reports-', ''), 'Settings-', '') = dbo.tblUserAccess.fldModuleName LEFT OUTER JOIN dbo.tblEmployees  ON dbo.tblUserAccess.fldAccessName = dbo.tblEmployees .fldUserAccesslID) as TblUserMo WHERE fldAccessName=@fldAccessName and fldSetupName=@fldSetupName order by fldArrangement", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldSetupName", _strSetupName);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccessName", _strAccessName);

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    if (dt.Rows.Count == 0)
                    {
                        using (SQLConnect.SqlCommandEx _SQLCMD2 = new SQLConnect.SqlCommandEx("SELECT * FROM tblSetupModules WHERE fldSetupName=@fldSetupName order by fldArrangement", _strLicenseKey))
                        {
                            _SQLCMD2.CommandObject.Parameters.AddWithValue("@fldSetupName", _strSetupName);
                            DataTable dt2 = new DataTable();
                            dt2 = _SQLCMD2.GetDataTable();
                            dt2.TableName = "Data";
                            _strOutput = "";
                            return dt2;

                        }
                    }

                    

                    _strOutput = "";
                   return dt;

                }

            }
            catch (Exception ex)
            {
                _strOutput = ex.Message;
                return null;
            }

        }
        #endregion

        #region Userinfo
        [WebMethod]
        public int LoadDaysLeft_Where(string _strLicenseKey)
        {

            try
            {

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT  DATEDIFF(day, GETDATE(), DATEADD(month, fldMonthsAllowed, fldDateRegisterred)) as DaysLeft FROM tblLicenseDetails", _strLicenseKey))
                {

                    foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                    {
                        return int.Parse(DR[0].ToString());

                    }
                    return 0;

                }
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        [WebMethod]
        public DataTable ValidateUser(string Username, string Password, string _strLicenseKey, out string _strOutput)
        {
            try
            {
                
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblEmployees WHERE fldUsername=@fldUsername AND fldPassword=@fldPassword", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUsername", Username); //CryptorEngine.Encrypt(Username, true)
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPassword", Password); //CryptorEngine.Encrypt(Password, true)

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";
                    _strOutput = "";
                    if (dt.Rows.Count >= 1)
                    {
                        foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                        {
                            if (DR[11].ToString() == "False")
                            {
                                _strOutput = "Account is deactivated, please contact your system administrator regarding this matter.";
                            }
                            else {

                                using (SQLConnect.SqlCommandEx _SQLCMD2 = new SQLConnect.SqlCommandEx("SELECT  * FROM tblLicenseDetails", _strLicenseKey))
                                {

                                    foreach (DataRow DR2 in _SQLCMD2.GetDataTable().Rows)
                                    {
                                        TimeSpan timespan = (DateTime.Parse(DR2[1].ToString()).AddMonths(int.Parse(DR2[2].ToString())) - DateTime.Now);
                                        if (timespan.Days <= 0)
                                        {
                                            _strOutput = "Your account is already expired, please contact your system administrator regarding this matter.";
                                        }
                                        else
                                        {
                                            if (timespan.TotalDays < 60)
                                            {
                                                _strOutput = "You only have " + timespan.Days.ToString() + " Day(s) to expire your account, please contact your system administrator regarding this matter.";
                                            }
                                        }
                                    }
                                    
                                }

                                
                            }


                        }                       
                        return dt;

                    }
                    else
                    {
                    _strOutput = "Invalid username or password";
                    return dt;
                }


            }
               
            }
            catch
            {
                _strOutput = "";
                return null;
            }
           
        }




 
        [WebMethod]
        public DataTable LoadUserInfo_PerUserID(int _intID, string _strLicenseKey)
        {

            try
            {
               
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblEmployees WHERE  fldID=@fidID or @fidID=-1", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fidID", _intID);

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt;

                }
            }
            catch
            {
                return null;
            }

        }

      
        public bool IsExistCurrentPassword(string _struserName, string _strCurrentPassowrd, string _strLicenseKey)
        {

            try
            {
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblEmployees WHERE  fldusername=@fldusername AND fldPassword=@fldPassword", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldusername", _struserName);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPassword", _strCurrentPassowrd);

                    if (_SQLCMD.GetDataTable().Rows.Count >= 1)
                        return true;
                    else
                        return false;



                }
            }
            catch
            {
                return true;
            }

        }

        public bool IsExistUser(string _strUserName, out int _intID, string _strLicenseKey)
        {

            try
            {
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblEmployees WHERE  fldUsername=@fldUsername", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUsername", _strUserName);


                    foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                    {
                        _intID = int.Parse(DR[0].ToString());
                        return true;
                    }
                    _intID = 0;
                    return false;



                }
            }
            catch
            {
                _intID = 0;
                return false;
            }

        }

        public bool IsExistFullName(string _strFullName, out int _intID, string _strLicenseKey)
        {

            try
            {
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblEmployees WHERE  fldname=@fldname", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldname", _strFullName);

                    foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                    {
                        _intID = int.Parse(DR[0].ToString());
                        return true;
                    }
                    _intID = 0;
                    return false;

                }
            }
            catch
            {
                _intID = 0;
                return false;
            }

        }


        public bool IsNumeric(Object Expression)
        {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
                return true;

            try
            {
                if (Expression is string)
                    Double.Parse(Expression as string);
                else
                    Double.Parse(Expression.ToString());
                return true;
            }
            catch { } // just dismiss errors but return false
            return false;

        }

        [WebMethod]
        public string UserInformation_SaveRecord(bool _IsEdit, string _strInputValidation, string _strInputs, byte[] _byteInputs, string _strLicenseKey)
        {

            try
            {
               
                string[] _strIV = _strInputValidation.Split('`');
                string[] _strInput = _strInputs.Split('`');


                if (_strInput[2] == "")
                {
                    return "false`Please enter your full name";
                }
                if (_strInput[3] == "")
                {
                    return "false`Please enter your username";
                }
                if (_strInput[4] == "")
                {
                    return "false`Please enter your password";
                }
               

                int _intTempID = 0;
                if (IsExistUser(CryptorEngine.Encrypt(_strInput[3], true), out _intTempID, _strLicenseKey))
                {
                    if (_intTempID.ToString() != _strInput[0] || (_strInput[0] == "0" && _intTempID.ToString() != "0"))
                        return "false`Username is already exist";
                }

             


                if (_IsEdit)
                {
                    if (IsExistCurrentPassword(CryptorEngine.Encrypt(_strIV[0], true), CryptorEngine.Encrypt(_strIV[1], true), _strLicenseKey) == false)
                    {
                        return "false`Invalid password, please enter your current password";
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("UPDATE tblEmployees SET fldName=@fldName,fldUserName=@fldUserName,fldPassword=@fldPassword,fldCompanyName=@fldCompanyName,fldAddress=@fldAddress,fldContactPerson=@fldContactPerson,fldContactNo=@fldContactNo,fldEmailAddress=@fldEmailAddress,fldSetupKey=@fldSetupKey,fldModulesAccess=@fldModulesAccess,fldUserPicture=@fldUserPicture,fldDelearName=@fldDelearName,fldDelearContactNo=@fldDelearContactNo,fldDelearEmailAddress=@fldDelearEmailAddress    WHERE fldID=@fldID", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                        //_SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateEntry", _strInput[1]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", _strInput[2]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUserName", CryptorEngine.Encrypt(_strInput[3], true));
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPassword", CryptorEngine.Encrypt(_strInput[4], true));
                     

                        _SQLCMD.CommandObject.ExecuteNonQuery();

                    }

                }
                else
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tblEmployees(fldDateEntry,fldName,fldUserName,fldPassword,fldCompanyName,fldAddress,fldContactPerson,fldContactNo,fldEmailAddress,fldSetupKey,fldModulesAccess,fldUserPicture,fldDelearName,fldDelearContactNo,fldDelearEmailAddress,fldValidMonths) VALUES(@fldDateEntry,@fldName,@fldUserName,@fldPassword,@fldCompanyName,@fldAddress,@fldContactPerson,@fldContactNo,@fldEmailAddress,@fldSetupKey,@fldModulesAccess,@fldUserPicture,@fldDelearName,@fldDelearContactNo,@fldDelearEmailAddress,@fldValidMonths)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateEntry", _strInput[1]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", _strInput[2]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUserName", CryptorEngine.Encrypt(_strInput[3], true));
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPassword", CryptorEngine.Encrypt(_strInput[4], true));
                       
                        _SQLCMD.CommandObject.ExecuteNonQuery();

                    }

                }
                return "true`Record Saved!";

            }
            catch (Exception ex)
            {
                return "false`Invalid License Key, Please contact your system administrator" + ex.Message;
            }

        }



        [WebMethod]
        public string UserChangePassword_SaveRecord(bool _IsEdit, string _strInputValidation, string _strInputs, byte[] _byteInputs, string _strLicenseKey)
        {

            try
            {
               
                string[] _strIV = _strInputValidation.Split('`');
                string[] _strInput = _strInputs.Split('`');


               
                if (_strInput[2] == "")
                {
                    return "false`Please enter your username";
                }
                if (_strInput[3] == "")
                {
                    return "false`Please enter your password";
                }


                int _intTempID = 0;
                if (IsExistUser(_strInput[2], out _intTempID, _strLicenseKey)) //CryptorEngine.Encrypt(_strInput[2], true)
                {
                    if (_intTempID.ToString() != _strInput[0] || (_strInput[0] == "0" && _intTempID.ToString() != "0"))
                        return "false`Username is already exist";
                }




                if (_IsEdit)
                {
                    if (IsExistCurrentPassword(_strIV[0], _strIV[1], _strLicenseKey) == false) //CryptorEngine.Encrypt(_strIV[0], true), CryptorEngine.Encrypt(_strIV[1], true)
                    {
                        return "false`Invalid password, please enter your current password";
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("UPDATE tblEmployees SET fldUserName=@fldUserName,fldPassword=@fldPassword WHERE fldID=@fldID", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);                        
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUserName", _strInput[2]); //CryptorEngine.Encrypt(_strInput[2], true)
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPassword", _strInput[3]); //CryptorEngine.Encrypt(_strInput[3], true)


                        _SQLCMD.CommandObject.ExecuteNonQuery();

                    }

                }
               
                return "true`Record Saved!";

            }
            catch (Exception ex)
            {
                return "false`Invalid License Key, Please contact your system administrator" + ex.Message;
            }

        }

        [WebMethod]
        public DataTable GetUserStatus(string _strUserID, string _strLicenseKey)
        {

            try
            {                
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT  tblUserStatus.fldCustomerID, CASE WHEN tblEmployees.fldIsActive = 0 then '' else tblUserStatus.fldStatus end as fldStatus, tblUserStatus.fldUserID, tblEmployees.fldIsActive FROM tblEmployees INNER JOIN tblUserStatus ON tblEmployees.fldID = tblUserStatus.fldUserID WHERE fldUserID=@fldUserID", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUserID", _strUserID);

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt;

                }
            }
            catch
            {
                return null;
            }

        }

        [WebMethod]
        public bool SetUserStatus(string _strCustomerID, string _strStatus, string _strUserID, string _strLicenseKey)
        {

            try
            {
              
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx(" DELETE FROM tbluserstatus WHERE fldUserID=@fldUserID", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUserID", _strUserID);
                    _SQLCMD.CommandObject.ExecuteNonQuery();

                }

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tbluserstatus(fldCustomerID,fldStatus,fldUserID) VALUES(@fldCustomerID,@fldStatus,@fldUserID)", _strLicenseKey))
                {

                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCustomerID", _strCustomerID);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldStatus", _strStatus);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUserID", _strUserID);
                    _SQLCMD.CommandObject.ExecuteNonQuery();

                }
                return true;
            }
            catch
            {
                return false;
            }

        }


        #endregion

        #region Commonfunctions

        bool _AssetCatalog_InUsed(string _strCode, string _strLicenseKey)
        {
            using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblAssetCatalog WHERE fldCategoryCode=@fldCode or fldClassificationCode=@fldCode or fldTypeCode=@fldCode or fldPurchasingUOMCode=@fldCode or fldPackageUOMCode=@fldCode or fldBrandCode=@fldCode or fldModelCode=@fldCode", _strLicenseKey))
            {
                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _strCode);
                if (_SQLCMD.GetDataTable().Rows.Count >= 1)
                    return true;
                else
                    return false;
            }

        }

        bool _AssetReceived_InUsed(string _strCode, string _strLicenseKey)
        {
            using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblReceivedHeader WHERE fldSupplierCode=@fldCode or fldWarehouseCode=@fldCode or fldTransactionByCode=@fldCode or fldReceivedByCode=@fldCode or fldAuthorizedByCode=@fldCode", _strLicenseKey))
            {
                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _strCode);
                if (_SQLCMD.GetDataTable().Rows.Count >= 1)
                    return true;
                else
                    return false;
            }

        }
        bool _AssetReceivedDetails_InUsed(string _strCode, string _strLicenseKey)
        {
            using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblReceivedDetails WHERE fldAssetCode=@fldCode", _strLicenseKey))
            {
                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _strCode);
                if (_SQLCMD.GetDataTable().Rows.Count >= 1)
                    return true;
                else
                    return false;
            }

        }
        bool _AssetStorage_InUsed(string _strCode, string _strLicenseKey)
        {
            using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblAssignedStorage WHERE fldSectionCode=@fldCode or fldShelfCode=@fldCode or fldRackCode=@fldCode", _strLicenseKey))
            {
                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _strCode);
                if (_SQLCMD.GetDataTable().Rows.Count >= 1)
                    return true;
                else
                    return false;
            }

        }

        bool _TagTransaction_InUsed(string _strUID, string _strLicenseKey)
        {
            using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblTagTransaction WHERE fldUID=@fldUID", _strLicenseKey))
            {
                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldUID", _strUID);
                if (_SQLCMD.GetDataTable().Rows.Count >= 1)
                    return true;
                else
                    return false;
            }

        }
        
        [WebMethod]
        public string DeleteRecord_RecordID(string _intID, string _strTableName, string _strLicenseKey)
        {

            try
            {

                if (_strTableName.ToUpper() == "COMPANIES/BRANCHES")
                {
                    _strTableName = "tblCompanyInfo";
                }
                if (_strTableName.ToUpper() == "DEPARTMENTS")
                    _strTableName = "tblDepartment";
                if (_strTableName.ToUpper() == "POSITIONS")
                    _strTableName = "tblposition";
                if (_strTableName.ToUpper() == "REASONOFOUTGOINGS")
                    _strTableName = "tblReasonofoutgoing";
                if (_strTableName.ToUpper() == "COSTCENTERS")
                    _strTableName = "tblCostcenter";
                if (_strTableName.ToUpper() == "LOCATIONS")
                    _strTableName = "tblLocation";
                if (_strTableName.ToUpper() == "WAREHOUSES")
                {
                    _strTableName = "tblWarehouse";
                    if (_AssetReceived_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "RACKS")
                {
                    _strTableName = "tblRack";
                    if (_AssetStorage_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "SECTIONS")
                {
                    _strTableName = "tblSection";
                    if (_AssetStorage_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "SHELFS")
                {
                    _strTableName = "tblShelf";
                    if (_AssetStorage_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }

                if (_strTableName.ToUpper() == "BRANDS")
                {
                    _strTableName = "tblBrand";
                    if (_AssetCatalog_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "TYPES")
                {
                    _strTableName = "tblType";
                    if (_AssetCatalog_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "MODELS")
                {
                    _strTableName = "tblModel";
                    if (_AssetCatalog_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "CONDITIONS")
                    _strTableName = "tblCondition";
                if (_strTableName.ToUpper() == "UOMS")
                {
                    _strTableName = "tblUOM";
                    if (_AssetReceived_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "CATEGORYS")
                {
                    _strTableName = "tblCategory";
                    if (_AssetCatalog_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "CLASSIFICATIONS")
                {
                    _strTableName = "tblClassification";
                    if (_AssetCatalog_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "EMPLOYEES")
                {
                    _strTableName = "tblEmployees";
                    if (_AssetReceived_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }

                if (_strTableName.ToUpper() == "SUPPLIERS")
                {
                    _strTableName = "tblSupplier";
                    if (_AssetReceived_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }

                if (_strTableName.ToUpper() == "RECEIVEDITEMS")
                    _strTableName = "tblReceivedDetails";

                if (_strTableName.ToUpper() == "ATTACHMENTS")
                    _strTableName = "tblAttachments";


                if (_strTableName.ToUpper() == "ITEMCATALOGS")
                {
                    if (_AssetReceivedDetails_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                    
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAssetSubPart WHERE  fldAssetCode=@fldAssetCode", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAssetCode", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAssetSupplier WHERE  fldAssetCode=@fldAssetCode", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAssetCode", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAssetSubPart WHERE NOT EXISTS (SELECT * FROM tblAssetCatalog WHERE tblAssetCatalog.fldCode = tblAssetSubPart.fldAssetCode)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAssetSupplier WHERE NOT EXISTS (SELECT * FROM tblAssetCatalog WHERE tblAssetCatalog.fldCode = tblAssetSupplier.fldAssetCode)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    _strTableName = "tblAssetCatalog";
                }
                if (_strTableName.ToUpper() == "ITEMSUBPART")
                    _strTableName = "tblAssetSubPart";
                if (_strTableName.ToUpper() == "ITEMSUPPLIER")
                {
                    _strTableName = "tblAssetSupplier";
                    if (_AssetReceived_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }
                if (_strTableName.ToUpper() == "ITEMRECEIVEDS")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblReceivedDetails WHERE  fldReceivedNo=@fldReceivedNo", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldReceivedNo", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblReceivedDetails WHERE NOT EXISTS (SELECT * FROM tblReceivedHeader WHERE tblReceivedHeader.fldCode = tblReceivedDetails.fldReceivedNo)", _strLicenseKey))
                    {                        
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAttachments WHERE  fldCode=@fldCode", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    //using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAttachments WHERE NOT EXISTS (SELECT * FROM tblReceivedHeader WHERE tblReceivedHeader.fldCode = tblAttachments.fldCode)", _strLicenseKey))
                    //{
                    //    _SQLCMD.CommandObject.ExecuteNonQuery();
                    //}

                    _strTableName = "tblReceivedHeader";

                }

                if (_strTableName.ToUpper() == "ITEMSerialization".ToUpper())
                {
                    _strTableName = "tblAssetTag";
                    if (_TagTransaction_InUsed(_intID.Split('`')[1], _strLicenseKey))
                    {
                        return "false`Record is already in used.";
                    }
                }

                if (_strTableName.ToUpper() == "ITEMWAREHOUSETRANSFER")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblWarehouseTransferDetails WHERE  fldTransferNo=@fldTransferNo", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldTransferNo", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblWarehouseTransferDetails WHERE NOT EXISTS (SELECT * FROM tblWarehouseTransferHeader WHERE tblWarehouseTransferHeader.fldCode = tblWarehouseTransferDetails.fldTransferNo)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAttachments WHERE  fldCode=@fldCode", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                
                    _strTableName = "tblWarehouseTransferHeader";
                }
                if (_strTableName.ToUpper() == "TRANSFERREDITEMS")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblTagTransaction WHERE EXISTS (SELECT * FROM (SELECT tblTagTransaction.fldTransCode, tblAssetTag.fldAssetCode, tblWarehouseTransferDetails.fldID,tblAssetTag.fldUID FROM tblTagTransaction INNER JOIN tblAssetTag ON tblTagTransaction.fldUID = tblAssetTag.fldUID INNER JOIN tblWarehouseTransferDetails ON tblTagTransaction.fldTransCode = tblWarehouseTransferDetails.fldTransferNo AND tblAssetTag.fldAssetCode = tblWarehouseTransferDetails.fldAssetCode AND  tblAssetTag.fldUOMCode = tblWarehouseTransferDetails.fldUOMCode AND tblAssetTag.fldBatchReceived = tblWarehouseTransferDetails.fldBatchReceived WHERE tblWarehouseTransferDetails.fldID=@fldID) as ddd WHERE ddd.fldTransCode = tblTagTransaction.fldTransCode and ddd.fldUID = tblTagTransaction.fldUID)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _intID.Split('`')[0]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    _strTableName = "tblWarehouseTransferDetails";
                }

                if (_strTableName.ToUpper() == "ITEMOUTGOING")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblOutgoingDetails WHERE  fldOutgoingNo=@fldOutgoingNo", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldOutgoingNo", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblOutgoingDetails WHERE NOT EXISTS (SELECT * FROM tblOutgoingHeader WHERE tblOutgoingHeader.fldCode = tblOutgoingDetails.fldOutgoingNo)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAttachments WHERE  fldCode=@fldCode", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }


                    _strTableName = "tblOutgoingHeader";
                }
                if (_strTableName.ToUpper() == "OUTGOINGITEMS")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblTagTransaction WHERE EXISTS (SELECT * FROM (SELECT tblTagTransaction.fldTransCode, tblAssetTag.fldAssetCode, tblOutgoingDetails.fldID,tblAssetTag.fldUID FROM tblTagTransaction INNER JOIN tblAssetTag ON tblTagTransaction.fldUID = tblAssetTag.fldUID INNER JOIN tblOutgoingDetails ON tblTagTransaction.fldTransCode = tblOutgoingDetails.fldOutgoingNo AND tblAssetTag.fldAssetCode = tblOutgoingDetails.fldAssetCode AND  tblAssetTag.fldUOMCode = tblOutgoingDetails.fldUOMCode AND tblAssetTag.fldBatchReceived = tblOutgoingDetails.fldBatchReceived WHERE tblOutgoingDetails.fldID=@fldID) as ddd WHERE ddd.fldTransCode = tblTagTransaction.fldTransCode and ddd.fldUID = tblTagTransaction.fldUID)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _intID.Split('`')[0]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    _strTableName = "tblOutgoingDetails";
                }


                if (_strTableName.ToUpper() == "ITEMINCOMING")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblIncomingDetails WHERE  fldIncomingNo=@fldIncomingNo", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldIncomingNo", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblIncomingDetails WHERE NOT EXISTS (SELECT * FROM tblIncomingHeader WHERE tblIncomingHeader.fldCode = tblIncomingDetails.fldIncomingNo)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAttachments WHERE  fldCode=@fldCode", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }


                    _strTableName = "tblIncomingHeader";
                }
                if (_strTableName.ToUpper() == "INCOMINGITEMS")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblTagTransaction WHERE EXISTS (SELECT * FROM (SELECT tblTagTransaction.fldTransCode, tblAssetTag.fldAssetCode, tblIncomingDetails.fldID,tblAssetTag.fldUID FROM tblTagTransaction INNER JOIN tblAssetTag ON tblTagTransaction.fldUID = tblAssetTag.fldUID INNER JOIN tblIncomingDetails ON tblTagTransaction.fldTransCode = tblIncomingDetails.fldIncomingNo AND tblAssetTag.fldAssetCode = tblIncomingDetails.fldAssetCode AND  tblAssetTag.fldUOMCode = tblIncomingDetails.fldUOMCode AND tblAssetTag.fldBatchReceived = tblIncomingDetails.fldBatchReceived WHERE tblIncomingDetails.fldID=@fldID) as ddd WHERE ddd.fldTransCode = tblTagTransaction.fldTransCode and ddd.fldUID = tblTagTransaction.fldUID)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _intID.Split('`')[0]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    _strTableName = "tblIncomingDetails";
                }


                if (_strTableName.ToUpper() == "ITEMINVCYCLECOUNT")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblInvCycleCountDetails WHERE  fldInvCycleCountNo=@fldInvCycleCountNo", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldInvCycleCountNo", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblInvCycleCountDetails WHERE NOT EXISTS (SELECT * FROM tblInvCycleCountHeader WHERE tblInvCycleCountHeader.fldCode = tblInvCycleCountDetails.fldInvCycleCountNo)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblAttachments WHERE  fldCode=@fldCode", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _intID.Split('`')[1]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }


                    _strTableName = "tblInvCycleCountHeader";
                }
                if (_strTableName.ToUpper() == "INVCYCLECOUNTITEMS")
                {
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM tblTagTransaction WHERE EXISTS (SELECT * FROM (SELECT tblTagTransaction.fldTransCode, tblAssetTag.fldAssetCode, tblInvCycleCountDetails.fldID,tblAssetTag.fldUID FROM tblTagTransaction INNER JOIN tblAssetTag ON tblTagTransaction.fldUID = tblAssetTag.fldUID INNER JOIN tblInvCycleCountDetails ON tblTagTransaction.fldTransCode = tblInvCycleCountDetails.fldInvCycleCountNo AND tblAssetTag.fldAssetCode = tblInvCycleCountDetails.fldAssetCode AND  tblAssetTag.fldUOMCode = tblInvCycleCountDetails.fldUOMCode AND tblAssetTag.fldBatchReceived = tblInvCycleCountDetails.fldBatchReceived WHERE tblInvCycleCountDetails.fldID=@fldID) as ddd WHERE ddd.fldTransCode = tblTagTransaction.fldTransCode and ddd.fldUID = tblTagTransaction.fldUID)", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _intID.Split('`')[0]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();
                    }
                    _strTableName = "tblInvCycleCountDetails";
                }

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE FROM " + _strTableName + " WHERE  fldid=@fldid", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldid", _intID.Split('`')[0]);
                    _SQLCMD.CommandObject.ExecuteNonQuery();

                    return "true`Record has been deleted";


                }
            }
            catch (Exception ex)
            {
                return "false`" + ex.Message;
            }

        }

        [WebMethod]
        public DataTable LoadEmployeesTransSelection_Where(string _strModuleName, string _strLicenseKey)
        {

            try
            {
           

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM View_EmployeeTransSelection WHERE  (fldModuleName=@fldModuleName OR @fldModuleName='') ", _strLicenseKey))
                {                    
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldModuleName", _strModuleName);


                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt;

                }
            }
            catch
            {
                return null;
            }

        }


        #endregion

        #region SettingUserAccess

        [WebMethod]
        public DataTable LoadSettingUserAccess_Where(string _strInputWhereStatement, string _strLicenseKey)
        {

            try
            {
                string[] _strInput = _strInputWhereStatement.Split('`');

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT distinct fldAccessName FROM tblUserAccess WHERE  (fldID=@fldID OR @fldID='') AND (fldAccessName like @fldAccessName OR @fldAccessName='%%') ", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccessName", "%" + _strInput[1] + "%");


                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt;

                }
            }
            catch
            {
                return null;
            }

        }

        [WebMethod]
        public string DELETESettingUserAccess_WHERE(string _strAccessname, string _strLicenseKey)
        {

            try
            {               
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE From tblUserAccess Where  fldAccessName=@fldAccessName", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccessName", _strAccessname);
                    _SQLCMD.CommandObject.ExecuteNonQuery();

                }
                return "true`Successfully Deleted";
            }
            catch(Exception ex)
            {
                return "false`"+ex.Message;
            }

        }
        [WebMethod]
        public string SettingUserAccess_SaveRecord(bool _IsEdit, string _strInputValidation, string _strInputs, string _strLicenseKey)
        {

            try
            {
                string[] _strIV = _strInputValidation.Split('`');
                string[] _strInput = _strInputs.Split('`');

               
                if (_strInput[1] == "")
                {
                    return "false`Please input Access Name";
                }

                if (!_IsEdit)
                {
                    int _intTempID = 0;
                    if (IsExistSettingUserAccessName(_strInput[1], out _intTempID, _strLicenseKey))
                    {
                        if (_intTempID.ToString() != _strInput[0] || (_strInput[0] == "0" && _intTempID.ToString() != "0"))
                            return "false`Access Name is already exist";
                    }
                }

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("DELETE From tblUserAccess Where  fldAccessName=@fldAccessName", _strLicenseKey))
                {                   
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccessName", _strInput[1]);                    
                    _SQLCMD.CommandObject.ExecuteNonQuery();

                }

                string[] _strModulesButtons = _strInput[2].Split('+');
                for (int x=0; x < _strModulesButtons.Length; x++)
                {
                    string[] _strModules = _strModulesButtons[x].Split('=');
                    if (_strModules.Length > 1)
                    {
                        using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tblUserAccess(fldAccessName,fldModuleName,fldButtons) VALUES(@fldAccessName,@fldModuleName,@fldButtons)", _strLicenseKey))
                        {
                            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccessName", _strInput[1]);
                            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldModuleName", _strModules[0]);
                            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldButtons", _strModules[1]);
                            _SQLCMD.CommandObject.ExecuteNonQuery();

                        }
                    }

                }


                if (!_IsEdit)
                {
                    return "true`Record Saved!";
                }
                else
                { return "true`Record Updated!"; }

            }
            catch (Exception ex)
            {
                return "false`" + ex.Message;
            }

        }

        public bool IsExistSettingUserAccessName(string _strName, out int _intID, string _strLicenseKey)
        {

            try
            {
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblUserAccess WHERE fldAccessName=@fldAccessName", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccessName", _strName);

                    foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                    {
                        _intID = int.Parse(DR[0].ToString());
                        return true;
                    }
                    _intID = 0;
                    return false;

                }
            }
            catch
            {
                _intID = 0;
                return false;
            }

        }


        #endregion








        //#region SettingType 

        //[WebMethod]
        //public DataTable LoadSettingType_Where(string _strInputWhereStatement, string _strLicenseKey)
        //{

        //    try
        //    {
        //        string[] _strInput = _strInputWhereStatement.Split('`');

        //        using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblType WHERE (fldIsActive=@fldIsActive or @fldIsActive='') and   (fldID=@fldID OR @fldID='') AND (fldDateEncoded BETWEEN @fldDateFrom AND @fldDateTo OR @fldDateFrom='') AND (fldName like @fldName OR @fldName='%%') order by fldName", _strLicenseKey))
        //        {
        //            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
        //            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateFrom", _strInput[1]);
        //            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateTo", _strInput[2]);
        //            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", "%" + _strInput[3] + "%");
        //            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldIsActive", _strInput[4]);

        //            DataTable dt = new DataTable();
        //            dt = _SQLCMD.GetDataTable();
        //            dt.TableName = "Data";

        //            return dt;

        //        }
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //}


        //[WebMethod]
        //public string SettingType_SaveRecord(bool _IsEdit, string _strInputValidation, string _strInputs, string _strLicenseKey)
        //{

        //    try
        //    {
        //        string[] _strIV = _strInputValidation.Split('`');
        //        string[] _strInput = _strInputs.Split('`');

        //        if (_strInput[3] == "")
        //        {
        //            return "false`Please input Type name";
        //        }


        //        int _intTempID = 0;
        //        if (IsExistSettingTypeName(_strInput[3], out _intTempID, _strLicenseKey))
        //        {
        //            if (_intTempID.ToString() != _strInput[0] || (_strInput[0] == "0" && _intTempID.ToString() != "0"))
        //                return "false`Type name is already exist";
        //        }

        //        if (_IsEdit)
        //        {

        //            using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("UPDATE tblType  SET fldDateEncoded=@fldDateEncoded,fldCode=@fldCode,fldName=@fldName,fldRemarks=@fldRemarks,fldIsActive=@fldIsActive WHERE fldID=@fldID", _strLicenseKey))
        //            {
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateEncoded", _strInput[1]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _strInput[2]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", _strInput[3]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRemarks", _strInput[4]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldIsActive", _strInput[5]);
        //                _SQLCMD.CommandObject.ExecuteNonQuery();

        //            }

        //        }
        //        else
        //        {

        //            using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tblType(fldDateEncoded,fldCode,fldName,fldRemarks,fldIsActive) VALUES(@fldDateEncoded,@fldCode,@fldName,@fldRemarks,@fldIsActive)", _strLicenseKey))
        //            {

        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateEncoded", _strInput[1]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _strInput[2]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", _strInput[3]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRemarks", _strInput[4]);
        //                _SQLCMD.CommandObject.Parameters.AddWithValue("@fldIsActive", _strInput[5]);
        //                _SQLCMD.CommandObject.ExecuteNonQuery();

        //            }

        //        }
        //        return "true`Record Saved!";

        //    }
        //    catch (Exception ex)
        //    {
        //        return "false`" + ex.Message;
        //    }

        //}

        //public bool IsExistSettingTypeCode(string _strCode, out int _intID, string _strLicenseKey)
        //{

        //    try
        //    {
        //        using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblType WHERE fldCode=@fldCode", _strLicenseKey))
        //        {
        //            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCode", _strCode);

        //            foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
        //            {
        //                _intID = int.Parse(DR[0].ToString());
        //                return true;
        //            }
        //            _intID = 0;
        //            return false;

        //        }
        //    }
        //    catch
        //    {
        //        _intID = 0;
        //        return false;
        //    }

        //}


        //public bool IsExistSettingTypeName(string _strName, out int _intID, string _strLicenseKey)
        //{

        //    try
        //    {
        //        using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblType WHERE fldName=@fldName", _strLicenseKey))
        //        {
        //            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", _strName);

        //            foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
        //            {
        //                _intID = int.Parse(DR[0].ToString());
        //                return true;
        //            }
        //            _intID = 0;
        //            return false;

        //        }
        //    }
        //    catch
        //    {
        //        _intID = 0;
        //        return false;
        //    }

        //}


        //#endregion



        #region CommuterAccounts
        [WebMethod]
        public long CountCommuterAccounts_Where(string _strLicenseKey)
        {

            try
            {
               

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblCommuterAccounts", _strLicenseKey))
                {
                  
                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt.Rows.Count;

                }
            }
            catch
            {
                return 0;
            }

        }
        [WebMethod]
        public long CountCommuterAccounts_Today(string _strLicenseKey)
        {

            try
            {


                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldid FROM tblCommuterAccounts WHERE CONVERT(date, fldLastseen) = CONVERT(date, GETDATE()) ", _strLicenseKey))
                {

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt.Rows.Count;

                }
            }
            catch
            {
                return 0;
            }

        }

        [WebMethod]
        public long PassengerCounts_Today(string _strLicenseKey)
        {

            try
            {


                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldRFIDQRCode FROM tblTravelHistory WHERE CONVERT(date, fldDateTime) = CONVERT(date, GETDATE()) ", _strLicenseKey))
                {

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt.Rows.Count;

                }
            }
            catch
            {
                return 0;
            }

        }


        [WebMethod]
        public DataTable LoadCommuterAccounts_Where(string _strInputWhereStatement, string _strLicenseKey)
        {

            try
            {
                string[] _strInput = _strInputWhereStatement.Split('`');

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblCommuterAccounts WHERE (fldID=@fldID OR @fldID='') AND (fldDateRegistered BETWEEN @fldDateFrom AND @fldDateTo OR @fldDateFrom='') AND (fldName like @fldName OR @fldName='%%') and (fldRFIDQRCode=@fldRFIDQRCode or @fldRFIDQRCode='') order by fldName", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateFrom", _strInput[1]);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateTo", _strInput[2]);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", "%" + _strInput[3] + "%");
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDQRCode", _strInput[4]);

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt;

                }
            }
            catch
            {
                return null;
            }

        }


      
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void LoadCommuterAccounts_Where_JSON(string _strInputWhereStatement, string _strLicenseKey)
        {           
            try
            {
                string[] _strInput = _strInputWhereStatement.Split('`');

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblCommuterAccounts WHERE fldNFC=@fldNFC and fldpassword=@fldpassword", _strLicenseKey))
                {
                  
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldNFC", _strInput[0]);
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldpassword", _strInput[1]);

                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row = null;

                    DataTable dt = _SQLCMD.GetDataTable();
                    foreach (DataRow rs in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, rs[col]);
                        }
                        rows.Add(row);
                    }

                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                    this.Context.Response.Write(serializer.Serialize(new { CycleCountTrans = rows }));




                }
            }
            catch (Exception ex)
            {               
            }

           


          
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void LoadCheckCommuterEmail_Where_JSON(string _strInputWhereStatement, string _strLicenseKey)
        {
            try
            {
                string[] _strInput = _strInputWhereStatement.Split('`');

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblCommuterAccounts WHERE fldEmailAddress=@fldEmailAddress", _strLicenseKey))
                {

                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldEmailAddress", _strInput[0]);
                    
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row = null;

                    DataTable dt = _SQLCMD.GetDataTable();
                    foreach (DataRow rs in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, rs[col]);
                        }
                        rows.Add(row);
                    }

                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                    this.Context.Response.Write(serializer.Serialize(new { CycleCountTrans = rows }));




                }
            }
            catch (Exception ex)
            {
            }





        }

        [WebMethod]
        public string CommuterAccounts_SaveRecord(bool _IsEdit, string _strInputs, string _strLicenseKey)
        {

            try
            {
              
                string[] _strInput = _strInputs.Split('`');

                if (_strInput[3] == "")
                {
                    return "false`Please input registration name";
                }


                int _intTempID = 0;
                if (IsExistCommuterAccountsRFIDQRCODE(_strInput[9], out _intTempID, _strLicenseKey))
                {
                    if (_intTempID.ToString() != _strInput[0] || (_strInput[0] == "0" && _intTempID.ToString() != "0"))
                        return "false`RFID/QRCODE is already in used";
                }

                if (IsExistCommuterAccountsRFIDQRCODE(_strInput[4], out _intTempID, _strLicenseKey))
                {
                    if (_intTempID.ToString() != _strInput[0] || (_strInput[0] == "0" && _intTempID.ToString() != "0"))
                        return "false`Email address is already in used";
                }

                if (_IsEdit)
                {

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("UPDATE tblCommuterAccounts  SET fldDateRegistered=@fldDateRegistered,fldAccountCode=@fldAccountCode,fldName=@fldName,fldEmailAddress=@fldEmailAddress,fldPhone=@fldPhone,fldBirthday=@fldBirthday,fldGender=@fldGender,fldNFC=@fldNFC,fldRFIDQRCode=@fldRFIDQRCode,fldPassword=@fldPassword,fldLastSeen=@fldLastSeen WHERE fldID=@fldID", _strLicenseKey))
                    {                        
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateRegistered", _strInput[1]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccountCode", _strInput[2]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", _strInput[3]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldEmailAddress", _strInput[4]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPhone", _strInput[5]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldBirthday", _strInput[6]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldGender", _strInput[7]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldNFC", _strInput[8]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDQRCode", _strInput[9]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPassword", _strInput[10]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldLastSeen", _strInput[11]);

                        _SQLCMD.CommandObject.ExecuteNonQuery();                  

                    }

                }
                else
                {

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tblCommuterAccounts(fldDateRegistered,fldAccountCode,fldName,fldEmailAddress,fldPhone,fldBirthday,fldGender,fldNFC,fldRFIDQRCode,fldPassword,fldLastSeen) VALUES(@fldDateRegistered,@fldAccountCode,@fldName,@fldEmailAddress,@fldPhone,@fldBirthday,@fldGender,@fldNFC,@fldRFIDQRCode,@fldPassword,@fldLastSeen)", _strLicenseKey))
                    {

                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldDateRegistered", String.Format("{0:MM/dd/yyyy HH:mm}", DateTime.Now));
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccountCode", "LRT" + DateTime.Now.ToString().Remove(DateTime.Now.ToString().Length - 2).Replace("/", "").Replace(" ", "").Replace(":", ""));
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldName", _strInput[3]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldEmailAddress", _strInput[4]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPhone", _strInput[5]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldBirthday", _strInput[6]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldGender", _strInput[7]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldNFC", _strInput[8]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDQRCode", _strInput[9]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPassword", _strInput[10]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldLastSeen", _strInput[11]);
                        _SQLCMD.CommandObject.ExecuteNonQuery();

                    }

                }
                return "true`Successfully registered!";

            }
            catch (Exception ex)
            {
                return "false`" + ex.Message;
            }

        }

        public bool IsExistCommuterAccountsRFIDQRCODE(string _strCode, out int _intID, string _strLicenseKey)
        {

            try
            {
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblCommuterAccounts WHERE fldRFIDQRCode=@fldRFIDQRCode", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDQRCode", _strCode);

                    foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                    {
                        _intID = int.Parse(DR[0].ToString());
                        return true;
                    }
                    _intID = 0;
                    return false;

                }
            }
            catch
            {
                _intID = 0;
                return false;
            }

        }

        public bool IsExistCommuterAccountsEmail(string _strCode, out int _intID, string _strLicenseKey)
        {

            try
            {
                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldID FROM tblCommuterAccounts WHERE fldEmailAddress=@fldEmailAddress", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldEmailAddress", _strCode);

                    foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                    {
                        _intID = int.Parse(DR[0].ToString());
                        return true;
                    }
                    _intID = 0;
                    return false;

                }
            }
            catch
            {
                _intID = 0;
                return false;
            }

        }




        #endregion

        #region RFID
        [WebMethod]
        public DataTable LoadRFIDSetting_Where(string _strInputWhereStatement, string _strLicenseKey)
        {

            try
            {
                string[] _strInput = _strInputWhereStatement.Split('`');

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblPCRFIDSettings WHERE (fldPCSerialNo=@fldPCSerialNo OR @fldPCSerialNo='') ", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPCSerialNo", _strInput[0]);
                  

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt;

                }
            }
            catch
            {
                return null;
            }

        }

        [WebMethod]
        public string RFIDSetting_SaveRecord(string _strInputs, string _strLicenseKey)
        {

            try
            {
                bool _IsEdit = false;
                string[] _strInput = _strInputs.Split('`');

             

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblPCRFIDSettings WHERE (fldPCSerialNo=@fldPCSerialNo OR @fldPCSerialNo='') ", _strLicenseKey))
                {
                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPCSerialNo", _strInput[1]);


                    foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                    {
                        _IsEdit = true;
                    }

               

                }

                if (_IsEdit)
                {

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("UPDATE tblPCRFIDSettings  SET fldPCSerialNo=@fldPCSerialNo,fldRFIDIPaddress=@fldRFIDIPaddress,fldRFIDPortNo=@fldRFIDPortNo,fldCurrentStatus=@fldCurrentStatus,fldLastCommand=CASE WHEN @fldLastCommand='' THEN fldLastCommand else @fldLastCommand end WHERE fldPCSerialNo=@fldPCSerialNo", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPCSerialNo", _strInput[1]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDIPaddress", _strInput[2]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDPortNo", _strInput[3]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCurrentStatus", _strInput[4]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldLastCommand", _strInput[5]);
                     
                        _SQLCMD.CommandObject.ExecuteNonQuery();

                    }

                }
                else
                {

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tblPCRFIDSettings(fldPCSerialNo,fldRFIDIPaddress,fldRFIDPortNo,fldCurrentStatus,fldLastCommand) VALUES(@fldPCSerialNo,@fldRFIDIPaddress,@fldRFIDPortNo,@fldCurrentStatus,@fldLastCommand)", _strLicenseKey))
                    {

                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldID", _strInput[0]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldPCSerialNo", _strInput[1]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDIPaddress", _strInput[2]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDPortNo", _strInput[3]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldCurrentStatus", _strInput[4]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldLastCommand", _strInput[5]);

                        _SQLCMD.CommandObject.ExecuteNonQuery();

                    }

                }
                return "true`Record saved!";

            }
            catch (Exception ex)
            {
                return "false`" + ex.Message;
            }

        }


        #endregion




        #region Rewards

        [WebMethod]
        public long ScannedAdsCount(string _strLicenseKey)
        {

            try
            {


                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT fldid FROM tblRewards", _strLicenseKey))
                {

                    DataTable dt = new DataTable();
                    dt = _SQLCMD.GetDataTable();
                    dt.TableName = "Data";

                    return dt.Rows.Count;

                }
            }
            catch
            {
                return 0;
            }

        }

        [WebMethod]
        public decimal RewardsTotal(string _strLicenseKey)
        {

            try
            {


                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT sum(fldRewardRate) FROM tblRewards", _strLicenseKey))
                {

                    DataTable dt = _SQLCMD.GetDataTable();
                    foreach (DataRow DR in dt.Rows)
                    {
                        return decimal.Parse(DR[0].ToString());
                    }

                    return 0;

                }
            }
            catch
            {
                return 0;
            }

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void LoadStationCrownVolume_Where_JSON(string _strInputWhereStatement, string _strLicenseKey)
        {
            try
            {
                string[] _strInput = _strInputWhereStatement.Split('`');

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM View_StationCrowdVolume order by fldstationcode", _strLicenseKey))
                {

                  
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row = null;

                    DataTable dt = _SQLCMD.GetDataTable();
                    foreach (DataRow rs in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, rs[col]);
                        }
                        rows.Add(row);
                    }

                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                    this.Context.Response.Write(serializer.Serialize(new { StationCrownVolume = rows }));




                }
            }
            catch (Exception ex)
            {
            }





        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void LoadRewards_Where_JSON(string _strInputWhereStatement, string _strLicenseKey)
        {
            try
            {
                string[] _strInput = _strInputWhereStatement.Split('`');

                using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT * FROM tblRewards WHERE fldAccountCode=@fldAccountCode", _strLicenseKey))
                {

                    _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccountCode", _strInput[0]);
                   
                    System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();

                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row = null;

                    DataTable dt = _SQLCMD.GetDataTable();
                    foreach (DataRow rs in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, rs[col]);
                        }
                        rows.Add(row);
                    }

                    this.Context.Response.ContentType = "application/json; charset=utf-8";
                    this.Context.Response.Write(serializer.Serialize(new { Rewards = rows }));




                }
            }
            catch (Exception ex)
            {
            }





        }

        [WebMethod]
        public string Rewards_SaveRecord(string _strInputs, string _strLicenseKey)
        {

            try
            {
                
                string[] _strInput = _strInputs.Split('`');

              
                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tblRewards(fldAccountCode,fldRewardCode,fldRewardRate) VALUES(@fldAccountCode,@fldRewardCode,@fldRewardRate)", _strLicenseKey))
                    {
                       
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldAccountCode", _strInput[0]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRewardCode", _strInput[1]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRewardRate", _strInput[2]);
                        

                        _SQLCMD.CommandObject.ExecuteNonQuery();

                    }

            
                return "true`Record saved!";

            }
            catch (Exception ex)
            {
                return "false`" + ex.Message;
            }

        }


        #endregion



        [WebMethod]
        public string TravelHistory_SaveRecord(string[] _strInputArray, string _strLicenseKey)
        {

            try
            {
                bool _isOK = true;


                foreach (string rfid in _strInputArray)
                {

                    string[] _strInput = rfid.Split('`');

                    using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("SELECT DATEDIFF(MINUTE, max(fldDateTime),GETDATE()) as fldMINs  FROM tblTravelHistory WHERE (fldStationCode=@fldStationCode and fldRFIDQRCode=@fldRFIDQRCode) ", _strLicenseKey))
                    {
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldStationCode", _strInput[0]);
                        _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDQRCode", _strInput[1]);

                        foreach (DataRow DR in _SQLCMD.GetDataTable().Rows)
                        {
                            if (DR[0].ToString() != "")
                            {
                                if (int.Parse(DR[0].ToString()) >= 30)
                                {
                                    _isOK = true;
                                }
                                else
                                {
                                    _isOK = false;
                                }
                            }
                        }
                    }

                    if (_isOK)
                    {
                        using (SQLConnect.SqlCommandEx _SQLCMD = new SQLConnect.SqlCommandEx("INSERT INTO tblTravelHistory(fldDateTime,fldStationCode,fldRFIDQRCode) VALUES(GETDATE(),@fldStationCode,@fldRFIDQRCode)", _strLicenseKey))
                        {


                            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldStationCode", _strInput[0]);
                            _SQLCMD.CommandObject.Parameters.AddWithValue("@fldRFIDQRCode", _strInput[1]);


                            _SQLCMD.CommandObject.ExecuteNonQuery();

                        }

                        try
                        {
                            if (_strInput[1] == "100000000000000000000002")
                                popUpMSg();
                        }
                        catch { }
                    }
                }
                return "true`Record saved!";

            }
            catch (Exception ex)
            {
                return "false`" + ex.Message;
            }

        }

        private void popUpMSg()
        {

            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic NGEwMGZmMjItY2NkNy0xMWUzLTk5ZDUtMDAwYzI5NDBlNjJj");

            byte[] byteArray = Encoding.UTF8.GetBytes("{"
                                                    + "\"app_id\": \"3f1ed038-63f2-41e6-a0da-a8d4279a6593\","
                                                    + "\"contents\": {\"en\": \"Thank you for riding with LRT-1. Take care of your belongings. Have a safe trip!\"},"
                                                    + "\"include_player_ids\": [\"3c2868bc-c422-4d47-857f-d3f13e0262de\"]}");

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }

            System.Diagnostics.Debug.WriteLine(responseContent);


        }

    }
}
