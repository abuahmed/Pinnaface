using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Enumerations;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Lists.xaml
    /// </summary>
    public partial class Lists : Window
    {
        public Lists()
        {
            ListViewModel.Errors = 0;
            InitializeComponent();
        }
        public Lists(ListTypes listType)
        {
            ListViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<ListTypes>(listType);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) ListViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) ListViewModel.Errors -= 1;
        }

        private void Lists_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtDisplayName.Focus();
        }

        private void Lists_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ListViewModel.CleanUp();
        }

        private void ListsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ////var originalSender = e.OriginalSource as FrameworkElement;
            ////if (originalSender != null)
            ////{
            ////    var row = originalSender.ParentOfType<ListViewItem>();
            ////    if (row != null)
            ////    {
            ////        TxtDisplayName.Text = (((ListDTO) row.DataContext).DisplayName).ToString();
            ////    }
            ////}
            WdwLists.DialogResult = true;
            WdwLists.Close();
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtDisplayName.Focus();
        }
    }
}
