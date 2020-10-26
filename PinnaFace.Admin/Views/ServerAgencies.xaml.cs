using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PinnaFace.Admin.ViewModel;

namespace PinnaFace.Admin.Views
{
    /// <summary>
    /// Interaction logic for Agencys.xaml
    /// </summary>
    public partial class ServerAgencys : Window
    {
        public ServerAgencys()
        {
            ServerAgencyViewModel.Errors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ServerAgencyViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ServerAgencyViewModel.Errors -= 1;
        }

        //private void CheckBox_Checked(object sender, RoutedEventArgs e)
        //{

        //}

        //private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        //{
        //    TxtAwajNumber.Focus();
        //}

        private void Agencys_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ServerAgencyViewModel.CleanUp();
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
