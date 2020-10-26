using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for VisaCondition.xaml
    /// </summary>
    public partial class VisaCondition : Window
    {
        public VisaCondition()
        {
            VisaViewModel.Errors = 0;
            InitializeComponent();
        }
        public VisaCondition(VisaConditionDTO condition)
        {
            VisaViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<VisaConditionDTO>(condition);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) VisaViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) VisaViewModel.Errors -= 1;
        }

        private void VisaCondition_OnUnloaded(object sender, RoutedEventArgs e)
        {
            VisaViewModel.CleanUp();
        }
    }
}
