using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Navigation;
using CefSharp;
using CefSharp.Wpf;
using GalaSoft.MvvmLight.Messaging;
using mshtml;
using PinnaFace.Core.Models;
using PinnaFace.WPF.ViewModel;
using Telerik.Windows.Controls;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace PinnaFace.WPF.Views
{
    /// <summary>
    /// Interaction logic for EnjazitBrowser.xaml
    /// </summary>
    public partial class EnjazitBrowser : Window
    {
        private System.Windows.Controls.ProgressBar pbar;
        public EnjazitBrowser(BrowserTarget target)
        {
            CefSettings settings = new CefSettings();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CEF";
            settings.RemoteDebuggingPort = 8080;
            settings.CachePath = path;

            //Initialize Cef with the provided settings
            if (!Cef.IsInitialized)
            Cef.Initialize(settings);

            InitializeComponent();
            Messenger.Default.Send<BrowserTarget>((BrowserTarget)target);
            Messenger.Reset();
            UrlAndButtonsVisibility(target);

            

        }
        public EnjazitBrowser(EmployeeDTO employee, BrowserTarget target)
        {
            CefSettings settings = new CefSettings();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\CEF";
            settings.RemoteDebuggingPort = 8080;
            settings.CachePath = path;

            //Initialize Cef with the provided settings
            if(!Cef.IsInitialized)
            Cef.Initialize(settings);

            InitializeComponent();
            Messenger.Default.Send<EmployeeDTO>(employee);
            Messenger.Default.Send<BrowserTarget>(target);
            Messenger.Reset();
            UrlAndButtonsVisibility(target);
        }

        public void UrlAndButtonsVisibility(BrowserTarget target)
        {
            //pbar = ProgressBar;
          
            if (target.Equals(BrowserTarget.Enjazit))
            {
                TxtenjazUrl.Text = "https://enjazit.com.sa/Account/Login/Company";//"";
                    //"file:///C:/Users/user/Desktop/Offline%20Pages/Enjaz2/Visa%20Services%20Platform%20(Enjaz).htm";
                BtnFillEnjaz.Visibility = Visibility.Visible;
                BtnEnjazDocuments.Visibility = Visibility.Visible;
                BtnFillMusaned.Visibility = Visibility.Collapsed;
                BtnImportVisa.Visibility = Visibility.Collapsed;
                BtnFillInsurance.Visibility = Visibility.Collapsed;
                LstItemsAutoCompleteBox.Visibility = Visibility.Visible;
                BtnReload.Visibility = Visibility.Visible;
                LstEnjazitEmployeeDetail.Visibility = Visibility.Visible;
            }
            else if (target.Equals(BrowserTarget.Musaned))
            {
                TxtenjazUrl.Text = "https://et.musaned.com.sa";// "file:///C:/Users/user/Desktop/Offline%20Pages/MusanedShowCandidate.html";//    "file:///C:/Users/user/Desktop/Offline%20Pages/MusanedContract.html";// 
                //"file:///C:/Users/user/Desktop/Offline%20Pages/MusanedEntry.html";
                BtnFillEnjaz.Visibility = Visibility.Collapsed;
                BtnEnjazDocuments.Visibility = Visibility.Collapsed;
                BtnFillMusaned.Visibility = Visibility.Visible;
                BtnImportVisa.Visibility = Visibility.Visible;
                BtnFillInsurance.Visibility = Visibility.Collapsed;
                LstItemsAutoCompleteBox.Visibility = Visibility.Visible;
                BtnReload.Visibility = Visibility.Visible;
                LstEnjazitEmployeeDetail.Visibility = Visibility.Visible;
            }
            else if (target.Equals(BrowserTarget.UnitedInsurance))
            {
                TxtenjazUrl.Text = "http://glp.unicportal.com.et:96/";
                    //"file:///C:/Users/user/Desktop/Offline%20Pages/United%20Insurance%20Company%20SC.html"; 
                BtnFillEnjaz.Visibility = Visibility.Collapsed;
                BtnEnjazDocuments.Visibility = Visibility.Collapsed;
                BtnFillMusaned.Visibility = Visibility.Collapsed;
                BtnImportVisa.Visibility = Visibility.Collapsed;
                BtnFillInsurance.Visibility = Visibility.Visible;
                LstItemsAutoCompleteBox.Visibility = Visibility.Visible;
                BtnReload.Visibility = Visibility.Visible;
                LstEnjazitEmployeeDetail.Visibility = Visibility.Visible;
            }
            else if (target.Equals(BrowserTarget.PinnaFace))
            {
                TxtenjazUrl.Text = "http://www.pinnaface.com/Employee/Thumbnail";
                BtnFillEnjaz.Visibility = Visibility.Collapsed;
                BtnEnjazDocuments.Visibility = Visibility.Collapsed;
                BtnFillMusaned.Visibility = Visibility.Collapsed;
                BtnImportVisa.Visibility = Visibility.Collapsed;
                BtnFillInsurance.Visibility = Visibility.Collapsed;
                LstItemsAutoCompleteBox.Visibility = Visibility.Collapsed;
                BtnReload.Visibility = Visibility.Collapsed;
                LstEnjazitEmployeeDetail.Visibility = Visibility.Collapsed;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenURLInBrowser(TxtenjazUrl.Text);
        }

        void Browser_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            var browser = sender as WebBrowser;

            if (browser == null || browser.Document == null)
                return;

            dynamic document = browser.Document;

            if (document.readyState != "complete")
                return;

            dynamic script = document.createElement("script");
            script.type = @"text/javascript";
            script.text = @"window.onerror = function(msg,url,line){return true;}";
            document.head.appendChild(script);
        }

        private void OpenURLInBrowser(string url)
        {
            //if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            //{
            //    url = "http://" + url;
            //}
            try
            {
                BrwEnjaz.Address = url;
                //BrwEnjaz.Navigate(new Uri(url));

            }
            catch (UriFormatException)
            {
                
            }
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button) sender;
            var btnName = btn.Name;

            if (btnName.Equals("BtnEnjazit"))
            {
              UrlAndButtonsVisibility(BrowserTarget.Enjazit);

            }
            else if (btnName.Equals("BtnMusaned"))
            {
                UrlAndButtonsVisibility(BrowserTarget.Musaned);
            }
            else if (btnName.Equals("BtnInsurance"))
            {
                UrlAndButtonsVisibility(BrowserTarget.UnitedInsurance);
            }
            else if (btnName.Equals("BtnPinnaFace"))
            {
                UrlAndButtonsVisibility(BrowserTarget.PinnaFace);
            }


            if (String.IsNullOrEmpty(TxtenjazUrl.Text) || TxtenjazUrl.Text.Equals("about:blank"))
            {
                MessageBox.Show("Enter a valid URL.");
                TxtenjazUrl.Focus();
                return;
            }
            OpenURLInBrowser(TxtenjazUrl.Text);
        }


        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (BrwEnjaz.CanGoBack)
                BrwEnjaz.Back();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (BrwEnjaz.CanGoForward)
                BrwEnjaz.Forward();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //brwEnjaz.ShowPrintDialog();
            //var doc = BrwEnjaz.Document as IHTMLDocument2;
            //doc.execCommand("Print", true, null);
        }

        int wid = 100;
        

        private void btnZOut_Click(object sender, RoutedEventArgs e)
        {
            wid = wid - 2;
            ZoomWebBrowser(wid);
        }

        private void btnZIn_Click(object sender, RoutedEventArgs e)
        {
            wid = wid + 2;
            ZoomWebBrowser(wid);
        }

        private void ZoomWebBrowser(int factor)
        {
           // SetZoom(BrwEnjaz, factor / 100);
        }

        public static void SetZoom(WebBrowser webBrowser1, double Zoom)
        {
            try
            {
                //HtmlDocument doc =(HtmlDocument) webBrowser1.Document;

                var doc = webBrowser1.Document as IHTMLDocument2;
                var tags = (IHTMLElementCollection)doc.all.tags("div");
                foreach (IHTMLElement el in tags)
                {
                    string controlName2 = el.getAttribute("tabindex").ToString();
                    if (controlName2 == "17")
                    {
                        //IHTMLElementCollection els = el.children.item;
                        IHTMLElement ell = el.children.item(null, 0);

                        //IHTMLElementCollection els2 = ell.children;
                        IHTMLElement elll = ell.children.item(null, 0);

                        //IHTMLElement elll = ell.children;
                        elll.innerText = "Ethiopia";

                        //IHTMLElementCollection els3 = el.children;
                        IHTMLElement ellll = el.children.item(null, 1);
                        ellll.setAttribute("value", "ETH");
                    }
                }

                //doc.parentWindow.execScript("docoment.body.style.zoom=" + Zoom.ToString().Replace(",", ".") + ";");
            }
            catch { }
        }

        private void brwEnjaz_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            wid = 100;
            ZoomWebBrowser(wid);
        }


        private void EnjazitBrowser_OnUnloaded(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Cleanup");
            //Cef.Shutdown();
            EnjazitBrowserViewModel.CleanUp();
        }

        private void LstItemsAutoCompleteBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LstItemsAutoCompleteBox.SearchText = string.Empty;
            LstItemsAutoCompleteBox.Focus();//
        }
        
        private void BrwEnjaz_OnNavigated(object sender, NavigationEventArgs e)
        {
            WdwEnjazit.Title = e.Uri.AbsoluteUri;
        }

        private void ChromWebBrowser_OnFrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                TxtenjazUrl.Text = e.Url;
                WdwEnjazit.Title = BrwEnjaz.Title??"Error";
                //BtnBackButton.IsEnabled = Browser.CanGoBack;
                BtnGo.IsEnabled = !string.IsNullOrWhiteSpace(TxtenjazUrl.Text);
                //BtnNextButton.IsEnabled = Browser.CanGoForward;
            }));
        }

        private void BrwEnjaz_OnFrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
           
        }

        private Stopwatch _exitStopWatch;
        public bool IsBusy { get; set; }
        private void BrwEnjaz_OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            //if (!e.IsLoading)
            //{
            //    this.Dispatcher.Invoke(() =>
            //    { //Invoke UI Thread
            //        ProgressBar.Visibility = Visibility.Collapsed;
            //    });
            //}
            //else
            //{
            //    this.Dispatcher.Invoke(() =>
            //    { //Invoke UI Thread
            //        ProgressBar.Visibility = Visibility.Visible;
            //    });
            //}

            if (e.IsLoading)
            {
                this.Dispatcher.Invoke(() =>
                { //Invoke UI Thread
                    ProgressBar.Visibility = Visibility.Visible;
                });

                IsBusy = true;
                if (_exitStopWatch == null)
                {
                    _exitStopWatch = new Stopwatch();
                }
                if (!_exitStopWatch.IsRunning)
                {
                    _exitStopWatch.Start();
                }
                while (_exitStopWatch.IsRunning && _exitStopWatch.Elapsed < TimeSpan.FromSeconds(3))
                {

                }
                if (_exitStopWatch.IsRunning)
                {
                    _exitStopWatch.Stop();
                }
                IsBusy = false;
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                { //Invoke UI Thread
                    ProgressBar.Visibility = Visibility.Collapsed;
                });

                if (_exitStopWatch != null)
                {
                    if (_exitStopWatch.IsRunning)
                    {
                        _exitStopWatch.Stop();
                    }
                }
                IsBusy = false;
            }
        }

        

        private void BrwEnjaz_OnLoadError(object sender, LoadErrorEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                //MessageBox.Show(e.ErrorText);//, e.ErrorCode);
            });
        }
    }
}
