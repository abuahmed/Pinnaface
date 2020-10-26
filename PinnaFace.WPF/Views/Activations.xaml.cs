using System.Windows;
using System.Windows.Controls;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for Activations.xaml
    /// </summary>
    public partial class Activations : Window
    {
        public Activations()
        {
            InitializeComponent();
        }

        int ProductKey_Length = 23, ProductKey_Split = 5;
        bool cont;

        private void ProductKeyValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ProductKeyValue.Text.Length >= ProductKey_Length - 1)
                return;

            if (ProductKeyValue.Text.Length > 0)
                if (ProductKeyValue.Text.Replace("-", "").Length % ProductKey_Split == 0 && !cont)
                {
                    cont = true;
                    ProductKeyValue.Text = ProductKeyValue.Text.Insert(ProductKeyValue.Text.Length, "-");
                }
                else
                {
                    cont = false;
                }

            ProductKeyValue.Select(ProductKeyValue.Text.Length, 1);
        }

        private void Activations_OnUnloaded(object sender, RoutedEventArgs e)
        {
            ActivationViewModel.CleanUp();
        }
    }
}
