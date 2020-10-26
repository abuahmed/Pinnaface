using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Net;
using System.Web.Security;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service;
using PinnaFace.WPF.Models;
using PinnaFace.WPF.Views;
using WebMatrix.WebData;

namespace PinnaFace.WPF.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        private static IUnitOfWork _unitOfWork;
        private UserDTO _user;
        private ICommand _loginCommand;
        private ICommand _closeLoginView, _checkForServerUpdates;
        private string _lockedVisibility, _unLockedVisibility, _serverUpdatesVisibility;
        private bool _rememberMe;

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            LockedVisibility = "Visible";
            UnLockedVisibility = "Collapsed";
            ServerUpdatesVisibility = "Collapsed";

            CleanUp();
            Singleton.Agency = new LocalAgencyService(true).GetLocalAgency();
            InitializeObjects.InitializeWebSecurity();

            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            var currentSetting = XmlSerializerCustom.GetUserSetting();
            User = new UserDTO
            {
                UserName =currentSetting.UserName,
                //Password = Properties.Settings.Default.Password
            };
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Properties

        public UserDTO User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged<UserDTO>(() => User);
            }
        }

        public bool RememberMe
        {
            get { return _rememberMe; }
            set
            {
                _rememberMe = value;
                RaisePropertyChanged<bool>(() => RememberMe);
            }
        }

        public string ServerUpdatesVisibility
        {
            get { return _serverUpdatesVisibility; }
            set
            {
                _serverUpdatesVisibility = value;
                RaisePropertyChanged<string>(() => ServerUpdatesVisibility);
            }
        }

        public string LockedVisibility
        {
            get { return _lockedVisibility; }
            set
            {
                _lockedVisibility = value;
                RaisePropertyChanged<string>(() => LockedVisibility);
            }
        }

        public string UnLockedVisibility
        {
            get { return _unLockedVisibility; }
            set
            {
                _unLockedVisibility = value;
                RaisePropertyChanged<string>(() => UnLockedVisibility);
            }
        }

        #endregion

        #region Commands

        public ICommand LoginCommand
        {
            get { return _loginCommand ?? (_loginCommand = new RelayCommand<object>(ExcuteLoginCommand, CanSave)); }
        }

        private void ExcuteLoginCommand(object obj)
        {
            try
            {
                var values = (object[]) obj;
                var psdBox = values[0] as PasswordBox;

                //Do Validation if not handled on the UI
                if (psdBox != null && psdBox.Password == "")
                {
                    psdBox.Focus();
                    return;
                }

                if (psdBox != null)
                {
                    var us = Membership.ValidateUser(User.UserName, psdBox.Password);

                    if (!us)
                    {
                        MessageBox.Show("Incorrect UserName/Password", "Error Logging",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        User.Password = "";
                        ServerUpdatesVisibility = "Visible";
                        return;
                    }

                    var userId = WebSecurity.GetUserId(User.UserName);
                    var user = new UserService(true).GetUser(userId);

                    if (user == null)
                    {
                        MessageBox.Show("Incorrect UserName/Password", "Error Logging",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        User.Password = "";
                        ServerUpdatesVisibility = "Visible";
                    }
                    else
                    {
                        LockedVisibility = "Collapsed";
                        UnLockedVisibility = "Visible";
                        //Thread.Sleep(1000);

                        Singleton.User = user;
                        Singleton.User.Password = psdBox.Password;
                        Singleton.UserRoles = new UserRolesModel();

                        Singleton.Setting = new UnitOfWork(DbContextUtil.GetDbContextInstance())
                            .Repository<SettingDTO>()
                            .FindById(1);

                        if (RememberMe)
                        {
                            var currentSetting = XmlSerializerCustom.GetUserSetting();

                            currentSetting.UserName = User.UserName;
                            currentSetting.Password = psdBox.Password;

                            XmlSerializerCustom.SetUserSetting(currentSetting);

                        }

                        switch (user.Status)
                        {
                            case UserTypes.Waiting:
                                new ChangePassword(psdBox.Password).Show();
                                break;
                            case UserTypes.Active:
                            {
                                NotifyUtility.ShowCustomBalloon("PinnaFace", "Welcome to PinnaFace", 4000);
                                new MainWindow().Show();
                            }
                                break;
                            default:
                                MessageBox.Show("Login Failed", "Error Logging", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                                break;
                        }

                        CloseWindow(values[1]);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.InnerException);
            }
        }

       
        public ICommand CheckForServerUpdatesCommand
        {
            get { return _checkForServerUpdates ?? (_checkForServerUpdates = new RelayCommand(CheckForServerUpdates)); }
        }

        

        public ICommand CloseLoginView
        {
            get { return _closeLoginView ?? (_closeLoginView = new RelayCommand<Object>(CloseWindow)); }
        }

        public void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        #endregion

        #region Check For Server Updates
        private static object _obj;
        private bool _noConnection;
        string _userName,_agencyName;
        private bool _updatesCompleted, _errorsFound;

        public IUnitOfWork GetNewUow2(IUnitOfWork uom)
        {
            uom.Dispose();
            return new UnitOfWork(new DbContextFactory().Create());
        }

        public void CheckForServerUpdates()
        {
            var loading = new Loading();
            _obj = loading;
            loading.Show();

            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                
                if (!CheckInternetConnection())
                {
                    _noConnection = true;
                    return;
                }
                _userName = "";
                _agencyName = "";

                _errorsFound = true; //Reset it Below
                IUnitOfWork sourceUnitOfWork = new UnitOfWorkServer(new ServerDbContextFactory().Create());
                IUnitOfWork destinationUnitOfWork = new UnitOfWork(new DbContextFactory().Create());

                if (!SyncUsers(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);
                if (!SyncMemberships(sourceUnitOfWork, destinationUnitOfWork)) return;

                _errorsFound = false;
                _updatesCompleted = true;


                sourceUnitOfWork.Dispose();
                destinationUnitOfWork.Dispose();
            }
            catch
            {
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CloseWindow(_obj);
            if (_noConnection)
            {
                MessageBox.Show("No Internet Connection...");
            }
            else if (_errorsFound)
            {
                MessageBox.Show("Problem On Getting Updates...");
            }
            else if (_updatesCompleted)
            {
                MessageBox.Show("Updates Completed Successfully...");
            }
        }

        public bool CheckInternetConnection()
        {
            try
            {
                //Check for internet connection

                if (Singleton.BuildType == BuildType.LocalDev)
                    return true;

                var stream = new WebClient().OpenRead("http://www.google.com");
            }
            catch
            {
                //_noConnection = true;
                return false;
            }
            return true;
        }

        public bool SyncUsers(IUnitOfWork sourceUnitOfWork,
           IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.UserRepository<UserDTO>().Query()// && a.DateLastModified > LastServerSyncDate)
                .Include(h => h.Agency)
                .Filter(a => !(bool)a.Synced)
                .Get(1).ToList();

            if (sourceList.Any())
            {
                //_updatesFound = true;
                var destLocalAgencies =
                    destinationUnitOfWork.Repository<AgencyDTO>().Query()
                        .Get(1)
                        .ToList();

                var destList =
                    destinationUnitOfWork.UserRepository<UserDTO>()
                        .Query()
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    var userId = 0;
                    if (destination == null)
                    {
                        continue;
                        //destination = new UserDTO();
                    }
                    userId = destination.UserId;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<UserDTO, UserDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("AgenciesWithAgents", option => option.Ignore())
                            .ForMember("Agent", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());

                        destination = Mapper.Map(source, destination);
                        destination.UserId = userId;
                    //    destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                    //        sourceUnitOfWork, destinationUnitOfWork);
                    //    destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                    //        sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, _userName, _agencyName);
                    }
                    try
                    {
                        #region Foreign Keys

                        var agencyDTO =
                            destLocalAgencies.FirstOrDefault(
                                c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
                        {
                            destination.Agency = agencyDTO;
                            destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?)null;
                        }

                        #endregion

                        destination.Synced = true;

                        if (userId == 0)
                            destinationUnitOfWork.UserRepository<UserDTO>()
                                .Insert(destination);
                        else
                            destinationUnitOfWork.UserRepository<UserDTO>()
                                .Update(destination);
                    }
                    catch (Exception ex)
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Crud",
                            ex.Message + Environment.NewLine + ex.InnerException, _userName, _agencyName);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Commit",
                        "Problem Commiting SyncUsers Method", _userName, _agencyName);
                    return false;
                }
            }

            return true;
        }

        public bool SyncMemberships(IUnitOfWork sourceUnitOfWork,
           IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.UserRepository<MembershipDTO>().Query()
                //.Filter(a => a.DateLastModified > LastServerSyncDate)
                .Get(1).ToList();
            var sourceUsers = sourceUnitOfWork.UserRepository<UserDTO>().Query()
                .Get(1).ToList();
            if (sourceList.Any())
            {
                //_updatesFound = true;
                var destUsers =
                    destinationUnitOfWork.UserRepository<UserDTO>().Query().Get(1).ToList();
                //var destRoles = destinationUnitOfWork.UserRepository<RoleDTO>().Query().Get(1).ToList();

                var destList =
                    destinationUnitOfWork.UserRepository<MembershipDTO>().Query()
                        .Get(1).ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    if (destination == null)
                    {
                        continue;
                        //destination = new MembershipDTO();
                    }
                    var id = destination.UserId;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<MembershipDTO, MembershipDTO>()
                            .ForMember("Synced", option => option.Ignore());
                        destination = Mapper.Map(source, destination);
                        destination.Id = id;

                        //destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                        //    sourceUnitOfWork, destinationUnitOfWork);
                        //destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                        //    sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, _userName, _agencyName);
                    }
                    try
                    {
                        var userguid = sourceUsers.FirstOrDefault(c => c.UserId == source.UserId);
                        var userDto =
                            destUsers.FirstOrDefault(c => userguid != null && c.RowGuid == userguid.RowGuid);
                        {
                            //users.User = userDto;
                            destination.UserId = userDto != null ? userDto.UserId : 1;
                        }
                        if (id == 0)
                            destinationUnitOfWork.UserRepository<MembershipDTO>()
                                .Insert(destination);
                        else
                            destinationUnitOfWork.UserRepository<MembershipDTO>()
                                .Update(destination);
                    }
                    catch
                    {
                        //_errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Crud",
                            "Problem On SyncMemberships Crud Method", _userName, _agencyName);
                        //return false;
                    }
                }

                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Commit",
                        "Problem Commiting SyncMemberships Method", _userName, _agencyName);
                    return false;
                }
            }

            return true;
        }
        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object obj)
        {
            return Errors == 0;
        }

        #endregion
    }
}