using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class ComplainSolutionViewModel : ViewModelBase
    {
        #region Fields
        private static IComplainService _complainService;
        private static IEmployeeService _employeeService;

        private EmployeeDTO _selectedEmployee;
        private int? _employeeId;
        private ComplainDTO _selectedComplain;

        private ICommand _complainDateViewCommand,
            _saveComplainViewCommand;
        private bool _editCommandVisibility;
       
        #endregion

        #region Constructor
        public ComplainSolutionViewModel()
        {
            CleanUp();
            _complainService = new ComplainService();
            _employeeService = new EmployeeService(false, true);

            Messenger.Default.Register<EmployeeDTO>(this, message =>
            {
                EmployeeId = message.Id;
            });
        }
        
        public static void CleanUp()
        {
            if (_complainService != null)
                _complainService.Dispose();
            if (_employeeService != null)
                _employeeService.Dispose();
        }
        #endregion

        #region Properties
        public int? EmployeeId
        {
            get { return _employeeId; }
            set
            {
                _employeeId = value;
                RaisePropertyChanged<int?>(() => EmployeeId);

                if (EmployeeId != null)
                {
                    var empCriteria = new SearchCriteria<EmployeeDTO>();
                    empCriteria.FiList.Add(e => e.Id == EmployeeId);
                    SelectedEmployee = _employeeService.GetAll(empCriteria).FirstOrDefault();
                    
                }
            }
        }
        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);

                if (SelectedEmployee != null)
                {
                    var search = new SearchCriteria<ComplainDTO>();
                    search.FiList.Add(c => c.Id == SelectedEmployee.CurrentComplainId);
                    SelectedComplain = _complainService.GetAll(search).FirstOrDefault();

                    if (SelectedComplain != null)
                    {
                        if (SelectedComplain.FinalSolutionDate == null)
                            SelectedComplain.FinalSolutionDate = DateTime.Now;
                    }
                }
                    
            }
        }
       
        public bool EditCommandVisibility
        {
            get { return _editCommandVisibility; }
            set
            {
                _editCommandVisibility = value;
                RaisePropertyChanged<bool>(() => EditCommandVisibility);
            }
        }

        public ComplainDTO SelectedComplain
        {
            get { return _selectedComplain; }
            set
            {
                _selectedComplain = value;
                RaisePropertyChanged<ComplainDTO>(() => SelectedComplain);
                
            }
        }

        #endregion

        #region Commands

        public ICommand ComplainDateViewCommand
        {
            get
            {
                return _complainDateViewCommand ??
                       (_complainDateViewCommand = new RelayCommand(ExcuteComplainDate));
            }
        }

        public void ExcuteComplainDate()
        {
            if (SelectedComplain == null) return;
            
            DateTime solDate = DateTime.Now;
            if (SelectedComplain.FinalSolutionDate != null)
                solDate=(DateTime) SelectedComplain.FinalSolutionDate;

            var calConv = new Calendar(solDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedComplain.FinalSolutionDate = calConv.DtSelectedDate.SelectedDate;
            }
        }
        
        public ICommand SaveComplainViewCommand
        {
            get { return _saveComplainViewCommand ?? (_saveComplainViewCommand = new RelayCommand<object>(ExecuteSaveComplainViewCommand, CanSave)); }
        }
        private void ExecuteSaveComplainViewCommand(object obj)
        {
            if (SaveComplain())
                CloseWindow(obj);
            else MessageBox.Show("Can't Close Complain");
        }

        private bool SaveComplain()
        {
            try
            {
                if (SelectedEmployee == null || SelectedComplain.FinalSolutionDate==null)
                    return false;

                SelectedComplain.Status = ComplainStatusTypes.Closed;
                _complainService.InsertOrUpdate(SelectedComplain);

                SelectedEmployee.CurrentComplain = null;
                _employeeService.InsertOrUpdate(SelectedEmployee);

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private ICommand _closeComplainViewCommand;
        public ICommand CloseComplainViewCommand
        {
            get { return _closeComplainViewCommand ?? (_closeComplainViewCommand = new RelayCommand<object>(ExecuteCloseComplainViewCommand)); }
        }
        private void ExecuteCloseComplainViewCommand(object obj)
        {
            CloseWindow(obj);
        }

        public void CloseWindow(object obj)
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
            return Errors == 0;
        }
        #endregion
    }
}