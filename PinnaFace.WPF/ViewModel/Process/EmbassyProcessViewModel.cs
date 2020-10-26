using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class EmbassyProcessViewModel : ViewModelBase
    {
        #region Fields
        private static IEmployeeService _employeeService;
        private DateTime _maxDate;
        private EmployeeDTO _employeeee;
        private EmbassyProcessDTO _currentEmbassyProcess;
        private ICommand _saveEmbassyProcessViewCommand;
        private string _headerText;
        #endregion

        #region Constructor
        public EmbassyProcessViewModel()
        {
            CleanUp();
            _employeeService = new EmployeeService(false, true);

            Messenger.Default.Register<EmployeeDTO>(this, message =>
            {
                var cri = new SearchCriteria<EmployeeDTO>();
                cri.FiList.Add(e => e.Id == message.Id);
                Employee = _employeeService.GetAll(cri).FirstOrDefault();

            });
        }
        public static void CleanUp()
        {
            if (_employeeService != null)
                _employeeService.Dispose();
        }
        #endregion

        #region Public Properties
        public DateTime MaxDate
        {
            get { return _maxDate; }
            set
            {
                _maxDate = value;
                RaisePropertyChanged<DateTime>(() => MaxDate);
            }
        }

        public EmployeeDTO Employee
        {
            get { return _employeeee; }
            set
            {
                _employeeee = value;
                RaisePropertyChanged<EmployeeDTO>(() => Employee);
                if (Employee != null)
                {
                    HeaderText = Employee.FullName + " - " + Employee.PassportNumber;

                    SelectedEmbassyProcess = Employee.EmbassyProcess ?? new EmbassyProcessDTO
                    {
                        AgencyId = Singleton.Agency.Id,
                        SubmitDate = DateTime.Now
                    };
                    
                    MaxDate = Singleton.ProductActivation.ExpiryDate;
                }
            }
        }

        public EmbassyProcessDTO SelectedEmbassyProcess
        {
            get { return _currentEmbassyProcess; }
            set
            {
                _currentEmbassyProcess = value;
                RaisePropertyChanged<EmbassyProcessDTO>(() => SelectedEmbassyProcess);
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
        public ICommand SaveEmbassyProcessViewCommand
        {
            get { return _saveEmbassyProcessViewCommand ?? (_saveEmbassyProcessViewCommand = new RelayCommand<Object>(ExecuteSaveEmbassyProcessViewCommand, CanSave)); }
        }
        private void SaveEmbassyProcess()
        {
            try
            {
                if (SelectedEmbassyProcess != null && Employee != null)
                {
                    SelectedEmbassyProcess.DateLastModified = DateTime.Now;
                    Employee.EmbassyProcess = SelectedEmbassyProcess;
                    _employeeService.InsertOrUpdate(Employee);
                }
            }
            catch
            {
                MessageBox.Show("Problem saving embassy process...");
            }
        }
        private void ExecuteSaveEmbassyProcessViewCommand(object obj)
        {
            SaveEmbassyProcess();
            CloseWindow(obj);
        }

        private ICommand _submitDateViewCommand;
        public ICommand SubmitDateViewCommand
        {
            get
            {
                return _submitDateViewCommand ??
                       (_submitDateViewCommand = new RelayCommand(ExcuteSubmitDate));
            }
        }
        public void ExcuteSubmitDate()
        {
            var calConv = new Calendar(SelectedEmbassyProcess.SubmitDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedEmbassyProcess.SubmitDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        private ICommand _closeEmbassyProcessViewCommand;
        public ICommand CloseEmbassyProcessViewCommand
        {
            get
            {
                return _closeEmbassyProcessViewCommand ?? (_closeEmbassyProcessViewCommand = new RelayCommand<Object>(CloseWindow));
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
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;
        }

        #endregion

        #region Reports

        #region List
        //private ICommand _printListCommandView;
        //public ICommand PrintListCommandView
        //{
        //    get
        //    {
        //        return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintList, CanSave));
        //    }
        //}
        //private void PrintList(object obj)
        //{
        //    SaveEmbassyProcess();
        //    GenerateReports.PrintList(Employee,obj!=null);
        //}
        #endregion

        #region Application

        private ICommand _printEmbApplicationViewCommand;
        public ICommand PrintEmbApplicationViewCommand  
        {
            get
            {
                return _printEmbApplicationViewCommand ?? (_printEmbApplicationViewCommand = new RelayCommand<Object>(PrintEmbassyApplication, CanSave));
            }
        }
        private void PrintEmbassyApplication(object obj)
        {
            SaveEmbassyProcess();   
            GenerateReports.PrintEmbassyApplication(Employee, obj != null);
        }

        #endregion

        #region RecruitingOrder
        private ICommand _printRecruitingOrderViewCommand;
        public ICommand PrintRecruitingOrderViewCommand
        {
            get
            {
                return _printRecruitingOrderViewCommand ?? (_printRecruitingOrderViewCommand = new RelayCommand<Object>(PrintRecruitingOrder, CanSave));
            }
        }
        private void PrintRecruitingOrder(object obj)
        {
            SaveEmbassyProcess();
            GenerateReports.PrintRecruitingOrder(Employee, obj != null);
        }
        #endregion

        //#region Pledge
        //private ICommand _printPledgeViewCommand;
        //public ICommand PrintPledgeViewCommand
        //{
        //    get
        //    {
        //        return _printPledgeViewCommand ?? (_printPledgeViewCommand = new RelayCommand<Object>(PrintPledge, CanSave));
        //    }
        //}
        //private void PrintPledge(object obj)
        //{
        //    SaveEmbassyProcess();
        //    GenerateReports.PrintPledge(Employee, obj != null);
        //}
        //#endregion

        //#region Confirmation
        //private ICommand _printConfirmationViewCommand;
        //public ICommand PrintConfirmationViewCommand
        //{
        //    get
        //    {
        //        return _printConfirmationViewCommand ?? (_printConfirmationViewCommand = new RelayCommand<Object>(PrintConfirmation, CanSave));
        //    }
        //}
        //private void PrintConfirmation(object obj)
        //{
        //    SaveEmbassyProcess();
        //    GenerateReports.PrintConfirmation(Employee, obj != null);
        //}
        //#endregion

        #region EmbassySelection
        private ICommand _printEmbassySelectionViewCommand;
        public ICommand PrintEmbassySelectionViewCommand
        {
            get
            {
                return _printEmbassySelectionViewCommand ?? (_printEmbassySelectionViewCommand = new RelayCommand<Object>(PrintEmbassySelection, CanSave));
            }
        }
        private void PrintEmbassySelection(object obj)
        {
            SaveEmbassyProcess();
            GenerateReports.PrintEmbassySelection(Employee, obj != null);
        }
        #endregion

        #region All In One

        private ICommand _printAllInOneCommandView;
        public ICommand PrintAllInOneCommandView
        {
            get
            {
                return _printAllInOneCommandView ?? (_printAllInOneCommandView = new RelayCommand<Object>(PrintAllInOne, CanSave));
            }
        }
        private void PrintAllInOne(object obj)
        {
            SaveEmbassyProcess();
            GenerateReports.PrintAllInOne(Employee, obj != null);
        }
        
        #endregion

        #endregion
        
    }
}
