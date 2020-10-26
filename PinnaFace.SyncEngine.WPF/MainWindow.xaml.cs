using System;
using System.ComponentModel;
using System.Net;
using System.Timers;
using System.Windows;
using Microsoft.Win32;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service;

namespace PinnaFace.SyncEngine.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _monitorTimerDelay = 1800000; //30 minute
        private Timer _monitorTimer;

        private string _userName;
        private string _agencyName;
        //private bool _noConnection;

        public MainWindow()
        {
            InitializeComponent();

            #region Add PinnaSync To Registry for Running on StartUp
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            var val = rkApp.GetValue("PinnaSync");

            if (val == null)
            {
                const string publisherName = "PinnaSofts";
                const string suiteName = "PinnaSync";
                const string productName = "PinnaSync";

                var startPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Programs),
                    "\\", publisherName, "\\", suiteName, "\\", productName, ".appref-ms");
                rkApp.SetValue(productName, startPath);
            }

            #endregion

            try
            {
                //if (Singleton.BuildType == BuildType.Production)
                    if (Environment.MachineName.ToLower().Equals("pinnaserver"))
                        Start();
            }
            catch (Exception exception)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "MainViewModel-Environment.MachineName", exception.Message + exception.InnerException, "", "");
            }
        }

        public void Start()
        {
            Singleton.UseServerDateTime = true; //TO Handle Datetime.Now from serverornot
            Singleton.BuildType = BuildType.Production;

            /*Uncomment below when it is required to run Sync from PinnaFace.SyncEngine*/

            //For Server Edition
            Singleton.Edition = PinnaFaceEdition.ServerEdition;
            Singleton.SqlceFileName = "PinnaFaceDbProd";//"PinnaFaceDb4Prod"; //
            Singleton.PhotoStorage = PhotoStorage.Database;

            ////For Compact Edition
            //Singleton.Edition = PinnaFaceEdition.CompactEdition;
            //Singleton.SqlceFileName = PathUtil.GetDatabasePath();
            //Singleton.PhotoStorage = PhotoStorage.FileSystem;

            _userName = "Default User";
            try
            {
                _agencyName = new LocalAgencyService(true).GetLocalAgency().AgencyName;
            }
            catch
            {
                _agencyName = "Default Agency";
            }


            IUnitOfWork sourceUnitOfWork = new UnitOfWork(new DbContextFactory().Create());
            try
            {
                LogUtil.LogError(ErrorSeverity.Info, "Start", "Service is started at " + DateTime.Now, "", "");
                var setting = new SettingService(true).GetSetting();
                if (setting != null)// && setting.StartSync)
                {
                    _monitorTimerDelay = setting.SyncDuration * 60000;//60000; //1 minute is equivalent to 60000
                    Initialize();
                    _monitorTimer.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Start Sync",
                    ex.Message + Environment.NewLine + ex.InnerException, _userName, _agencyName);
            }
            finally
            {
                sourceUnitOfWork.Dispose();
            }
        }

        private void Initialize()
        {
            try
            {
                LogUtil.LogError(ErrorSeverity.Info, "Initialize", "Service is started at " + DateTime.Now, "", "");
                _monitorTimer = new Timer(_monitorTimerDelay);
                _monitorTimer.Elapsed += OnMonitorTimerElapsed;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Initialize Sync",
                    ex.Message + Environment.NewLine + ex.InnerException, _userName, _agencyName);
            }
        }

        private void OnMonitorTimerElapsed(object source, ElapsedEventArgs e)
        {
            try
            {
                //var setting = new SettingService(true).GetSetting();
                //if (setting != null && !setting.StartSync)
                //{
                //    return;
                //}

                LogUtil.LogError(ErrorSeverity.Info, "OnMonitorTimerElapsed", "Service is started at " + DateTime.Now, "", "");
                _monitorTimer.Enabled = false;

                var worker = new BackgroundWorker();
                
                worker.DoWork += Sync;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted2;
                worker.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "OnMonitorTimerElapsed", exception.Message + exception.InnerException, "", "");
            }

        }

        private void Sync(object sender, DoWorkEventArgs e)
        { 
            var syncTask = new Tasks.SyncTask();

            if (!syncTask.CheckInternetConnection())
            {
                LogUtil.LogError(ErrorSeverity.Critical, "CheckInternetConnection",
                    "No Internet Connection", _userName, _agencyName);
                //_noConnection = true;
                return;
            }

            try
            {
                syncTask.Sync();
               
            }
            catch (Exception exception)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Sync", exception.Message + exception.InnerException, "", "");
            }

            try
            {
                LogUtil.LogError(ErrorSeverity.Critical, "FileUploader Started", "", "", "");
                var fileUploader = new Common.FileUploader();
                fileUploader.UploadFiles();
                LogUtil.LogError(ErrorSeverity.Critical, "FileUploader Completed", "", "", "");
                
            }
            catch (Exception ex)
            {

                LogUtil.LogError(ErrorSeverity.Critical, "FileUploader.UploadFiles problem",
                        ex.Message + Environment.NewLine + ex.InnerException, "", "");
            }
        }

        private void worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            _monitorTimer.Enabled = true;
            //if (_noConnection)
            //    TxtUpdatingText.Text = "There is no Internet connection...";
            //else
            //{
            //    TxtUpdatingText.Text = "Syncing Completed";
            //    //UpdatingText = _updatesFound
            //    //    ? "There exists new updates, refresh to see the updates"
            //    //    : "No updates found...";
            //}
            ////else if (_refreshed)
            ////{
            ////    UpdatingText = _noConnection ? "There is no Internet connection..." : "No updates found...";
            ////}
        }
        
    }
}
