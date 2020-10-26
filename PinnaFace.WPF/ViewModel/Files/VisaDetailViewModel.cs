using System;
using System.Collections.Generic;
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
using PinnaFace.WPF.Models;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class VisaDetailViewModel : ViewModelBase
    {
        #region Fields

        private static IVisaService _visaService;
        private EmployeeDTO _selectedEmployee;
        private IEnumerable<EmployeeDTO> _visaEmployees;
        private AgentDTO _selectedAgent;
        private VisaDTO _selectedVisa;
        private ObservableCollection<AgentDTO> _agents;

        private ICommand _refreshCommand,
            _saveVisaViewCommand,
            _addNewVisaViewCommand,
            _deleteVisaViewCommand,
            _assignVisaViewCommand;

        private bool _editCommandVisibility, _assignCommandVisibility, _emptyControlVisibility;
        private string _totalNumberOfVisas, _employeesCount;
        private VisaModel _parameterizedVisa;
        
        #endregion

        #region Constructor

        public VisaDetailViewModel()
        {
            Load();
        }

        private void Load()
        {
            CleanUp();
            _visaService = new VisaService(false,true);

            EditCommandVisibility = false;
            EmptyControlVisibility = true;

            CheckRoles();
            GetLiveAgents();
            
            Messenger.Default.Register<VisaModel>(this, message =>
            {
                ParameterizedVisa = message;
            });
        }

        public static void CleanUp()
        {
            if (_visaService != null)
                _visaService.Dispose();
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

                if (SelectedEmployee != null && SelectedEmployee.Id != 0)
                {
                    AssignCommandVisibility = true;
                    ShowEmployeesAssigned(SelectedEmployee.VisaId);
                }
                else
                {
                    AssignCommandVisibility = false;
                }
            }
        }
        
        public VisaModel ParameterizedVisa
        {
            get { return _parameterizedVisa; }
            set
            {
                _parameterizedVisa = value;
                RaisePropertyChanged<VisaModel>(() => ParameterizedVisa);

                if (ParameterizedVisa != null && ParameterizedVisa.VisaId!=null)
                {
                    var search = new SearchCriteria<VisaDTO>();
                    search.FiList.Add(v => v.Id == ParameterizedVisa.VisaId);
                    var paramVisas = _visaService.GetAll(search);//.ToList();
                    var paramVisa=paramVisas.FirstOrDefault();
                    SelectedVisa = paramVisa;

                    ShowEmployeesAssigned(ParameterizedVisa.VisaId);
                }

                else
                    ExecuteAddNewVisaViewCommand();
            }
        }

        public string TotalNumberOfVisas
        {
            get { return _totalNumberOfVisas; }
            set
            {
                _totalNumberOfVisas = value;
                RaisePropertyChanged<string>(() => TotalNumberOfVisas);
            }
        }

        public bool EmptyControlVisibility
        {
            get { return _emptyControlVisibility; }
            set
            {
                _emptyControlVisibility = value;
                RaisePropertyChanged<bool>(() => EmptyControlVisibility);
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

        #endregion

        #region List Boxes

        private ICommand _sponsorAddressViewCommand, _visaConditionViewCommand;

        public ICommand SponsorAddressViewCommand
        {
            get
            {
                return _sponsorAddressViewCommand ?? (_sponsorAddressViewCommand = new RelayCommand(SponsorAddress));
            }
        }

        public void SponsorAddress()
        {
            var addr = new AddressEntry(SelectedVisa.Sponsor.Address);
            addr.ShowDialog();
            if (addr.DialogResult != null && (bool) addr.DialogResult)
                SaveVisa();
        }

        public ICommand VisaConditionViewCommand
        {
            get { return _visaConditionViewCommand ?? (_visaConditionViewCommand = new RelayCommand(VisaCondition)); }
        }

        public void VisaCondition()
        {
            var cond = new VisaCondition(SelectedVisa.Condition);
            cond.ShowDialog();

            if (cond.DialogResult != null && (bool) cond.DialogResult)
                SaveVisa();
        }

        #endregion

        #region Visa
        
        public string EmployeesCount
        {
            get { return _employeesCount; }
            set
            {
                _employeesCount = value;
                RaisePropertyChanged<string>(() => EmployeesCount);
            }
        }

        public IEnumerable<EmployeeDTO> VisaEmployees
        {
            get { return _visaEmployees; }
            set
            {
                _visaEmployees = value;
                RaisePropertyChanged<IEnumerable<EmployeeDTO>>(() => VisaEmployees);

                if (VisaEmployees == null)
                {
                    EmployeesCount = "Not Assigned";
                }
                else if (VisaEmployees.ToList().Count == 0)
                {
                    EmployeesCount = "Not Assigned";
                }
                else
                {
                    var emps =
                        VisaEmployees.Aggregate("",
                            (current, emp) => current + ", " + emp.FirstName.ToUpper() + "(" + emp.PassportNumber + ")");

                    EmployeesCount = VisaEmployees.ToList().Count + "(" + emps.Substring(1) + ")";
                }
            }
        }

        public VisaDTO SelectedVisa
        {
            get { return _selectedVisa; }
            set
            {
                _selectedVisa = value;
                RaisePropertyChanged<VisaDTO>(() => SelectedVisa);

                if (SelectedVisa != null && SelectedVisa.Sponsor != null)
                {
                    // SelectedAppliedCountry =
                    //ApplyCountries.FirstOrDefault(e => e.Value == (int)SelectedVisa.VisaCountry);

                    SelectedAgent = Agents.FirstOrDefault(v => v.Id == SelectedVisa.ForeignAgentId);
                    //if (SelectedEmployee == null)
                    //{
                    ////ShowEmployeesAssigned(SelectedVisa.Id);
                    //}
                    EditCommandVisibility = true;
                }
                else
                    EditCommandVisibility = false;
            }
        }
        
        #endregion

        #region Commands

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ExcuteRefreshCommand)); }
        }

        private void ExcuteRefreshCommand()
        {
            Load();
        }

        public ICommand AddNewVisaViewCommand
        {
            get
            {
                return _addNewVisaViewCommand ??
                       (_addNewVisaViewCommand = new RelayCommand(ExecuteAddNewVisaViewCommand));
            }
        }

        private void ExecuteAddNewVisaViewCommand()
        {
            if (Agents.Count == 0)
            {
                AddNewAgent();
            }
            SelectedAgent = Agents.FirstOrDefault(); 
           
            if (SelectedAgent != null)
            {
                SelectedVisa = CommonUtility.GetNewVisaDTO(SelectedAgent.Id);
            }
            EmployeesCount = "Not Assigned";
            EmptyControlVisibility = true;
            EditCommandVisibility = false;

            //_checkDuplicate = true;
        }

        public bool AssignCommandVisibility
        {
            get { return _assignCommandVisibility; }
            set
            {
                _assignCommandVisibility = value;
                RaisePropertyChanged<bool>(() => AssignCommandVisibility);
            }
        }

        public ICommand AssignVisaViewCommand
        {
            get
            {
                return _assignVisaViewCommand ??
                       (_assignVisaViewCommand = new RelayCommand<object>(ExecuteAssignVisaViewCommand, CanSave));
            }
        }

        private void ExecuteAssignVisaViewCommand(object obj)
        {
            try
            {
                if (!SaveVisa()) return;

                AttachVisa(true);

                CloseWindow(obj);
            }
            catch
            {
                MessageBox.Show("Can't Assign Visa to " + SelectedEmployee.FullName);
            }
        }
        
        public ICommand SaveVisaViewCommand
        {
            get
            {
                return _saveVisaViewCommand ??
                       (_saveVisaViewCommand = new RelayCommand<object>(ExecuteSaveVisaViewCommand, CanSave));
            }
        }

        private void ExecuteSaveVisaViewCommand(object obj)
        {
            SaveVisa();
            if(obj!=null)
            CloseWindow(obj);
        }

        private bool SaveVisa()
        {
            try
            {
                SelectedVisa.ForeignAgentId = SelectedAgent.Id;

                SelectedVisa.DateLastModified = DateTime.Now;
                SelectedVisa.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedVisa.Sponsor.DateLastModified = DateTime.Now;
                SelectedVisa.Sponsor.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedVisa.Sponsor.Address.DateLastModified = DateTime.Now;
                SelectedVisa.Sponsor.Address.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedVisa.Condition.DateLastModified = DateTime.Now;
                SelectedVisa.Condition.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;

                _visaService.InsertOrUpdate(SelectedVisa);
                AttachVisa(true);

                //_checkDuplicate = false;
                return true;
            }
            catch
            {
                MessageBox.Show("Can't Save Visa");
                return false;
            }
        }

        public ICommand DeleteVisaViewCommand
        {
            get
            {
                return _deleteVisaViewCommand ??
                       (_deleteVisaViewCommand = new RelayCommand(ExecuteDeleteVisaViewCommand));
            }
        }

        private void ExecuteDeleteVisaViewCommand()
        {
            try
            {
                if (MessageBox.Show("Are you Sure You want to Delete this Visa?", "Delete Visa",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes)
                    return;
                //Check Constraints Before Deleting
                var ids = DbCommandUtil.QueryCommand("Select VisaId as Id from Employees " +
                                                     " where Id='" + SelectedVisa.Id + "' and enabled='1'").ToList();
                if (ids.Count==0)
                {
                    SelectedVisa.Enabled = false;
                    _visaService.InsertOrUpdate(SelectedVisa);
                }
                else MessageBox.Show("Problem deleting Visa, There may exist Employees Assigned to this Visa," +
                                " you have to first update or delete those Employees related with " +
                                SelectedVisa.VisaNumber,
                               "Can't Delete Visa", MessageBoxButton.OK, MessageBoxImage.Error);
                //Visas.Remove(SelectedVisa);
            }
            catch
            {
                MessageBox.Show(
                    "Problem deleting Visa, There may exist Employees Assigned to this Visa, you have to first update or delete those Employees related with " +
                    SelectedVisa.VisaNumber, "Can't Delete Visa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AttachVisa(bool isAttach)
        {
            try
            {
                var empService = new EmployeeService();
                if (isAttach && VisaEmployees.Count() > SelectedVisa.VisaQuantity)
                {
                    MessageBox.Show("Visa Quantity is less than the number of employees you are going to assign to!!",
                        "Visa Already Assigned",MessageBoxButton.OK,MessageBoxImage.Error);
                    return;
                }
                
                foreach (var employ in VisaEmployees)
                {
                    var emp = empService.GetAll()
                        .FirstOrDefault(e => e.Id == employ.Id);
                    
                    if (emp != null)
                    {
                        #region If Is Detach
                        if (!isAttach)
                        {

                            if (emp.LabourProcess != null || emp.EmbassyProcess != null ||
                                emp.FlightProcess != null || emp.CurrentComplain != null)
                            {
                                if (MessageBox.Show(
                                    "There exists other processes depending on this visa, do you want to also delete them?",
                                    "Detach Visa",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                                {
                                    return;
                                }

                                emp.LabourProcess = null;
                                emp.EmbassyProcess = null;
                                emp.FlightProcess = null;
                                emp.CurrentComplain = null;
                            }
                        } 
                        #endregion
                        
                        emp.VisaId = isAttach ? SelectedVisa.Id : (int?)null;
                        emp.AgentId = SelectedVisa.ForeignAgentId;
                        if (!isAttach) emp.Visa = null;
                    }
                    empService.InsertOrUpdate(emp);
                }
                empService.Dispose();
            }
            catch
            {
            }
            finally
            {
                if (!isAttach)
                    VisaEmployees = null; //SelectedEmployee = null;
            }
        }

        public void ShowEmployeesAssigned(int? visaIdNum)
        {
            if (visaIdNum != null && visaIdNum != 0)
            {
                var searchEmp = new SearchCriteria<EmployeeDTO>();
                searchEmp.FiList.Add(v => v.VisaId == visaIdNum);
                VisaEmployees = new EmployeeService(true, false).GetAll(searchEmp).ToList();
            }
            ////Not Neccesary Error Prone
            //if (SelectedEmployee != null)
            //{
            //    bool empFound = false;
            //    if (VisaEmployees != null && VisaEmployees.Any())
            //    {
            //        foreach (var visaEmployee in VisaEmployees)
            //        {
            //            if (SelectedEmployee.Id == visaEmployee.Id)
            //                empFound = true;
            //        }
            //    }

            //    if (VisaEmployees == null)
            //        VisaEmployees = new List<EmployeeDTO>();

            //    if (!empFound)
            //    {
            //        IList<EmployeeDTO> empps = new List<EmployeeDTO>();
            //        empps.Add(SelectedEmployee);

            //        VisaEmployees = VisaEmployees.Union(empps.ToList()).ToList();
            //    }
            //}
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

        #region Agents

        public ObservableCollection<AgentDTO> Agents
        {
            get { return _agents; }
            set
            {
                _agents = value;
                RaisePropertyChanged<ObservableCollection<AgentDTO>>(() => Agents);
            }
        }

        public AgentDTO SelectedAgent
        {
            get { return _selectedAgent; }
            set
            {
                _selectedAgent = value;
                RaisePropertyChanged<AgentDTO>(() => SelectedAgent);
            }
        }

        private void GetLiveAgents()
        {
            var agentsList = new ForeignAgentService(true, false).GetAll().ToList();
            Agents = new ObservableCollection<AgentDTO>(agentsList);


            if (Agents.Count == 0)
            {
                AddNewAgent();
            }
        }

        public void AddNewAgent()
        {
            var foreignAgent = new ForeignAgents();
            foreignAgent.ShowDialog();

            Agents = new ObservableCollection<AgentDTO>(new ForeignAgentService(true, false).GetAll().ToList());

            if (Agents.Count == 0)
            {
                EmptyControlVisibility = false;
                MessageBox.Show("There should be at least 1 foreign agent registered!");
                return;
            }
            EmptyControlVisibility = true;
        }

        #endregion

        #region Duplicate Check

        //private bool _checkDuplicate;
        //private string _visaNumberDuplicateCheck;

        //public string VisaNumberDuplicateCheck
        //{
        //    get { return _visaNumberDuplicateCheck; }
        //    set
        //    {
        //        _visaNumberDuplicateCheck = value;
        //        RaisePropertyChanged<string>(() => VisaNumberDuplicateCheck);

        //        if (!_checkDuplicate) return;
        //        if (string.IsNullOrEmpty(VisaNumberDuplicateCheck)) return;

        //        var criteria = new SearchCriteria<VisaDTO>();
        //        criteria.FiList.Add(v => v.VisaNumber == value && v.Id != SelectedVisa.Id);

        //        var newVisa = new VisaService(true, false)
        //            .GetAll(criteria)
        //            .FirstOrDefault();

        //        if (newVisa == null) return;
        //        MessageBox.Show("There Exists Visa with the same Visa Number: " + VisaNumberDuplicateCheck);
        //        SelectedVisa.VisaNumber = "";

        //        VisaNumberDuplicateCheck = "";
        //    }
        //}

        #endregion

        #region Previleges

        private string _viewVisibility, _addNewVisibility, _saveVisibility, _deleteVisibility;

        public string ViewVisibility
        {
            get { return _viewVisibility; }
            set
            {
                _viewVisibility = value;
                RaisePropertyChanged<string>(() => ViewVisibility);
            }
        }

        public string AddNewVisibility
        {
            get { return _addNewVisibility; }
            set
            {
                _addNewVisibility = value;
                RaisePropertyChanged<string>(() => AddNewVisibility);
            }
        }

        public string SaveVisibility
        {
            get { return _saveVisibility; }
            set
            {
                _saveVisibility = value;
                RaisePropertyChanged<string>(() => SaveVisibility);
            }
        }

        public string DeleteVisibility
        {
            get { return _deleteVisibility; }
            set
            {
                _deleteVisibility = value;
                RaisePropertyChanged<string>(() => DeleteVisibility);
            }
        }

        private void CheckRoles()
        {
            ViewVisibility = UserUtil.UserHasRole(RoleTypes.ViewVisa) ? "Visible" : "Collapsed";

            if (UserUtil.UserHasRole(RoleTypes.EditVisa))
            {
                SaveVisibility = "Visible";
                ViewVisibility = "Visible";
            }
            else
                SaveVisibility = "Collapsed";

            if (UserUtil.UserHasRole(RoleTypes.AddVisa))
            {
                AddNewVisibility = "Visible";
                ViewVisibility = "Visible";
                SaveVisibility = "Visible";
            }
            else
                AddNewVisibility = "Collapsed";

            if (UserUtil.UserHasRole(RoleTypes.DeleteVisa))
            {
                DeleteVisibility = "Visible";
                ViewVisibility = "Visible";
            }
            else
                DeleteVisibility = "Collapsed";
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