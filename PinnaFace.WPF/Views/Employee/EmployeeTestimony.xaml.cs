using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeTestimony.xaml
    /// </summary>
    public partial class EmployeeTestimony : Window
    {
        public EmployeeTestimony()
        {
            EmployeeTestimonyViewModel.Errors = 0;
            InitializeComponent();
        }
        public EmployeeTestimony(EmployeeDTO employee)
        {
            EmployeeTestimonyViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeeTestimonyViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeeTestimonyViewModel.Errors -= 1;
        }
        private void LstTestimoniesAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LstTestimoniesAutoCompleteBox.SearchText = string.Empty;
        }

        private void wdwEmployeeTestimony_Loaded(object sender, RoutedEventArgs e)
        {
            TxtFullName.Focus();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            TxtFullName.Focus();
        }

        private void EmployeeTestimony_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeeTestimonyViewModel.CleanUp();
        }
    }
}
