using SelectPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Observer.Services
{
    public static class EmailServiceManager
    {
        public static bool initiated = false;
        public static SmtpClient client = new SmtpClient();
        public static string SignatureEnd = "<br><br><br>____________________<br><i>This is an automated notification service.</i><br><i>You are receiving this email because your email address is currently assigned to one of the observing API. To unsubscribe, please email to WebObs@amazecraft.net</i>";

        public static void CatchNotification(string Url, string ObserveName, string Recipient)
        {
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Url);
            //request.Credentials = new NetworkCredential("sg1-wss1\\itzmoon", "Qwerqwer11");
            //request.Method = WebRequestMethods.Ftp.DownloadFile;
            using (FileStream inputStream = new FileStream(@Url, FileMode.OpenOrCreate))
            {
                Attachment attachment = new System.Net.Mail.Attachment(inputStream, "preview.pdf");
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("webobs@amazecraft.net", "WebObs");
                mail.To.Add(new MailAddress(Recipient));
                mail.Subject = ObserveName + " Update Notification";
                mail.Body = "The system has detected new changes." + SignatureEnd;
                mail.Attachments.Add(attachment);
                mail.IsBodyHtml = true;
                InitiateEmail();
                client.Send(mail);
            }
                //Stream contentStream = request.GetResponse().GetResponseStream();
        }

        public static void CatchNotificationWithoutScreenshot(string Custom, string ObserveName, string Recipient)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("webobs@amazecraft.net", "WebObs");
            mail.To.Add(new MailAddress(Recipient));
            mail.Subject = ObserveName + " Update Notification";
            mail.Body = Custom+SignatureEnd;
            mail.IsBodyHtml = true;
            InitiateEmail();
            client.Send(mail);
        }

        public static void CatchErrorNotification(string ObserveName, string Recipient)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("webobs@amazecraft.net", "WebObs");
            mail.To.Add(new MailAddress(Recipient));
            mail.Subject = ObserveName + " Update Notification";
            mail.Body = "The system has disabled the observe as it has detected errors." + SignatureEnd;
            mail.IsBodyHtml = true;
            InitiateEmail();
            client.Send(mail);
        }

        public static void AccountPendingVerification(string Name, string KeyLink, string Recipient)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("webobs@amazecraft.net", "WebObs");
            mail.To.Add(new MailAddress(Recipient));
            mail.Subject = "AmazeCraft Account Verification";
            mail.Body = "Hi, "+Name+", <br><br>Your account is ready to be used, please access the link as follows to verify your account.<br><br><a href='"+KeyLink+"'>"+KeyLink+"</a>" + SignatureEnd;
            mail.IsBodyHtml = true;
            InitiateEmail();
            client.Send(mail);
        }

        public static void AccountEmailLogin(string Name, string KeyLink, string Recipient)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("webobs@amazecraft.net", "WebObs");
            mail.To.Add(new MailAddress(Recipient));
            mail.Subject = "Email Login";
            mail.Body = "Hi, "+Name+", <br><br>You have just initiated an email login request.<br>Please click the following link to approve the request.<br><br><a href='"+KeyLink+"'>"+KeyLink+"</a>" + SignatureEnd;
            mail.IsBodyHtml = true;
            InitiateEmail();
            client.Send(mail);
        }

        public static void InitiateEmail()
        {
            if (!initiated)
            {
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Host = "smtp.zoho.com";
                client.Credentials = new System.Net.NetworkCredential("webobs@amazecraft.net", "*XjK0@@Xr9");
            }
        }
    }
}