using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;

namespace PinnaFace.WPF.ViewModel
{
    public class EmployeeHawalaViewModel : ViewModelBase
    {
        #region Fields

        private string _headerText;
        private EmployeeDTO _selectedEmployee;

        #endregion

        #region Constructor

        public EmployeeHawalaViewModel()
        {
            CleanUp();
            
            Messenger.Default.Register<EmployeeDTO>(this, message => { SelectedEmployee = message; });
        }

        public static void CleanUp()
        {
        }

        #endregion

        #region Properties

        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);

                HeaderText = SelectedEmployee.FullName + " - " + SelectedEmployee.PassportNumber;

                if (SelectedEmployee != null)
                {
                    if (SelectedEmployee.Hawala == null)
                    SelectedEmployee.Hawala = new EmployeeHawalaDTO()
                    {
                        BankName = BankList.Cbe,
                        SwiftCode = SwiftCodeList.Cbe
                    };
                }
            }
        }

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }

        #endregion

        #region Commands

        private ICommand _saveHawalaCommand, _closeCommand;

        public ICommand SaveEmployeeHawalaCommand
        {
            get
            {
                return _saveHawalaCommand ??
                       (_saveHawalaCommand = new RelayCommand<Object>(ExcuteSaveEmployeeHawalaCommand, CanSave));
            }
        }

        private void ExcuteSaveEmployeeHawalaCommand(object obj)
        {
            try
            {
                SelectedEmployee.Hawala.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.Hawala.DateLastModified = DateTime.Now;
                CloseWindow(obj);
            }

            catch
            {
                MessageBox.Show("Can't Save Hawala!");
            }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand<Object>(CloseWindow)); }
        }

        public void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
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