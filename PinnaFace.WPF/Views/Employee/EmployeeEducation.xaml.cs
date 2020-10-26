using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeEducation.xaml
    /// </summary>
    public partial class EmployeeEducation : Window
    {
        public EmployeeEducation()
        {
            EmployeeEducationViewModel.Errors = 0;
            InitializeComponent();
        }
        public EmployeeEducation(EmployeeDTO employee)
        {
            EmployeeEducationViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeeEducationViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeeEducationViewModel.Errors -= 1;
        }

        private void wdwEmployeeEducation_Loaded(object sender, RoutedEventArgs e)
        {
            //TxtYearCompleted.Focus();
        }

        private void EmployeeEducation_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeeEducationViewModel.CleanUp();
        }
    }
}
