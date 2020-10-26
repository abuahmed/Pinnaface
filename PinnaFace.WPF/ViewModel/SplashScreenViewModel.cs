using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web.Security;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
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
using PinnaKeys.OA;
using WebMatrix.WebData;
using Settings = PinnaFace.WPF.Properties.Settings;

namespace PinnaFace.WPF.ViewModel
{
    public class SplashScreenViewModel : ViewModelBase
    {
        #region Fields

        private static IUnitOfWork _unitOfWork;
        private ActivationModel _activationModel;
        private bool _activations;
        private bool _blockedKey;
        private Exception _exceptions;
        private bool _higherDbVersion, _higherSystemVersion;
        private string _licensedTo;
        private bool _login;
        private object _splashWindow;
        private bool _suspeciousActivation;

        #endregion

        #region Constructor

        public SplashScreenViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            _exceptions = null;
            Messenger.Default.Register<object>(this, message => { SplashWindow = message; });

            //try
            //{
            //    RegistryKey reg = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            //    reg.SetValue("PinnaFace.SyncEngine", System.Windows.Forms.Application.ExecutablePath);
            //}
            //catch
            //{

            //}

        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Properties

        public object SplashWindow
        {
            get { return _splashWindow; }
            set
            {
                _splashWindow = value;
                RaisePropertyChanged<object>(() => SplashWindow);
                if (SplashWindow != null)
                    CheckActivation();
            }
        }

        public string LicensedTo
        {
            get { return _licensedTo; }
            set
            {
                _licensedTo = value;
                RaisePropertyChanged<string>(() => LicensedTo);
            }
        }

        #endregion

        #region Actions

        private ICommand _closeSplashView;

        public ICommand CloseSplashView
        {
            get { return _closeSplashView ?? (_closeSplashView = new RelayCommand<Object>(CloseWindow)); }
        }

