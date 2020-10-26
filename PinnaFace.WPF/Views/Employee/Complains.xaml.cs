using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Complains.xaml
    /// </summary>
    public partial class Complains : Window
    {
        public Complains()
        {
            ComplainViewModel.Errors = 0;
            InitializeComponent();
        }
        public Complains(EmployeeDTO employee)
        {
            ComplainViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ComplainViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ComplainViewModel.Errors -= 1;
        }

        private void LstItemsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void LstItemsAutoCompleteBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
        }

        private void Complains_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ComplainViewModel.CleanUp();
        }
    }
}
