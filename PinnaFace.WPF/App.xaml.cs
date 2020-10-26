using System;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using SplashScreen = PinnaFace.WPF.Views.SplashScreen;

namespace PinnaFace.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int MINIMUM_SPLASH_TIME = 5500; // Miliseconds
        private const int SPLASH_FADE_TIME = 500;     // Miliseconds
        public bool DoHandle { get; set; }

        
        public App()
        {
            //System.Web.Security.Roles.Enabled = true;
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
            StyleManager.ApplicationTheme = new TransparentTheme();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            WebBrowserHelper.FixBrowserVersion();

            var splashScreen = new SplashScreen();
            splashScreen.Show();

            //PinnaFace.WPF.ViewModel.SplashScreenViewModel splashViewModel = );
            //splashScreen.IsLoaded
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (DoHandle)
            {
                MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
            }
            else
            {
                MessageBox.Show("Application is going to close! ", "Uncaght Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = false;
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            MessageBox.Show(ex.Message, "Uncaught Thread Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public class EntryPoint
    {

        [STAThread]
        public static void Main(string[] args)
        {
            //if (args != null && args.Length > 0)
            //{
            //    var aa = args[0];
             
            //    MessageBox.Show(aa.ToString());
            //}
            try
            {
                var aa = Environment.GetCommandLineArgs();
                MessageBox.Show(aa[1].ToString());
            }
            catch (Exception)
            { }
            
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
