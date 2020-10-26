using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class ComplainViewModel : ViewModelBase
    {
        #region Fields

        private static IComplainService _complainService;
        private static IEmployeeService _employeeService;

        private ICommand _addNewComplainViewCommand;

        private ComplainRemarkDTO _complainRemark;
        private ObservableCollection<ComplainDTO> _complains;
        private IEnumerable<ComplainDTO> _complainsList;

        private ICommand _deleteComplainViewCommand;

        private bool _editCommandVisibility;
        private EmployeeDTO _employee;
        private ObservableCollection<EmployeeDTO> _employees;
        private ICommand _refreshCommand;
        private ICommand _saveComplainViewCommand;
        private ICommand _saveRemarkViewCommand;
        private ComplainDTO _selectedComplain;
        private EmployeeDTO _selectedEmployeeForSearch;
        private string _totalNumberofComplains;

        #endregion

        #region Constructor

        public ComplainViewModel()
        {
            CleanUp();
            _complainService = new ComplainService();
            _employeeService = new EmployeeService();

            EditCommandVisibility = false;

            //Messenger.Default.Register<EmployeeDTO>(this, message =>
            //{
            //    SelectedEmployee = _employeeService.Find(message.Id.ToString());
            //});
            GetLiveComplains();
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

        public EmployeeDTO Employee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                RaisePropertyChanged<EmployeeDTO>(() => Employee);
                if (Employee != null)
                {
                    GetLiveComplains();
                    if (Complains.Any())
                    {
                        SelectedComplain = Complains.FirstOrDefault();
                    }
                    else
                    {
                        ExecuteAddNewComplainViewCommand();
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

        public string TotalNumberofComplains
        {
            get { return _totalNumberofComplains; }
            set
            {
                _totalNumberofComplains = value;
                RaisePropertyChanged<string>(() => TotalNumberofComplains);
            }
        }

        public ComplainDTO SelectedComplain
        {
            get { return _selectedComplain; }
            set
            {
                _selectedComplain = value;
                RaisePropertyChanged<ComplainDTO>(() => SelectedComplain);

                SelectedComplainRemark = new ComplainRemarkDTO
                {
                    AgencyId = Singleton.Agency.Id,
                    RemarkDate = DateTime.Now
                };

                if (SelectedComplain != null)
                {
                    ComplainRemarks = new ObservableCollection<ComplainRemarkDTO>(SelectedComplain.Remarks);
                    EditCommandVisibility = false;
                }
                else
                {
                    EditCommandVisibility = false;
                }
            }
        }

        public IEnumerable<ComplainDTO> ComplainsList
        {
            get { return _complainsList; }
            set
            {
                _complainsList = value;
                RaisePropertyChanged<IEnumerable<ComplainDTO>>(() => ComplainsList);
            }
        }

        public ObservableCollection<ComplainDTO> Complains
        {
            get { return _complains; }
            set
            {
                _complains = value;
                RaisePropertyChanged<ObservableCollection<ComplainDTO>>(() => Complains);
            }
        }

        public ObservableCollection<EmployeeDTO> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged<ObservableCollection<EmployeeDTO>>(() => Employees);
            }
        }

        public EmployeeDTO SelectedEmployeeForSearch
        {
            get { return _selectedEmployeeForSearch; }
            set
            {
                _selectedEmployeeForSearch = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployeeForSearch);

                if (SelectedEmployeeForSearch != null && !string.IsNullOrEmpty(SelectedEmployeeForSearch.EmployeeDetail))
                {
                    //SelectedEmployee = SelectedEmployeeForSearch;
                    SelectedEmployeeForSearch.EmployeeDetail = "";

                    var complainsList =
                        ComplainsList.Where(c => c.EmployeeId == SelectedEmployeeForSearch.Id).ToList();
                    Complains = new ObservableCollection<ComplainDTO>(complainsList);
                }
            }
        }

        #endregion

        #region Commands

        private ICommand _closeComplainViewCommand;
        private ObservableCollection<ComplainRemarkDTO> _complainRemarks;

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ExcuteRefreshCommand)); }
        }

        public ICommand AddNewComplainViewCommand
        {
            get
            {
                return _addNewComplainViewCommand ??
                       (_addNewComplainViewCommand = new RelayCommand(ExecuteAddNewComplainViewCommand));
            }
        }

        public ICommand SaveComplainViewCommand
        {
            get
            {
                return _saveComplainViewCommand ??
                       (_saveComplainViewCommand = new RelayCommand<Object>(ExecuteSaveComplainViewCommand, CanSave));
            }
        }

        public ICommand DeleteComplainViewCommand
        {
            get
            {
                return _deleteComplainViewCommand ??
                       (_deleteComplainViewCommand = new RelayCommand(ExecuteDeleteComplainViewCommand));
            }
        }

        public ICommand CloseComplainViewCommand
        {
            get
            {
                return _closeComplainViewCommand ??
                       (_closeComplainViewCommand = new RelayCommand<Object>(ExecuteCloseComplainViewCommand));
            }
        }

        public ICommand SaveRemarkViewCommand
        {
            get
            {
                return _saveRemarkViewCommand ??
                       (_saveRemarkViewCommand = new RelayCommand<Object>(ExcuteSaveRemarkViewCommand, CanSave));
            }
        }

        public ComplainRemarkDTO SelectedComplainRemark
        {
            get { return _complainRemark; }
            set
            {
                _complainRemark = value;
                RaisePropertyChanged<ComplainRemarkDTO>(() => SelectedComplainRemark);
            }
        }

        public ObservableCollection<ComplainRemarkDTO> ComplainRemarks
        {
            get { return _complainRemarks; }
            set
            {
                _complainRemarks = value;
                RaisePropertyChanged<ObservableCollection<ComplainRemarkDTO>>(() => ComplainRemarks);
            }
        }

        private void ExcuteRefreshCommand()
        {
            GetLiveComplains();
        }

        private void ExecuteAddNewComplainViewCommand()
        {
            new ComplainDetail().ShowDialog();
            //SelectedComplain = new ComplainDTO
            //{
            //    //SelectedEmployee = SelectedEmployee,
            //    //EmployeeId = SelectedEmployee.Id,
            //    ComplainDate = DateTime.Now,
            //    FinalSolutionDate = DateTime.Now,
            //    Priority = ComplainProrityTypes.Medium,
            //    Type = ComplainTypes.DidNotCall,
            //    Complain = "...",
            //    Status = ComplainStatusTypes.Opened
            //};

            //SelectedComplainRemark = new ComplainRemarkDTO
            //{
            //    Complain = SelectedComplain
            //};
        }

        private void ExecuteSaveComplainViewCommand(object obj)
        {
            try
            {
                var complain = new ComplainDetail(SelectedComplain.Employee);
                complain.ShowDialog();
            }
            catch
            {
                
            }

        }

        private bool SaveComplain()
        {
            try
            {
                _complainService.InsertOrUpdate(SelectedComplain);

                if (SelectedComplain.Status == ComplainStatusTypes.Opened)
                {
                    Employee.CurrentStatus = ProcessStatusTypes.WithComplain;
                }
                else
                {
                    if (SelectedComplain.Status == ComplainStatusTypes.Closed &&
                        Complains.Count(c => c.Status == ComplainStatusTypes.Opened) == 0)
                        Employee.CurrentStatus = ProcessStatusTypes.OnGoodCondition;
                }
                _employeeService.InsertOrUpdate(Employee);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ExecuteDeleteComplainViewCommand()
        {
            try
            {
                if (SelectedComplain.Status != ComplainStatusTypes.Confirmed)
                {
                    return;
                }
                if (
                    MessageBox.Show("are you sure you want to delete the complain?", "Delete Complain",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    SelectedComplain.Enabled = false;
                    foreach (var complainRemarkDTO in SelectedComplain.Remarks)
                    {
                        complainRemarkDTO.Enabled = false;
                    }

                    _complainService.InsertOrUpdate(SelectedComplain);
                    GetLiveComplains();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't delete employee, try again later..." +
                    Environment.NewLine + ex.Message +
                    Environment.NewLine + ex.InnerException, "Error Delete Complain");
            }
        }

        private void ExecuteCloseComplainViewCommand(object obj)
        {
            try
            {
                var complain = new ComplainSolution(SelectedComplain.Employee);
                complain.ShowDialog();
            }
            catch
            {
            }
            //CloseWindow(obj);
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.DialogResult = true;
            window.Close();
        }

        private void ExcuteSaveRemarkViewCommand(object obj)
        {
            try
            {
                if (!SaveComplain()) return;

                SelectedComplainRemark.Complain = SelectedComplain;
                _complainService.InsertOrUpdateRemark(SelectedComplainRemark);

                GetLiveComplainRemarks();
                SelectedComplainRemark = new ComplainRemarkDTO
                {
                    RemarkDate = DateTime.Now
                };
            }
            catch
            {
                MessageBox.Show("Can't Save Remark");
            }
        }

        private void GetLiveComplainRemarks()
        {
            ComplainRemarks = new ObservableCollection<ComplainRemarkDTO>(
                _complainService
                    .GetAllRemarks()
                    .Where(c => c.ComplainId == SelectedComplain.Id)
                    .ToList()
                );
        }

        #endregion

        private void GetLiveComplains()
        {
            try
            {
                ComplainsList = _complainService.GetAll().OrderByDescending(c=>c.Id).ToList();
                
                int serNo = 1;
                foreach (var complainDTO in ComplainsList)
                {
                    complainDTO.SerialNumber = serNo;
                    serNo++;
                }

                Complains = new ObservableCollection<ComplainDTO>(ComplainsList);
                TotalNumberofComplains = Complains.Count.ToString("N0");

                List<EmployeeDTO> emps = ComplainsList.Select(c => c.Employee).Distinct().ToList();
                Employees = new ObservableCollection<EmployeeDTO>(emps);
            }
            catch
            {
            }
        }

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object obj)
        {
            return Errors == 0;
        }

        #endregion
    }
}