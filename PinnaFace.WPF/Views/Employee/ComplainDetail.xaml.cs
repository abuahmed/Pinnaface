using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for ComplainDetail.xaml
    /// </summary>
    public partial class ComplainDetail : Window
    {
        public ComplainDetail()
        {
            ComplainDetailViewModel.Errors = 0;
            ComplainDetailViewModel.RemarkErrors = 0;
            InitializeComponent();
        }
        public ComplainDetail(EmployeeDTO employee)
        {
            ComplainDetailViewModel.Errors = 0;
            ComplainDetailViewModel.RemarkErrors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ComplainDetailViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ComplainDetailViewModel.Errors -= 1;
        }
        private void Validation_RemarkError(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ComplainDetailViewModel.RemarkErrors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ComplainDetailViewModel.RemarkErrors -= 1;
        }
        private void ComplainDetail_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ComplainDetailViewModel.CleanUp();
        }
    }
}
