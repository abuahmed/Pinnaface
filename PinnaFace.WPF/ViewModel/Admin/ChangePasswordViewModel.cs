﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Models;
using PinnaFace.WPF.Views;
using WebMatrix.WebData;

namespace PinnaFace.WPF.ViewModel
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        #region Fields
        private UserDTO _user;
        private ICommand _changePasswordCommand;
        private ICommand _closeChangePasswordView;
        private ChangePasswordModel _changePassword;
        private bool _loadMainWindow;
        private static IUserService _userService;
        private string _oldPasswordVisibility;
        #endregion

        #region Constructor
        public ChangePasswordViewModel()
        {
            CleanUp();
            _userService = new UserService();
            ChangePassword = new ChangePasswordModel();
            User = _userService.GetUser(Singleton.User.UserId);
        }
        public static void CleanUp()
        {
            if (_userService != null)
                _userService.Dispose();
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
                if (User != null && User.Status == UserTypes.Waiting)
                    LoadMainWindow = true;
                else
                    LoadMainWindow = false;
            }
        }
        public ChangePasswordModel ChangePassword
        {
            get { return _changePassword; }
            set
            {
                _changePassword = value;
                RaisePropertyChanged<ChangePasswordModel>(() => ChangePassword);
            }
        }
        public bool LoadMainWindow
        {
            get { return _loadMainWindow; }
            set
            {
                _loadMainWindow = value;
                RaisePropertyChanged<bool>(() => LoadMainWindow);
                if (LoadMainWindow)
                {
                    ChangePassword.OldPassword = Singleton.User.Password;
                    OldPasswordVisibility = "Collapsed";
                }
                else
                {
                    ChangePassword.OldPassword = "";
                    OldPasswordVisibility = "Visible";
                }
            }
        }

        
        public string OldPasswordVisibility
        {
            get { return _oldPasswordVisibility; }
            set
            {
                _oldPasswordVisibility = value;
                RaisePropertyChanged<string>(() => OldPasswordVisibility);
            }
        }

        #endregion

        #region Commands
        public ICommand ChangePasswordCommand
        {
            get
            {
                return _changePasswordCommand ?? (_changePasswordCommand = new RelayCommand<Object>(ExcuteChangePasswordCommand, CanSave));
            }
        }
        private void ExcuteChangePasswordCommand(object obj)
        {
            var values = (object[])obj;
            var oldPsdBox = values[0] as PasswordBox;
            var psdBox = values[1] as PasswordBox;
            var confirmPsdBox = values[2] as PasswordBox;
            //Do Validation if not handled on the UI
            if (confirmPsdBox != null && (psdBox != null && psdBox.Password != confirmPsdBox.Password))
            {
                MessageBox.Show("Passwords doesn't match");
                confirmPsdBox.Password = "";
                confirmPsdBox.Focus();
                return;
            }
            if (!LoadMainWindow)
                if (oldPsdBox != null) ChangePassword.OldPassword = oldPsdBox.Password;
            if (psdBox != null) ChangePassword.Password = psdBox.Password;
            if (confirmPsdBox != null) ChangePassword.ConfirmPassword = confirmPsdBox.Password;

            if (ChangePassword.OldPassword == ChangePassword.Password)
            {
                MessageBox.Show("Old Password same as the New Password, add a new Password");
                return;
            }

            //var oldpassword = ChangePassword.OldPassword;
            //if (LoadMainWindow)
            //    oldpassword = ChangePassword.OldPassword;

            var user = _userService.GetUser(Singleton.User.UserId); // UnitOfWork.Repository<UserDTO>().FindById(Singleton.User.Id); //we may not need this line
            
            try
            {
                user.Status = UserTypes.Active;//may needs checking...
                user.DateLastModified = DateTime.Now;

                if (!WebSecurity.ChangePassword(user.UserName, ChangePassword.OldPassword, ChangePassword.Password))
                {
                    MessageBox.Show("Can't Change Password, try again...", "Error Changing Password", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (oldPsdBox == null) return;
                    oldPsdBox.Password = "";
                    oldPsdBox.Focus();
                }
                _userService.InsertOrUpdate(user);

                if (LoadMainWindow)
                    new MainWindow().Show();
                CloseWindow(values[3]);
            }
            catch
            {
                MessageBox.Show("Can't Change Password, try again...", "Error Changing Password", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public ICommand CloseChangePasswordView
        {
            get
            {
                return _closeChangePasswordView ?? (_closeChangePasswordView = new RelayCommand<Object>(CloseWindow));
            }
        }
        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
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
