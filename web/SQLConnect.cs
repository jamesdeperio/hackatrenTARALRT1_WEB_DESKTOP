using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.IO;
//using Microsoft.SqlServer.Management.Smo;
//using Microsoft.SqlServer.Management.Common;

namespace NXYSOFT_RMS
{
    public class SQLConnect
    {
      


        //public static void CreateDatabase(string filePath)
        //{
        //    if (File.Exists(filePath))
        //    {
        //        FileInfo file = new FileInfo(filePath);
        //        string script = file.OpenText().ReadToEnd();

        //        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        //        {

        //            Server server = new Server(new ServerConnection(conn));
        //            ReturnValue = server.ConnectionContext.ExecuteNonQuery(script);
        //        }

        //        file.OpenText().Close();
        //    }

        //}


        //public static void configuration(string _strLicenseKey)
        //{
        //    string input;
        //    try
        //    {
              
        //        using (StreamReader file = File.OpenText(AppDomain.CurrentDomain.BaseDirectory +  @"LicenseKeys\"  + _strLicenseKey + ".cfg"))
        //        {
        //            while ((input = file.ReadLine()) != null)
        //            {
        //                string[] Tempinput = input.Split('`');
        //                if (Tempinput[0] == "ServerIP")                     
        //                    ServerIP = Tempinput[1];
        //                if (Tempinput[0] == "ServerUsername")                     
        //                    ServerUsername = Tempinput[1];
        //                if (Tempinput[0] == "ServerPassword")
        //                    ServerPassword = Tempinput[1];
        //                if (Tempinput[0] == "ServerDB")
        //                    ServerDB = Tempinput[1];
        //                if (Tempinput[0] == "Status")
        //                    _strStatus = Tempinput[1];
        //                if (Tempinput[0] == "Package")
        //                    _strPackage = Tempinput[1];
                        
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



        public static string GetConnectionString(string _strLicenseKey)
        {
            //configuration(_strLicenseKey);
            string ServerIP = string.Empty;
            string ServerUsername = string.Empty;
            string ServerPassword = string.Empty;
            string ServerDB = string.Empty;

            string LicensePath = string.Empty;
            string input;
            try
            {

               
                using (StreamReader file = File.OpenText(AppDomain.CurrentDomain.BaseDirectory+"Server.cfg"))
                {
                    while ((input = file.ReadLine()) != null)
                    {
                        string[] Tempinput = input.Split('`');
                        if (Tempinput[0] == "ServerIP")
                            ServerIP = Tempinput[1].ToString();
                        if (Tempinput[0] == "ServerUsername")
                            ServerUsername = Tempinput[1];
                        if (Tempinput[0] == "ServerPassword")
                            ServerPassword = Tempinput[1];
                        if (Tempinput[0] == "ServerDB")
                            ServerDB = Tempinput[1];
                   

                    }
                }

              

            }
            catch (Exception ex)
            {
                throw ex;
            }
          
            return "Data Source=" + ServerIP + ";Initial Catalog=" + ServerDB + " ;User ID=" + ServerUsername + ";Password=" + ServerPassword;
            //return "Server =" + ServerIP + "; Database=" + ServerDB + " ; Trusted_Connection = True";
        }

        public static string CleanSQL(string data)
        {
            return data.Replace("'", "''");
        }

        
        static SqlConnection _conn = new SqlConnection(GetConnectionString(""));

        public static int ExecuteNonQuery(string SQL, string _strLicenseKey)
        {



            if (_conn.State == ConnectionState.Open) _conn.Close();
                _conn.Open();

                SqlCommand cmd = new SqlCommand(SQL, _conn);
                int result = cmd.ExecuteNonQuery();

                //_conn.Close();
                return result;
          

        }

