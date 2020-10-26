using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;
using PinnaFace.Core;
using PinnaFace.Admin.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using WebMatrix.WebData;

namespace PinnaFace.Admin.ViewModel
{
    public class ServerUserViewModel : ViewModelBase
    {
        #region Fields

        private static PinnaFace.Repository.Interfaces.IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private ObservableCollection<UserDTO> _users;
        private ObservableCollection<UserAgencyAgentDTO> _usersWithAgencyWithAgent;
        private UserAgencyAgentDTO _selectedUserWithAgencyWithAgent;
        private UserDTO _selectedUser, _selectedUserForSearch;
        private ICommand _addRoleViewCommand;
        private ICommand _addNewUserWithAgencyWithAgentViewCommand, _deleteUserWithAgencyWithAgentViewCommand;
        private RoleDTO _selectedRole, _selectedRoleToAdd;
        private ObservableCollection<RoleDTO> _selectedRoles;
        private ICommand _saveUserViewCommand, _addNewUserViewCommand, _sendConfirmationEmailViewCommand;
        private ICommand _closeUserViewCommand;
        private bool _editCommandVisibility;
        private string _confirmationToken;

        #endregion  

        #region Constructor

        public ServerUserViewModel()
        {
            CleanUp();

            var iDbContext = new ServerDbContextFactory().Create();
            _unitOfWork = new UnitOfWorkServer(iDbContext);

            _userService = new UserService(iDbContext);

            Load();

            SelectedUser = new UserDTO();
            SelectedRole = new RoleDTO();
            SelectedRoleToAdd = new RoleDTO();

            Users = new ObservableCollection<UserDTO>();
            Roles = new ObservableCollection<RoleDTO>();
            SelectedRoles = new ObservableCollection<RoleDTO>();
            FilteredRoles = new ObservableCollection<RoleDTO>();

            GetLiveRoles();
            GetLiveUsers();
            
            EditCommandVisibility = false;
            NewPasswordExpandibility = false;

            //#region Web Security
            //const string serverName = ".";
            //const string sqlceFileName = "PinnaFaceDbWeb";
            //const string sQlServConString = "data source=" + serverName + ";initial catalog=" + sqlceFileName +
            //                                ";user id=sa;password=amihan";
            //const string connectionStringName = sQlServConString;
            //const string providerName = "System.Data.SqlClient";
            
            //if (!WebSecurity.Initialized)
            //    WebSecurity.InitializeDatabaseConnection(connectionStringName, providerName, "Users",
            //        "UserId", "UserName", false);
            //#endregion
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

        public string ConfirmationToken
        {
            get { return _confirmationToken; }
            set
            {
                _confirmationToken = value;
                RaisePropertyChanged<string>(() => ConfirmationToken);
            }
        }
        

        public UserDTO SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                RaisePropertyChanged<UserDTO>(() => SelectedUser);
                if (SelectedUser != null && SelectedUser.UserId != 0)
                {
                    try
                    {
                        var ct = "";
                        ConfirmationToken = "";
                        var member =
                            new UserService(true).GetAllMemberShips()
                                .FirstOrDefault(m => m.UserId == SelectedUser.UserId);
                        if (member != null) ct = member.ConfirmationToken;
                        var unEncrypted = SelectedUser.UserName;
                        if(!string.IsNullOrEmpty(ct))
                        {
                            ConfirmationToken = "http://www.pinnaface.com/Account/RegisterConfimation?ct=" +
                                    ct + "&un=" + unEncrypted + "";
                        }
                    }
                    catch 
                    {
                        
                    }

                    IList<RoleDTO> selectedRolesList = SelectedUser.Roles.Select(userroles => userroles.Role).ToList();
                    SelectedRoles = new ObservableCollection<RoleDTO>(selectedRolesList);
                    try
                    {
                        FilteredRoles = new ObservableCollection<RoleDTO>(Roles.Except(SelectedRoles));
                    }
                    catch
                    {
                    }
                    UserNameEnability = false;
                    EditCommandVisibility = true;

                    LoadUsersWithAgencyWithAgent();
                    
                    SelectedLocalAgency = null;
                    SelectedForeignAgent = null;

                    if (SelectedUser.AgencyId != null)
                    {
                        if (LocalAgencies != null)
                            SelectedLocalAgency = LocalAgencies.FirstOrDefault(a => a.Id == SelectedUser.AgencyId);
                    }
                    if (SelectedUser.AgentId != null)
                    {
                        if (ForeignAgents != null)
                            SelectedForeignAgent = ForeignAgents.FirstOrDefault(a => a.Id == SelectedUser.AgentId);
                    }
                }
                else
                {
                    UserNameEnability = true;
                    EditCommandVisibility = false;
                }
            }
        }

