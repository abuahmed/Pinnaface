using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using PinnaFace.Core.Common;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.Admin.ViewModel
{
    public class SendEmailViewModel : ViewModelBase
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private EmailDTO _emailDetail;
        private string _emailAttachmentDetail;
        private ICommand _sendEmailCommand; 
        #endregion
        
        #region Constructor
        public SendEmailViewModel(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;

            Messenger.Default.Register<EmailDTO>(this, (message) =>
            {
                EmailDetail = message;
            });
        } 
        #endregion

        #region Public Properties
        public IUnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set
            {
                _unitOfWork = value;
                RaisePropertyChanged<IUnitOfWork>(() => UnitOfWork);
            }
        }
        public EmailDTO EmailDetail
        {
            get { return _emailDetail; }
            set
            {
                _emailDetail = value;
                RaisePropertyChanged<EmailDTO>(() => EmailDetail);
                if (EmailDetail != null)
                {
                    if (EmailDetail.AttachmentFileName != "")
                    {
                        EmailAttachmentDetail = EmailDetail.AttachmentFileName + ".doc";
                    }
                    else
                    {
                        EmailAttachmentDetail = "No Attachment..";
                    }
                }
            }
        }
        public string EmailAttachmentDetail
        {
            get { return _emailAttachmentDetail; }
            set
            {
                _emailAttachmentDetail = value;
                RaisePropertyChanged<string>(() => EmailAttachmentDetail);
            }
        } 

        #endregion
        
        #region Commands
        public ICommand SendEmailCommand
        {
            get
            {
                return _sendEmailCommand ?? (_sendEmailCommand = new RelayCommand<Object>(ExcuteSendEmail, CanSave));
            }
        }

        public void ExcuteSendEmail(object obj)
        {
            try
            {
                //var localAgency = _unitOfWork.Repository<AgencyDTO>().GetAllIncludingChilds(a => a.Address).FirstOrDefault();

                //if (localAgency != null)
                //{
                var fromAddress = new MailAddress("agencyonefes@gmail.com", "OneFace Key");
                const string fromPassword = "Agency1!";

                var toAddress = new MailAddress(EmailDetail.Recepient, EmailDetail.RecepientName);

                var addressBcc = new MailAddress("ibrayas@gmail.com", "AbuAhmed Yassin");

                var message = new MailMessage();

                message.To.Add(toAddress);


                //if (ReportToAttach != null)
                //{
                //    var oStream = (MemoryStream)ReportToAttach.ExportToStream(ExportFormatType.WordForWindows);
                //    var at = new Attachment(oStream, EmailDetail.AttachmentFileName + ".doc", "application/doc");
                //    message.Attachments.Add(at);
                //}

                message.Subject = EmailDetail.Subject;
                message.Body = EmailDetail.Body;
                message.From = fromAddress;

                message.CC.Add(addressBcc);

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                smtp.Send(message);
                //}

                MessageBox.Show("Email Sent Successfully!!", "Email Sent", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseWindow(obj);
            }
            catch
            {
                MessageBox.Show("Email Sending Failed, Check your Connection and try again...", "Error Sending Email... ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;
        }
        private void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }
        #endregion
    }
}
