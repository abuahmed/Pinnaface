using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for DiscontinueProcess.xaml
    /// </summary>
    public partial class DiscontinueProcess : Window
    {
        public DiscontinueProcess()
        {
            DiscontinueProcessViewModel.Errors = 0;
            InitializeComponent();
        }
        public DiscontinueProcess(EmployeeDTO employee)
        {
            DiscontinueProcessViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) DiscontinueProcessViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) DiscontinueProcessViewModel.Errors -= 1;
        }

        private void DiscontinueProcess_OnUnloaded(object sender, RoutedEventArgs e)
        {
            DiscontinueProcessViewModel.CleanUp();
        }
    }
}