        private void LoadUsersWithAgencyWithAgent()
        {
            UsersWithAgencyWithAgent = new ObservableCollection<UserAgencyAgentDTO>();
            var usrsWithAgencyWithAgent = _unitOfWork.UserRepository<UserAgencyAgentDTO>().Query()
                .Include(u => u.AgencyAgent, u => u.AgencyAgent.Agency, u => u.AgencyAgent.Agent)
                .Get().Where(u => u.UserId == SelectedUser.UserId).ToList();

            if (usrsWithAgencyWithAgent.Any())
            {
                UsersWithAgencyWithAgent =
                    new ObservableCollection<UserAgencyAgentDTO>(usrsWithAgencyWithAgent);
            }
        }

        public UserDTO SelectedUserForSearch
        {
            get { return _selectedUserForSearch; }
            set
            {
                _selectedUserForSearch = value;
                RaisePropertyChanged<UserDTO>(() => this.SelectedUserForSearch);

                if (SelectedUserForSearch != null && !string.IsNullOrEmpty(SelectedUserForSearch.UserDetail))
                {
                    SelectedUser = SelectedUserForSearch;
                    SelectedUserForSearch.UserDetail = "";
                }
            }
        }

        public ObservableCollection<UserDTO> Users
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged<ObservableCollection<UserDTO>>(() => Users);

