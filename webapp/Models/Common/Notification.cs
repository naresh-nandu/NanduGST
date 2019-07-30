using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Configuration;

namespace SmartAdminMvc.Models.Common
{
    public partial class Notification
    {
        public static void SendSMS(string MobileNo, string Message)
        {
            if (HttpContext.Current.Session["Partner_Company"].ToString() != "Hamara Kendra")
            {
                if (ConfigurationManager.AppSettings["SMS_Settings_New"] == "true")
                {
                    string userid = ConfigurationManager.AppSettings["SMSNew_UserId"];
                    string password = ConfigurationManager.AppSettings["SMSNew_Password"];
                    string appid = ConfigurationManager.AppSettings["SMSNew_AppId"];
                                        
                    WebClient web = new WebClient();
                    web.DownloadString("https://push3.maccesssmspush.com/servlet/com.aclwireless.pushconnectivity.listeners.TextListener?userId=" + userid + "&pass=" + password + "&appid=" + appid + "&subappid=" + appid + "&contenttype=1&to=" + MobileNo + "&from=WePGST&text=" + Message + "&selfid=true&alert=1&dlrreq=true");
                    web.Dispose();
                }
                else
                {
                    string userid = ConfigurationManager.AppSettings["SMSOld_UserId"];
                    string password = ConfigurationManager.AppSettings["SMSOld_Password"];
                    string appid = ConfigurationManager.AppSettings["SMSOld_AppId"];

                    string mobnum = "+91" + MobileNo;
                    string msg = Message;
                    WebClient web = new WebClient();
                    web.DownloadString("http://www.unicel.in/SendSMS/smspost.php?uname=" + userid + "&pass=" + password + "&send=" + appid + "&dest=" + mobnum + "&msg=" + msg + "&type=1");
                    web.Dispose();
                }
            }
            if (HttpContext.Current.Session["Partner_Company"].ToString() == "Hamara Kendra")
            {
                string smsText = Message;
                string mobileno = MobileNo;
                string URI = "http://www.webpostservice.com/sendsms/sendsms.php?username=hamara&password=YRyOv1&type=TEXT&sender=HAMARA&mobile=" + mobileno + "&message=" + smsText;
                WebRequest req = WebRequest.Create(URI);
                WebResponse resp = req.GetResponse();
                var sr = new System.IO.StreamReader(resp.GetResponseStream());
                sr.ReadToEnd();
            }
        }