        public static object ExecuteScalar(string SQL, string _strLicenseKey)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString(_strLicenseKey)))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(SQL, conn);
                object result = cmd.ExecuteScalar();

                conn.Close();
                return result;
            }
        }

        public class SqlCommandEx : IDisposable
        {
            
            
            public SqlCommand CommandObject;
            public SqlConnection _conn;
            public SqlCommandEx(string SQL, string _strLicenseKey)
            {
                try
                {
                    string ServerIP = string.Empty;
                    string ServerUsername = string.Empty;
                    string ServerPassword = string.Empty;
                    string ServerDB = string.Empty;
                    string LicensePath = string.Empty;
                    string input;
                    try
                    {


                    using (StreamReader file = File.OpenText(AppDomain.CurrentDomain.BaseDirectory+ _strLicenseKey + ".cfg"))
                    {
                    while ((input = file.ReadLine()) != null)
                    {
                        string[] Tempinput = input.Split('`');
                        if (Tempinput[0] == "ServerIP")
                            ServerIP = Tempinput[1].ToString();
                        if (Tempinput[0] == "ServerUsername")
                            ServerUsername = Tempinput[1];
                        if (Tempinput[0] == "ServerPassword")
                            ServerPassword = Tempinput[1];
                        if (Tempinput[0] == "ServerDB")
                            ServerDB = Tempinput[1];
                   

                    }
                }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }



                    _conn = new SqlConnection("Data Source=" + ServerIP + ";Initial Catalog=" + ServerDB + " ;User ID=" + ServerUsername + ";Password=" + ServerPassword);

                    if (_conn.State == ConnectionState.Open)
                        _conn.Close();

                    _conn.Open();
                    CommandObject = new SqlCommand(SQL, _conn);
                    CommandObject.CommandType = CommandType.Text;


                }
                catch { }
            }

            public DataTable GetDataTable()
            {
                SqlDataAdapter da = new SqlDataAdapter(CommandObject);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }

            public DataSet GetDataSet()
            {
                SqlDataAdapter da = new SqlDataAdapter(CommandObject);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            public void AddWithValue(string parameter, object value)
            {
                CommandObject.Parameters.AddWithValue(parameter, value);
            }

            public void Dispose()
            {
                _conn.Close();
                _conn.Dispose();
            }
        }

        public static DataTable GetDataTable(string SQL, string _strLicenseKey)
        {


            string ServerIP = string.Empty;
            string ServerUsername = string.Empty;
            string ServerPassword = string.Empty;
            string ServerDB = string.Empty;
            string LicensePath = string.Empty;
            string input;
            try
            {


                using (StreamReader file = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"LicenseKeys\Path.cfg"))
                {
                    while ((input = file.ReadLine()) != null)
                    {
                        string[] Tempinput = input.Split('`');
                        if (Tempinput[0].ToUpper() == "Path".ToUpper())
                            LicensePath = Tempinput[1].ToString();


                    }
                }

                using (StreamReader file = File.OpenText(LicensePath + _strLicenseKey + ".cfg"))
                {
                        while ((input = file.ReadLine()) != null)
                        {
                            string[] Tempinput = input.Split('`');
                            if (Tempinput[0] == "ServerIP")
                                ServerIP = Tempinput[1].ToString();
                            if (Tempinput[0] == "ServerUsername")
                                ServerUsername = Tempinput[1];
                            if (Tempinput[0] == "ServerPassword")
                                ServerPassword = Tempinput[1];
                            if (Tempinput[0] == "ServerDB")
                                ServerDB = Tempinput[1];
                            //if (Tempinput[0] == "Status")
                            //    Public_VarWeb._strStatus = Tempinput[1];
                            //if (Tempinput[0] == "Package")
                            //    Public_VarWeb._strPackage = Tempinput[1];

                        }
                    }



                }
                catch (Exception ex)
                {
                    throw ex;
                }






                using (SqlConnection conn = new SqlConnection("Data Source=" + ServerIP + ";Initial Catalog=" + ServerDB + " ;User ID=" + ServerUsername + ";Password=" + ServerPassword))
                {
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter(SQL, conn);
                    DataTable result = new DataTable();
                    da.Fill(result);

                    conn.Close();
                    return result;
                }
            }
        
    }
}