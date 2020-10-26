using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using PinnaFace.WPF.Models;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
  //   <ItemGroup>
  //  <Content Include="$(SolutionDir)packages\cef.redist.x64.75.1.14\CEF\**\*" Exclude="$(SolutionDir)packages\cef.redist.x64.75.1.14\CEF\x64\**\*;$(SolutionDir)packages\cef.redist.x64.75.1.14\CEF\locales\**\*.pak">
  //    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
  //    <Visible>false</Visible>
  //  </Content>
  //</ItemGroup>
  //<ItemGroup>
  //  <Content Include="$(SolutionDir)packages\cef.redist.x64.75.1.14\CEF\**\en-US.*;$(SolutionDir)packages\cef.redist.x64.75.1.14\CEF\**\de.*">
  //    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
  //    <Visible>false</Visible>
  //  </Content>
  //</ItemGroup>
  //<ItemGroup>
  //  <Content Include="$(SolutionDir)packages\cef.redist.x64.75.1.14\CEF\x64\**\*">
  //    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
  //    <Visible>false</Visible>
  //  </Content>
  //</ItemGroup>
  //<ItemGroup>
  //  <Content Include="$(SolutionDir)packages\CefSharp.Common.75.1.142\CefSharp\x64\**\CefSharp.BrowserSubprocess.*">
  //    <Link>%(RecursiveDir)%(Filename)%(Extension)</Link>
  //    <Visible>false</Visible>
  //  </Content>
  //</ItemGroup>

    public class VisaViewModel : ViewModelBase
    {
        #region Fields

        private const bool NoFilterByType = false;
        private static IVisaService _visaService;
        private static object _obj;

        private ICommand _addNewVisaViewCommand;

        private ObservableCollection<AgentDTO> _agentsForSearch;
        private bool _assignCommandVisibility;

        private ICommand _assignVisaViewCommand;

        private ICommand _deleteVisaViewCommand;

        private bool _editCommandVisibility;
        private bool _emptyControlVisibility;
        private ICommand _refreshCommand;
        private string _searchText;
        private AgentDTO _selectedAgentForSearch;
        private EmployeeDTO _selectedEmployee;
        private VisaDTO _selectedVisa; //, _selectedVisaForSearch, _paramVisa;

        private bool _showAll;
        private string _totalNumberOfVisas;
        private ICommand _viewEditVisaViewCommand;
        private VisaAssignedTypes _visaAssignedTypes;
        private ObservableCollection<VisaDTO> _visas; //, _visasForSearch;
        private IEnumerable<VisaDTO> _visasList;

        #endregion

        #region Constructor

        public VisaViewModel()
        {
            _showAll = false;
            Load();
        }

        private void Load()
        {
            CleanUp();
            _visaService = new VisaService();

            EditCommandVisibility = false;
            EmptyControlVisibility = true;

            CheckRoles();
            GetLiveAgents();
            GetLiveVisas();

            Messenger.Default.Register<VisaModel>(this, message =>
            {
                //ParamVisa=new VisaDTO
                //{
                //    Id = message.VisaId??0
                //};
                SelectedEmployee = SelectedEmployee ?? message.Employee;
            });
        }

        public static void CleanUp()
        {
            if (_visaService != null)
                _visaService.Dispose();
        }

        public void AssignVisa(object obj)
        {
            ExecuteAssignVisaViewCommand(obj);
        }

        #endregion

        #region Properties

        //public VisaDTO ParamVisa
        //{
        //    get { return _paramVisa; }
        //    set
        //    {
        //        _paramVisa = value;
        //        RaisePropertyChanged<VisaDTO>(() => ParamVisa);

        //        if (ParamVisa != null && ParamVisa.Id != 0)
        //        {
        //            ExecuteAssignVisaViewCommand(null);
        //        }
        //    }
        //}

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
                }
                else
                {
                    AssignCommandVisibility = false;
                }
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

        #region Commands

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new RelayCommand(ExcuteRefreshCommand)); }
        }

        public ICommand AssignVisaViewCommand
        {
            get
            {
                return _assignVisaViewCommand ??
                       (_assignVisaViewCommand = new RelayCommand<object>(ExecuteAssignVisaViewCommand, CanSave));
            }
        }

        public ICommand AddNewVisaViewCommand
        {
            get
            {
                return _addNewVisaViewCommand ??
                       (_addNewVisaViewCommand = new RelayCommand(ExecuteAddNewVisaViewCommand));
            }
        }

        public ICommand ViewEditVisaViewCommand
        {
            get
            {
                return _viewEditVisaViewCommand ??
                       (_viewEditVisaViewCommand = new RelayCommand(ExcuteViewEditVisaViewCommand));
            }
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

        public ICommand DeleteVisaViewCommand
        {
            get
            {
                return _deleteVisaViewCommand ??
                       (_deleteVisaViewCommand = new RelayCommand(ExecuteDeleteVisaViewCommand));
            }
        }

        private void ExcuteRefreshCommand()
        {
            _showAll = true;
            Load();
        }

        private void ExecuteAssignVisaViewCommand(object obj)
        {
            try
            {
                if (AttachVisa(true))
                    CloseWindow(obj);
            }
            catch
            {
                MessageBox.Show("Can't Assign Visa to " + SelectedEmployee.FullName);
            }
        }

        public bool AttachVisa(bool isAttach)
        {
            var empService = new EmployeeService();
            try
            {
                IList<EmployeeDTO> visaEmployees = SelectedVisa.Employees;

                if (isAttach && visaEmployees.Count() == SelectedVisa.VisaQuantity)
                {
                    MessageBox.Show("Visa Quantity is less than the number of employees you are going to assign to!!",
                        "Visa Already Assigned", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                //foreach (var employ in visaEmployees)
                //{
                EmployeeDTO emp = empService.GetAll()
                    .FirstOrDefault(e => e.Id == SelectedEmployee.Id);

                if (emp != null)
                {
                    emp.VisaId = isAttach ? SelectedVisa.Id : (int?) null;
                    emp.AgentId = SelectedVisa.ForeignAgentId;
                    if (!isAttach) emp.Visa = null;
                }
                empService.InsertOrUpdate(emp);
                //}
            }
            catch
            {
                MessageBox.Show("Can't Assign Visa to " + SelectedEmployee.FullName);
                return false;
            }
            finally
            {
                empService.Dispose();
                //if (!isAttach)
                //VisaEmployees = null; //SelectedEmployee = null;
            }
            return true;
        }

        private void ExecuteAddNewVisaViewCommand()
        {
            var visa = new VisaDetail(new VisaModel());
            visa.ShowDialog();

            bool? dialogueResult = visa.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                Load();
                EmptyControlVisibility = true;
                EditCommandVisibility = false;
            }
           
        }

        private void ExcuteViewEditVisaViewCommand()
        {
            var visa = new VisaDetail(new VisaModel
            {
                VisaId = SelectedVisa.Id
            });
            visa.ShowDialog();

            bool? dialogueResult = visa.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                Load();

                EmptyControlVisibility = true;
                EditCommandVisibility = false;
            }
        }

        private void ExecuteDeleteVisaViewCommand()
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to delete this Visa?", "Delete Visa",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes)
                    return;
                //Check Constraints Before Deleting
                List<CommandModel> ids = DbCommandUtil.QueryCommand("Select VisaId as Id from Employees " +
                                                                    " where Id='" + SelectedVisa.Id +
                                                                    "' and enabled='1'").ToList();
                if (ids.Count == 0)
                {
                    SelectedVisa.Enabled = false;
                    /**********/
                    SelectedVisa.Condition.Enabled = false;
                    SelectedVisa.Sponsor.Enabled = false;
                    SelectedVisa.Sponsor.Address.Enabled = false;
                    /**********/
                    _visaService.InsertOrUpdate(SelectedVisa);
                    Load();
                }
                else
                    MessageBox.Show("Problem deleting Visa, There may exist Employees Assigned to this Visa," +
                                    " you have to first update or delete those Employees related with visa: " +
                                    SelectedVisa.VisaNumber,
                        "Can't Delete Visa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem deleting Visa, There may exist Employees Assigned to this Visa," +
                                " you have to first update or delete those Employees related with " +
                                SelectedVisa.VisaNumber + Environment.NewLine + ex.Message + Environment.NewLine +
                                ex.InnerException,
                    "Can't Delete Visa", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        #region Visa

        public VisaAssignedTypes SelectedVisaAssignedTypes
        {
            get { return _visaAssignedTypes; }
            set
            {
                _visaAssignedTypes = value;
                RaisePropertyChanged<VisaAssignedTypes>(() => SelectedVisaAssignedTypes);
                GetLiveVisas();
            }
        }

        public IEnumerable<VisaDTO> VisasList
        {
            get { return _visasList; }
            set
            {
                _visasList = value;
                RaisePropertyChanged<IEnumerable<VisaDTO>>(() => VisasList);
            }
        }

        public ObservableCollection<VisaDTO> Visas
        {
            get { return _visas; }
            set
            {
                _visas = value;
                RaisePropertyChanged<ObservableCollection<VisaDTO>>(() => Visas);
                if (Visas != null && Visas.Any())
                {
                    SelectedVisa = Visas.FirstOrDefault();
                }
                else
                {
                    if (SelectedEmployee != null && !string.IsNullOrEmpty(SelectedEmployee.FullName))
                        ExecuteAddNewVisaViewCommand();
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
                    EditCommandVisibility = true;
                }
                else
                    EditCommandVisibility = false;
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => SearchText);
                //if (!string.IsNullOrWhiteSpace(SearchText))
                //{
                _showAll = false;
                GetLiveVisas();
                //}
            }
        }

        public void GetLiveVisas()
        {
            try
            {
                //if (!_showAll)
                //{
                //    GetVisaList();
                //    Visas = new ObservableCollection<VisaDTO>(VisasList);
                //}
                //else
                //{
                var loading = new Loading();
                _obj = loading;
                loading.Show();

                var worker = new BackgroundWorker();
                worker.DoWork += DoWork;

                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
                worker.RunWorkerAsync();


                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "GetLiveVisas");
            }
        }

        public bool GetVisaList()
        {
            try
            {
                int totCount;
                var criteria = new SearchCriteria<VisaDTO>();

                if (!_showAll)
                {
                    criteria.Page = 1;
                    criteria.PageSize = 10;
                }

                if (SelectedAgentForSearch != null && SelectedAgentForSearch.Id != -1)
                {
                    criteria.FiList.Add(e => e.ForeignAgentId == SelectedAgentForSearch.Id);
                }

                #region Search Text

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    string searchText = SearchText.ToLower();
                    criteria.FiList.Add(bp => bp.VisaNumber.ToLower().Contains(searchText) ||
                                              (bp.VisaNumber.ToString().ToLower().Contains(searchText)) ||
                                              (bp.Sponsor != null && bp.Sponsor.Address != null &&
                                               (bp.Sponsor.FullName.ToString().ToLower().Contains(searchText) ||
                                                bp.Sponsor.PassportNumber.ToString().ToLower().Contains(searchText) ||
                                                bp.Sponsor.Address.Mobile.ToString().ToLower().Contains(searchText)))
                        );
                }

                #endregion

                //if (SelectedVisaForSearch != null && SelectedVisaForSearch.Id != -1)
                //{
                //    criteria.FiList.Add(e => e.Id == SelectedVisaForSearch.Id);
                //}
                //else if (SelectedEmployee != null && (SelectedEmployee.VisaId != null && SelectedEmployee.Id != -1))
                //{
                //    criteria.FiList.Add(e => e.Id == SelectedEmployee.VisaId);
                //}

                VisasList = _visaService.GetAll(criteria, out totCount).ToList();

                if (!NoFilterByType)
                    switch (SelectedVisaAssignedTypes)
                    {
                        case VisaAssignedTypes.All:
                            break;
                        case VisaAssignedTypes.AssignedVisa:
                            VisasList = VisasList.Where(v => v.Employees.Count > 0).ToList();
                            break;
                        case VisaAssignedTypes.NotAssgnedVisa:
                            VisasList = VisasList.Where(v => v.Employees.Count == 0).ToList();
                            break;
                    }

                if (totCount < VisasList.Count())
                    totCount = VisasList.Count();

                TotalNumberOfVisas = "Total No. of Visas = " + totCount;

                int sno = 1;
                foreach (VisaDTO visaDTO in VisasList)
                {
                    visaDTO.SerialNumber = sno;
                    sno++;
                }

                #region Visas For Search

                //var dbs = new UnitOfWork(DbContextUtil.GetDbContextInstance());
                //try
                //{
                //    var emps = dbs.Repository<VisaDTO>()
                //        .Query()
                //        .Include(s => s.Sponsor, c => c.Condition)
                //        .Get().ToList();
                //    VisasForSearch = new ObservableCollection<VisaDTO>(emps);
                //}
                //catch
                //{
                //    MessageBox.Show("Problem getting visas for search");
                //}
                //finally
                //{
                //    dbs.Dispose();
                //}

                #endregion

                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                var loading = _obj as Window;
                if (loading != null) loading.Close();

                Visas = new ObservableCollection<VisaDTO>(VisasList);
            }
            catch
            {
                MessageBox.Show("Worker_RunWorkerCompleted");
            }
        }

        private void DoWork(object sender, DoWorkEventArgs ee)
        {
            try
            {
                GetVisaList();
            }
            catch
            {
                MessageBox.Show("DoWork");
            }
        }

        #endregion

        #region Agents

        public ObservableCollection<AgentDTO> AgentsForSearch
        {
            get { return _agentsForSearch; }
            set
            {
                _agentsForSearch = value;
                RaisePropertyChanged<ObservableCollection<AgentDTO>>(() => AgentsForSearch);
            }
        }

        public AgentDTO SelectedAgentForSearch
        {
            get { return _selectedAgentForSearch; }
            set
            {
                _selectedAgentForSearch = value;
                RaisePropertyChanged<AgentDTO>(() => SelectedAgentForSearch);
                //if (SelectedAgent != null && SelectedAgent.Id != 0)
                GetLiveVisas();
            }
        }

        private void GetLiveAgents()
        {
            List<AgentDTO> agentsList = new ForeignAgentService(true, false).GetAll().ToList();
            //Agents = new ObservableCollection<AgentDTO>(agentsList);


            //if (Agents.Count == 0)
            //{
            //    var Agent = new ForeignAgents();
            //    Agent.ShowDialog();

            //    Agents = new ObservableCollection<AgentDTO>(new ForeignAgentService(true, false).GetAll().ToList());

            //    if (Agents.Count == 0)
            //    {
            //        EmptyControlVisibility = false;
            //        MessageBox.Show("There should be at least 1 foreign agent registered!");
            //        return;
            //    }
            //    EmptyControlVisibility = true;
            //}

            AgentsForSearch = new ObservableCollection<AgentDTO>(agentsList);

            if (AgentsForSearch.Count > 1)
                AgentsForSearch.Insert(0, new AgentDTO
                {
                    AgentName = "All",
                    Id = -1
                });
        }

        #endregion

        #region Previleges

        private string _addNewVisibility;
        private string _deleteVisibility;
        private string _saveVisibility;
        private string _viewVisibility;

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