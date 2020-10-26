using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EmployeeDetail.xaml
    /// </summary>
    public partial class EmployeeDetail : Window
    {
        public EmployeeDetail()
        {
            EmployeeDetailViewModel.Errors = 0;
            InitializeComponent();
        }

        public EmployeeDetail(EmployeeDTO employee)
        {
            EmployeeDetailViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }

        public EmployeeDetail(Int32 employeeId)
        {
            EmployeeDetailViewModel.Errors = 0;
            InitializeComponent();
            Messenger.Default.Send<Int32>(employeeId);
            Messenger.Reset();
        }

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) EmployeeDetailViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) EmployeeDetailViewModel.Errors -= 1;
        }

        private void txtPassportNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_checkDuplicate)
                if (TxtPassportNum.Text.Length == 9)
                {
                    var passportNum = TxtPassportNum.Text;
                    var employeeId = Convert.ToInt32(TxtId.Text);
                    var empserv = new EmployeeService(true, false);
                    var cri = new SearchCriteria<EmployeeDTO>();
                    cri.FiList.Add(f => f.PassportNumber == passportNum && f.Id != employeeId);
                    var emp = empserv.GetAll(cri).FirstOrDefault();
                    if (emp != null)
                    {
                        MessageBox.Show("There Exists Employee with the same Passport Number: " + passportNum,
                            "Data Exists", MessageBoxButton.OK, MessageBoxImage.Error);
                        TxtPassportNum.Text = "";
                    }
                    //Messenger.Default.Send<EmployeeDuplicateCheck>(new EmployeeDuplicateCheck { PassportNumber = TxtPassportNum.Text });
                }
        }

        //private void txtCodeNumber_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    //Messenger.Default.Send<EmployeeDTO>(new EmployeeDTO { CodeNumber = TxtCodeNumber.Text });
        //}

        private void dtBirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DtBirthDate.SelectedDate == null) return;
                double age = DateTime.Now.Subtract(DtBirthDate.SelectedDate.Value).Days;
                age = age/365.25;
                try
                {
                    LblAge.Text = age.ToString().Substring(0, 4); //" Age :" +
                }
                catch
                {
                    LblAge.Text = age.ToString(); //" Age :" +
                }

                //if (age < 21)
                //MessageBox.Show("Age is below 21...", "UnderAge", MessageBoxButton.OK, MessageBoxImage.Error);
                if (age < 21)
                {
                   LblAge.Foreground = Brushes.Red;// age < 21 ?  : Brushes.Green;
                   LblAge.Text = "እድሜ: " + LblAge.Text + " አመት "; //Years Old 
                }
                else
                {
                    LblAge.Text = "";
                }
            }
            catch
            {
            }
        }

        private void dtIssueDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DtIssueDate.SelectedDate != null)
                {
                    ////var isDate = DtIssueDate.SelectedDate.Value;
                    ////var expDate = isDate.AddDays(-1);

                    ////var day = expDate.Day;
                    ////var mon = expDate.Month;
                    ////var yer = expDate.Year + 5;
                    ////DtExpiryDate.SelectedDate = new DateTime(yer, mon, day);
                    //var month = DtIssueDate.SelectedDate.Value.Month;
                    //if (month > 1 && month < 9)
                    //{
                        DtExpiryDate.SelectedDate = DtIssueDate.SelectedDate.Value.AddYears(5).AddDays(-1);
                    //}
                    //else
                    //{
                    //    DtExpiryDate.SelectedDate = DtIssueDate.SelectedDate.Value.AddYears(5).AddDays(-2);
                    //}
                }
            }
            catch
            {

            }
        }

        private void dtExpiryDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DtExpiryDate.SelectedDate == null) return;
                var age = DtExpiryDate.SelectedDate.Value.Year - DateTime.Now.Year;
                var month = DtExpiryDate.SelectedDate.Value.Month;
                var monthnow = DateTime.Now.Month;

                int leftmonths;
                switch (age)
                {
                    case 0:
                        leftmonths = month - monthnow;
                        break;
                    case 1:
                        leftmonths = month + (12 - monthnow);
                        break;
                    default:
                        leftmonths = month + (12 - monthnow) + ((age - 1)*12);
                        break;
                }

                LblMonthsLeft.Text = leftmonths.ToString(); // +" Months Left";
                if (leftmonths < 6)
                {
                    LblMonthsLeft.Foreground = Brushes.Red;//leftmonths < 6 ? Brushes.Red : Brushes.Green;
                    LblMonthsLeft.Text = "ፓስፖርቱ የቀረው: " + leftmonths.ToString() + " ወራት";
                }
                else
                {
                    LblMonthsLeft.Text = "";
                }
                // " Months Left To Expire";
            }
            catch
            {
            }
        }

        private void EmployeeDetail_OnUnloaded(object sender, RoutedEventArgs e)
        {
            EmployeeDetailViewModel.CleanUp();
        }

        private bool _checkDuplicate;

        private void WdwEmployeeDetail_Loaded(object sender, RoutedEventArgs e)
        {
            TxtFirstName.Focus();
            _checkDuplicate = true;
        }

        private void ImgbtnEmployeeShortImage_MouseLeftButtonDown(object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            //MessageBox.Show("Photo Clicked");
        }

        private void dtDocumentReceivedDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //try
            //{
            ////    if (DtDocumentReceivedDate.SelectedDate == null) return;

            ////    var text = TxtCurrentStatus.Text;
            ////    //MessageBox.Show(tag);
            ////    var daysTook = DateTime.Now.Subtract(DtDocumentReceivedDate.SelectedDate.Value).Days;
                   
            ////    if (daysTook > 45)
            ////    {
            ////        LblDaysTook.Foreground = Brushes.Red;//leftmonths < 6 ? Brushes.Red : Brushes.Green;
            ////    }
            ////    else
            ////    {
            ////        LblDaysTook.Foreground = Brushes.Green;
            ////    }
            ////    LblDaysTook.Text = daysTook.ToString() + " Days Ago";
                
            //}
            //catch
            //{
            //}
        }
    }
}