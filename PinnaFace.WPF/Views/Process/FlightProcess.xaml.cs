using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for FlightProcess.xaml
    /// </summary>
    public partial class FlightProcess : Window
    {
        public FlightProcess()
        {
            FlightProcessViewModel.Errors = 0;
            InitializeComponent();
        }
        public FlightProcess(EmployeeDTO employee)
        {
            FlightProcessViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) FlightProcessViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) FlightProcessViewModel.Errors -= 1;
        }

        private void FlightProcess_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtToCity.Focus();
        }

        private void FlightProcess_OnUnloaded(object sender, RoutedEventArgs e)
        {
            FlightProcessViewModel.CleanUp();
        }
    }
}
