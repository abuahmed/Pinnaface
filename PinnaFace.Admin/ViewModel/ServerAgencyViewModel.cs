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
    public class ServerAgencyViewModel : ViewModelBase
    {
        #region Fields

        private static PinnaFace.Repository.Interfaces.IUnitOfWork _unitOfWork;
        private ObservableCollection<AgencyDTO> _users;
        private AgencyDTO _selectedAgency, _selectedAgencyForSearch;
        private ICommand _saveAgencyViewCommand, _addNewAgencyViewCommand;
        private ICommand _closeAgencyViewCommand;
        private bool _editCommandVisibility;

        #endregion

        #region Constructor

        public ServerAgencyViewModel()
        {
            CleanUp();

            var iDbContext = new ServerDbContextFactory().Create();
            _unitOfWork = new UnitOfWorkServer(iDbContext);

            SelectedAgency = new AgencyDTO();
            Agencys = new ObservableCollection<AgencyDTO>();
            GetLiveAgencys();
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

        public AgencyDTO SelectedAgency
        {
            get { return _selectedAgency; }
            set
            {
                _selectedAgency = value;
                RaisePropertyChanged<AgencyDTO>(() => SelectedAgency);
                if (SelectedAgency != null)
                {
                }
            }
        }

        public AgencyDTO SelectedAgencyForSearch
        {
            get { return _selectedAgencyForSearch; }
            set
            {
                _selectedAgencyForSearch = value;
                RaisePropertyChanged<AgencyDTO>(() => this.SelectedAgencyForSearch);

                //if (SelectedAgencyForSearch != null && !string.IsNullOrEmpty(SelectedAgencyForSearch.AgencyDetail))
                //{
                //    SelectedAgency = SelectedAgencyForSearch;
                //    SelectedAgencyForSearch.AgencyDetail = "";
                //}
            }
        }

        public ObservableCollection<AgencyDTO> Agencys
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged<ObservableCollection<AgencyDTO>>(() => Agencys);

                if (Agencys.Count > 0)
                    SelectedAgency = Agencys.FirstOrDefault();
                else
                    ExecuteAddNewAgencyViewCommand();
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewAgencyViewCommand
        {
            get
            {
                return _addNewAgencyViewCommand ??
                       (_addNewAgencyViewCommand = new RelayCommand(ExecuteAddNewAgencyViewCommand));
            }
        }

        private void ExecuteAddNewAgencyViewCommand()
        {
            SelectedAgency = new AgencyDTO();
        }

        public ICommand SaveAgencyViewCommand
        {
            get
            {
                return _saveAgencyViewCommand ??
                       (_saveAgencyViewCommand = new RelayCommand(ExecuteSaveAgencyViewCommand, CanSave));
            }
        }

        private void ExecuteSaveAgencyViewCommand()
        {
            try
            {

                _unitOfWork.Repository<AgencyDTO>().InsertUpdate(SelectedAgency);
                _unitOfWork.Commit();
                GetLiveAgencys();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //public ICommand DeleteAgencyViewCommand
        //{
        //    get
        //    {
        //        return _deleteAgencyViewCommand ??
        //               (_deleteAgencyViewCommand = new RelayCommand(ExecuteDeleteAgencyViewCommand));
        //    }
        //}

        //private void ExecuteDeleteAgencyViewCommand()
        //{
        //    try
        //    {
        //        if (
        //            MessageBox.Show("Are you Sure You want to Delete the user?", "Delete Agency",
        //                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        //        {
        //            SelectedAgency.Enabled = false;
        //            _userService.Disable(SelectedAgency);
        //            //_unitOfWork.Repository<AgencyDTO>().Update(SelectedAgency);
        //            //_unitOfWork.Commit();

        //            GetLiveAgencys();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        public ICommand CloseAgencyViewCommand
        {
            get
            {
                return _closeAgencyViewCommand ??
                       (_closeAgencyViewCommand = new RelayCommand<Object>(ExecuteCloseAgencyViewCommand));
            }
        }

        private void ExecuteCloseAgencyViewCommand(object obj)
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

        #region Load Agencys

        private void GetLiveAgencys()
        {
            var usrs = _unitOfWork.Repository<AgencyDTO>().Query()
                .Include(a => a.Address, a => a.Header, a => a.Footer)
                .Get();

            int sNo = 1;
            foreach (var userDto in usrs)
            {
                userDto.SerialNumber = sNo;
                sNo++;
            }
            Agencys = new ObservableCollection<AgencyDTO>(usrs.ToList());
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