using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmbassyProcess.xaml
    /// </summary>
    public partial class EmbassyProcess : Window
    {
        public EmbassyProcess()
        {
            EmbassyProcessViewModel.Errors = 0;
            InitializeComponent();
        }
        public EmbassyProcess(EmployeeDTO employee)
        {
            EmbassyProcessViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmbassyProcessViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmbassyProcessViewModel.Errors -= 1;
        }

        private void EmbassyProcess_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtEnjazNumber.Focus();
        }

        private void EmbassyProcess_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmbassyProcessViewModel.CleanUp();
        }
    }
}
