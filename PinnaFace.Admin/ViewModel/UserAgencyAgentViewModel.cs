using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;

namespace PinnaFace.Admin.ViewModel
{
    public class UserAgencyAgentViewModel : ViewModelBase
    {
        #region Fields
        private static PinnaFace.Repository.Interfaces.IUnitOfWork _unitOfWork;

        private ICommand _addNewUserWithAgencyWithAgentViewCommand;
        private ICommand _closeUserWithAgencyWithAgentViewCommand;
        private ICommand _deleteUserWithAgencyWithAgentViewCommand;
        private ICommand _saveUserWithAgencyWithAgentViewCommand;
        
        private UserDTO _selectedUser;
        private UserAgencyAgentDTO _selectedUserWithAgencyWithAgentDto;

        private ObservableCollection<AgencyDTO> _localAgencies;
        private AgencyDTO _selectedLocalAgencyDto;

        private AgentDTO _selectedForeignAgentDto;
        private ObservableCollection<AgentDTO> _foreignAgents;

        #endregion

        #region Constructor

        public UserAgencyAgentViewModel()
        {
            CleanUp();

            var iDbContext = new ServerDbContextFactory().Create();
            _unitOfWork = new UnitOfWorkServer(iDbContext);

            Load();

            Messenger.Default.Register<UserDTO>(this, message =>
            {
                SelectedUser = message;
            });
            
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
            _unitOfWork.Dispose();
        }

        #endregion

        #region Public Properties

        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RaisePropertyChanged<UserDTO>(() => SelectedUser);
                if (SelectedUser != null)
                {
                    SelectedUserWithAgencyWithAgent = AddNewUserWithAgencyWithAgentDto();
                }
            
            }
        }

        public UserAgencyAgentDTO SelectedUserWithAgencyWithAgent
        {
            get { return _selectedUserWithAgencyWithAgentDto; }
            set
            {
                _selectedUserWithAgencyWithAgentDto = value;
                RaisePropertyChanged<UserAgencyAgentDTO>(() => SelectedUserWithAgencyWithAgent);
            }
        }

        public AgencyDTO SelectedLocalAgencyDto
        {
            get { return _selectedLocalAgencyDto; }
            set
            {
                _selectedLocalAgencyDto = value;
                RaisePropertyChanged<AgencyDTO>(() => SelectedLocalAgencyDto);
            }
        }
        public ObservableCollection<AgencyDTO> LocalAgencies
        {
            get { return _localAgencies; }
            set
            {
                _localAgencies = value;
                RaisePropertyChanged<ObservableCollection<AgencyDTO>>(() => LocalAgencies);
            }
        }

        public AgentDTO SelectedForeignAgentDto
        {
            get { return _selectedForeignAgentDto; }
            set
            {
                _selectedForeignAgentDto = value;
                RaisePropertyChanged<AgentDTO>(() => SelectedForeignAgentDto);
            }
        }
        public ObservableCollection<AgentDTO> ForeignAgents
        {
            get { return _foreignAgents; }
            set
            {
                _foreignAgents = value;
                RaisePropertyChanged<ObservableCollection<AgentDTO>>(() => ForeignAgents);
            }
        }
        
        public UserAgencyAgentDTO AddNewUserWithAgencyWithAgentDto()
        {
            if (SelectedUser != null)
                return new UserAgencyAgentDTO
                {
                    UserId = SelectedUser.UserId,
                    //AgencyWithAgent = new AgencyAgentDTO()
                    //{
                        
                    //}
                };
            return null;
        }

        private void Load()
        {
            //var criAg = new SearchCriteria<AgencyDTO>();
            //var localAgencyDtos = new LocalAgencyService(true).GetAll(criAg).ToList();
            
           var localAgencyDtos = _unitOfWork.Repository<AgencyDTO>().Query()
                        .Include(u => u.Address)
                        .Get().ToList();
            LocalAgencies = new ObservableCollection<AgencyDTO>(localAgencyDtos);

            //var cri = new SearchCriteria<AgentDTO>();
            //var foreignAgentDtos = new ForeignAgentService(true, false).GetAll(cri).ToList();
            var foreignAgentDtos = _unitOfWork.Repository<AgentDTO>().Query()
                        .Include(u => u.Address)
                        .Get().ToList();
            ForeignAgents = new ObservableCollection<AgentDTO>(foreignAgentDtos);
        }

        #endregion

        #region Commands

        public ICommand AddNewUserWithAgencyWithAgentViewCommand
        {
            get
            {
                return _addNewUserWithAgencyWithAgentViewCommand ??
                       (_addNewUserWithAgencyWithAgentViewCommand =
                           new RelayCommand(ExecuteAddNewUserWithAgencyWithAgentViewCommand));
            }
        }

        public ICommand SaveUserWithAgencyWithAgentViewCommand
        {
            get
            {
                return _saveUserWithAgencyWithAgentViewCommand ??
                       (_saveUserWithAgencyWithAgentViewCommand =
                           new RelayCommand<Object>(ExecuteSaveUserWithAgencyWithAgentViewCommand, CanSave));
            }
        }

        public ICommand DeleteUserWithAgencyWithAgentViewCommand
        {
            get
            {
                return _deleteUserWithAgencyWithAgentViewCommand ??
                       (_deleteUserWithAgencyWithAgentViewCommand =
                           new RelayCommand(ExecuteDeleteUserWithAgencyWithAgentViewCommand));
            }
        }

        public ICommand CloseUserWithAgencyWithAgentViewCommand
        {
            get
            {
                return _closeUserWithAgencyWithAgentViewCommand ??
                       (_closeUserWithAgencyWithAgentViewCommand =
                           new RelayCommand<Object>(ExecuteCloseUserWithAgencyWithAgentViewCommand));
            }
        }

        private void ExecuteAddNewUserWithAgencyWithAgentViewCommand()
        {
            SelectedUserWithAgencyWithAgent = AddNewUserWithAgencyWithAgentDto();
        }

        private void ExecuteSaveUserWithAgencyWithAgentViewCommand(object obj)
        {
            try
            {
                if (SelectedUserWithAgencyWithAgent != null)// && SelectedUserWithAgencyWithAgent.AgencyWithAgent != null)
                {
                    var agencyWithAg = new AgencyAgentDTO
                    {
                        AgencyId = SelectedLocalAgencyDto.Id,
                        AgentId = SelectedForeignAgentDto.Id
                    };
                    
                    //SelectedUserWithAgencyWithAgent.AgencyWithAgent.AgentId = SelectedForeignAgentDto.Id;
                    _unitOfWork.Repository<AgencyAgentDTO>()
                        .InsertUpdate(agencyWithAg);
                    //_unitOfWork.Commit();

                    SelectedUserWithAgencyWithAgent.AgencyWithAgentId = agencyWithAg.Id;
                    _unitOfWork.UserRepository<UserAgencyAgentDTO>()
                        .Insert(SelectedUserWithAgencyWithAgent);

                    _unitOfWork.Commit();

                    //_unitOfWork.Dispose();

                    CloseWindow(obj);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ExecuteDeleteUserWithAgencyWithAgentViewCommand()
        {
            try
            {
                //if (
                //    MessageBox.Show("Are you Sure You want to Delete this?", "Delete Testimony",
                //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                //{
                //    SelectedLocalAgencyDto.Enabled = false;
                //    _employeeRelativeService.InsertOrUpdate(SelectedLocalAgencyDto);
                //    GetLiveEmployeeTestimonies();
                //}
            }
            catch
            {
            }
        }

        private void ExecuteCloseUserWithAgencyWithAgentViewCommand(object obj)
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