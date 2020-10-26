using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeExperience.xaml
    /// </summary>
    public partial class EmployeeExperience : Window
    {
        public EmployeeExperience()
        {
            EmployeeExperienceViewModel.Errors = 0;
            InitializeComponent();
        }
        public EmployeeExperience(EmployeeDTO employee)
        {
            EmployeeExperienceViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeeExperienceViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeeExperienceViewModel.Errors -= 1;
        }

        private void EmployeeApplication_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeeExperienceViewModel.CleanUp();
        }
    }
}
