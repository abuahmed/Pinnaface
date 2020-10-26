using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for LabourProcess.xaml
    /// </summary>
    public partial class LabourProcess : Window
    {
        public LabourProcess()
        {
            LabourProcessViewModel.Errors = 0;
            InitializeComponent();
        }
        public LabourProcess(EmployeeDTO employee)
        {
            LabourProcessViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) LabourProcessViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) LabourProcessViewModel.Errors -= 1;
        }

        private void LabourProcess_OnUnloaded(object sender, RoutedEventArgs e)
        {
            LabourProcessViewModel.CleanUp();
        }
    }
}
