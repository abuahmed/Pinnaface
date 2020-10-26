using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using PinnaFace.WPF.ViewModel;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Agency.xaml
    /// </summary>
    public partial class LocalAgency : Window
    {
        InputLanguage _amharicInput;
        InputLanguage _englishInput;
        public LocalAgency()
        {
            LocalAgencyViewModel.Errors = 0;
            InitializeComponent();            
        }
        private void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added) LocalAgencyViewModel.Errors += 1;
            if (e.Action == ValidationErrorEventAction.Removed) LocalAgencyViewModel.Errors -= 1;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnLetterHeader_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void wdwLocalAgency_Loaded(object sender, RoutedEventArgs e)
        {
            _amharicInput = InputLanguage.CurrentInputLanguage;
            _englishInput = InputLanguage.CurrentInputLanguage;
            var count = InputLanguage.InstalledInputLanguages.Count;
            foreach (InputLanguage inLang in InputLanguage.InstalledInputLanguages) 
            {
                if (inLang.LayoutName.Contains("Amharic"))
                    _amharicInput = inLang;
                if (inLang.LayoutName.Contains("English"))
                    _englishInput = inLang;
            }

            TxtLocalAgencyNameAmharic.Focus();
        }

        private void txtLocalAgencyNameAmharic_KeyUp(object sender, KeyEventArgs e)
        {
            InputLanguage.CurrentInputLanguage = _amharicInput;
        }

        private void txtLocalAgencyNameAmharic_KeyDown(object sender, KeyEventArgs e)
        {
            InputLanguage.CurrentInputLanguage = _amharicInput;
        }

        private void LocalAgency_OnUnloaded(object sender, RoutedEventArgs e)
        {
            LocalAgencyViewModel.CleanUp();
        }
    }
}
