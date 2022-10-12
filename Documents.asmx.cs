using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace WebApplication2
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
        
        
        SqlConnection con = new SqlConnection(ConfigurationSettings.AppSettings["cs"]);
        public void SendJSON(string JSONtxt)
        {
            Context.Response.Clear();
            Context.Response.ContentType = "application/json";
            Context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            Context.Response.Flush();
            Context.Response.Write(JSONtxt);
        } // end SendJSON

        public bool token(string pNum,string  tokenID)
        {
            DateTime currentDate = DateTime.Now;
            string Date=currentDate.ToString("yyyyMMdd");
            string hash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Date + pNum+ Date))).Replace("-", "");
            if (hash == tokenID)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        [WebMethod]

        public  void AddDocReadDetails(int PNO,string DocumentName, string tokenID,string EmpType)
        {
            DateTime currentDate = DateTime.Now;
            token(PNO.ToString(), tokenID);
            if (token(PNO.ToString(), tokenID)) 
                
            {
                if (!String.IsNullOrEmpty(EmpType) && !EmpType.Any(char.IsDigit))
                {
                    
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into AddDocReadDetails (PNO,DocumentName,DATETIME,EmpType) values (@PNO,@DocumentName,@DATETIME,@EmpType)", con);

                cmd.Parameters.AddWithValue("@PNO", PNO);
                    cmd.Parameters.AddWithValue("@DocumentName", DocumentName);
                    cmd.Parameters.AddWithValue("@DATETIME", currentDate);
                    cmd.Parameters.AddWithValue("@EmpType", EmpType);
                    int result = cmd.ExecuteNonQuery();
              
                if (result > 0)
                {
                    string json = "{\"status\":\"1\"," +
                   "\"message\":\"Successfully\"}";
                    SendJSON(json);
                   
                }
                else
                {
                    string json = "{\"status\":\"0 \"," +
                   "\"message\":\"failed\"}";
                    SendJSON(json);

                }
                con.Close();
            }
            catch (Exception ex)
            {
                    string a=ex.Message;
                string json = "";

                if (ex.Message.Contains("Violation of PRIMARY KEY constraint 'PNO'."))
                {
                    json = "{\"status\":\"1\", \"message\":\"Already Read\"}";

                }
                else
                {
                    json = "{\"status\":\"0\", \"message\":\""+ex.Message+"\"}";
                }
               
                SendJSON(json);
                con.Close();
            }
                }
                else
                {
                    string json = "{\"status\":\"0\"," +
                                             "\"message\":\"invalid Employee type\"}";
                    SendJSON(json);
                }
          
            
            }

            else
            {
                string json = "{\"status\":\"0\"," +
                         "\"message\":\"invalid token\"}";
                SendJSON(json);
            }
        }


      
    }
}
