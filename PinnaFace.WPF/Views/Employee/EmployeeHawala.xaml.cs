using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeHawala.xaml
    /// </summary>
    public partial class EmployeeHawala : Window
    {
        public EmployeeHawala()
        {
            EmployeeHawalaViewModel.Errors = 0;
            InitializeComponent();
        }
        public EmployeeHawala(EmployeeDTO employee)
        {
            EmployeeHawalaViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeeHawalaViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeeHawalaViewModel.Errors -= 1;
        }

        private void wdwEmployeeHawala_Loaded(object sender, RoutedEventArgs e)
        {
            TxtYearCompleted.Focus();
        }

        private void EmployeeHawala_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeeHawalaViewModel.CleanUp();
        }
    }
}
