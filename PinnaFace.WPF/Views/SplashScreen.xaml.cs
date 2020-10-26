using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            
        }

        private void wdwSpashScreen_Loaded(object sender, RoutedEventArgs e)
        {            
            Messenger.Default.Send<object>(sender);
            Messenger.Reset();
        }

        private void SplashScreen_OnUnloaded(object sender, RoutedEventArgs e)
        {
            SplashScreenViewModel.CleanUp();
        }
    }
}
