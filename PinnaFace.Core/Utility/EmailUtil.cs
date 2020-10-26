using System;
using System.Linq;
using System.Net.Mail;

namespace PinnaFace.Core
{
    public static class EmailUtil
    {
        public static string SendEMail(string email, string subject, string body)
        {
            try
            {
                var msg = new MailMessage
                {
                    From = new MailAddress("admin@pinnaface.com", "PinnaFace admin")
                };
                msg.To.Add(new MailAddress(email));

                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = body;

                //pinnauser@pinnaface.com pinn@u53r
                const string stringFromPass = "**********";
                var smtp = new SmtpClient
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    //EnableSsl = true,
                    Host = "mail.pinnaface.com",
                    Port = 2525
                };

                var secureString = new System.Security.SecureString();
                stringFromPass.ToCharArray().ToList().ForEach(secureString.AppendChar);

                var credentials = new System.Net.NetworkCredential("admin@pinnaface.com", "UmuAhmed1!");//secureString
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credentials;

                smtp.Send(msg);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message + "(" + ex.InnerException + ")";
            }
        }

        //private static void SendEMail(string email, string subject, string body)
        //{
        //    //pinnauser@pinnaface.com pinn@u3er
        //    const string stringFromPass = "****";
        //    var smtp = new SmtpClient
        //    {
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        EnableSsl = true,
        //        Host = "smtp.gmail.com",
        //        Port = 465
        //    };

        //    var secureString = new System.Security.SecureString();
        //    stringFromPass.ToCharArray().ToList().ForEach(secureString.AppendChar);//.ForEach(p => secureString.AppendChar(p));
        //    var credentials = new System.Net.NetworkCredential("ibrayas@gmail.com", secureString);
        //    smtp.UseDefaultCredentials = false;
        //    smtp.Credentials = credentials;

        //    var msg = new MailMessage
        //    {
        //        From = new MailAddress("ibrayas@gmail.com")
        //    };
        //    msg.To.Add(new MailAddress(email));

        //    msg.Subject = subject;
        //    msg.IsBodyHtml = true;
        //    msg.Body = body;

        //    smtp.Send(msg);
        //}
    }
}