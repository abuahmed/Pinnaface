using System;
using System.Linq;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.WPF.Models;
using PinnaFace.WPF.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for VisaDetail.xaml
    /// </summary>
    public partial class VisaDetail : Window
    {
        //public VisaDetail()
        //{
        //    VisaDetailViewModel.Errors = 0;
        //    InitializeComponent();
        //}
        //public VisaDetail(EmployeeDTO employee)
        //{
        //    VisaDetailViewModel.Errors = 0;
        //    InitializeComponent();
        //    Messenger.Default.Send<EmployeeDTO>(employee);
        //    Messenger.Reset();
        //}
        public VisaDetail(VisaModel visa)
        {
            VisaDetailViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<VisaModel>(visa);
            Messenger.Reset();
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) VisaDetailViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) VisaDetailViewModel.Errors -= 1;
        }
        private void txtVisaNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (checkDuplicate)
                if (TxtVisaNumber.Text.Length == 10)
                {
                    var visaNumber = TxtVisaNumber.Text;
                    var visaId = Convert.ToInt32(TxtId.Text);
                    var visaserv = new VisaService(true, false);
                    var cri = new SearchCriteria<VisaDTO>();
                    cri.FiList.Add(f => f.VisaNumber == visaNumber && f.Id != visaId);
                    var visa = visaserv.GetAll(cri).FirstOrDefault();
                    if (visa != null)
                    {
                        MessageBox.Show("There Exists Visa with the same Visa Number: " + visaNumber, "Data Exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        TxtVisaNumber.Text = "";
                    }
                    //Messenger.Default.Send<EmployeeDuplicateCheck>(new EmployeeDuplicateCheck { PassportNumber = TxtPassportNum.Text });
                }

            //if (TxtVisaNumber.Text.Length == 10)
            //{
            //    Messenger.Default.Send<VisaModel>(new VisaModel { VisaNumber = TxtVisaNumber.Text });
            //}
        }
        private void txtVisaDate_TextChanged(object sender, TextChangedEventArgs e)
        { 
        
        }

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

        private bool checkDuplicate;
        private void VisaDetail_OnLoaded(object sender, RoutedEventArgs e)
        {
            TxtVisaNumber.Focus();
            checkDuplicate = true;
        }

        private void BtnAddNew_OnClick(object sender, RoutedEventArgs e)
        {
            TxtVisaNumber.Focus();
            
        }

        private void VisaDetail_OnUnloaded(object sender, RoutedEventArgs e)
        {
            VisaDetailViewModel.CleanUp();
        }
    }
}
