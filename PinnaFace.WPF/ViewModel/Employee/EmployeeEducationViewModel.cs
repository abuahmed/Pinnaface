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
    public class EmployeeEducationViewModel : ViewModelBase
    {
        #region Fields
        private EmployeeDTO _selectedEmployee;
        private ICommand _saveEmployeeEducationViewCommand;
        #endregion

        #region Constructor
        public EmployeeEducationViewModel()
        {
            CleanUp();
            
            Messenger.Default.Register<EmployeeDTO>(this, message =>
            {
                SelectedEmployee = message;
            });
            
        }
        public static void CleanUp()
        {
        }
        #endregion

        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);
                if (SelectedEmployee != null)
                {
                    if(SelectedEmployee.Education==null)
                        SelectedEmployee.Education=new EmployeeEducationDTO
                        {
                            ArabicLanguage = LanguageExperience.Poor,
                            EnglishLanguage = LanguageExperience.Poor
                        };
                }
            }
        }


        public ICommand SaveEmployeeEducationCommand
        {
            get { return _saveEmployeeEducationViewCommand ?? (_saveEmployeeEducationViewCommand = new RelayCommand<Object>(ExecuteSaveEmployeeEducationViewCommand, CanSave)); }
        }
        private void ExecuteSaveEmployeeEducationViewCommand(object obj)
        {
            try
            {
                SelectedEmployee.Education.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.Education.DateLastModified = DateTime.Now;
                CloseWindow(obj);
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
