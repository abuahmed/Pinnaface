using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeePhoto.xaml
    /// </summary>
    public partial class EmployeePhoto : Window
    {
        public EmployeePhoto()
        {
            EmployeePhotoViewModel.Errors = 0;
            InitializeComponent();
        }
        public EmployeePhoto(EmployeeDTO employee)
        {
            EmployeePhotoViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeePhotoViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeePhotoViewModel.Errors -= 1;
        }

        private void EmployeePhoto_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeePhotoViewModel.CleanUp();
        }
    }
}
