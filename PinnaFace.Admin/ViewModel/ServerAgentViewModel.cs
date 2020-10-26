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
    public class ServerAgentViewModel : ViewModelBase
    {
        #region Fields

        private static PinnaFace.Repository.Interfaces.IUnitOfWork _unitOfWork;
        private ObservableCollection<AgentDTO> _users;
        private AgentDTO _selectedAgent, _selectedAgentForSearch;
        private ICommand _saveAgentViewCommand, _addNewAgentViewCommand;
        private ICommand _closeAgentViewCommand;
        private bool _editCommandVisibility;

        #endregion

        #region Constructor

        public ServerAgentViewModel()
        {
            CleanUp();

            var iDbContext = new ServerDbContextFactory().Create();
            _unitOfWork = new UnitOfWorkServer(iDbContext);

            SelectedAgent = new AgentDTO();
            Agents = new ObservableCollection<AgentDTO>();
            GetLiveAgents();
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

        public AgentDTO SelectedAgent
        {
            get { return _selectedAgent; }
            set
            {
                _selectedAgent = value;
                RaisePropertyChanged<AgentDTO>(() => SelectedAgent);
                if (SelectedAgent != null)
                {
                }
            }
        }

        public AgentDTO SelectedAgentForSearch
        {
            get { return _selectedAgentForSearch; }
            set
            {
                _selectedAgentForSearch = value;
                RaisePropertyChanged<AgentDTO>(() => this.SelectedAgentForSearch);

                //if (SelectedAgentForSearch != null && !string.IsNullOrEmpty(SelectedAgentForSearch.AgentDetail))
                //{
                //    SelectedAgent = SelectedAgentForSearch;
                //    SelectedAgentForSearch.AgentDetail = "";
                //}
            }
        }

        public ObservableCollection<AgentDTO> Agents
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged<ObservableCollection<AgentDTO>>(() => Agents);

                if (Agents.Count > 0)
                    SelectedAgent = Agents.FirstOrDefault();
                else
                    ExecuteAddNewAgentViewCommand();
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewAgentViewCommand
        {
            get
            {
                return _addNewAgentViewCommand ??
                       (_addNewAgentViewCommand = new RelayCommand(ExecuteAddNewAgentViewCommand));
            }
        }

        private void ExecuteAddNewAgentViewCommand()
        {
            SelectedAgent = new AgentDTO();
        }

        public ICommand SaveAgentViewCommand
        {
            get
            {
                return _saveAgentViewCommand ??
                       (_saveAgentViewCommand = new RelayCommand(ExecuteSaveAgentViewCommand, CanSave));
            }
        }

        private void ExecuteSaveAgentViewCommand()
        {
            try
            {

                _unitOfWork.Repository<AgentDTO>().InsertUpdate(SelectedAgent);
                _unitOfWork.Commit();
                GetLiveAgents();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //public ICommand DeleteAgentViewCommand
        //{
        //    get
        //    {
        //        return _deleteAgentViewCommand ??
        //               (_deleteAgentViewCommand = new RelayCommand(ExecuteDeleteAgentViewCommand));
        //    }
        //}

        //private void ExecuteDeleteAgentViewCommand()
        //{
        //    try
        //    {
        //        if (
        //            MessageBox.Show("Are you Sure You want to Delete the user?", "Delete Agent",
        //                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        //        {
        //            SelectedAgent.Enabled = false;
        //            _userService.Disable(SelectedAgent);
        //            //_unitOfWork.Repository<AgentDTO>().Update(SelectedAgent);
        //            //_unitOfWork.Commit();

        //            GetLiveAgents();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        public ICommand CloseAgentViewCommand
        {
            get
            {
                return _closeAgentViewCommand ??
                       (_closeAgentViewCommand = new RelayCommand<Object>(ExecuteCloseAgentViewCommand));
            }
        }

        private void ExecuteCloseAgentViewCommand(object obj)
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

        #region Load Agents

        private void GetLiveAgents()
        {
            var usrs = _unitOfWork.Repository<AgentDTO>().Query()
                .Include(a => a.Address, a => a.Header, a => a.Footer)
                .Get();

            int sNo = 1;
            foreach (var userDto in usrs)
            {
                userDto.SerialNumber = sNo;
                sNo++;
            }
            Agents = new ObservableCollection<AgentDTO>(usrs.ToList());
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