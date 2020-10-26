using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;

namespace PinnaFace.Admin.ViewModel
{
    public class ServerSettingViewModel : ViewModelBase
    {
        #region Fields

        private static PinnaFace.Repository.Interfaces.IUnitOfWork _unitOfWork;
        private ObservableCollection<SettingDTO> _users;
        private SettingDTO _selectedSetting, _selectedSettingForSearch;
        private ICommand _saveSettingViewCommand, _addNewSettingViewCommand;
        private ICommand _closeSettingViewCommand;
        private bool _editCommandVisibility;

        #endregion

        #region Constructor

        public ServerSettingViewModel()
        {
            CleanUp();

            var iDbContext = new ServerDbContextFactory().Create();
            _unitOfWork = new UnitOfWorkServer(iDbContext);
        
            SelectedSetting = new SettingDTO();
            Settings = new ObservableCollection<SettingDTO>();

            GetLiveSettings();
            EditCommandVisibility = false;
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Properties

        public bool EditCommandVisibility
        {
            get { return _editCommandVisibility; }
            set
            {
                _editCommandVisibility = value;
                RaisePropertyChanged<bool>(() => EditCommandVisibility);
            }
        }

        public SettingDTO SelectedSetting
        {
            get { return _selectedSetting; }
            set
            {
                _selectedSetting = value;
                RaisePropertyChanged<SettingDTO>(() => SelectedSetting);
                if (SelectedSetting != null)
                {
                }
            }
        }

        public SettingDTO SelectedSettingForSearch
        {
            get { return _selectedSettingForSearch; }
            set
            {
                _selectedSettingForSearch = value;
                RaisePropertyChanged<SettingDTO>(() => this.SelectedSettingForSearch);

                //if (SelectedSettingForSearch != null && !string.IsNullOrEmpty(SelectedSettingForSearch.SettingDetail))
                //{
                //    SelectedSetting = SelectedSettingForSearch;
                //    SelectedSettingForSearch.SettingDetail = "";
                //}
            }
        }

        public ObservableCollection<SettingDTO> Settings
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged<ObservableCollection<SettingDTO>>(() => Settings);

                if (Settings.Count > 0)
                    SelectedSetting = Settings.FirstOrDefault();
                else
                    ExecuteAddNewSettingViewCommand();
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewSettingViewCommand
        {
            get
            {
                return _addNewSettingViewCommand ??
                       (_addNewSettingViewCommand = new RelayCommand(ExecuteAddNewSettingViewCommand));
            }
        }

        private void ExecuteAddNewSettingViewCommand()
        {
            SelectedSetting = new SettingDTO();
        }

        public ICommand SaveSettingViewCommand
        {
            get
            {
                return _saveSettingViewCommand ??
                       (_saveSettingViewCommand = new RelayCommand(ExecuteSaveSettingViewCommand, CanSave));
            }
        }

        private void ExecuteSaveSettingViewCommand()
        {
            try
            {
                _unitOfWork.Repository<SettingDTO>().InsertUpdate(SelectedSetting);
                _unitOfWork.Commit();
                GetLiveSettings();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        public ICommand CloseSettingViewCommand
        {
            get
            {
                return _closeSettingViewCommand ??
                       (_closeSettingViewCommand = new RelayCommand<Object>(ExecuteCloseSettingViewCommand));
            }
        }

        private void ExecuteCloseSettingViewCommand(object obj)
        {
            CloseWindow(obj);
        }

        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.DialogResult = true;
            window.Close();
        }

        #endregion
        
        #region Load Settings

        private void GetLiveSettings()
        {
            var usrs = _unitOfWork.Repository<SettingDTO>().Query().Include(a=>a.Agency)
                .Get();

            int sNo = 1;
            foreach (var userDto in usrs)
            {
                userDto.SerialNumber = sNo;
                sNo++;
            }
            Settings = new ObservableCollection<SettingDTO>(usrs.ToList());
        }
        
        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave()
        {
            return Errors == 0;
        }

        #endregion
    }
}