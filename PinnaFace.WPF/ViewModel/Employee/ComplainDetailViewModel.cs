using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class ComplainDetailViewModel : ViewModelBase
    {
        #region Fields

        private static IComplainService _complainService;
        private static IEmployeeService _employeeService;

        private ICommand _addNewComplainViewCommand;

        private ICommand _complainDateViewCommand;

        private int? _complainId;

        private ComplainRemarkDTO _complainRemark;
        private ICommand _deleteComplainViewCommand;
        private bool _editCommandVisibility;
        private int? _employeeId;

        private ICommand _saveComplainViewCommand,
            _saveRemarkViewCommand;

        private ComplainDTO _selectedComplain;
        private EmployeeDTO _selectedEmployee;

        #endregion

        #region Constructor

        public ComplainDetailViewModel()
        {
            CleanUp();
            _complainService = new ComplainService();
            _employeeService = new EmployeeService(false, true);

            EditCommandVisibility = false;
            
            Messenger.Default.Register<EmployeeDTO>(this, message => { EmployeeId = message.Id; });

            Messenger.Default.Register<int>(this, message => { ComplainId = message; });
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
                    SelectedComplain = SelectedEmployee.CurrentComplain ?? new ComplainDTO
                    {
                        ComplainDate = DateTime.Now,
                        Priority = ComplainProrityTypes.Medium,
                        Type = ComplainTypes.DidNotCall,
                        Complain = EnumUtil.GetEnumDesc(ComplainTypes.DidNotCall),
                        Status = ComplainStatusTypes.Opened,
                        Employee = SelectedEmployee
                    };
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

        public int? ComplainId
        {
            get { return _complainId; }
            set
            {
                _complainId = value;
                RaisePropertyChanged<int?>(() => ComplainId);

                if (ComplainId != null)
                {
                    if (SelectedEmployee == null)
                    {
                        var empCriteria = new SearchCriteria<EmployeeDTO>();
                        empCriteria.FiList.Add(e => e.CurrentComplainId == ComplainId);
                        SelectedEmployee = _employeeService.GetAll(empCriteria).FirstOrDefault();
                    }
                }
                //else
                //{
                //    if(SelectedEmployee==null)
                //    ExecuteAddNewComplainViewCommand();
                //}
            }
        }

        #endregion

        #region Commands

        private ICommand _closeComplainViewCommand;
        private ObservableCollection<ComplainRemarkDTO> _complainRemarks;

        public ICommand ComplainDateViewCommand
        {
            get
            {
                return _complainDateViewCommand ??
                       (_complainDateViewCommand = new RelayCommand(ExcuteComplainDate));
            }
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
                       (_saveComplainViewCommand = new RelayCommand<object>(ExecuteSaveComplainViewCommand, CanSave));
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
                       (_closeComplainViewCommand = new RelayCommand<object>(ExecuteCloseComplainViewCommand));
            }
        }

        public ICommand SaveRemarkViewCommand
        {
            get
            {
                return _saveRemarkViewCommand ??
                       (_saveRemarkViewCommand = new RelayCommand(ExcuteSaveRemarkViewCommand, CanSaveRemark));
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

        public void ExcuteComplainDate()
        {
            var calConv = new Calendar(SelectedComplain.ComplainDate);
            calConv.ShowDialog();
            bool? dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedComplain.ComplainDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        private void ExecuteAddNewComplainViewCommand()
        {
            SelectedComplain = new ComplainDTO
            {
                AgencyId = Singleton.Agency.Id,
                ComplainDate = DateTime.Now,
                Priority = ComplainProrityTypes.Medium,
                Type = ComplainTypes.DidNotCall,
                Complain = EnumUtil.GetEnumDesc(ComplainTypes.DidNotCall),
                Status = ComplainStatusTypes.Opened
            };

            SelectedComplainRemark = new ComplainRemarkDTO
            {
                AgencyId = Singleton.Agency.Id,
                Complain = SelectedComplain
            };
        }

        private void ExecuteSaveComplainViewCommand(object obj)
        {
            if (SaveComplain())
                CloseWindow(obj);
            else MessageBox.Show("Can't Save Complain");
        }

        private bool SaveComplain()
        {
            try
            {
                if (SelectedEmployee == null)
                    return false;

                SelectedEmployee.CurrentComplain = SelectedComplain;
                _employeeService.InsertOrUpdate(SelectedEmployee);
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
                new ComplainSolution(SelectedComplain).ShowDialog();
                //ComplainId = SelectedComplain.Id;
                ////if (
                ////    MessageBox.Show("Are you Sure You want to Delete the complain?", "Delete Complain",
                ////        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                ////{
                ////    SelectedComplain.Enabled = false;
                ////    _complainService.InsertOrUpdate(SelectedComplain);
                ////    GetEmployeeComplains();
                ////}
            }
            catch
            {
                MessageBox.Show("Problem deleting the complain?", "Error Delete Complain");
            }
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

        private void ExcuteSaveRemarkViewCommand()
        {
            try
            {
                SelectedComplain.Status = ComplainStatusTypes.OnProcess;
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

        #region Print Complain

        private ICommand _printComplain;

        public ICommand PrintComplainViewCommand
        {
            get { return _printComplain ?? (_printComplain = new RelayCommand(ExecutePrintComplain)); }
        }

        public void ExecutePrintComplain()
        {
            if (!SaveComplain())
                MessageBox.Show("Can't Save Complain");
            else
                GenerateReports.PrintComplain(SelectedEmployee);
        }

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public static int RemarkErrors { get; set; }

        public bool CanSave(object obj)
        {
            return Errors == 0;
        }

        public bool CanSaveRemark()
        {
            return RemarkErrors == 0;
        }

        #endregion
    }
}