        public static void SendEmail(string Email, string subject, string body)
        {
            if (HttpContext.Current.Session["Partner_Company"].ToString() != "Hamara Kendra")
            {
                string BodyMsg = string.Empty;
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        BodyMsg = "<html xmlns='http://www.w3.org/1999/xhtml'><body><div><strong>Dear Customer,<br />";
                        BodyMsg += "</strong></div><div style='margin-left: 40px'><br />" + body + "</div>";
                        BodyMsg += "<div><br /><br /><br /><strong><span style='color: #000000' > Thanks & Regards,</span><br /> Wep Digital Services</ span ><br/> ";
                        BodyMsg += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <img src='https://www.wepgst.com/Content/Images/icon-wep-logo.gif'></span></strong><span style='color: #CC0000'><br />Cashless, Paperless,<br />Omnipresent Solutions</span></div></body></html>";

                        MailMessage mm = new MailMessage();
                        MailAddress fromMail = new MailAddress("noreply@wepdigital.com", "WeP GST Panel - Making Life Easier");
                        // Sender e-mail address.
                        mm.From = fromMail;
                        // Recipient e-mail address.
                        mm.To.Add(new MailAddress(Email));
                        mm.Bcc.Add(new MailAddress("raja.m@wepindia.com"));
                        mm.Subject = subject;
                        mm.Body = BodyMsg;
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.office365.com";
                        smtp.EnableSsl = true;
                        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                        NetworkCred.UserName = "noreply@wepdigital.com";
                        NetworkCred.Password = "wep@12345";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
            }
            if (HttpContext.Current.Session["Partner_Company"].ToString() == "Hamara Kendra")
            {
                string BodyMsg = string.Empty;
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        BodyMsg = "<html xmlns='http://www.w3.org/1999/xhtml'><body><div><strong>Dear Customer,<br />";
                        BodyMsg += "</strong></div><div style='margin-left: 40px'><br />" + body + "</div>";
                        BodyMsg += "<div><br /><br /><br /><strong><span style='color: #000000' > Regards,<br /> Always with best Services,<br /> Team Hamara Kendra</ span ><br/> ";                        

                        MailMessage mm = new MailMessage();
                        MailAddress fromMail = new MailAddress("customercarehk@ipsindia.co.in", "GST Services");
                        // Sender e-mail address.
                        mm.From = fromMail;
                        // Recipient e-mail address.
                        mm.To.Add(new MailAddress(Email));
                        mm.Subject = "Hamara Kendra GST Suvidha";
                        mm.Body = BodyMsg;
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.mail.yahoo.com";
                        smtp.EnableSsl = true;
                        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                        NetworkCred.UserName = "customercarehk@ipsindia.co.in";
                        NetworkCred.Password = "Ipseservices@121";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 25;
                        smtp.Send(mm);
                    }
                }
            }
        }

        public static void SendEmail_Reconciliation(string Email,string userEmail, string subject, string body)
        {
            if (HttpContext.Current.Session["Partner_Company"].ToString() != "Hamara Kendra")
            {
                string BodyMsg = string.Empty;
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        BodyMsg = "<html xmlns='http://www.w3.org/1999/xhtml'><body><div><strong>Dear Supplier,<br />";
                        BodyMsg += "</strong></div><div style='margin-left: 40px'><br />" + body + "</div>";
                        BodyMsg += "<div><br /><br /><br /><strong><span style='color: #000000' > Thanks & Regards,</span><br /> Wep Digital Services</ span ><br/> ";
                        BodyMsg += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <img src='https://www.wepgst.com/Content/Images/icon-wep-logo.gif'></span></strong><span style='color: #CC0000'><br />Cashless, Paperless,<br />Omnipresent Solutions</span></div></body></html>";

                        MailMessage mm = new MailMessage();
                        MailAddress fromMail = new MailAddress("noreply@wepdigital.com", "WeP GST Panel - Making Life Easier");
                        // Sender e-mail address.
                        mm.From = fromMail;
                        // Recipient e-mail address.
                        mm.To.Add(new MailAddress(Email));
                        mm.CC.Add(new MailAddress(userEmail));
                        mm.Bcc.Add(new MailAddress("raja.m@wepindia.com"));
                        mm.Subject = subject;
                        mm.Body = BodyMsg;
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.office365.com";
                        smtp.EnableSsl = true;
                        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                        NetworkCred.UserName = "noreply@wepdigital.com";
                        NetworkCred.Password = "wep@12345";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }
            }
            if (HttpContext.Current.Session["Partner_Company"].ToString() == "Hamara Kendra")
            {
                string BodyMsg = string.Empty;
                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        BodyMsg = "<html xmlns='http://www.w3.org/1999/xhtml'><body><div><strong>Dear Customer,<br />";
                        BodyMsg += "</strong></div><div style='margin-left: 40px'><br />" + body + "</div>";
                        BodyMsg += "<div><br /><br /><br /><strong><span style='color: #000000' > Regards,<br /> Always with best Services,<br /> Team Hamara Kendra</ span ><br/> ";

                        MailMessage mm = new MailMessage();
                        MailAddress fromMail = new MailAddress("customercarehk@ipsindia.co.in", "GST Services");
                        // Sender e-mail address.
                        mm.From = fromMail;
                        // Recipient e-mail address.
                        mm.To.Add(new MailAddress(Email));
                        mm.Subject = "Hamara Kendra GST Suvidha";
                        mm.Body = BodyMsg;
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.mail.yahoo.com";
                        smtp.EnableSsl = true;
                        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                        NetworkCred.UserName = "customercarehk@ipsindia.co.in";
                        NetworkCred.Password = "Ipseservices@121";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 25;
                        smtp.Send(mm);
                    }
                }
            }
        }


        public static void SendEmailtoASPPanel(string Email, string Name, string subject, string body)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    MailMessage mm = new MailMessage();
                    MailAddress fromMail = new MailAddress("noreply@wepdigital.com", Name + " ( " + Email + " )");
                    // Sender e-mail address.
                    mm.From = fromMail;
                    // Recipient e-mail address.
                    mm.To.Add(new MailAddress("raja.m@wepindia.com"));
                    mm.CC.Add(new MailAddress("swarupa.anand@wepindia.com"));
                    mm.Bcc.Add(new MailAddress("raja.m@wepindia.com"));
                    mm.Subject = subject;
                    mm.Body = body;
                    mm.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.office365.com";
                    smtp.EnableSsl = true;
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = "noreply@wepdigital.com";
                    NetworkCred.Password = "wep@12345";
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }
    }
}