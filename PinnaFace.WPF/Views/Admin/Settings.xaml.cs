using System.Windows;
using System.Windows.Controls;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            SettingViewModel.Errors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) SettingViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) SettingViewModel.Errors -= 1;
        }

        private void Settings_OnUnloaded(object sender, RoutedEventArgs e)
        {
            SettingViewModel.CleanUp();
        }
    }
}
