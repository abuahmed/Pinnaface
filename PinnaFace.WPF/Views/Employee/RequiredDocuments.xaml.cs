using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;
using Telerik.Windows.Controls;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for RequiredDocuments.xaml
    /// </summary>
    public partial class RequiredDocuments : Window
    {
        //public RequiredDocuments()
        //{
        //    RequiredDocumentsViewModel.Errors = 0;
        //    InitializeComponent();
        //    CollapseCheckBoxes();
        //    ChkAgreement.Visibility = Visibility.Visible;
        //}
        public RequiredDocuments(EmployeeDTO employee)
        {
            RequiredDocumentsViewModel.Errors = 0;
            InitializeComponent();
            CollapseCheckBoxes();
            ChkAgreement.Visibility = Visibility.Visible;
            
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Reset();
        }
        

        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) RequiredDocumentsViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) RequiredDocumentsViewModel.Errors -= 1;
        }

        private void EmployeeApplication_OnUnloaded(object sender, RoutedEventArgs e)
        {
            RequiredDocumentsViewModel.CleanUp();
        }

        private void RadTabControlBase_OnSelectionChanged(object sender, RadSelectionChangedEventArgs e)
        {
            CollapseCheckBoxes();
            if(ChkAgreement==null)
                return;
            var tabItem = (RadTabItem) TabControlDocs.SelectedItem;
            switch (tabItem.Name)
            {
                case "TabAgreement":
                    ChkAgreement.Visibility = Visibility.Visible;
                    break;
                case "TabPassport":
                    ChkPassport.Visibility = Visibility.Visible;
                    break;
                case "TabIdCard":
                    ChkIdCard.Visibility = Visibility.Visible;
                    break;
                case "TabContactIdCard":
                    ChkContactIdCard.Visibility = Visibility.Visible;
                    break;
                case "TabFingerPrint":
                    ChkFingerPrint.Visibility = Visibility.Visible;
                    break;
                case "TabMedical":
                    ChkMedical.Visibility = Visibility.Visible;
                    break;
                case "TabPreDeparture":
                    ChkPreDeparture.Visibility = Visibility.Visible;
                    break;
                case "TabGradeEight":
                    ChkGradeEight.Visibility = Visibility.Visible;
                    break;
                case "TabCoc":
                    ChkCoc.Visibility = Visibility.Visible;
                    break;
                case "TabInsurance":
                    ChkInsurance.Visibility = Visibility.Visible;
                    break;
            }
        }
        public void CollapseCheckBoxes()
        {
            try
            {
                ChkAgreement.Visibility = Visibility.Hidden;
                ChkPassport.Visibility = Visibility.Hidden;
                ChkIdCard.Visibility = Visibility.Hidden;
                ChkContactIdCard.Visibility = Visibility.Hidden;
                ChkFingerPrint.Visibility = Visibility.Hidden;
                ChkMedical.Visibility = Visibility.Hidden;
                ChkPreDeparture.Visibility = Visibility.Hidden;
                ChkGradeEight.Visibility = Visibility.Hidden;
                ChkCoc.Visibility = Visibility.Hidden;
                ChkInsurance.Visibility = Visibility.Hidden;
            }
            catch
            {
            }
        }
        public void CollapseColorTabItems()
        {
            try
            {
                TabAgreement.Background = System.Windows.Media.Brushes.DarkRed;
                //var tbk = new TextBlock() { Text = "End manager"};
                //TabAgreement.Header = tbk;

                //TextBlock headerElement = new TextBlock();
                //headerElement.Text = "End manager";
                //headerElement.Background = System.Windows.Media.Brushes.Red;//Brushes.Red;
                //TabAgreement.Header = headerElement;

                TabPassport.Background = System.Windows.Media.Brushes.DarkRed;
                TabIdCard.Background = System.Windows.Media.Brushes.DarkRed;
                TabContactIdCard.Background = System.Windows.Media.Brushes.DarkRed;
                TabFingerPrint.Background = System.Windows.Media.Brushes.DarkRed;
                TabMedical.Background = System.Windows.Media.Brushes.DarkRed;
                TabPreDeparture.Background = System.Windows.Media.Brushes.DarkRed;
                TabGradeEight.Background = System.Windows.Media.Brushes.DarkRed;
                TabCoc.Background = System.Windows.Media.Brushes.DarkRed;
                TabInsurance.Background = System.Windows.Media.Brushes.DarkRed;
            }
            catch
            {
            }
        }
        private void ColorTabItems()
        {
            CollapseColorTabItems();
            try
            {
                if (ChkAgreement != null && ChkAgreement.IsChecked != null && (bool) ChkAgreement.IsChecked)
                    TabAgreement.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkPassport != null && ChkPassport.IsChecked != null && (bool)ChkPassport.IsChecked)
                    TabPassport.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkIdCard != null && ChkIdCard.IsChecked != null && (bool)ChkIdCard.IsChecked)
                    TabIdCard.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkContactIdCard != null && ChkContactIdCard.IsChecked != null && (bool)ChkContactIdCard.IsChecked)
                    TabContactIdCard.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkFingerPrint != null && ChkFingerPrint.IsChecked != null && (bool)ChkFingerPrint.IsChecked)
                    TabFingerPrint.Background = System.Windows.Media.Brushes.DarkGreen;

                if (ChkMedical != null && ChkMedical.IsChecked != null && (bool)ChkMedical.IsChecked)
                    TabMedical.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkPreDeparture != null && ChkPreDeparture.IsChecked != null && (bool)ChkPreDeparture.IsChecked)
                    TabPreDeparture.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkGradeEight != null && ChkGradeEight.IsChecked != null && (bool)ChkGradeEight.IsChecked)
                    TabGradeEight.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkCoc != null && ChkCoc.IsChecked != null && (bool)ChkCoc.IsChecked)
                    TabCoc.Background = System.Windows.Media.Brushes.DarkGreen;
                if (ChkInsurance != null && ChkInsurance.IsChecked != null && (bool)ChkInsurance.IsChecked)
                    TabInsurance.Background = System.Windows.Media.Brushes.DarkGreen; 
               
            }
            catch
            {
            }
        }

        private void RequiredDocuments_OnLoaded(object sender, RoutedEventArgs e)
        {
            ColorTabItems();
            TabControlDocs.SelectedIndex = 1;
        }

        private void ChkDocuments_OnChecked(object sender, RoutedEventArgs e)
        {
            var chkBox = (CheckBox) sender;
            if(this.IsLoaded)
            {
                //if(ChkPassport.IsChecked != null && (bool) ChkPassport.IsChecked)
                GrDocumentThumbnail.Visibility = Visibility.Visible;
                BtnAttachAgreement.Visibility = Visibility.Visible;
                BtnPrintDocumentCommand.Visibility = Visibility.Visible;
                ColorTabItems();
                //MessageBox.Show(chkBox.Name.ToString());
            }
            ////
        }

        private void ChkDocuments_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var chkBox = (CheckBox)sender;
            if (this.IsLoaded)
            {
                GrDocumentThumbnail.Visibility = Visibility.Hidden;
                BtnAttachAgreement.Visibility = Visibility.Hidden;
                BtnPrintDocumentCommand.Visibility = Visibility.Hidden;
                ColorTabItems();
                //MessageBox.Show(chkBox.Name.ToString());
            }
            ////
        }
    }
}
