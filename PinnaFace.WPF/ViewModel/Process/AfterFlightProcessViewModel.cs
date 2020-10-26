using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class AfterFlightProcessViewModel : ViewModelBase
    {
        #region Fields

        private string _headerText;
        private ICommand _saveFlightProcessViewCommand;
        private EmployeeDTO _selectedEmployee;

        #endregion

        #region Constructor

        public AfterFlightProcessViewModel()
        {
            CleanUp();

            Messenger.Default.Register<EmployeeDTO>(this, message => { SelectedEmployee = message; });
            
        }

        public static void CleanUp()
        {
        }

        #endregion

        private ICommand _submitDateViewCommand;

        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);
                if (SelectedEmployee != null)
                {
                    HeaderText = SelectedEmployee.FullName + " - " + SelectedEmployee.PassportNumber;
                    if (SelectedEmployee.AfterFlightStatusDate == null)
                        SelectedEmployee.AfterFlightStatusDate = DateTime.Now;
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

        public ICommand SaveFlightProcessCommand
        {
            get
            {
                return _saveFlightProcessViewCommand ??
                       (_saveFlightProcessViewCommand =
                           new RelayCommand<object>(ExecuteSaveFlightProcessViewCommand, CanSave));
            }
        }

        public ICommand SubmitDateViewCommand
        {
            get
            {
                return _submitDateViewCommand ??
                       (_submitDateViewCommand = new RelayCommand(ExcuteSubmitDate));
            }
        }

        private void ExecuteSaveFlightProcessViewCommand(object obj)
        {
            try
            {
                SelectedEmployee.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.DateLastModified = DateTime.Now;
                CloseWindow(obj);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ExcuteSubmitDate()
        {
            if (SelectedEmployee.AfterFlightStatusDate == null)
                SelectedEmployee.AfterFlightStatusDate = DateTime.Now;

            var calConv = new Calendar(SelectedEmployee.AfterFlightStatusDate.Value);
            calConv.ShowDialog();
            bool? dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedEmployee.AfterFlightStatusDate = calConv.DtSelectedDate.SelectedDate;
            }
        }

        private void CloseWindow(object obj)
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