using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Service;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region Fields

        private static readonly EmployeeViewModel EmployeeViewModel = new ViewModelLocator().Employee;
        private static readonly ReportsViewModel ReportsViewModel = new ViewModelLocator().Reports;
        private ICommand _backupRestoreCommand;
        private ViewModelBase _currentViewModel;
        private ICommand _employeeViewCommand;
        private string _headerText, _headerTitle; //, _databaseVersion;
        private ICommand _reportsViewCommand;

        //private int _monitorTimerDelay = 1800000; //30 minute
        //private Timer _monitorTimer;

        //private string _userName;
        //private string _agencyName;

        #endregion

        #region Constructor

        public MainViewModel()
        {
            EmployeeViewModel.LoadData = true;
            CurrentViewModel = EmployeeViewModel;

            //    HeaderText = "PinnaFace - (" + Singleton.ProductActivation.LicensedTo + ") " +
            //    Singleton.User.UserName + " - " +
            //    DateTime.Now.ToString("dd/MM/yyyy") + " - " +
            //    CalendarUtil.GetEthCalendarFormated(DateTime.Now, "/") + " - " +
            //    new ProductActivationDTO().BiosSn;

            HeaderTitle = "PinnaFace, Overseas Employment Management System (" +
                          Singleton.User.UserName + ") - " +
                          DateTime.Now.ToString("dd/MM/yyyy") + " (" +
                          CalendarUtil.GetEthCalendarFormated(DateTime.Now, "-") + ") - " +
                          new ProductActivationDTO().BiosSn;

            string header = Singleton.ProductActivation.LicensedTo;
            HeaderText = header.Length > 40
                ? Singleton.ProductActivation.LicensedTo.Substring(0, 38) + "..."
                : Singleton.ProductActivation.LicensedTo; // "PinnaFace - Employees List";

            CheckRoles();

            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();

            //try
            //{
            //    if(Singleton.BuildType==BuildType.Production)
            //    if (Environment.MachineName.ToLower().Equals("pinnaserver")) 
            //        Start();

            //}
            //catch (Exception exception)
            //{
            //    LogUtil.LogError(ErrorSeverity.Critical, "MainViewModel-Environment.MachineName", exception.Message + exception.InnerException, "", "");
            //}
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            #region Update Product Activation
            try
            {
                var unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

                ProductActivationDTO productActivation = unitOfWork.Repository<ProductActivationDTO>()
                    .Query().Get()
                    .FirstOrDefault();

                if (productActivation != null)
                {
                    if (productActivation.DatabaseVersionDate < Singleton.SystemVersionDate)
                    {
                        productActivation.DatabaseVersionDate = Singleton.SystemVersionDate;
                        unitOfWork.Repository<ProductActivationDTO>().Update(productActivation);
                        unitOfWork.Commit();
                    }
                    //DatabaseVersion = productActivation.DatabaseVersionDate.ToString();
                }
                unitOfWork.Dispose();
            }
            catch (Exception exception)
            {
                LogUtil.LogError(ErrorSeverity.Fatal,
                    "MainViewModel.Set productActivation.DatabaseVersionDate to higher",
                    exception.Message + Environment.NewLine + exception.InnerException, "", "");
            } 
            #endregion

            //#region Update Addresses
            //var localAgency = new LocalAgencyService(true).GetLocalAgency();

            //try
            //{
            //    var unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

            //    var addresses = unitOfWork.Repository<AddressDTO>()
            //        .Query().Filter(a => a.AgencyId == null).Get().ToList();
            //    if (addresses.Count > 0)
            //    {
            //        foreach (var addressDTO in addresses)
            //        {
            //            addressDTO.AgencyId = localAgency.Id;
            //            unitOfWork.Repository<AddressDTO>().Update(addressDTO);
            //        }
            //        unitOfWork.Commit();
            //    }


            //    unitOfWork.Dispose();
            //}
            //catch (Exception exception)
            //{
            //    LogUtil.LogError(ErrorSeverity.Fatal,
            //        "MainViewModel.Set productActivation.DatabaseVersionDate to higher",
            //        exception.Message + Environment.NewLine + exception.InnerException, "", "");
            //} 
            //#endregion

            //#region Update Attachments
            //try
            //{
            //    var unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());


            //    var addresses = unitOfWork.Repository<AttachmentDTO>()
            //        .Query().Filter(a => a.AgencyId == null).Get().ToList();

            //    if (addresses.Count > 0)
            //    {
            //        foreach (var addressDTO in addresses)
            //        {
            //            addressDTO.AgencyId = localAgency.Id;
            //            unitOfWork.Repository<AttachmentDTO>().Update(addressDTO);
            //        }
            //        unitOfWork.Commit();
            //    }
            //    unitOfWork.Dispose();
            //}
            //catch (Exception exception)
            //{
            //    LogUtil.LogError(ErrorSeverity.Fatal,
            //        "MainViewModel.Set productActivation.DatabaseVersionDate to higher",
            //        exception.Message + Environment.NewLine + exception.InnerException, "", "");
            //} 
            //#endregion
            
            //try
            //{
            //    LogUtil.LogError(ErrorSeverity.Critical, "FileUploader Started", "", "", "");
            //    var fileUploader = new FileUploader();
            //    fileUploader.UploadFiles();
            //    LogUtil.LogError(ErrorSeverity.Critical, "FileUploader Completed", "", "", "");
            //}
            //catch (Exception ex)
            //{

            //    LogUtil.LogError(ErrorSeverity.Critical, "FileUploader.UploadFiles problem",
            //            ex.Message + Environment.NewLine + ex.InnerException, "", "");
            //}
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        #endregion

        #region Public Properties

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                if (_headerText == value)
                    return;
                _headerText = value;
                RaisePropertyChanged("HeaderText");
            }
        }

        ////Value Can be found n AboutBox
        //public string DatabaseVersion
        //{
        //    get { return _databaseVersion; }
        //    set
        //    {
        //        if (_databaseVersion == value)
        //            return;
        //        _databaseVersion = value;
        //        RaisePropertyChanged("DatabaseVersion");
        //    }
        //}
        public string HeaderTitle
        {
            get { return _headerTitle; }
            set
            {
                if (_headerTitle == value)
                    return;
                _headerTitle = value;
                RaisePropertyChanged("HeaderTitle");
            }
        }

        #endregion

        #region Commands

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        public ICommand EmployeeViewCommand
        {
            get
            {
                return _employeeViewCommand ?? (_employeeViewCommand = new RelayCommand(ExecuteEmployeeViewCommand));
            }
        }

        public ICommand ReportsViewCommand
        {
            get { return _reportsViewCommand ?? (_reportsViewCommand = new RelayCommand(ExecuteReportsViewCommand)); }
        }

        public ICommand BackupRestoreCommand
        {
            get
            {
                return _backupRestoreCommand ??
                       (_backupRestoreCommand = new RelayCommand<object>(ExcuteBackupRestoreCommand));
            }
        }

        private void ExecuteEmployeeViewCommand()
        {
            HeaderText = "Employees";
            EmployeeViewModel.LoadData = true;
            CurrentViewModel = EmployeeViewModel;
        }

        private void ExecuteReportsViewCommand()
        {
            HeaderText = "PinnaFace - Reports";
            CurrentViewModel = ReportsViewModel;
        }

        private void ExcuteBackupRestoreCommand(object obj)
        {
            var backupRestore = new BackupRestore(obj);
            backupRestore.Show();
        }

        #endregion

        #region Enjaz

        //private ICommand _goToEnjazViewCommand;
        private ICommand _goToMainEnjazViewCommand;

        public ICommand GoToMainEnjazViewCommand
        {
            get
            {
                return _goToMainEnjazViewCommand ??
                       (_goToMainEnjazViewCommand = new RelayCommand(ExcuteGoToMainEnjazViewCommand));
            }
        }

        private void ExcuteGoToMainEnjazViewCommand()
        {
            try
            {
                var enjaz = new EnjazitBrowser(BrowserTarget.Enjazit);
                enjaz.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + exception.InnerException);
            }
        }

        #endregion

        #region Musaned

        private ICommand _goToMainMusanedViewCommand;
        //private ICommand _goToMusanedViewCommand;

        public ICommand GoToMainMusanedViewCommand
        {
            get
            {
                return _goToMainMusanedViewCommand ??
                       (_goToMainMusanedViewCommand = new RelayCommand(ExcuteGoToMainMusanedViewCommand));
            }
        }

        private void ExcuteGoToMainMusanedViewCommand()
        {
            var enjaz = new EnjazitBrowser(BrowserTarget.Musaned);
            enjaz.Show();
        }

        #endregion

        #region Insurance

        //private ICommand _goToInsuranceViewCommand;
        private ICommand _goToMainInsuranceViewCommand;

        public ICommand GoToMainInsuranceViewCommand
        {
            get
            {
                return _goToMainInsuranceViewCommand ??
                       (_goToMainInsuranceViewCommand = new RelayCommand(ExcuteGoToMainInsuranceViewCommand));
            }
        }

        private void ExcuteGoToMainInsuranceViewCommand()
        {
            var enjaz = new EnjazitBrowser(BrowserTarget.UnitedInsurance);
            enjaz.Show();
        }

        #endregion

        #region PinnaFace

        //private ICommand _goToPinnaFaceViewCommand;
        private ICommand _goToMainPinnaFaceViewCommand;

        public ICommand GoToMainPinnaFaceViewCommand
        {
            get
            {
                return _goToMainPinnaFaceViewCommand ??
                       (_goToMainPinnaFaceViewCommand = new RelayCommand(ExcuteGoToMainPinnaFaceViewCommand));
            }
        }

        private void ExcuteGoToMainPinnaFaceViewCommand()
        {
            var enjaz = new EnjazitBrowser(BrowserTarget.PinnaFace);
            enjaz.Show();
        }

        #endregion

        #region Previlege Visibility

        private string _adminVisibility;
        private string _agencyVisibility, _agentVisibility;
        private string _complainVisibility;
        private string _reportsVisibility;
        private string _usersMgmtVisibility;
        private string _visaVisibility;

        public string AgencyVisibility
        {
            get { return _agencyVisibility; }
            set
            {
                _agencyVisibility = value;
                RaisePropertyChanged<string>(() => AgencyVisibility);
            }
        }

        public string AgentVisibility
        {
            get { return _agentVisibility; }
            set
            {
                _agentVisibility = value;
                RaisePropertyChanged<string>(() => AgentVisibility);
            }
        }

        public string VisaVisibility
        {
            get { return _visaVisibility; }
            set
            {
                _visaVisibility = value;
                RaisePropertyChanged<string>(() => VisaVisibility);
            }
        }

        public string ReportsVisibility
        {
            get { return _reportsVisibility; }
            set
            {
                _reportsVisibility = value;
                RaisePropertyChanged<string>(() => ReportsVisibility);
            }
        }

        public string AdminVisibility
        {
            get { return _adminVisibility; }
            set
            {
                _adminVisibility = value;
                RaisePropertyChanged<string>(() => AdminVisibility);
            }
        }

        public string UsersMgmtVisibility
        {
            get { return _usersMgmtVisibility; }
            set
            {
                _usersMgmtVisibility = value;
                RaisePropertyChanged<string>(() => UsersMgmtVisibility);
            }
        }

        public string ComplainVisibility
        {
            get { return _complainVisibility; }
            set
            {
                _complainVisibility = value;
                RaisePropertyChanged<string>(() => ComplainVisibility);
            }
        }

        private void CheckRoles()
        {
            AgencyVisibility = UserUtil.UserHasRole(RoleTypes.LocalAgency) ? "Visible" : "Collapsed";

            AgentVisibility = UserUtil.UserHasRole(RoleTypes.ForeignAgentDetail) ? "Visible" : "Collapsed";

            VisaVisibility = UserUtil.UserHasRole(RoleTypes.ViewVisa) ? "Visible" : "Collapsed";

            ReportsVisibility = UserUtil.UserHasRole(RoleTypes.ViewReports) ? "Visible" : "Collapsed";

            UsersMgmtVisibility = UserUtil.UserHasRole(RoleTypes.Users) ? "Visible" : "Collapsed";

            ComplainVisibility = UserUtil.UserHasRole(RoleTypes.OpenComplain) ||
                                 UserUtil.UserHasRole(RoleTypes.CloseComplain) ||
                                 UserUtil.UserHasRole(RoleTypes.ReOpenComplain) ||
                                 UserUtil.UserHasRole(RoleTypes.ConfirmComplain) ||
                                 UserUtil.UserHasRole(RoleTypes.EditComplain)
                ? "Visible"
                : "Collapsed";
            AdminVisibility = UserUtil.UserHasRole(RoleTypes.Options) ? "Visible" : "Collapsed";
        }

        #endregion

        //private void Initialize()
        //{
        //    try
        //    {
        //        LogUtil.LogError(ErrorSeverity.Info, "Initialize", "Service is started at " + DateTime.Now, "", "");
        //        _monitorTimer = new Timer(_monitorTimerDelay);
        //        _monitorTimer.Elapsed += OnMonitorTimerElapsed;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtil.LogError(ErrorSeverity.Critical, "Initialize Sync",
        //            ex.Message + Environment.NewLine + ex.InnerException, _userName, _agencyName);
        //    }
        //}

        //public void Start()
        //{
        //    try
        //    {
        //        _userName = Singleton.User.UserName;
        //        _agencyName = Singleton.Agency.AgencyName;
        //    }
        //    catch
        //    {
        //        _userName = "Default User";
        //        _agencyName = "Default Agency";
        //    }

        //    IUnitOfWork sourceUnitOfWork = new UnitOfWork(new DbContextFactory().Create());
        //    try
        //    {
        //        LogUtil.LogError(ErrorSeverity.Info, "Start", "Service is started at " + DateTime.Now, "", "");
        //        var setting = new SettingService(true).GetSetting();
        //        if (setting != null)// && setting.StartSync)
        //        {
        //            _monitorTimerDelay = setting.SyncDuration * 60000;//60000; //1 minute is equivalent to 60000
        //            Initialize();
        //            _monitorTimer.Enabled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtil.LogError(ErrorSeverity.Critical, "Start Sync",
        //            ex.Message + Environment.NewLine + ex.InnerException, _userName, _agencyName);
        //    }
        //    finally
        //    {
        //        sourceUnitOfWork.Dispose();
        //    }
        //}

        //private void OnMonitorTimerElapsed(object source, ElapsedEventArgs e)
        //{ 
        //    try
        //    {
        //        var setting = new SettingService(true).GetSetting();
        //        if (setting != null && !setting.StartSync)
        //        {
        //            return;
        //        }

        //        LogUtil.LogError(ErrorSeverity.Info, "OnMonitorTimerElapsed", "Service is started at " + DateTime.Now, "", "");
        //        _monitorTimer.Enabled = false;

        //        var worker = new BackgroundWorker();

        //        worker.DoWork += Sync;
        //        worker.RunWorkerCompleted += worker_RunWorkerCompleted2;
        //        worker.RunWorkerAsync();
        //    }
        //    catch(Exception exception)
        //    {
        //        LogUtil.LogError(ErrorSeverity.Critical, "OnMonitorTimerElapsed", exception.Message + exception.InnerException, "", "");
        //    }

        //}

        //private void Sync(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        var syncTask = new SyncTask();
        //        syncTask.Sync();
        //    }
        //    catch (Exception exception)
        //    {
        //        LogUtil.LogError(ErrorSeverity.Critical, "OnMonitorTimerElapsed", exception.Message + exception.InnerException, "", "");
        //    }
        //}

        //private void worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    _monitorTimer.Enabled = true;
        //    //if (_noConnection)
        //    //    UpdatingText = "There is no Internet connection...";
        //    //else
        //    //{
        //    //    UpdatingText = _updatesFound
        //    //        ? "There exists new updates, refresh to see the updates"
        //    //        : "No updates found...";
        //    //}
        //    ////else if (_refreshed)
        //    ////{
        //    ////    UpdatingText = _noConnection ? "There is no Internet connection..." : "No updates found...";
        //    ////}
        //}
    }
}