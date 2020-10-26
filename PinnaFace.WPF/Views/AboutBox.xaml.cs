using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void wdwSpashScreen_Loaded(object sender, RoutedEventArgs e)
        {            
            Messenger.Default.Send<object>(sender);
            Messenger.Reset();
        }

        private void AboutBox_OnUnloaded(object sender, RoutedEventArgs e)
        {
            AboutBoxViewModel.CleanUp();
        }
    }
}
