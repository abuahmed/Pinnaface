using PinnaFace.Admin.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using System.Windows;
using System.Windows.Controls;

namespace PinnaFace.Admin.Views
{
    /// <summary>
    /// Interaction logic for UserWithAgencyWithAgentVIew.xaml
    /// </summary>
    public partial class UserWithAgencyWithAgentVIew : Window
    {
        public UserWithAgencyWithAgentVIew()
        {
            UserAgencyAgentViewModel.Errors = 0;
            InitializeComponent();
        }
        public UserWithAgencyWithAgentVIew(UserDTO userDto)
        {
            UserAgencyAgentViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<UserDTO>(userDto);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) UserAgencyAgentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) UserAgencyAgentViewModel.Errors -= 1;
        }
  
        private void UserWithAgencyWithAgent_OnUnloaded(object sender, RoutedEventArgs e)
        {
            UserAgencyAgentViewModel.CleanUp();
        }
    }
}
