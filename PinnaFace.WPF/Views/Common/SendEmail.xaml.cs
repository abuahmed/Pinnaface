﻿using System.Windows;
using System.Windows.Controls;
using CrystalDecisions.CrystalReports.Engine;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Common;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
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
        public SendEmail(ReportDocument report,EmailDTO emailDetail)
        {
            SendEmailViewModel.Errors = 0;
            InitializeComponent();            
            Messenger.Default.Send<ReportDocument>(report);
            Messenger.Default.Send<EmailDTO>(emailDetail);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) SendEmailViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) SendEmailViewModel.Errors -= 1;
        }

        private void SendEmail_OnUnloaded(object sender, RoutedEventArgs e)
        {
            SendEmailViewModel.CleanUp();
        }
    }
}
