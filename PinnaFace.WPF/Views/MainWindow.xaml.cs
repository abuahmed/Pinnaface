using System;
using System.Windows;
using PinnaFace.Core.Enumerations;
using PinnaFace.WPF.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AgencyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new LocalAgency().ShowDialog();
        }

        private void AgentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ForeignAgents().ShowDialog();
        }

        private void VisaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new VisaDetail(new VisaModel()).ShowDialog();
        }

        #region Report Links

        private void LabourMonthly_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.LabourMonthly).ShowDialog();
        }

        private void LabourReturned_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.LabourReturned).ShowDialog();
        }

        private void LabourLost_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.LabourLost).ShowDialog();
        }

        private void LabourDiscontinued_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.LabourDiscontinued).ShowDialog();
        }

        private void LabourContractEnd_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.LabourContractEnd).ShowDialog();
        }

        private void EmbassyMonthly_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.EmbassyMonthly).ShowDialog();
        }

        private void TicketList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.TicketList).ShowDialog();
        }
        private void TicketAmountList_Click(object sender, RoutedEventArgs e)
        {
            new Duration(ReportTypes.TicketAmountList).ShowDialog();
        }

        private void SummaryList_Click(object sender, RoutedEventArgs e)
        {
            new SummaryFilter().ShowDialog();
            //new Duration(ReportTypes.SummaryList).ShowDialog();
        }
        #endregion

        private void UsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Users().ShowDialog();
        }

        private void ChangePasswordMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ChangePassword().ShowDialog();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Settings().ShowDialog();
        }

        private void BackupRestoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new BackupRestore().ShowDialog();
        }

        private void CalConvertorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new CalendarConvertor(DateTime.Now).ShowDialog();
        }

        private void VisaListMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Visas().ShowDialog();
        }

        private void ComplainListMenuItem_OnClickMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Complains().ShowDialog();
        }
        private void ComplainMenuItem_OnClickMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Complains().ShowDialog();
        }

        private void AddnewCOmplainMenuItem_OnClickMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ComplainDetail().ShowDialog();
        }

        private void AboutMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            new AboutBox().ShowDialog();
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentSetting = XmlSerializerCustom.GetUserSetting();
                if (currentSetting != null)
                {
                    currentSetting.UserName = "";
                    currentSetting.Password = "";
                    XmlSerializerCustom.SetUserSetting(currentSetting);
                }
            }
            catch
            {
            }

            Close();
        }

        private void SendErrorReportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new SendReport().ShowDialog();
        }
    }
}
