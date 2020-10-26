using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.WPF.ViewModel
{
    public class EmployeeMedicalInsuranceViewModel : ViewModelBase
    {
        #region Fields
        private EmployeeDTO _selectedEmployee;
        private ICommand _saveInsuranceProcessViewCommand;
        private string _headerText;
        #endregion

        #region Constructor
        public EmployeeMedicalInsuranceViewModel()
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
                    HeaderText = SelectedEmployee.FullName ;
                    if (SelectedEmployee.InsuranceProcess == null)
                        SelectedEmployee.InsuranceProcess = new InsuranceProcessDTO
                        {
                            SubmitDate = DateTime.Now,
                            BeginingDate = DateTime.Now,
                            EndDate = DateTime.Now,
                            //Status = ProcessStatusTypes.OnProcess
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

        public ICommand SaveInsuranceProcessCommand
        {
            get { return _saveInsuranceProcessViewCommand ?? (_saveInsuranceProcessViewCommand = new RelayCommand<Object>(ExecuteSaveInsuranceProcessViewCommand, CanSave)); }
        }
        private void ExecuteSaveInsuranceProcessViewCommand(object obj)
        {
            try
            {
                SelectedEmployee.InsuranceProcess.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.InsuranceProcess.DateLastModified = DateTime.Now;
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
    //public class EmployeeMedicalInsuranceViewModel : ViewModelBase
    //{
    //    #region Fields
    //    private static IEmployeeService _employeeService;
    //    private EmployeeDTO _employee;
    //    private InsuranceProcessDTO _currentInsuranceProcess;
    //    private ICommand _saveInsuranceProcessViewCommand;
    //    private string _headerText;

    //    #endregion

    //    #region Constructor
    //    public EmployeeMedicalInsuranceViewModel()
    //    {
    //        CleanUp();
    //        _employeeService = new EmployeeService(false,true);

    //        Messenger.Default.Register<EmployeeDTO>(this, message =>
    //        {
    //            var cri = new SearchCriteria<EmployeeDTO>();
    //            cri.FiList.Add(e => e.Id == message.Id);
    //            SelectedEmployee = _employeeService.GetAll(cri).FirstOrDefault();
    //        });

    //    }
    //    public static void CleanUp()
    //    {
    //        if (_employeeService != null)
    //            _employeeService.Dispose();
    //    }
    //    #endregion

    //    public EmployeeDTO SelectedEmployee
    //    {
    //        get { return _employee; }
    //        set
    //        {
    //            _employee = value;
    //            RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);
    //            if (SelectedEmployee != null)
    //            {
    //                HeaderText = SelectedEmployee.FullName + " - " + SelectedEmployee.PassportNumber;

    //                SelectedInsuranceProcess = SelectedEmployee.EmployeeMedicalInsurance ?? new InsuranceProcessDTO
    //                {
    //                    SubmitDate = DateTime.Now,
    //                    BeginingDate = DateTime.Now,
    //                    EndDate = DateTime.Now,
    //                    Status = ProcessStatusTypes.OnProcess
    //                };
    //            }
    //        }
    //    }
    //    public InsuranceProcessDTO SelectedInsuranceProcess
    //    {
    //        get { return _currentInsuranceProcess; }
    //        set
    //        {
    //            _currentInsuranceProcess = value;
    //            RaisePropertyChanged<InsuranceProcessDTO>(() => SelectedInsuranceProcess);
    //        }
    //    }
    //    public string HeaderText
    //    {
    //        get { return _headerText; }
    //        set
    //        {
    //            _headerText = value;
    //            RaisePropertyChanged<string>(() => HeaderText);
    //        }
    //    }
    //    public ICommand SaveInsuranceProcessViewCommand
    //    {
    //        get { return _saveInsuranceProcessViewCommand ?? (_saveInsuranceProcessViewCommand = new RelayCommand<Object>(ExecuteSaveInsuranceProcessViewCommand, CanSave)); }
    //    }
    //    private void ExecuteSaveInsuranceProcessViewCommand(object obj)
    //    {
    //        try
    //        {
    //            if (SelectedInsuranceProcess != null && SelectedEmployee != null)
    //            {
    //                SelectedEmployee.EmployeeMedicalInsurance = SelectedInsuranceProcess;
    //                _employeeService.InsertOrUpdate(SelectedEmployee);
    //            }
    //            CloseWindow(obj);
    //        }
    //        catch
    //        {
    //            MessageBox.Show("Problem Saving insuarnec process?", "Error Insurance Process");
    //        }
    //    }

    //    private void CloseWindow(object obj)
    //    {
    //        if (obj != null)
    //        {
    //            var window = obj as Window;
    //            if (window != null)
    //            {
    //                window.DialogResult = true;
    //                window.Close();
    //            }
    //        }
    //    }

    //    #region Validation
    //    public static int Errors { get; set; }
    //    public bool CanSave(object obj)
    //    {
    //        if (Errors == 0)
    //            return true;
    //        return false;
    //    }

    //    #endregion
    //}
}
