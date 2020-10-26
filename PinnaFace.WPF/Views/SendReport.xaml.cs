using System.Windows;
using System.Windows.Controls;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for SendReport.xaml
    /// </summary>
    public partial class SendReport : Window
    {
        public SendReport()
        {
            InitializeComponent();
        }
        
        private void SendReport_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ActivationViewModel.CleanUp();
        }
    }
}
