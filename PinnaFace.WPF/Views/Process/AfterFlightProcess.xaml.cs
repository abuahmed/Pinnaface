using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for AfterFlightProcess.xaml
    /// </summary>
    public partial class AfterFlightProcess : Window
    {
        public AfterFlightProcess()
        {
            AfterFlightProcessViewModel.Errors = 0;
            InitializeComponent();
        }
        public AfterFlightProcess(EmployeeDTO employee)
        {
            AfterFlightProcessViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) AfterFlightProcessViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) AfterFlightProcessViewModel.Errors -= 1;
        }

        private void AfterFlightProcess_OnLoaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void AfterFlightProcess_OnUnloaded(object sender, RoutedEventArgs e)
        {
            AfterFlightProcessViewModel.CleanUp();
        }
    }
}