        private void CheckActivation()
        {
            //var machine = Environment.MachineName;
            //var user = Environment.UserName;
            //Dates dt = new Dates();
            //var bdate = dt.HijriToGreg("19/09/1439", "dd/MM/yyyy");//"02/06/2018"
            //var dat = CalendarUtil.GetEthCalendarFormated(new DateTime(2018, 6, 2), "/");//25/09/2010

            //var deliveryDate = new DateTime(2018, 6, 2).AddMonths(9);//02/03/2019
            //var delDate = CalendarUtil.GetEthCalendarFormated(deliveryDate, "/");//23/06/2011

            //var dir = new DirectoryInfo(Environment.CurrentDirectory);
            //IEnumerable<FileInfo> fileList = dir.GetFiles("*.*", SearchOption.TopDirectoryOnly);
            //var component = "";
            //foreach (FileInfo fileInfo in fileList)
            //{
            //    string fileName = fileInfo.Name.Replace(fileInfo.Extension, "");// + "  " + fileInfo.Extension;

            //    if (fileInfo.Extension == ".xml" || fileInfo.Extension == ".pdb")
            //        continue;
            //    component = component + Environment.NewLine +
            //        //"<ComponentRef Id='" + fileName + "'/>";
            //        "<Component Id='" + fileName + "' Guid='" + Guid.NewGuid() + "'>" +
            //    "<File Source='$(var.PinnaFace.WPF.TargetDir)" + fileInfo.Name + "' Name='" + fileInfo.Name + "'" +
            //    " Id='" + fileName + "DLL' KeyPath='yes'/>" +
            //    "</Component>";
            //}

            try
            {
                //Singleton.BuildType=BuildType.LocalDev;
                //var versionNum = DbCommandUtil.GetCurrentDbVersion();
                var appLoc = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\PinnaSofts";
                var dir = new DirectoryInfo(appLoc);
                if(dir.Exists)
                dir.Delete(true);

                var appLoc2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PinnaSofts";
                var dir2 = new DirectoryInfo(appLoc2);
                if (dir2.Exists)
                    dir2.Delete(true);

                var appLocData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Apps\\2.0\\Data";
                var dirData = new DirectoryInfo(appLocData);
                IEnumerable<FileInfo> fileList = dirData.GetFiles("user.config", SearchOption.AllDirectories);
                foreach (FileInfo fileInfo in fileList)
                {
                    fileInfo.Delete();
                }
            }
            catch 
            {
            }
            
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        public static void testSingle()
        {
            var instance = Singleton.Instance;
            var aa = instance.SystemVersionDate;
        }
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var instance = Singleton.Instance;
                //Check Database Vs System Version
                _activationModel = ValidateProduct();

                if (_activationModel != null
                    && _activationModel.DatabaseVersionDate != 0
                    && _activationModel.MaximumSystemVersion != 0)
                {
                    if (instance.SystemVersionDate < _activationModel.DatabaseVersionDate)
                        _higherDbVersion = true;

                    if (Singleton.SystemVersionDate > _activationModel.MaximumSystemVersion)
                        _higherSystemVersion = true;
                }

                if (!_higherDbVersion && !_higherSystemVersion)
                {
                    ProductActivationDTO activation =
                        _unitOfWork.Repository<ProductActivationDTO>().Query().Get().FirstOrDefault();

                    if (activation == null)
                    {
                        _activations = true;
                    }
                    else if (activation.KeyStatus == KeyStatus.Blocked)
                    {
                        _blockedKey = true;
                    }
                    else
                    {
                        LicensedTo = activation.LicensedTo;
                        //Thread.Sleep(1000); //To show to whom the license belongs
                        if (activation.RegisteredBiosSn.Contains(new ProductActivationDTO().BiosSn))
                        {
                            //Check if this product is expired and also check if it is Renewed on the server
                            //Otherwise close the app OR disable features
                            if (activation.KeyStatus == KeyStatus.Expired || DateTime.Now > activation.ExpiryDate)
                            {
                                ActivationKey key = CheckProductRenewalOnActivationServer(activation.ProductKey);
                                if (key != null)
                                {
                                    if (key.ExpiryDate != activation.ExpiryDate)
                                    {
                                        activation.ExpiryDate = key.ExpiryDate;
                                        activation.LastRenewedDate = key.LastRenewedDate;
                                        activation.KeyStatus = KeyStatus.Active;
                                    }
                                    else
                                    {
                                        activation.KeyStatus = KeyStatus.Expired;
                                    }
                                }
                                else activation.KeyStatus = KeyStatus.Expired;


                                _unitOfWork.Repository<ProductActivationDTO>().InsertUpdate(activation);
                                _unitOfWork.Commit();
                            }
                            else if (activation.KeyStatus == KeyStatus.Active)
                            {
                                Singleton.ProductActivation = activation;
                                _login = true;
                            }
                        }
                        else
                        {
                            if (Singleton.Edition == PinnaFaceEdition.ServerEdition)
                            {
                                _activations = true;
                            }
                            else _suspeciousActivation = true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _exceptions = exception;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_login)
            {
                DirectLogin();
                //new Login().Show();
            }
            else if (_activations)
                new Activations().Show();
            else if (_suspeciousActivation)
            {
                if (MessageBox.Show(
                    "The Product has already been activated on another computer, Do you want to reset the Key",
                    "Suspicious Activation", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                {
                    ProductActivationDTO activation =
                        _unitOfWork.Repository<ProductActivationDTO>().Query().Get().FirstOrDefault();
                    _unitOfWork.Repository<ProductActivationDTO>().Delete(activation);
                    _unitOfWork.Commit();

                    new Activations().Show();
                }
            }
            else if (_higherDbVersion)
            {
                MessageBox.Show(
                    "Database version is Newer than the system version, " + Environment.NewLine +
                    "Database Version: " + _activationModel.DatabaseVersionDate + Environment.NewLine +
                    "Installed System Version: " + Singleton.SystemVersionDate + Environment.NewLine +
                    "Please get latest system version",
                    "Old System Version", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (_higherSystemVersion)
            {
                MessageBox.Show(
                    "You are not allowed to use this version of the system, " + Environment.NewLine +
                    "Allowed system Version: " + _activationModel.DatabaseVersionDate + Environment.NewLine +
                    "Installed System Version: " + Singleton.SystemVersionDate + Environment.NewLine +
                    "activate your product or contact PINNAFACE OFFICE",
                    "Newer System Version", MessageBoxButton.OK, MessageBoxImage.Error);
                new Activations().Show();
            }
            else if (_blockedKey)
                MessageBox.Show("Your acount is Blocked!",
                    "Blocked Key",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            else if (_exceptions != null)
            {
                MessageBox.Show("Got Exception While Starting The System On Splash Screen", "Got Exception",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                LogUtil.LogError(ErrorSeverity.Critical, "CheckActivation()", _exceptions.Message + Environment.NewLine + _exceptions.InnerException,"","");
            }
            else
                MessageBox.Show(
                    Singleton.Edition == PinnaFaceEdition.ServerEdition
                        ? "Problem opening oneface, may be the server computer or the network not working properly! try again later.."
                        : "Problem opening oneface! try again later..", "Error Opening",
                    MessageBoxButton.OK, MessageBoxImage.Error);

            CloseWindow(SplashWindow);
        }

        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.Close();
        }

        

        private void DirectLogin()
        {
            try
            {
                InitializeObjects.InitializeWebSecurity();

                var currentSetting = XmlSerializerCustom.GetUserSetting();
                var userDTO = new UserDTO
                {
                    UserName = currentSetting.UserName,
                    Password = currentSetting.Password
                };

                var userExists = false;

                if (!string.IsNullOrEmpty(userDTO.UserName) && !string.IsNullOrEmpty(userDTO.Password))
                    userExists = Membership.ValidateUser(userDTO.UserName, userDTO.Password);

                if (!userExists)
                {
                    new Login().Show();
                }
                else
                {
                    Singleton.Agency = new LocalAgencyService(true).GetLocalAgency();
                    int userId = WebSecurity.GetUserId(userDTO.UserName);
                    UserDTO user = new UserService(true).GetUser(userId);

                    if (user == null)
                    {
                        MessageBox.Show("Incorrect UserName/Password", "Error Logging",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        userDTO.Password = "";
                    }
                    else
                    {
                        Singleton.User = user;
                        Singleton.User.Password = userDTO.Password;
                        Singleton.UserRoles = new UserRolesModel();

                        Singleton.Setting = new UnitOfWork(DbContextUtil.GetDbContextInstance())
                            .Repository<SettingDTO>()
                            .FindById(1);


                        switch (user.Status)
                        {
                            case UserTypes.Waiting:
                                new ChangePassword(userDTO.Password).Show();
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
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.InnerException);
            }
        }

        #endregion

        public bool CheckCurrentDateTimeWithTheServer()
        {
            return true;
        }

        public ActivationModel ValidateProduct()
        {
            //var dt = DbCommandUtil.GetCurrentSqlDate(true);
            //var loc = dt.ToLocalTime();

            return Singleton.Edition == PinnaFaceEdition.CompactEdition
                ? DbCommandUtil.ValidateProductSqlCe()
                : DbCommandUtil.ValidateProductSql();
        }

        private ActivationKey CheckProductRenewalOnActivationServer(string productKey)
        {
            try
            {
                string connectionStringName = DbCommandUtil.GetActivationConnectionString();

                var dbContext = new EntitiesModel(connectionStringName);
                ActivationKey key = dbContext.ActivationKeys
                    .FirstOrDefault(a => a.ProductKey == productKey
                                         && a.KeyStatus == 0 && a.ProductType == 0);
                return key;
            }
            catch
            {
                return null;
            }
        }
    }

    public static class XmlSerializerCustom
    {
        public static bool SetUserSetting(UserSettingXml usersToStore)
        {
            try
            {
                string filepath = Path.Combine(PathUtil.GetFolderPath(), "userSettingsXml.xml");
                if (File.Exists(filepath))
                    File.Delete(filepath);
                using (var fs = new FileStream(filepath, FileMode.OpenOrCreate))
                {
                    var serializer = new XmlSerializer(usersToStore.GetType());
                    serializer.Serialize(fs, usersToStore);
                }
                return true;
            }
            catch 
            {
                return false;
            }

        }
        public static UserSettingXml GetUserSetting()
        {
            try
            {
                string filepath = Path.Combine(PathUtil.GetFolderPath(), "userSettingsXml.xml");
                if (!File.Exists(filepath))
                {
                    var set = new UserSettingXml();
                    SetUserSetting(set);
                    return set;
                }
                else
                {
                    var retrievedUsers = new UserSettingXml();
                    using (var fs2 = new FileStream(filepath, FileMode.Open))
                    {
                        var serializer = new XmlSerializer(retrievedUsers.GetType());
                        retrievedUsers = serializer.Deserialize(fs2) as UserSettingXml;
                    }

                    return retrievedUsers;
                }
            }
            catch
            {
                return new UserSettingXml();
            }
        }
    }

    public class UserSettingXml
    {
        public UserSettingXml()
        {
            ListType = 0;
            UserName = "";
            Password = "";
        }
        public int ListType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}