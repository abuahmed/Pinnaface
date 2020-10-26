using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Common;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.WPF.ViewModel
{
    public class SendEmailViewModel : ViewModelBase
    {
        #region Private Fields
        private static IUnitOfWork _unitOfWork;
        private ReportDocument _reportToAttach;
        private EmailDTO _emailDetail;
        private ICommand _sendEmailCommand;
        private string _emailAttachmentDetail;
        #endregion

        #region Constructor
        public SendEmailViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            Messenger.Default.Register<ReportDocument>(this, message =>
            {
                ReportToAttach = message;
            });
            Messenger.Default.Register<EmailDTO>(this, message =>
            {
                EmailDetail = message;
            });
        }
        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }
        #endregion

        #region Public Properties
        public ReportDocument ReportToAttach
        {
            get { return _reportToAttach; }
            set
            {
                _reportToAttach = value;
                RaisePropertyChanged<ReportDocument>(() => ReportToAttach);
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
                return _sendEmailCommand ?? (_sendEmailCommand = new RelayCommand(ExcuteSendEmail, CanSave));
            }
        }
        public void ExcuteSendEmail()
        {
            try
            {
                var localAgency = _unitOfWork.Repository<AgencyDTO>().Query().Get().FirstOrDefault();

                if (localAgency != null)
                {
                    var fromAddress = new MailAddress("agencyonefes@gmail.com", localAgency.AgencyName);
                    const string fromPassword = "Agency1!";

                    var toAddress = new MailAddress(EmailDetail.Recepient, EmailDetail.RecepientName);

                    var addressBcc = new MailAddress(localAgency.Address.PrimaryEmail, localAgency.Address.PrimaryEmail);

                    var message = new MailMessage();

                    message.To.Add(toAddress);
                    
                    if (ReportToAttach != null)
                    {
                        var oStream = (MemoryStream)ReportToAttach.ExportToStream(ExportFormatType.WordForWindows);
                        //using (FileStream file =new FileStream())
                        //{
                        //    oStream.CopyTo(file);
                        //}
                        var at = new Attachment(oStream, EmailDetail.AttachmentFileName + ".doc", "application/doc");
                        message.Attachments.Add(at);
                    }

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
                }

                MessageBox.Show("Email Sent Successfully!!", "Email Sent", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("Email Sending Failed, Check your Connection and try again...", "Error Sending Email... ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 
        #endregion
        
        #region Validation
        public static int Errors { get; set; }
        public bool CanSave()
        {
            if (Errors == 0)
                return true;
            return false;
        }

        #endregion
    }
}
