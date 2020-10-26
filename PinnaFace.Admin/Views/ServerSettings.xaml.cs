using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PinnaFace.Admin.ViewModel;

namespace PinnaFace.Admin.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class ServerSettings : Window
    {
        public ServerSettings()
        {
            ServerSettingViewModel.Errors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ServerSettingViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ServerSettingViewModel.Errors -= 1;
        }

        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{

        //}

        //private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        //{
        //    TxtAwajNumber.Focus();
        //}

        private void Settings_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ServerSettingViewModel.CleanUp();
        }

        private void LstItemsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }
    }
}
