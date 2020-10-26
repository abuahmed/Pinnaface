using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Quiksoft.EasyMail.SMTP;

namespace PinnaFace.Core.Common
{
    public class EmailDTO : EntityBase
    {
        public EmailDTO()
        {
            EmailMessage msg=new EmailMessage();
            //msg.SaveMessage();
            //Quiksoft.EasyMail.SMTP
        }

        [Required]
        public string Subject
        {
            get { return GetValue(() => Subject); }
            set { SetValue(() => Subject, value); }
        }        
        [Required]
        public string Body
        {
            get { return GetValue(() => Body); }
            set { SetValue(() => Body, value); }
        }        
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
        public string Recepient
        {
            get { return GetValue(() => Recepient); }
            set { SetValue(() => Recepient, value); }
        }      
        public string RecepientName
        {
            get { return GetValue(() => RecepientName); }
            set { SetValue(() => RecepientName, value); }
        }        
        public IList<string> Recepients
        {
            get { return GetValue(() => Recepients); }
            set { SetValue(() => Recepients, value); }
        }
        public string AttachmentFileName
        {
            get { return GetValue(() => AttachmentFileName); }
            set { SetValue(() => AttachmentFileName, value); }
        }        
    }
}
