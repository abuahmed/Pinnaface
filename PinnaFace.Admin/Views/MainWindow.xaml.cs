using System.Windows;

namespace PinnaFace.Admin.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            new ChangePassword().ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

     
        private void ServerUsersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ServerUsers().Show();
        }
        private void BackupRestoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ServerSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ServerSettings().Show();
        }
        private void ServerAgenciesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ServerAgencys().Show();
        }
        private void ServerAgentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ServerAgents().Show();
        }
        private void ServerProductActivationsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new ServerProductActivations().Show();
        }
    }
}
