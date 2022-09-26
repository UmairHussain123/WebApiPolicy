using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-ENT3TDE\SQLEXPRESS;Initial Catalog=policy;Integrated Security=True");
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
            string hash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(pNum))).Replace("-", "");
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

        public  void ReadPolicy(int PNO,int read,string tokenID)
        {
            DateTime currentDate = DateTime.Now;
            token(PNO.ToString(), tokenID);
            if (token(PNO.ToString(), tokenID)) 
            {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("insert into policy_Read (PNO,IS_Read,DATETIME) values (@PNO,@IS_READ,@DATETIME)", con);

                cmd.Parameters.AddWithValue("@PNO", PNO);
                cmd.Parameters.AddWithValue("@IS_READ", read);
                cmd.Parameters.AddWithValue("@DATETIME", currentDate);
                int result = cmd.ExecuteNonQuery();
              
                if (result > 0)
                {
                    string json = "{\"status\":\"1\"," +
                   "\"message\":\"Successfully\"}";
                    SendJSON(json);
                    //Context.Response.Write(md5Hash);
                }
                else
                {
                    string json = "{\"status\":\"0\"," +
                   "\"message\":\"failed\"}";
                    SendJSON(json);

                }
                con.Close();
            }
            catch (Exception ex)
            {
                string json = "";

                if (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
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
                         "\"message\":\"invalid token\"}";
                SendJSON(json);
            }
        }
    }
}
