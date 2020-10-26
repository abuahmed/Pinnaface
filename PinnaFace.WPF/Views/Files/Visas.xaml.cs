using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.Models;
using PinnaFace.WPF.ViewModel;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Visas.xaml
    /// </summary>
    public partial class Visas : Window
    {
        public Visas()
        {
            //VisaViewModel.Errors = 0;
            InitializeComponent();
        }
        public Visas(EmployeeDTO employee)
        {
            VisaViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        public Visas(VisaModel employee)
        {
            VisaViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<VisaModel>(employee);
            Messenger.Reset();
        }
        //private void Validation_Error(object sender, ValidationErrorEventArgs e)
        //{
        //    if (e.Action == ValidationErrorEventAction.Added) VisaViewModel.Errors += 1;
        //    if (e.Action == ValidationErrorEventAction.Removed) VisaViewModel.Errors -= 1;
        //}
        //private void txtVisaNumber_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (TxtVisaNumber.Text.Length == 10)
        //    {
        //        Messenger.Default.Send<VisaDTO>(new VisaDTO { VisaNumber = TxtVisaNumber.Text });
        //    }
        //}
        //private void txtVisaDate_TextChanged(object sender, TextChangedEventArgs e)
        //{ 
        
        //}

        //private void LstItemsAutoCompleteBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    LstItemsAutoCompleteBox.SearchText = string.Empty;
        //}

        //private void LstItemsAutoCompleteBox_GotFocus_1(object sender, RoutedEventArgs e)
        //{
        //    LstItemsAutoCompleteBox.SearchText = string.Empty;
        //}

        //private void LstItemsAutoCompleteBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        //{
        //    LstItemsAutoCompleteBox.SearchText = string.Empty;
        //}

        //private void Visas_OnLoaded(object sender, RoutedEventArgs e)
        //{
        //    TxtVisaNumber.Focus();
        //}

        //private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        //{
        //    TxtVisaNumber.Focus();
        //}

        private void Visas_OnUnloaded(object sender, RoutedEventArgs e)
        {
            VisaViewModel.CleanUp();
        }
        
        private void VisasGridView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var originalSender = e.OriginalSource as FrameworkElement;
            if (originalSender != null)
            {
                var row = originalSender.ParentOfType<GridViewRow>();
                if (row != null)
                {
                    var visaId = ((VisaDTO)row.DataContext).Id;
                    new VisaDetail(new VisaModel{ VisaId = visaId }).ShowDialog();
                    //Messenger.Default.Send<VisaModel>(new VisaModel() { VisaId = visaId });
                }
                
            }
        }
    }
}
