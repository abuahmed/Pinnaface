using System.Windows;
using System.Windows.Controls;
using CrystalDecisions.CrystalReports.Engine;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for ReportViewerCommon.xaml
    /// </summary>
    public partial class ReportViewerCommon : Window
    {
        //ReportDocument reportDoc;

        public ReportViewerCommon()
        {
            InitializeComponent();
        }
        public ReportViewerCommon(ReportDocument report)
        {
            InitializeComponent();
            //reportDoc = report;
            Messenger.Default.Send<ReportDocument>(report);
            Messenger.Reset();
        }
        public ReportViewerCommon(ReportDocument report, ReportTypes reportType,EmailDTO emailDetail)
        {
            InitializeComponent();
            //reportDoc = report;
            Messenger.Default.Send<ReportDocument>(report);
            Messenger.Default.Send<ReportTypes>(reportType);
            Messenger.Default.Send<EmailDTO>(emailDetail);
            Messenger.Reset();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CrvReportViewer.ViewerCore.ReportSource = ReportViewerViewModel.ReportToView;// reportDoc;
        }

        //private void TxtDoAction_OnTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    CrvReportViewer.ViewerCore.ReportSource = ReportViewerViewModel.ReportToView;// reportDoc;
        //}

        private void ReportViewerCommon_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ReportViewerViewModel.CleanUp();
        }
    }
}
