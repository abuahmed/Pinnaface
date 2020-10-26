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
using MessageBox = System.Windows.Forms.MessageBox;

namespace PinnaFace.WPF.ViewModel
{
    public class FlightProcessViewModel : ViewModelBase
    {
        #region Fields
        private static IEmployeeService _employeeService;
        private EmployeeDTO _employee;
        private FlightProcessDTO _currentFlightProcess;
        private ICommand _saveFlightProcessViewCommand, _cityListEnglishViewCommand;
        private string _headerText;
        #endregion

        #region Constructor
        public FlightProcessViewModel()
        {
            CleanUp();
            _employeeService = new EmployeeService(false,true);

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

        public EmployeeDTO Employee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                RaisePropertyChanged<EmployeeDTO>(() => Employee);
                if (Employee != null)
                {
                    HeaderText = Employee.FullName + " - " + Employee.PassportNumber;
                    SelectedFlightProcess = Employee.FlightProcess ?? new FlightProcessDTO
                    {
                        AgencyId = Singleton.Agency.Id,
                        SubmitDate = DateTime.Now
                    };
                }
            }
        }
        public FlightProcessDTO SelectedFlightProcess
        {
            get { return _currentFlightProcess; }
            set
            {
                _currentFlightProcess = value;
                RaisePropertyChanged<FlightProcessDTO>(() => SelectedFlightProcess);
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
        public ICommand CityListEnglishViewCommand
        {
            get
            {
                return _cityListEnglishViewCommand ??
                       (_cityListEnglishViewCommand = new RelayCommand(ExcuteCityListEnglishViewCommand));
            }
        }
        public void ExcuteCityListEnglishViewCommand()
        {
            var listWindow = new Lists(ListTypes.City);

            listWindow.ShowDialog();
            if (listWindow.DialogResult != null && (bool)listWindow.DialogResult)
            {
                SelectedFlightProcess.TicketCity = listWindow.TxtDisplayName.Text;
            }
        }

        public ICommand SaveFlightProcessViewCommand
        {
            get { return _saveFlightProcessViewCommand ?? (_saveFlightProcessViewCommand = new RelayCommand<Object>(ExecuteSaveFlightProcessViewCommand, CanSave)); }
        }
        private void ExecuteSaveFlightProcessViewCommand(object obj)
        {
            try
            {
                if (SelectedFlightProcess != null && Employee != null)
                {
                   SelectedFlightProcess.DateLastModified = DateTime.Now;
                   Employee.FlightProcess = SelectedFlightProcess;
                    _employeeService.InsertOrUpdate(Employee);

                }
                CloseWindow(obj);
            }
            catch
            {
                MessageBox.Show("Problem Saving On Flight Data!!!");
            }
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
            var calConv = new Calendar(SelectedFlightProcess.SubmitDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedFlightProcess.SubmitDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        private void CloseWindow(object obj)
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
