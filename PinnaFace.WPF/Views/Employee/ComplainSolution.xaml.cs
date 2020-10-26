using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for ComplainSolution.xaml
    /// </summary>
    public partial class ComplainSolution : Window
    {
        public ComplainSolution()
        {
            ComplainSolutionViewModel.Errors = 0;
            InitializeComponent();
        }
        public ComplainSolution(ComplainDTO complain)
        {
            ComplainSolutionViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<ComplainDTO>(complain);
            Messenger.Reset();
        }
        public ComplainSolution(EmployeeDTO employee)
        {
            ComplainSolutionViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ComplainSolutionViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ComplainSolutionViewModel.Errors -= 1;
        }
     
        private void ComplainSolution_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ComplainSolutionViewModel.CleanUp();
        }

        private void ComplainSolution_OnClosing(object sender, CancelEventArgs e)
        {
            
        }
    }
}
