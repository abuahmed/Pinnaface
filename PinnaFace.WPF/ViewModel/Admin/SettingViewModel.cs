using System;
using System.Linq;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.WPF.ViewModel
{
    public class SettingViewModel : ViewModelBase
    {
        #region Fields

        private static IUnitOfWork _unitOfWork;
        private SettingDTO _currentSetting;
        private ICommand _saveSettingViewCommand;
        private bool _startSyncService;
        private bool _syncServiceInstalled;
        private string _serviceInstalled, _serviceNotInstalled;
        #endregion

        #region Constructor

        public SettingViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            CurrentSetting = _unitOfWork.Repository<SettingDTO>().Query().Get().FirstOrDefault();

            //SyncServiceInstalled = false;
            //ServiceController service =
            //    ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "PinnaFace.SyncEngine");
            //if (service != null)
            //    SyncServiceInstalled = true;
            //service.Status.Equals(ServiceControllerStatus.);
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Public Properties

        public SettingDTO CurrentSetting
        {
            get { return _currentSetting; }
            set
            {
                _currentSetting = value;
                RaisePropertyChanged<SettingDTO>(() => CurrentSetting);
            }
        }

        //public bool SyncServiceInstalled
        //{
        //    get { return _syncServiceInstalled; }
        //    set
        //    {
        //        _syncServiceInstalled = value;
        //        RaisePropertyChanged<bool>(() => SyncServiceInstalled);
        //        if (SyncServiceInstalled)
        //        {
        //            ServiceInstalled = "Visible";
        //            ServiceNotInstalled = "Collapsed";
                    
        //            var service = new ServiceController("PinnaFace.SyncEngine");
        //            if (service.Status.Equals(ServiceControllerStatus.Running) ||
        //                service.Status.Equals(ServiceControllerStatus.StartPending))
        //                StartSyncService = true;

        //            service.Close();
        //        }
        //        else
        //        {
        //            ServiceInstalled = "Collapsed";
        //            ServiceNotInstalled = "Visible";
        //        }
        //    }
        //}
        //public string ServiceInstalled
        //{
        //    get { return _serviceInstalled; }
        //    set
        //    {
        //        _serviceInstalled = value;
        //        RaisePropertyChanged<string>(() => ServiceInstalled);
        //    }
        //}
        //public string ServiceNotInstalled
        //{
        //    get { return _serviceNotInstalled; }
        //    set
        //    {
        //        _serviceNotInstalled = value;
        //        RaisePropertyChanged<string>(() => ServiceNotInstalled);
        //    }
        //}
        //public bool StartSyncService
        //{
        //    get { return _startSyncService; }
        //    set
        //    {
        //        _startSyncService = value;
        //        RaisePropertyChanged<bool>(() => StartSyncService);
        //        if (SyncServiceInstalled)
        //        {
        //            try
        //            {
        //                var service = new ServiceController("PinnaFace.SyncEngine");
        //                if (StartSyncService)
        //                {
        //                    if (service.Status.Equals(ServiceControllerStatus.Stopped) ||
        //                        service.Status.Equals(ServiceControllerStatus.StopPending))
        //                        service.Start();
        //                }
        //                else
        //                {
        //                    if (service.Status.Equals(ServiceControllerStatus.Running) ||
        //                        service.Status.Equals(ServiceControllerStatus.StartPending))
        //                        service.Stop();
        //                }
        //                service.Close();
        //            }
        //            catch(Exception ex)
        //            {
        //                MessageBox.Show(ex.Message+Environment.NewLine+ex.InnerException,"Problem on the Service Project");
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Sync Service Not Installed");
        //        }
        //    }
        //}

        #endregion

        #region Commands

        public ICommand SaveSettingCommand
        {
            get
            {
                return _saveSettingViewCommand ??
                       (_saveSettingViewCommand = new RelayCommand<Object>(ExecuteSaveSettingViewCommand, CanSave));
            }
        }

        private void ExecuteSaveSettingViewCommand(object obj)
        {
            try
            {
                CurrentSetting.Synced = false;
                _unitOfWork.Repository<SettingDTO>().InsertUpdate(CurrentSetting);
                _unitOfWork.Commit();
                
                CloseWindow(obj);
            }
            catch
            {
                MessageBox.Show("Can't Save Setting!");
            }
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.DialogResult = true;
            window.Close();
        }

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object obj)
        {
            if (Errors == 0)
                return true;
            return false;
        }

        #endregion
    }
}