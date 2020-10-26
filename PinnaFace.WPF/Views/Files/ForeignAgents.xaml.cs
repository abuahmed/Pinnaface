using System.Windows;
using System.Windows.Controls;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for ForeignAgents.xaml
    /// </summary>
    public partial class ForeignAgents : Window
    {
        public ForeignAgents()
        {
            ForeignAgentViewModel.Errors = 0;
            InitializeComponent();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ForeignAgentViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ForeignAgentViewModel.Errors -= 1;
        }

        private void ForeignAgents_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtForeignAgentCode.Focus();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtForeignAgentCode.Focus();
        }

        private void ForeignAgents_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ForeignAgentViewModel.CleanUp();
        }
    }
}