                if (Users.Count > 0)
                    SelectedUser = Users.FirstOrDefault();
                else
                    ExecuteAddNewUserViewCommand();
            }
        }

        public ObservableCollection<UserAgencyAgentDTO> UsersWithAgencyWithAgent
        {
            get { return _usersWithAgencyWithAgent; }
            set
            {
                _usersWithAgencyWithAgent = value;
                RaisePropertyChanged<ObservableCollection<UserAgencyAgentDTO>>(() => UsersWithAgencyWithAgent);
            }
        }

        public UserAgencyAgentDTO SelectedUserWithAgencyWithAgent
        {
            get { return _selectedUserWithAgencyWithAgent; }
            set
            {
                _selectedUserWithAgencyWithAgent = value;
                RaisePropertyChanged<UserAgencyAgentDTO>(() => SelectedUserWithAgencyWithAgent);
            }
        }
        #endregion

        #region Commands

        public ICommand AddNewUserViewCommand
        {
            get
            {
                return _addNewUserViewCommand ??
                       (_addNewUserViewCommand = new RelayCommand(ExecuteAddNewUserViewCommand));
            }
        }

        private void ExecuteAddNewUserViewCommand()
        {
            SelectedUser = new UserDTO
            {
                Status = UserTypes.Waiting,
                NewPassword = new Random().Next(123456, 134567).ToString(),
            };

            SelectedRoles = new ObservableCollection<RoleDTO>();
            GetLiveRoles();
            FilteredRoles = new ObservableCollection<RoleDTO>(Roles.Skip(5).ToList());
            UserNameEnability = true;

            AllRolesChecked = true;
            NewPasswordExpandibility = true;
            AddRoleEnability = false;
            RemoveRoleEnability = false;
        }

        public ICommand AddNewUserWithAgencyWithAgentViewCommand
        {
            get
            {
                return _addNewUserWithAgencyWithAgentViewCommand ??
                       (_addNewUserWithAgencyWithAgentViewCommand =
                           new RelayCommand(ExecuteAddNewUserWithAgencyWithAgentViewCommand));
            }
        }

        private void ExecuteAddNewUserWithAgencyWithAgentViewCommand()
        {
            new UserWithAgencyWithAgentVIew(SelectedUser).ShowDialog();
            LoadUsersWithAgencyWithAgent();
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
        private void ExecuteDeleteUserWithAgencyWithAgentViewCommand()
        {
            try
            {
                if (SelectedUserWithAgencyWithAgent != null)
                {
                    SelectedUserWithAgencyWithAgent.Enabled = false;
                    _unitOfWork.UserRepository<UserAgencyAgentDTO>().Update(SelectedUserWithAgencyWithAgent);
                    _unitOfWork.Commit();

                    LoadUsersWithAgencyWithAgent();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.Message);
            }
        }
        public ICommand SaveUserViewCommand
        {
            get
            {
                return _saveUserViewCommand ??
                       (_saveUserViewCommand = new RelayCommand(ExecuteSaveUserViewCommand, CanSave));
            }
        }

        private void ExecuteSaveUserViewCommand()
        {
            try
            {
                if (Users.Any(u => u.UserName == SelectedUser.UserName
                                   && u.UserId != SelectedUser.UserId))
                {
                    MessageBox.Show("There exist user with same username, try another username... ");
                    return;
                }
                SelectedUser.Roles = new List<UsersInRoles>();
                foreach (var role in SelectedRoles)
                {
                    SelectedUser.Roles.Add(new UsersInRoles {UserId = SelectedUser.UserId, Role = role});
                }

                if (!string.IsNullOrEmpty(SelectedUser.NewPassword))
                {
                    SelectedUser.Password = SelectedUser.NewPassword;
                    SelectedUser.ConfirmPassword = SelectedUser.NewPassword;
                    //SelectedUser.TempPassword = SelectedUser.NewPassword;
                }

                SelectedUser.DateLastModified = DateTime.Now;
                
                var dbContext2 = new ServerDbContextFactory().Create();

                
                if (SelectedUser.UserId == 0)
                {
                    SelectedUser.DateRecordCreated = DateTime.Now;

                    WebSecurity.CreateUserAndAccount(SelectedUser.UserName, SelectedUser.Password,
                        new
                        {
                            Status = 0,
                            Enabled = true,
                            RowGuid = Guid.NewGuid(),
                            Email = SelectedUser.Email ?? "",
                            CreatedByUserId = 1,
                            DateRecordCreated = DateTime.Now,
                            ModifiedByUserId = 1,
                            DateLastModified = DateTime.Now
                        }, 
                        true);
                    
                    SelectedUser.UserId = WebSecurity.GetUserId(SelectedUser.UserName);


                    var membershipDto = dbContext2.Set<MembershipDTO>().ToList()
                        .FirstOrDefault(m => m.UserId == SelectedUser.UserId);

                    if (membershipDto != null)
                    {
                        membershipDto.RowGuid = Guid.NewGuid();
                        membershipDto.Enabled = true;
                        membershipDto.CreatedByUserId = 1;
                        membershipDto.DateRecordCreated = DateTime.Now;
                        membershipDto.ModifiedByUserId = 1;
                        membershipDto.DateLastModified = DateTime.Now;
                        dbContext2.Set<MembershipDTO>().Add(membershipDto);
                        dbContext2.Entry(membershipDto).State = EntityState.Modified;

                        dbContext2.SaveChanges();
                    }
                }
                try
                {
                    

                    if (SelectedLocalAgency != null)
                        SelectedUser.AgencyId = SelectedLocalAgency.Id;
                    else if (SelectedForeignAgent != null)
                        SelectedUser.AgentId = SelectedForeignAgent.Id;
                    
                    var lofRoles = SelectedRoles; // _unitOfWork.UserRepository<RoleDTO>().Query().Get().ToList();
                    foreach (var role in lofRoles)
                    {
                        dbContext2.Set<UsersInRoles>().Add(new UsersInRoles
                        {
                            RoleId = role.RoleId,
                            UserId = WebSecurity.GetUserId(SelectedUser.UserName)
                        });
                    }
                    dbContext2.SaveChanges();

                }
                catch(Exception ex)
                {
                }


                _unitOfWork.Commit();

                GetLiveUsers();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public ICommand SendConfirmationEmailViewCommand
        {
            get
            {
                return _sendConfirmationEmailViewCommand ??
                       (_sendConfirmationEmailViewCommand = new RelayCommand(ExecuteSendConfirmationEmailViewCommand));
            }
        }

        private void ExecuteSendConfirmationEmailViewCommand()
        {
           RegisterConfimation(SelectedUser.UserName);
        }

        public void RegisterConfimation(string userName)
        {
            var ct = "";
            Singleton.Edition = PinnaFaceEdition.WebEdition;
            using (var dbcon = DbContextUtil.GetDbContextInstance())
            {
                var user = dbcon.Set<UserDTO>().FirstOrDefault(m => m.UserName == userName);
                
                var member = dbcon.Set<MembershipDTO>().FirstOrDefault(m => m.UserId == user.UserId);
                if (member != null) ct = member.ConfirmationToken;

                if (string.IsNullOrEmpty(ct)) MessageBox.Show("Can't Get ConfirmationToken");
            }

            var unEncrypted = userName;// EncryptionUtility.Encrypt(userName);
            var confirmAddress = "http://www.pinnaface.com/Account/RegisterConfimation?ct=" +
                                 ct + "&un=" + unEncrypted + "";

            var confirmLink = "<a href='" + confirmAddress + "'>" + confirmAddress + "</a>";

            //get user emailid
            var email = new UserService(true).GetByName(userName).Email; // user.Email;


            //send mail
            const string subject = "PinnaFace - Activate Account";
            //var body = "<b>Please find the Account Confirmation Token</b><br/>" + confirmLink; //edit it

            var sr = new System.IO.StreamReader("../../Views/ConfirmAccount.html");
            string body = sr.ReadToEnd();
            body = body.Replace("ConfirmationToken", confirmAddress);
            body = body.Replace("#ConfirmAccount#", confirmAddress);
            

            var sentStatus = EmailUtil.SendEMail(email, subject, body);
           
            if(string.IsNullOrEmpty(sentStatus))
            MessageBox.Show("Mail Sent.");
            else MessageBox.Show("Error occured while sending email." + sentStatus);
        }

        //public void RegisterConfirmation()
        //{
        //    var fromAddress = new MailAddress("something@gmail.com", "Name");

        //    const string fromPassword = "pwd123";
        //    const string subject = "System generated test mail ";
        //    //string email = bind_email(analyst);

        //    var confirmLink =
        //        "<a href='http://www.pinnaface.com/Account/RegisterConfimation?ct=" +
        //        ct + "&un=" + unEncrypted + "'>Confirm Account</a>";

        //    System.IO.StreamReader sr = new System.IO.StreamReader("~/Views/ConfirmAccount.html");
        //    string body1 = sr.ReadToEnd();
        //    body1 = body1.Replace("#ConfirmationToken#", confirmLink.ToString());
            

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //    };
        //    using (var message = new MailMessage()
        //    {
        //        From = fromAddress,
        //        Subject = subject,
        //        Body = body1,
        //        IsBodyHtml = true
        //    })
        //    {
        //        message.To.Add(email);
        //        smtp.Send(message);
        //    }
        //}
        public ICommand CloseUserViewCommand
        {
            get
            {
                return _closeUserViewCommand ??
                       (_closeUserViewCommand = new RelayCommand<Object>(ExecuteCloseUserViewCommand));
            }
        }

        private void ExecuteCloseUserViewCommand(object obj)
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

        #region Roles

        private ObservableCollection<RoleDTO> _roles, _filteredRoles;
        private bool _addRoleEnability, _removeRoleEnability, _allRolesChecked, _userNameEnability;
        private string _noOfTotalRolesGiven;

        public RoleDTO SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                RaisePropertyChanged<RoleDTO>(() => SelectedRole);
                RemoveRoleEnability = SelectedRole != null;
            }
        }

        public RoleDTO SelectedRoleToAdd
        {
            get { return _selectedRoleToAdd; }
            set
            {
                _selectedRoleToAdd = value;
                RaisePropertyChanged<RoleDTO>(() => SelectedRoleToAdd);

                AddRoleEnability = SelectedRoleToAdd != null;
            }
        }

        public ObservableCollection<RoleDTO> SelectedRoles
        {
            get { return _selectedRoles; }
            set
            {
                _selectedRoles = value;
                RaisePropertyChanged<ObservableCollection<RoleDTO>>(() => SelectedRoles);

                if (SelectedRoles != null && Roles != null)
                    NoOfTotalRolesGiven = SelectedRoles.Count.ToString("N0") + " of " + Roles.Count.ToString("N0") + " Roles";
            }
        }

        public ObservableCollection<RoleDTO> Roles
        {
            get { return _roles; }
            set
            {
                _roles = value;
                RaisePropertyChanged<ObservableCollection<RoleDTO>>(() => Roles);
            }
        }

        public ObservableCollection<RoleDTO> FilteredRoles
        {
            get { return _filteredRoles; }
            set
            {
                _filteredRoles = value;
                RaisePropertyChanged<ObservableCollection<RoleDTO>>(() => FilteredRoles);
            }
        }

        public bool AddRoleEnability
        {
            get { return _addRoleEnability; }
            set
            {
                _addRoleEnability = value;
                RaisePropertyChanged<bool>(() => AddRoleEnability);
            }
        }

        public bool UserNameEnability
        {
            get { return _userNameEnability; }
            set
            {
                _userNameEnability = value;
                RaisePropertyChanged<bool>(() => UserNameEnability);
            }
        }

        public bool RemoveRoleEnability
        {
            get { return _removeRoleEnability; }
            set
            {
                _removeRoleEnability = value;
                RaisePropertyChanged<bool>(() => RemoveRoleEnability);
            }
        }

        public bool AllRolesChecked
        {
            get { return _allRolesChecked; }
            set
            {
                _allRolesChecked = value;
                RaisePropertyChanged<bool>(() => AllRolesChecked);

                try
                {
                    if (AllRolesChecked)
                    {
                        SelectedRoles = new ObservableCollection<RoleDTO>(Roles);
                        FilteredRoles = new ObservableCollection<RoleDTO>();
                    }
                    else
                    {
                        SelectedRoles = new ObservableCollection<RoleDTO>();
                        FilteredRoles = new ObservableCollection<RoleDTO>(Roles.Except(SelectedRoles));
                    }
                }
                catch
                {
                    MessageBox.Show("Can't Remove Role");
                }
            }
        }

        public string NoOfTotalRolesGiven
        {
            get { return _noOfTotalRolesGiven; }
            set
            {
                _noOfTotalRolesGiven = value;
                RaisePropertyChanged<string>(() => NoOfTotalRolesGiven);

            }
        }

        public ICommand AddRoleViewCommand
        {
            get
            {
                return _addRoleViewCommand ?? (_addRoleViewCommand = new RelayCommand(ExcuteAddRoleViewCommand, CanSave));
            }
        }

        private void ExcuteAddRoleViewCommand()
        {
            try
            {
                SelectedRoles.Add(SelectedRoleToAdd);
                FilteredRoles = new ObservableCollection<RoleDTO>(Roles.Except(SelectedRoles));
            }
            catch
            {
                MessageBox.Show("Can't Save Role");
            }
        }

        public ICommand RemoveRoleViewCommand
        {
            get
            {
                return _deleteRoleViewCommand ??
                       (_deleteRoleViewCommand = new RelayCommand(ExecuteRemoveRoleViewCommand));
            }
        }

        private void ExecuteRemoveRoleViewCommand()
        {
            try
            {
                SelectedRoles.Remove(SelectedRole);
                FilteredRoles = new ObservableCollection<RoleDTO>(Roles.Except(SelectedRoles));
            }
            catch
            {
                MessageBox.Show("Can't Remove Role");
            }
        }

        #endregion

        #region LocalAgencies

        private ObservableCollection<AgencyDTO> _localAgencies;
        private AgencyDTO _selectedLocalAgency;

        public AgencyDTO SelectedLocalAgency
        {
            get { return _selectedLocalAgency; }
            set
            {
                _selectedLocalAgency = value;
                RaisePropertyChanged<AgencyDTO>(() => SelectedLocalAgency);
                //RemoveLocalAgencyEnability = SelectedLocalAgency != null;
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
        
        #endregion

        #region ForeignAgents

        private ObservableCollection<AgentDTO> _foreignAgents;
        private AgentDTO _selectedForeignAgent;

        public AgentDTO SelectedForeignAgent
        {
            get { return _selectedForeignAgent; }
            set
            {
                _selectedForeignAgent = value;
                RaisePropertyChanged<AgentDTO>(() => SelectedForeignAgent);
                //RemoveForeignAgentEnability = SelectedForeignAgent != null;
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
        #endregion

        #region Load Agency Agents
        private void Load()
        {
            var localAgencies = _unitOfWork.Repository<AgencyDTO>().Query()
                         .Include(u => u.Address)
                         .Get().ToList();
            LocalAgencies = new ObservableCollection<AgencyDTO>(localAgencies);


            var foreignAgentDtos = _unitOfWork.Repository<AgentDTO>().Query()
                        .Include(u => u.Address)
                        .Get().ToList();
            ForeignAgents = new ObservableCollection<AgentDTO>(foreignAgentDtos);
        } 
        #endregion

        #region GetNewPassword

        private ICommand _getNewPassword;

        public ICommand GetNewPassword
        {
            get { return _getNewPassword ?? (_getNewPassword = new RelayCommand(ExcuteGetNewPassword)); }
        }
        private void ExcuteGetNewPassword()
        {
            SelectedUser.TempPassword = new Random().Next(123456, 134567).ToString();

            if (SelectedUser.UserId != 0)
            {
                var userName = SelectedUser.UserName;
                var token = WebSecurity.GeneratePasswordResetToken(userName);
                WebSecurity.ResetPassword(token, SelectedUser.TempPassword);

                using (var dbCon = new ServerDbContextFactory().Create())
                {
                    var usr = dbCon.Set<UserDTO>().FirstOrDefault(u => u.UserId == SelectedUser.UserId);
                    if (usr != null)
                    {
                        usr.TempPassword = SelectedUser.TempPassword;

                        dbCon.Set<UserDTO>().Add(usr);
                        dbCon.Entry(usr).State = EntityState.Modified;

                        dbCon.SaveChanges();
                    }
                }
            }
        }


        private bool _newPasswordExpandibility;
        private ICommand _deleteRoleViewCommand;

        public bool NewPasswordExpandibility
        {
            get { return _newPasswordExpandibility; }
            set
            {
                _newPasswordExpandibility = value;
                RaisePropertyChanged<bool>(() => NewPasswordExpandibility);
            }
        }

        #endregion

        #region Load Users Roles

        private void GetLiveUsers()
        {
            UserNameEnability = false;
            var usrs = _unitOfWork.UserRepository<UserDTO>().Query()
                .Include(u => u.Agency, u => u.Agent, u => u.AgenciesWithAgents)
                .Get().ToList();

            int sNo = 1;
            foreach (var userDto in usrs)
            {
                userDto.SerialNumber = sNo;

                UserDTO dto = userDto;
                var mem =
                    _unitOfWork.UserRepository<MembershipDTO>()
                        .Query()
                        .Filter(u => u.UserId == dto.UserId)
                        .Get()
                        .FirstOrDefault();
                
                userDto.Membership = mem ?? new MembershipDTO();

                sNo++;
            }
            
            Users = new ObservableCollection<UserDTO>(usrs.ToList());
        }

        private void GetLiveRoles()
        {
            var len = PinnaFace.Core.CommonUtility.GetRolesList().Count();
            Roles = new ObservableCollection<RoleDTO>(_userService.GetAllRoles().ToList().OrderBy(i => i.RoleId));
            Roles = new ObservableCollection<RoleDTO>(Roles.Take(len));
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