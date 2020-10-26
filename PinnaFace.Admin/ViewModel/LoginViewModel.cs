using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.Security;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
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
using PinnaFace.Admin.Views;
using WebMatrix.WebData;

namespace PinnaFace.Admin.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region Fields

        private static IUnitOfWork _unitOfWork;
        private UserDTO _user;
        private ICommand _loginCommand;
        private ICommand _closeLoginView, _checkForServerUpdates;
        private string _lockedVisibility, _unLockedVisibility, _serverUpdatesVisibility;

        #endregion

        #region Constructor

        public LoginViewModel()
        {
            LockedVisibility = "Visible";
            UnLockedVisibility = "Collapsed";
            ServerUpdatesVisibility = "Collapsed";

            CleanUp();
            //Just to create the database if it is not done yet
            Singleton.Agency = new LocalAgencyService(true).GetLocalAgency();

            InitializeWebSecurity();

            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

            User = new UserDTO
            {
                UserName = "superweb"
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
            var values = (object[])obj;
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
                var user = new UserService().GetUser(userId);

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

                    switch (user.Status)
                    {
                        case UserTypes.Waiting:
                            new ChangePassword(psdBox.Password).Show();
                            break;
                        case UserTypes.Active:
                            new MainWindow().Show();
                            break;
                        default:
                            MessageBox.Show("Login Failed", "Error Logging", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }

                    CloseWindow(values[1]);
                }
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
        private bool _updatesCompleted, _errorsFound;

        public IUnitOfWork GetNewUow2(IUnitOfWork uom)
        {
            uom.Dispose();
            return new UnitOfWork(new DbContextFactory().Create());
        }

        public void CheckForServerUpdates()
        {
            //var loading = new Loading();
            //_obj = loading;
            //loading.Show();

            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                //var syncTask = new SyncTask();
                //if (!syncTask.CheckInternetConnection())
                //{
                //    _noConnection = true;
                //    return;
                //}

                //_errorsFound = true; //Reset it Below
                //IUnitOfWork sourceUnitOfWork = new UnitOfWorkServer(new ServerDbContextFactory().Create());
                //IUnitOfWork destinationUnitOfWork = new UnitOfWork(new DbContextFactory().Create());

                //if (!syncTask.SyncUsers(sourceUnitOfWork, destinationUnitOfWork)) return;
                //destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);
                //if (!syncTask.SyncMemberships(sourceUnitOfWork, destinationUnitOfWork)) return;

                //_errorsFound = false;
                //_updatesCompleted = true;


                //sourceUnitOfWork.Dispose();
                //destinationUnitOfWork.Dispose();
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
        #endregion

        #region Initialize WebSecurity
        public void InitializeWebSecurity()
        {
            var dbContext2 = DbContextUtil.GetDbContextInstance();

            try
            {
                if (!WebSecurity.Initialized)
                {
                    //System.Web.Security.Roles.Enabled = true;
                    //System.Web.PreApplicationStartMethodAttribute;
                    WebSecurity.InitializeDatabaseConnection(Singleton.ConnectionStringName, Singleton.ProviderName,
                        "Users",
                        "UserId", "UserName", false);
                }


                if (!new UserService(true).GetAll().Any())
                {
                    #region Seed Default Roles and Users

                    IList<RoleDTO> listOfRoles = CommonUtility.GetRolesList();

                    int sno = 0;
                    foreach (var rol in listOfRoles)
                    {
                        var role = rol;
                        role.RowGuid = Guid.Parse(CommonUtility.GetRolesGuidList()[sno]);
                        dbContext2.Set<RoleDTO>().Add(role);
                        sno++;
                    }
                    dbContext2.SaveChanges();

                    var superName = "superweb";//Singleton.ProductActivation.SuperName; // 
                    var superPass = "P@ssw0rd1!";//Singleton.ProductActivation.SuperPass; //
                    WebSecurity.CreateUserAndAccount(superName, superPass,
                        new
                        {
                            Status = 1,
                            //AgencyId = Singleton.Agency.Id,
                            Enabled = true,
                            Synced = false,
                            RowGuid = Guid.NewGuid(),
                            Email = "ibrayas@gmail.com",//Singleton.Agency.Address.PrimaryEmail, // 
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        }, true);

                    var adminName = "superadmin";//Singleton.ProductActivation.AdminName; // 
                    var adminPass = "P@ssw0rd";//Singleton.ProductActivation.AdminPass; //
                    WebSecurity.CreateUserAndAccount(adminName, adminPass,
                        new
                        {
                            Status = 0,
                            //AgencyId = Singleton.Agency.Id,
                            Enabled = true,
                            Synced = false,
                            RowGuid = Guid.NewGuid(),
                            Email = "adminuser@pinnaface.com",//"", //
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });
                    var user1Name = "superuser";// Singleton.ProductActivation.User1Name; // 
                    var user1Pass = "pa12345";//Singleton.ProductActivation.User1Pass; // 
                    WebSecurity.CreateUserAndAccount(user1Name, user1Pass,
                        new
                        {
                            Status = 0,
                            //AgencyId = Singleton.Agency.Id,
                            Enabled = true,
                            Synced = false,
                            RowGuid = Guid.NewGuid(),
                            Email = "pinnauser@pinnaface.com",//"", //
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        });

                    //add row guid for membership table members
                    //var members = new UserService().GetAllMemberShips().ToList();
                    var members = dbContext2.Set<MembershipDTO>().ToList();
                    foreach (var membershipDTO in members)
                    {
                        membershipDTO.Synced = false;
                        membershipDTO.RowGuid = Guid.NewGuid();
                        membershipDTO.Enabled = true;
                        membershipDTO.CreatedByUserId = 1;
                        membershipDTO.DateRecordCreated = DateTime.Now;
                        membershipDTO.ModifiedByUserId = 1;
                        membershipDTO.DateLastModified = DateTime.Now;
                        dbContext2.Set<MembershipDTO>().Add(membershipDTO);
                        dbContext2.Entry(membershipDTO).State = EntityState.Modified;
                    }
                    dbContext2.SaveChanges();

                    var lofRoles = new UserService().GetAllRoles().ToList();
                    foreach (var role in lofRoles.Skip(3))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId(superName)
                        });
                    }

                    foreach (var role in lofRoles.Skip(4))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId(adminName)
                        });
                    }

                    foreach (var role in lofRoles.Skip(5))
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId(user1Name)
                        });
                    }

                    dbContext2.SaveChanges();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem on InitializeWebSecurity" +
                                Environment.NewLine + ex.Message +
                                Environment.NewLine + ex.InnerException);
            }
            finally
            {
                dbContext2.Dispose();
            }
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
