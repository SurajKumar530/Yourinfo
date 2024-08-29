using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static partial class SiteUtility
    {


        public static CEncryptDecrpt objCnctyption = new CEncryptDecrpt();
        public static string ConString { get; set; }








        public static string EncPwd(string pwd)
        {
            return objCnctyption.FalconEncrypt(pwd);
        }

        public static string Encrypt(string key)
        {
            return objCnctyption.FalconEncrypt(key);
        }
        public static string Decrypt(string key)
        {
            try
            {
                return objCnctyption.FalconDcrypt(key);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string ConvertDataTableToHTML(DataTable dt)
        {
            string html = "";
            //add header row
            html += "<THEAD>";
            html += "<tr>";
            for (int i = 0; i < dt.Columns.Count; i++)
                html += "<td>" + dt.Columns[i].ColumnName + "</td>";
            html += "</tr>";
            html += "</THEAD>";
            //add rows
            html += "<TBODY>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dt.Columns.Count; j++)
                    html += "<td>" + dt.Rows[i][j].ToString() + "</td>";
                html += "</tr>";
            }
            html += "</TBODY>";

            return html;
        }


        public static bool IsAllColumnExist(this DataTable tableNameToCheck, List<string> columns)
        {
            bool iscolumnExist = true;
            try
            {
                if (null != tableNameToCheck && tableNameToCheck.Columns != null)
                {
                    foreach (string columnName in columns)
                    {
                        if (!tableNameToCheck.Columns.Contains(columnName))
                        {
                            iscolumnExist = false;
                            break;
                        }
                    }
                }
                else
                {
                    iscolumnExist = false;
                }
            }
            catch (Exception)
            {

            }
            return iscolumnExist;
        }

        public static DateTime ConvertDDMMYYYY(this string strDate)
        {

            var str = strDate.Split('-');
            var sdate = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]), Convert.ToInt32(str[0]));
            return sdate;
        }


        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }


        //public static void ResigerLog4net()
        //{
        //    log4net.Config.XmlConfigurator.Configure();
        //}

        public static void SendMailWithCCBCC(string Subject, string body, string to, string from, string fromName, string CC, string BCC)
        {
            try
            {
                //  String str;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(from, fromName, System.Text.Encoding.UTF8);
                mailMessage.To.Add(to);

                if (CC != "")
                {
                    string[] arrTo = CC.Split(',');

                    if (arrTo.Length == 1)
                    {
                        mailMessage.CC.Add(arrTo[0]);

                    }
                    else
                    {
                        foreach (string Comma1 in arrTo)
                        {
                            mailMessage.CC.Add(Comma1);

                        }
                    }
                }
                if (BCC != "")
                {
                    string[] arrTo1 = BCC.Split(',');

                    if (arrTo1.Length == 1)
                    {
                        mailMessage.Bcc.Add(arrTo1[0]);

                    }
                    else
                    {
                        foreach (string Comma in arrTo1)
                        {

                            mailMessage.Bcc.Add(Comma);

                        }
                    }
                }
                mailMessage.Subject = Subject;
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                SmtpClient oSmtpClient = new SmtpClient();
                oSmtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }


        public static string SendOtpAsync(string uri)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(uri).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    return responseContent.ReadAsStringAsync().GetAwaiter().GetResult();
                }
                else
                {

                    return "error" + response.StatusCode.ToString();
                }
            }
        }

        public static string EncodeBase64(string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }

    }



    public enum NotifyType
    {
        Success,
        Error,
        Warning
    }
}
