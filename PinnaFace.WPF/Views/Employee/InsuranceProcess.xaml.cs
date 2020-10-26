using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeMedicalInsurance.xaml
    /// </summary>
    public partial class InsuranceProcess : Window
    {
        public InsuranceProcess()
        {
            EmployeeMedicalInsuranceViewModel.Errors = 0;
            InitializeComponent();
        }
        public InsuranceProcess(EmployeeDTO employee)
        {
            EmployeeMedicalInsuranceViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeeMedicalInsuranceViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeeMedicalInsuranceViewModel.Errors -= 1;
        }

        private void InsuranceProcess_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtPolicyNumber.Focus();
        }

        private void InsuranceProcess_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeeMedicalInsuranceViewModel.CleanUp();
        }
    }
}
