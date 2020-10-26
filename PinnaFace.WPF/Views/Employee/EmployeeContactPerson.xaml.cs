using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeRelative.xaml
    /// </summary>
    public partial class EmployeeContactPerson : Window
    {
        public EmployeeContactPerson()
        {
            EmployeeContactPersonViewModel.Errors = 0;
            InitializeComponent();
        }
        public EmployeeContactPerson(EmployeeDTO employee)
        {
            EmployeeContactPersonViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeeContactPersonViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeeContactPersonViewModel.Errors -= 1;
        }

        private void LstContactsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LstContactsAutoCompleteBox.SearchText = string.Empty;
        }

        private void wdwEmployeeRelative_Loaded(object sender, RoutedEventArgs e)
        {
            if (TxtFullName.Text.Contains("-"))
                TxtFullName.Text = TxtFullName.Text.Replace('-', ' ');
            TxtFullName.Focus();
        }

        private void EmployeeContactPerson_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeeContactPersonViewModel.CleanUp();

        }

        private void EmployeeContactPerson_OnClosing(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtFullName.Text) || string.IsNullOrWhiteSpace(TxtFullName.Text))
                TxtFullName.Text = "-";
        }
    }
}
