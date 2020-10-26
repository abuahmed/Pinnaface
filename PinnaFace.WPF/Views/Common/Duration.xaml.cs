using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Enumerations;
using System.Windows;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Duration.xaml
    /// </summary>
    public partial class Duration : Window
    {
        public Duration()
        {
            InitializeComponent();
        }
        public Duration(ReportTypes reportType)
        {
            InitializeComponent();
            Messenger.Default.Send<ReportTypes>(reportType);
            Messenger.Reset();
        }

        private void ReportDuration_OnUnloaded(object sender, RoutedEventArgs e)
        {
            //DurationViewModel.CleanUp();
        }
    }
}
