using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Admin.ViewModel;
using PinnaFace.Core.Common;

namespace PinnaFace.Admin.Views
{
    /// <summary>
    /// Interaction logic for SendEmail.xaml
    /// </summary>
    public partial class SendEmail : Window
    {
        public SendEmail()
        {
            SendEmailViewModel.Errors = 0;
            InitializeComponent();
        }
        public SendEmail(EmailDTO _emailDetail)
        {
            SendEmailViewModel.Errors = 0;
            InitializeComponent();            
            Messenger.Default.Send<EmailDTO>(_emailDetail);
        }
           
        //public SendEmail(ReportDocument _report,EmailDTO _emailDetail)
        //{
        //    SendEmailViewModel.Errors = 0;
        //    InitializeComponent();            
        //    Messenger.Default.Send<ReportDocument>(_report);
        //    Messenger.Default.Send<EmailDTO>(_emailDetail);
        //    Messenger.Reset();
        //}
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) SendEmailViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) SendEmailViewModel.Errors -= 1;
        }
    }
}
