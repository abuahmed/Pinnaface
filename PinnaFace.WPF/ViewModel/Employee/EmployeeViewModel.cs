using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Office.Interop.Excel;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Models;
using PinnaFace.WPF.Views;
using Application = Microsoft.Office.Interop.Excel.Application;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using MessageBox = System.Windows.MessageBox;
using Window = System.Windows.Window;

namespace PinnaFace.WPF.ViewModel
{
    public partial class EmployeeViewModel : ViewModelBase
    {
        #region Fields

        private static IEmployeeService _employeeService;
        private static object _obj;
        private bool _showAll;
        private ICommand _addNewEmployeeCommand;
        private string _applicationListVisibility;
        private string _contactListVisibility;
        private ICommand _deleteEmployeeCommand;
        private string _educationListVisibility;
        private IEnumerable<EmployeeDTO> _employeeList;
        private ObservableCollection<EmployeeDTO> _employees;
        private bool _emptyControlVisibility, _isEmployeeSelectedEnability, _selectAllCheked;
        private ICommand _loadCommand;
        private bool _loadData;
        private ICommand _saveEmployeeCommand;
        private EmployeeDTO _selectedEmployee;//, _selectedEmployeeForSearch;
        private SearchEmployee _searchEmployee;
        private string _totalNumberOfEmployees;
        private ICommand _viewEditEmployeeCommand;
        private string _visaListVisibility;

        #endregion

        #region Constructor

        public EmployeeViewModel()
        {
            _showAll = false;
            FilterStartDate = DateTime.Now.AddYears(-1);
            FilterEndDate = DateTime.Now.AddMonths(1);

            //InitializeTimer();
        }
        //public void InitializeTimer()
        //{
        //    var syncTask = new SyncTask();
        //    syncTask.Start();
        //}

        public ICommand LoadCommand
        {
            get { return _loadCommand ?? (_loadCommand = new RelayCommand(Load)); }
        }

        public bool LoadData
        {
            get { return _loadData; }
            set
            {
                _loadData = value;
                RaisePropertyChanged(() => LoadData);
                if (LoadData)
                {
                    _showAll = true;
                    Load();
                    var currentSetting = XmlSerializerCustom.GetUserSetting();
                    SelectedStatus = (ProcessStatusTypesForDisplay)currentSetting.ListType;
                }
                //LoadData = false;
            }
        }

        public void Load()
        {
            CleanUp();
            _employeeService = new EmployeeService(false, true);

            CheckRoles();
            GetLiveAgents();
            GetLiveEmployees();

            Messenger.Default.Register<SearchEmployee>(this, message =>
            {
                SearchEmployee = message;
            });

        }

        public static void CleanUp()
        {
            if (_employeeService != null)
                _employeeService.Dispose();
            //if (_attachmentService != null)
            //    _attachmentService.Dispose();
        }

        #endregion

        #region Public Properties

        public IEnumerable<EmployeeDTO> EmployeeList
        {
            get { return _employeeList; }
            set
            {
                _employeeList = value;
                RaisePropertyChanged(() => EmployeeList);
            }
        }

        public ObservableCollection<EmployeeDTO> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(() => Employees);

                if (Employees.Any())
                {
                    SelectedEmployee = Employees.FirstOrDefault();
                    EmptyControlVisibility = true;
                }
                else
                    EmptyControlVisibility = false; //ExcuteAddNewEmployeeCommand();

                int i = 1;
                foreach (EmployeeDTO employeeDto in Employees)
                {
                    employeeDto.SerialNumber = i;
                    i++;
                }
            }
        }

        public SearchEmployee SearchEmployee
        {
            get { return _searchEmployee; }
            set
            {
                _searchEmployee = value;
                RaisePropertyChanged(() => SearchEmployee);

                if(SearchEmployee!=null&& SearchEmployee.EmpId!=0)
                if (SelectedEmployee != null && SelectedEmployee.Id != 0)
                {
                    var empDetail = new EmployeeDetail(SelectedEmployee.Id);
                    empDetail.ShowDialog();

                    //bool? dialogueResult = empDetail.DialogResult;
                    //if (dialogueResult != null && (bool)dialogueResult) 
                        Load();
                }
            }
        }

        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged(() => SelectedEmployee);

                IsEmployeeSelectedEnability = SelectedEmployee != null;
                ////Here also manage steps, like hide complain mgmt before completing flight process
                ////also hide close complaining for those with no complain in firstplace
                CheckRoles();
                EnableDisableControls();
                //ShowShortPhoto();
            }
        }

        //public EmployeeDTO SelectedEmployeeForSearch
        //{
        //    get { return _selectedEmployeeForSearch; }
        //    set
        //    {
        //        _selectedEmployeeForSearch = value;
        //        RaisePropertyChanged(() => SelectedEmployeeForSearch);
        //        if (SelectedEmployeeForSearch != null && !string.IsNullOrEmpty(SelectedEmployeeForSearch.EmployeeDetail))
        //        {
        //            SelectedEmployeeForSearch.EmployeeDetail = "";
        //            GetLiveEmployees();
        //        }
        //    }
        //}

        public string TotalNumberOfEmployees
        {
            get { return _totalNumberOfEmployees; }
            set
            {
                _totalNumberOfEmployees = value;
                RaisePropertyChanged(() => TotalNumberOfEmployees);
            }
        }

        public bool EmptyControlVisibility
        {
            get { return _emptyControlVisibility; }
            set
            {
                _emptyControlVisibility = value;
                RaisePropertyChanged(() => EmptyControlVisibility);
            }
        }

        public string VisaListVisibility
        {
            get { return _visaListVisibility; }
            set
            {
                _visaListVisibility = value;
                RaisePropertyChanged(() => VisaListVisibility);
            }
        }

        public string ContactListVisibility
        {
            get { return _contactListVisibility; }
            set
            {
                _contactListVisibility = value;
                RaisePropertyChanged(() => ContactListVisibility);
            }
        }

        public string EducationListVisibility
        {
            get { return _educationListVisibility; }
            set
            {
                _educationListVisibility = value;
                RaisePropertyChanged(() => EducationListVisibility);
            }
        }

        public string ApplicationListVisibility
        {
            get { return _applicationListVisibility; }
            set
            {
                _applicationListVisibility = value;
                RaisePropertyChanged(() => ApplicationListVisibility);
            }
        }

        public bool IsEmployeeSelectedEnability
        {
            get { return _isEmployeeSelectedEnability; }
            set
            {
                _isEmployeeSelectedEnability = value;
                RaisePropertyChanged(() => IsEmployeeSelectedEnability);
            }
        }

        public bool SelectAllCheked
        {
            get { return _selectAllCheked; }
            set
            {
                _selectAllCheked = value;
                RaisePropertyChanged(() => SelectAllCheked);
                
                if (SelectAllCheked)
                {
                    foreach (var employeeDTO in Employees)
                    {
                        employeeDTO.IsSelected = true;
                    }
                }
                else
                {
                    foreach (var employeeDTO in Employees)
                    {
                        employeeDTO.IsSelected = false;
                    }
                }
                
            }
        }
        #endregion

        #region List Boxes

        private ObservableCollection<AddressDTO> _employeeAddressDetail;
        private ICommand _employeeAddressViewCommand;

        public ObservableCollection<AddressDTO> EmployeeAdressDetail
        {
            get { return _employeeAddressDetail; }
            set
            {
                _employeeAddressDetail = value;
                RaisePropertyChanged(() => EmployeeAdressDetail);
            }
        }
        public ICommand EmployeeAddressViewCommand
        {
            get
            {
                return _employeeAddressViewCommand ?? (_employeeAddressViewCommand = new RelayCommand(EmployeeAddress));
            }
        }

        public void EmployeeAddress()
        {
            var addr = new AddressEntry(SelectedEmployee.Address);
            addr.ShowDialog();
            bool? dialogueResult = addr.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                SaveEmployee();
            }
        }
        #endregion

        #region Photo
        //private BitmapImage _employeeShortImage;
        //public BitmapImage EmployeeShortImage
        //{
        //    get { return _employeeShortImage; }
        //    set
        //    {
        //        _employeeShortImage = value;
        //        RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
        //    }
        //}
        //public void ShowShortPhoto()
        //{
        //    if (SelectedEmployee != null)
        //    {
        //        AttachmentDTO shortPhotoAttachment =
        //            new AttachmentService().Find(SelectedEmployee.PhotoId.ToString());
        //        if (shortPhotoAttachment != null)
        //        {
        //            if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
        //            {
        //                var photoPath = ImageUtil.GetPhotoPath();
        //                var fiName = shortPhotoAttachment.AttachmentUrl;
        //                if (fiName != null)
        //                {
        //                    var fname = Path.Combine(photoPath, fiName);
        //                    EmployeeShortImage = !string.IsNullOrWhiteSpace(shortPhotoAttachment.AttachmentUrl)
        //                        ? new BitmapImage(new Uri(fname, true))
        //                        : null;
        //                }
        //                else
        //                    EmployeeShortImage = null;

        //            }
        //            else
        //            {
        //                EmployeeShortImage = ImageUtil.ToImage(shortPhotoAttachment.AttachedFile);
        //            }
        //            //
        //        }
        //    }
        //    //else
        //    //    shortPhotoAttachment = SelectedEmployee.Photo;
        //} 
        #endregion
        
        #region Commands

        private ICommand 
            _employeePhotoViewCommand, _employeeBioDataViewCommand, 
            _employeeCvViewCommand, _loadSummaryViewCommand;

        public ICommand AddNewEmployeeCommand
        {
            get
            {
                return _addNewEmployeeCommand ?? (_addNewEmployeeCommand = new RelayCommand(ExcuteAddNewEmployeeCommand));
            }
        }

        public ICommand ViewEditEmployeeCommand
        {
            get
            {
                return _viewEditEmployeeCommand ??
                       (_viewEditEmployeeCommand = new RelayCommand(ExcuteViewEditEmployeeCommand));
            }
        }

        public ICommand SaveEmployeeCommand
        {
            get
            {
                return _saveEmployeeCommand ??
                       (_saveEmployeeCommand = new RelayCommand(ExcuteSaveEmployeeCommand, CanSave));
            }
        }

        public ICommand DeleteEmployeeCommand
        {
            get
            {
                return _deleteEmployeeCommand ??
                       (_deleteEmployeeCommand = new RelayCommand(ExcuteDeleteEmployeeCommand, CanSave));
            }
        }

        public ICommand EmployeePhotoViewCommand
        {
            get { return _employeePhotoViewCommand ?? (_employeePhotoViewCommand = new RelayCommand(EmployeePhoto)); }
        }

        public ICommand EmployeeBioDataViewCommand
        {
            get { return _employeeBioDataViewCommand ?? (_employeeBioDataViewCommand = new RelayCommand(LoadBioData)); }
        }

        public ICommand EmployeeCvViewCommand
        {
            get { return _employeeCvViewCommand ?? (_employeeCvViewCommand = new RelayCommand(LoadCv)); }
        }

        public ICommand LoadSummaryViewCommand
        {
            get { return _loadSummaryViewCommand ?? (_loadSummaryViewCommand = new RelayCommand(LoadSummary)); }
        }

        public void EmployeePhoto()
        {
            var employeePhoto = new EmployeePhoto(SelectedEmployee);
            employeePhoto.ShowDialog();
            bool? dialogueResult = employeePhoto.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                SaveEmployee();
            }
        }

        public void ExcuteAddNewEmployeeCommand()
        {
            EmptyControlVisibility = true;

            var empDetail = new EmployeeDetail(0);
            empDetail.ShowDialog();

            //bool? dialogueResult = empDetail.DialogResult;
            //if (dialogueResult != null && (bool) dialogueResult)
                Load();
        }

        public void ExcuteViewEditEmployeeCommand()
        {
            EmptyControlVisibility = true;

            if (SelectedEmployee != null && SelectedEmployee.Id != 0)
            {
                var empDetail = new EmployeeDetail(SelectedEmployee.Id);
                empDetail.ShowDialog();

                //bool? dialogueResult = empDetail.DialogResult;
                //if (dialogueResult != null && (bool) dialogueResult) 
                    Load();
            }
        }

        public void ExcuteSaveEmployeeCommand()
        {
            SaveEmployee();
        }

        public bool SaveEmployee()
        {
            try
            {
                _employeeService.InsertOrUpdate(SelectedEmployee);
                
                return true;
            }
            catch
            {
                MessageBox.Show("Problem Saving Employee Data");
                return false;
            }
        }

        public void ExcuteDeleteEmployeeCommand()
        {
            try
            {
                if (SelectedEmployee.Visa != null && SelectedEmployee.VisaId!=null)//should be for those with web enabled
                {//and VisaId is not Null
                    MessageBox.Show("Can't delete employee, first detach visa...");
                    return;
                }

                if (
                    MessageBox.Show("Are you sure you want to delete the employee data?", "Delete Employee",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
                    MessageBoxResult.Yes)
                {
                    var ids = DbCommandUtil.QueryCommand("Select EmployeeId as Id from Complains " +
                                                     " where Id='" + SelectedEmployee.Id + "' and enabled='1'").ToList();
                    if (ids.Count == 0)
                    {
                        SelectedEmployee.Enabled = false;
                        /**********/
                        SelectedEmployee.Address.Enabled = false;
                        SelectedEmployee.ContactPerson.Enabled = false;
                        SelectedEmployee.ContactPerson.Address.Enabled = false;
                        SelectedEmployee.RequiredDocuments.Enabled = false;

                        SelectedEmployee.Education.Enabled = false;
                        SelectedEmployee.Experience.Enabled = false;
                        SelectedEmployee.Hawala.Enabled = false;

                        if (SelectedEmployee.InsuranceProcess != null) SelectedEmployee.InsuranceProcess.Enabled = false;
                        if (SelectedEmployee.LabourProcess != null) SelectedEmployee.LabourProcess.Enabled = false;
                        if (SelectedEmployee.EmbassyProcess != null) SelectedEmployee.EmbassyProcess.Enabled = false;
                        if (SelectedEmployee.FlightProcess != null) SelectedEmployee.FlightProcess.Enabled = false;

                        SelectedEmployee.Photo.Enabled = false;
                        SelectedEmployee.StandPhoto.Enabled = false;

                        ////Should be cleared before Disablng Employee it self
                        //SelectedEmployee.Visa.Enabled = false;
                        //SelectedEmployee.CurrentComplain.Enabled = false;
                        /**********/
                        string stat = _employeeService.InsertOrUpdate(SelectedEmployee);

                        if (string.IsNullOrEmpty(stat))
                            Employees.Remove(SelectedEmployee);
                    } 
                    else 
                        MessageBox.Show("Can't delete employee, first delete complains dependant on this Employee...");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Can't delete employee, try again later..." + 
                    Environment.NewLine + ex.Message + 
                    Environment.NewLine + ex.InnerException);
            }
        }

        #endregion

        #region Load Employee

        public void GetLiveEmployees()
        {
            //if (!_showAll)
            //{
            //    GetEmployeeList();
            //    Employees = new ObservableCollection<EmployeeDTO>(EmployeeList);
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

        public void SetOrderNumbers()
        {
            using (var unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance()))
            {
                var empExits = unitOfWork.Repository<EmployeeDTO>()
                    .Query().Get().FirstOrDefault(emp => emp.OrderNumber == 0);

                if (empExits != null)
                {
                    var employees = unitOfWork.Repository<EmployeeDTO>()
                        .Query().Get(1).OrderBy(em => em.Id).ToList();
                    var orderNum = 1;
                    foreach (var employeeDTO in employees)
                    {
                        if (employeeDTO.OrderNumber == 0)
                        {
                            employeeDTO.OrderNumber = orderNum;
                            unitOfWork.Repository<EmployeeDTO>().Update(employeeDTO);
                        }
                        orderNum++;
                    }
                    unitOfWork.Commit();
                }
            }
            

        }
        public void GetEmployeeList()
        {
            int totCount;
            var criteria = new SearchCriteria<EmployeeDTO>();

            if (!_showAll)
            {
                criteria.Page = 1;
                criteria.PageSize = 10;
            }

            #region For Agent

            if (SelectedAgent != null && SelectedAgent.Id != -1)
            {
                criteria.FiList.Add(e =>
                    e.Visa != null && e.VisaId != null &&
                    e.VisaId != 0 && e.Visa.ForeignAgentId != 0 &&
                    e.Visa.ForeignAgentId == SelectedAgent.Id);
            }

            #endregion

            #region Selected Employee

            //if (SelectedEmployeeForSearch != null && SelectedEmployeeForSearch.Id != -1)
            //{
            //    criteria.FiList.Add(e => e.Id == SelectedEmployeeForSearch.Id);
            //}

            #endregion

            #region Search Text

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var searchText = SearchText.ToLower();
                criteria.FiList.Add(bp => bp.FullName.ToLower().Contains(searchText) || bp.ContractNumber.ToLower().Contains(searchText) ||
                                          bp.PassportNumber.ToLower().Contains(searchText) ||
                                          (bp.Address != null && bp.Address.Mobile.ToLower().Contains(searchText)) ||
                                          (bp.Visa != null && bp.Visa.VisaNumber.ToString().ToLower().Contains(searchText)) ||
                                          (bp.Visa != null && bp.Visa.Sponsor != null &&
                                          (bp.Visa.Sponsor.FullName.ToString().ToLower().Contains(searchText) ||
                                          bp.Visa.Sponsor.PassportNumber.ToString().ToLower().Contains(searchText)))
                                          );
            }

            #endregion

            #region For Status
            
            switch (SelectedStatus)
            {
                case ProcessStatusTypesForDisplay.All:
                    break;
                case ProcessStatusTypesForDisplay.New:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.New);
                    break;
                case ProcessStatusTypesForDisplay.VisaAssigned:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.VisaAssigned);
                    break;
                case ProcessStatusTypesForDisplay.OnProcess:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.OnProcess);
                    break;
                case ProcessStatusTypesForDisplay.LabourProcess:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.OnProcess
                                             && e.LabourProcess != null);
                    break;
                case ProcessStatusTypesForDisplay.EmbassyProcess:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.OnProcess
                                             && e.EmbassyProcess != null);
                    break;
                case ProcessStatusTypesForDisplay.FlightProcess:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.FlightProcess);//&& e.FlightProcess != null
                    break;
                case ProcessStatusTypesForDisplay.Canceled:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.Canceled);//&& e.FlightProcess != null
                    break;
                case ProcessStatusTypesForDisplay.BookedDepartured:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.BookedDepartured);
                    break;
                case ProcessStatusTypesForDisplay.OnGoodCondition:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.OnGoodCondition);
                    break;
                case ProcessStatusTypesForDisplay.Discontinued:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.Discontinued);
                    break;
                case ProcessStatusTypesForDisplay.Lost:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.Lost);
                    break;
                case ProcessStatusTypesForDisplay.Returned:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.Returned);
                    break;
                case ProcessStatusTypesForDisplay.WithComplain:
                    criteria.FiList.Add(e => e.CurrentStatus == ProcessStatusTypes.WithComplain);
                    break;
            }

            try
            {
                var currentSetting = XmlSerializerCustom.GetUserSetting();
                if (currentSetting != null)
                {
                    currentSetting.ListType = (int)SelectedStatus;
                    XmlSerializerCustom.SetUserSetting(currentSetting);
                }
            }
            catch
            {
            }

            #endregion

            #region Date Change

            if (FilterStartDate > DateTime.Now.AddYears(-10) && FilterEndDate > DateTime.Now.AddYears(-10))
            {
                DateTime beginDate = new DateTime(FilterStartDate.Year, FilterStartDate.Month, FilterStartDate.Day, 0, 0,
                    0),
                    endDate = new DateTime(FilterEndDate.Year, FilterEndDate.Month, FilterEndDate.Day, 23, 59, 59);
                switch (SelectedStatus)
                {
                    case ProcessStatusTypesForDisplay.LabourProcess:
                        criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.OnProcess && l.LabourProcess != null &&
                                                 l.LabourProcess.SubmitDate >= beginDate &&
                                                 l.LabourProcess.SubmitDate <= endDate);
                        break;
                    case ProcessStatusTypesForDisplay.EmbassyProcess:
                        criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.OnProcess && l.EmbassyProcess!=null &&
                                                 l.EmbassyProcess.SubmitDate >= beginDate &&
                                                 l.EmbassyProcess.SubmitDate <= endDate);
                        break;
                    //case ProcessStatusTypesForDisplay.FlightProcess:
                    //    criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.OnProcess && l.FlightProcess != null &&
                    //                             l.FlightProcess.SubmitDate >= beginDate &&
                    //                             l.FlightProcess.SubmitDate <= endDate);
                    //    break;
                    case ProcessStatusTypesForDisplay.BookedDepartured:
                        criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.BookedDepartured && l.FlightProcess != null &&
                                                 l.FlightProcess.SubmitDate >= beginDate &&
                                                 l.FlightProcess.SubmitDate <= endDate);
                        break;
                    case ProcessStatusTypesForDisplay.OnGoodCondition:
                        criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.OnGoodCondition && l.FlightProcess != null &&
                                                 l.FlightProcess.SubmitDate >= beginDate &&
                                                 l.FlightProcess.SubmitDate <= endDate);
                        break;
                    case ProcessStatusTypesForDisplay.Returned:
                        criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.Returned &&
                                                 l.AfterFlightStatusDate >= beginDate &&
                                                 l.AfterFlightStatusDate <= endDate);
                        break;
                    case ProcessStatusTypesForDisplay.Lost:
                        criteria.FiList.Add(l => l.CurrentStatus == ProcessStatusTypes.Lost &&
                                                 l.AfterFlightStatusDate >= beginDate &&
                                                 l.AfterFlightStatusDate <= endDate);
                        break;
                }
            }

            #endregion

            EmployeeList = _employeeService.GetAll(criteria, out totCount).ToList();

            if (totCount < EmployeeList.Count())
                totCount = EmployeeList.Count();

            PeriodEnability = SelectedStatus != ProcessStatusTypesForDisplay.All
                              && SelectedStatus != ProcessStatusTypesForDisplay.OnProcess
                              && SelectedStatus != ProcessStatusTypesForDisplay.WithComplain;

            TotalNumberOfEmployees = "Total = " + totCount.ToString(CultureInfo.InvariantCulture);
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                Employees = new ObservableCollection<EmployeeDTO>(EmployeeList);

                var loading = _obj as Window;
                if (loading != null) loading.Close();
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
                SetOrderNumbers();

                GetEmployeeList();
            }
            catch
            {
                MessageBox.Show("DoWork");
            }
        }
        #endregion

        #region Filter List

        private DateTime _filterEndDate;
        private DateTime _filterStartDate;
        private bool _periodEnability;
        private string _searchText;
        private ProcessStatusTypesForDisplay _selectedStatus;

        public bool PeriodEnability
        {
            get { return _periodEnability; }
            set
            {
                _periodEnability = value;
                RaisePropertyChanged(() => PeriodEnability);
            }
        }

        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged(() => FilterStartDate);
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                _filterEndDate = value;
                RaisePropertyChanged(() => FilterEndDate);
            }
        }

        public AgentDTO SelectedAgent
        {
            get { return _selectedAgent; }
            set
            {
                _selectedAgent = value;
                RaisePropertyChanged(() => SelectedAgent);
                if (SelectedAgent != null)
                    GetLiveEmployees();
            }
        }

        public ProcessStatusTypesForDisplay SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                _selectedStatus = value;
                RaisePropertyChanged(() => SelectedStatus);

                PeriodEnability = false;
                if(LoadData)
                    LoadData = false;
                else 
                GetLiveEmployees();
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => SearchText);
                ////if (!string.IsNullOrWhiteSpace(SearchText))
                //_showAll = false;
                //GetLiveEmployees();
                //_showAll = true;
                ////FilterBySearchText(SearchText);
            }
        }

        //public void FilterBySearchText(string searchTxt)
        //{
        //        if (!string.IsNullOrWhiteSpace(searchTxt))
        //        {
        //            IEnumerable<EmployeeDTO> empList =
        //                EmployeeList.Where(e => e.EmployeeDetail.ToLower().Contains(searchTxt));
        //            Employees = new ObservableCollection<EmployeeDTO>(empList);
        //        }
        //        else
        //            Employees = new ObservableCollection<EmployeeDTO>(EmployeeList);
        //}
        
        #endregion

        #region Previleges and Enability
        
        #region Fields
        private string _manageEmployee, _viewEmployee, _addEmployee, _editEmployee, _deleteEmployee, _employeeReports;
        private string _manageVisa, _viewVisa, _assignVisa, _addVisa, _editVisa, _deleteVisa, _detachVisa;
        private string _manageComplain, _openComplain, _editComplain, _closeComplain, _reOpenComplain, _confirmComplain;
        private string _manageLabour, _labourEntry, _discontinueProcess, _deleteLabourData, _labourReports;
        private string _manageEmbassy, _embassyEntry, _deleteEmbassyData, _embassyReports, _enjazEntry;
        private string _manageFlight, _flightEntry, _deleteFlightData, _flightReports, _afterFlightEntry;

        #endregion

        #region Properties
        public string ManageEmployee
        {
            get { return _manageEmployee; }
            set
            {
                _manageEmployee = value;
                RaisePropertyChanged(() => ManageEmployee);
            }
        }
        public string ViewEmployee
        {
            get { return _viewEmployee; }
            set
            {
                _viewEmployee = value;
                RaisePropertyChanged(() => ViewEmployee);
            }
        }
        public string AddEmployee
        {
            get { return _addEmployee; }
            set
            {
                _addEmployee = value;
                RaisePropertyChanged(() => AddEmployee);
            }
        }
        public string EditEmployee
        {
            get { return _editEmployee; }
            set
            {
                _editEmployee = value;
                RaisePropertyChanged(() => EditEmployee);
            }
        }
        public string DeleteEmployee
        {
            get { return _deleteEmployee; }
            set
            {
                _deleteEmployee = value;
                RaisePropertyChanged(() => DeleteEmployee);
            }
        }
        public string EmployeeReports
        {
            get { return _employeeReports; }
            set
            {
                _employeeReports = value;
                RaisePropertyChanged(() => EmployeeReports);
            }
        }

        public string ManageVisa
        {
            get { return _manageVisa; }
            set
            {
                _manageVisa = value;
                RaisePropertyChanged(() => ManageVisa);
            }
        }
        public string ViewVisa
        {
            get { return _viewVisa; }
            set
            {
                _viewVisa = value;
                RaisePropertyChanged(() => ViewVisa);
            }
        }
        public string AssignVisa
        {
            get { return _assignVisa; }
            set
            {
                _assignVisa = value;
                RaisePropertyChanged(() => AssignVisa);
            }
        }
        public string AddVisa
        {
            get { return _addVisa; }
            set
            {
                _addVisa = value;
                RaisePropertyChanged(() => AddVisa);
            }
        }
        public string EditVisa
        {
            get { return _editVisa; }
            set
            {
                _editVisa = value;
                RaisePropertyChanged(() => EditVisa);
            }
        }
        public string DeleteVisa
        {
            get { return _deleteVisa; }
            set
            {
                _deleteVisa = value;
                RaisePropertyChanged(() => DeleteVisa);
            }
        }
        public string DetachVisa
        {
            get { return _detachVisa; }
            set
            {
                _detachVisa = value;
                RaisePropertyChanged(() => DetachVisa);
            }
        }

        public string ManageComplain
        {
            get { return _manageComplain; }
            set
            {
                _manageComplain = value;
                RaisePropertyChanged(() => ManageComplain);
            }
        }
        public string OpenComplain
        {
            get { return _openComplain; }
            set
            {
                _openComplain = value;
                RaisePropertyChanged(() => OpenComplain);
            }
        }
        public string EditComplain
        {
            get { return _editComplain; }
            set
            {
                _editComplain = value;
                RaisePropertyChanged(() => EditComplain);
            }
        }
        public string CloseComplain
        {
            get { return _closeComplain; }
            set
            {
                _closeComplain = value;
                RaisePropertyChanged(() => CloseComplain);
            }
        }
        public string ReOpenComplain
        {
            get { return _reOpenComplain; }
            set
            {
                _reOpenComplain = value;
                RaisePropertyChanged(() => ReOpenComplain);
            }
        }
        public string ConfirmComplain
        {
            get { return _confirmComplain; }
            set
            {
                _confirmComplain = value;
                RaisePropertyChanged(() => ConfirmComplain);
            }
        }

        public string ManageLabour
        {
            get { return _manageLabour; }
            set
            {
                _manageLabour = value;
                RaisePropertyChanged(() => ManageLabour);
            }
        }
        public string LabourEntry
        {
            get { return _labourEntry; }
            set
            {
                _labourEntry = value;
                RaisePropertyChanged(() => LabourEntry);
            }
        }
        public string DiscontinueProcess
        {
            get { return _discontinueProcess; }
            set
            {
                _discontinueProcess = value;
                RaisePropertyChanged(() => DiscontinueProcess);
            }
        }
        public string DeleteLabourData
        {
            get { return _deleteLabourData; }
            set
            {
                _deleteLabourData = value;
                RaisePropertyChanged(() => DeleteLabourData);
            }
        }
        public string LabourReports
        {
            get { return _labourReports; }
            set
            {
                _labourReports = value;
                RaisePropertyChanged(() => LabourReports);
            }
        }

        public string ManageEmbassy
        {
            get { return _manageEmbassy; }
            set
            {
                _manageEmbassy = value;
                RaisePropertyChanged(() => ManageEmbassy);
            }
        }
        public string EmbassyEntry
        {
            get { return _embassyEntry; }
            set
            {
                _embassyEntry = value;
                RaisePropertyChanged(() => EmbassyEntry);
            }
        }
        public string DeleteEmbassyData
        {
            get { return _deleteEmbassyData; }
            set
            {
                _deleteEmbassyData = value;
                RaisePropertyChanged(() => DeleteEmbassyData);
            }
        }
        public string EmbassyReports
        {
            get { return _embassyReports; }
            set
            {
                _embassyReports = value;
                RaisePropertyChanged(() => EmbassyReports);
            }
        }
        public string EnjazEntry
        {
            get { return _enjazEntry; }
            set
            {
                _enjazEntry = value;
                RaisePropertyChanged(() => EnjazEntry);
            }
        }

        public string ManageFlight
        {
            get { return _manageFlight; }
            set
            {
                _manageFlight = value;
                RaisePropertyChanged(() => ManageFlight);
            }
        }
        public string FlightEntry
        {
            get { return _flightEntry; }
            set
            {
                _flightEntry = value;
                RaisePropertyChanged(() => FlightEntry);
            }
        }
        public string DeleteFlightData
        {
            get { return _deleteFlightData; }
            set
            {
                _deleteFlightData = value;
                RaisePropertyChanged(() => DeleteFlightData);
            }
        }
        public string FlightReports
        {
            get { return _flightReports; }
            set
            {
                _flightReports = value;
                RaisePropertyChanged(() => FlightReports);
            }
        }
        public string AfterFlightEntry
        {
            get { return _afterFlightEntry; }
            set
            {
                _afterFlightEntry = value;
                RaisePropertyChanged(() => AfterFlightEntry);
            }
        }
        #endregion

        private void EnableDisableControls()
        {
            if (SelectedEmployee != null)
            {
                if (SelectedEmployee.Visa == null)
                {
                    ManageLabour = "Collapsed";
                    ManageEmbassy = "Collapsed";
                    ManageFlight = "Collapsed";
                    ManageComplain = "Collapsed";

                    DetachVisa = "Collapsed";
                    DiscontinueProcess = "Collapsed";
                    AfterFlightEntry = "Collapsed";
                }
                if (SelectedEmployee.Visa != null)
                {
                    DeleteEmployee = "Collapsed";
                }
                if (SelectedEmployee.CurrentStatus == ProcessStatusTypes.WithComplain)
                {
                    OpenComplain = "Collapsed";
                    ManageLabour = "Collapsed";
                    ManageEmbassy = "Collapsed";
                    ManageFlight = "Collapsed";
                    
                    DetachVisa = "Collapsed";
                    DiscontinueProcess = "Collapsed";
                    AfterFlightEntry = "Collapsed";
                }

                if (SelectedEmployee.CurrentStatus == ProcessStatusTypes.OnGoodCondition ||
                    SelectedEmployee.CurrentStatus == ProcessStatusTypes.Returned ||
                    SelectedEmployee.CurrentStatus == ProcessStatusTypes.Lost)
                {
                    ManageLabour = "Collapsed";
                    ManageEmbassy = "Collapsed";
                    //ManageFlight = "Collapsed";

                    CloseComplain = "Collapsed";
                    DiscontinueProcess = "Collapsed";
                }
                if (SelectedEmployee.LabourProcess != null || SelectedEmployee.EmbassyProcess != null ||
                    SelectedEmployee.FlightProcess != null)
                {
                    ManageVisa = "Collapsed";
                    DeleteEmployee = "Collapsed";
                }

                if (SelectedEmployee.LabourProcess == null)
                {
                    DiscontinueProcess = "Collapsed";
                    DeleteLabourData = "Collapsed";
                    LabourReports = "Collapsed";

                    ////Can do the next ones Labour data should't be mandatory only Visa is required.
                    //ManageEmbassy = "Collapsed";
                    //ManageFlight = "Collapsed";
                }

                if (SelectedEmployee.EmbassyProcess == null)
                {
                    DeleteEmbassyData = "Collapsed";
                    EmbassyReports = "Collapsed";

                    ////Can do the next ones Embassy data should't be mandatory only Visa is required.
                    //ManageFlight = "Collapsed";
                }

                if (SelectedEmployee.FlightProcess == null)
                {
                    DeleteFlightData = "Collapsed";
                    AfterFlightEntry = "Collapsed";
                    FlightReports = "Collapsed";

                    ManageComplain = "Collapsed";
                }
                if (SelectedEmployee.FlightProcess != null && !SelectedEmployee.FlightProcess.Departured)
                {
                    ManageComplain = "Collapsed";
                    AfterFlightEntry = "Collapsed";
                }

            }
        }

        private void CheckRoles()
        {
            //Employee
            ManageEmployee = Singleton.UserRoles.ViewEmployee == "Visible" ||
                Singleton.UserRoles.AddEmployee == "Visible" ||
                Singleton.UserRoles.EditEmployee == "Visible" ||
                Singleton.UserRoles.DeleteEmployee == "Visible"
                ? "Visible" : "Collapsed";
            ViewEmployee = Singleton.UserRoles.ViewEmployee;
            AddEmployee = Singleton.UserRoles.AddEmployee;
            EditEmployee = Singleton.UserRoles.EditEmployee;
            DeleteEmployee = Singleton.UserRoles.DeleteEmployee;

            //Visa
            ManageVisa = Singleton.UserRoles.ViewVisa == "Visible" ||
                Singleton.UserRoles.AssignVisa == "Visible" ||
                Singleton.UserRoles.AddVisa == "Visible" ||
                Singleton.UserRoles.EditVisa == "Visible" ||
                Singleton.UserRoles.DeleteVisa == "Visible" ||
                Singleton.UserRoles.DetachVisa == "Visible"
                ? "Visible" : "Collapsed";

            ViewVisa = Singleton.UserRoles.ViewVisa;
            AssignVisa = Singleton.UserRoles.AssignVisa;
            AddVisa = Singleton.UserRoles.AddVisa;
            EditVisa = Singleton.UserRoles.EditVisa;
            DeleteVisa = Singleton.UserRoles.DeleteVisa;
            DetachVisa = Singleton.UserRoles.DetachVisa;

            //Complain 
            ManageComplain = Singleton.UserRoles.OpenComplain == "Visible" ||
                Singleton.UserRoles.EditComplain == "Visible" ||
                Singleton.UserRoles.CloseComplain == "Visible" ||
                Singleton.UserRoles.ReOpenComplain == "Visible" ||
                Singleton.UserRoles.ConfirmComplain == "Visible"
                ? "Visible" : "Collapsed";
            OpenComplain = Singleton.UserRoles.OpenComplain;
            EditComplain = Singleton.UserRoles.EditComplain;
            CloseComplain = Singleton.UserRoles.CloseComplain;
            ReOpenComplain = Singleton.UserRoles.ReOpenComplain;
            ConfirmComplain = Singleton.UserRoles.ConfirmComplain;

            //Labour
            ManageLabour = Singleton.UserRoles.LabourEntry == "Visible" ||
                Singleton.UserRoles.DiscontinueProcess == "Visible" ||
                Singleton.UserRoles.DeleteLabourData == "Visible" ||
                Singleton.UserRoles.LabourReports == "Visible" 
                ? "Visible" : "Collapsed";
            LabourEntry = Singleton.UserRoles.LabourEntry;
            DiscontinueProcess = Singleton.UserRoles.DiscontinueProcess;
            DeleteLabourData = Singleton.UserRoles.DeleteLabourData;
            LabourReports = Singleton.UserRoles.LabourReports;

            //Embassy
            ManageEmbassy = Singleton.UserRoles.EmbassyEntry == "Visible" ||
                Singleton.UserRoles.DeleteEmbassyData == "Visible" ||
                Singleton.UserRoles.EmbassyReports == "Visible" ||
                Singleton.UserRoles.EnjazEntry == "Visible" 
                ? "Visible" : "Collapsed";
            EmbassyEntry = Singleton.UserRoles.EmbassyEntry;
            DeleteEmbassyData = Singleton.UserRoles.DeleteEmbassyData;
            EmbassyReports = Singleton.UserRoles.EmbassyReports;
            EnjazEntry = Singleton.UserRoles.EnjazEntry;

            //Flight
            ManageFlight = Singleton.UserRoles.FlightEntry == "Visible" ||
                Singleton.UserRoles.DeleteFlightData == "Visible" ||
                Singleton.UserRoles.FlightReports == "Visible" ||
                Singleton.UserRoles.AfterFlightEntry == "Visible" 
                ? "Visible" : "Collapsed";
            FlightEntry = Singleton.UserRoles.FlightEntry;
            DeleteFlightData = Singleton.UserRoles.DeleteFlightData;
            FlightReports = Singleton.UserRoles.FlightReports;
            AfterFlightEntry = Singleton.UserRoles.AfterFlightEntry;

        }

        #endregion

        #region Export To Excel

        private ICommand _exportToExcelCommand;

        public ICommand ExportToExcelCommand
        {
            get
            {
                return _exportToExcelCommand ?? (_exportToExcelCommand = new RelayCommand(ExecuteExportToExcelCommand));
            }
        }

        private void ExecuteExportToExcelCommand()
        {
            var emps = Employees.Where(e => e.IsSelected).ToList();
            if (emps.Count == 0)
            {
                MessageBox.Show("First Choose the Employees to be exported to excel!");
                return;
            }

            string[] columnsHeader =
            {
                "Passport No.", "Full Name", "Sex", "Date of Birth", "Place of Birth",
                "Visa No.", "Sponsor Name", "Sponsor Id", "Sponsor City",
                "Labour Submit Data", "Embassy Submit Data", "Flight Date",
                "Status"
            };

            Cursor.Current = Cursors.WaitCursor;
            var xlApp = new Application();

            try
            {
                Workbook excelBook = xlApp.Workbooks.Add();
                var excelWorksheet = (Worksheet) excelBook.Worksheets[1];
                xlApp.Visible = true;

                int rowsTotal = emps.Count;
                int colsTotal = columnsHeader.Count();

                Worksheet with1 = excelWorksheet;
                with1.Cells.Select();
                with1.Cells.Delete();

                int iC = 0;
                for (iC = 0; iC <= colsTotal - 1; iC++)
                {
                    with1.Cells[1, iC + 1].Value = columnsHeader[iC];
                }

                int I = 0;
                for (I = 0; I <= rowsTotal - 1; I++)
                {
                    with1.Cells[I + 2, 0 + 1].value = emps[I].PassportNumber;
                    with1.Cells[I + 2, 1 + 1].value = emps[I].FullName;
                    with1.Cells[I + 2, 2 + 1].value = EnumUtil.GetEnumDesc(emps[I].Sex);
                    with1.Cells[I + 2, 3 + 1].value = emps[I].DateOfBirth.ToShortDateString();
                    with1.Cells[I + 2, 4 + 1].value = emps[I].PlaceOfBirth;
                    VisaDTO visaDTO = emps[I].Visa;
                    if (visaDTO != null)
                    {
                        with1.Cells[I + 2, 5 + 1].value = visaDTO.VisaNumber;
                        if (visaDTO.Sponsor != null)
                        {
                            with1.Cells[I + 2, 6 + 1].value = visaDTO.Sponsor.FullName;
                            with1.Cells[I + 2, 7 + 1].value = visaDTO.Sponsor.PassportNumber;
                            with1.Cells[I + 2, 8 + 1].value = visaDTO.Sponsor.Address.City;
                        }
                    }

                    LabourProcessDTO labourSubmitDate = emps[I].LabourProcess;
                    if (labourSubmitDate != null)
                        with1.Cells[I + 2, 9 + 1].value = labourSubmitDate.SubmitDate.ToShortDateString();
                    EmbassyProcessDTO embassySubmitDate = emps[I].EmbassyProcess;
                    if (embassySubmitDate != null)
                        with1.Cells[I + 2, 10 + 1].value = embassySubmitDate.SubmitDate.ToShortDateString();
                    FlightProcessDTO flightDate = emps[I].FlightProcess;
                    if (flightDate != null)
                        with1.Cells[I + 2, 11 + 1].value = flightDate.SubmitDate.ToShortDateString();

                    with1.Cells[I + 2, 12 + 1].value = EnumUtil.GetEnumDesc(emps[I].CurrentStatus);
                }

                with1.Rows["1:1"].Font.FontStyle = "Bold";
                with1.Rows["1:1"].Font.Size = 12;

                with1.Cells.Columns.AutoFit();
                with1.Cells.Select();
                with1.Cells.EntireColumn.AutoFit();
                with1.Cells[1, 1].Select();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //RELEASE ALLOACTED RESOURCES
                Cursor.Current = Cursors.Default;
                xlApp = null;
            }
        }

        #endregion
        
        #region Agents

        private ObservableCollection<AgentDTO> _agents;
        private AgentDTO _selectedAgent;

        public ObservableCollection<AgentDTO> Agents
        {
            get { return _agents; }
            set
            {
                _agents = value;
                RaisePropertyChanged(() => Agents);
            }
        }
        
        private void GetLiveAgents()
        {
            List<AgentDTO> agentsList = new ForeignAgentService(true, false)
                .GetAll()
                .ToList();
            Agents = new ObservableCollection<AgentDTO>(agentsList);

            Agents.Insert(0, new AgentDTO
            {
                AgentName = "All",
                Id = -1
            });
        }

        #endregion

        #region Open Windows

        #region Enjaz

        private ICommand _goToEnjazViewCommand;
        private ICommand _goToMainEnjazViewCommand;

        public ICommand GoToMainEnjazViewCommand
        {
            get
            {
                return _goToMainEnjazViewCommand ??
                       (_goToMainEnjazViewCommand = new RelayCommand(ExcuteGoToMainEnjazViewCommand));
            }
        }

        public ICommand GoToEnjazViewCommand
        {
            get
            {
                return _goToEnjazViewCommand ??
                       (_goToEnjazViewCommand = new RelayCommand(ExcuteGoToEnjazViewCommand, CanSave));
            }
        }

        private void ExcuteGoToMainEnjazViewCommand()
        {
            try
            {
                var enjaz = new EnjazitBrowser(BrowserTarget.Enjazit);
                enjaz.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + exception.InnerException);
            }
        }

        private void ExcuteGoToEnjazViewCommand()
        {
            //if (SaveEmployee())
            //{
                var enjaz = new EnjazitBrowser(SelectedEmployee, BrowserTarget.Enjazit);
                enjaz.Show();
            //}
        }

        #endregion

        #region Musaned

        private ICommand _goToMusanedViewCommand;
        private ICommand _goToMainMusanedViewCommand;

        public ICommand GoToMainMusanedViewCommand
        {
            get
            {
                return _goToMainMusanedViewCommand ??
                       (_goToMainMusanedViewCommand = new RelayCommand(ExcuteGoToMainMusanedViewCommand));
            }
        }

        public ICommand GoToMusanedViewCommand
        {
            get
            {
                return _goToMusanedViewCommand ??
                       (_goToMusanedViewCommand = new RelayCommand(ExcuteGoToMusanedViewCommand, CanSave));
            }
        }

        private void ExcuteGoToMainMusanedViewCommand()
        {
            var enjaz = new EnjazitBrowser(BrowserTarget.Musaned);
            enjaz.Show();
        }

        private void ExcuteGoToMusanedViewCommand()
        {
            //if (SaveEmployee())
            //{
            var enjaz = new EnjazitBrowser(SelectedEmployee, BrowserTarget.Musaned);
            enjaz.Show();
            //}
        }

        #endregion

        #region Insurance

        private ICommand _goToInsuranceViewCommand;
        private ICommand _goToMainInsuranceViewCommand;

        public ICommand GoToMainInsuranceViewCommand
        {
            get
            {
                return _goToMainInsuranceViewCommand ??
                       (_goToMainInsuranceViewCommand = new RelayCommand(ExcuteGoToMainInsuranceViewCommand));
            }
        }

        public ICommand GoToInsuranceViewCommand
        {
            get
            {
                return _goToInsuranceViewCommand ??
                       (_goToInsuranceViewCommand = new RelayCommand(ExcuteGoToInsuranceViewCommand, CanSave));
            }
        }

        private void ExcuteGoToMainInsuranceViewCommand()
        {
            var enjaz = new EnjazitBrowser(BrowserTarget.UnitedInsurance);
            enjaz.Show();
        }

        private void ExcuteGoToInsuranceViewCommand()
        {
            //if (SaveEmployee())
            //{
            var enjaz = new EnjazitBrowser(SelectedEmployee, BrowserTarget.UnitedInsurance);
            enjaz.Show();
            //}
        }

        #endregion

        #region Visa
        
        private ICommand _visaDetachCommand;
        private ICommand _visaViewCommand;

        public ICommand VisaViewCommand
        {
            get { return _visaViewCommand ?? (_visaViewCommand = new RelayCommand(ExcuteVisaViewCommand, CanSave)); }
        }
        public ICommand VisaDetachCommand
        {
            get
            {
                return _visaDetachCommand ?? (_visaDetachCommand = new RelayCommand(ExcuteVisaDetachCommand, CanSave));
            }
        }
        
        private void ExcuteVisaViewCommand()
        {
            if(SaveVisa())
                Load();
        }

        private bool SaveVisa()
        {
            //if (SaveEmployee())
            //{
            if (SelectedEmployee.VisaId == null)
            {
                var visa = new Visas(new VisaModel
                {
                    Employee = SelectedEmployee
                });
                visa.ShowDialog();
                var dialogueResult = visa.DialogResult;
                if (dialogueResult == null || !(bool)dialogueResult) return false;
                return true;
            }
            else
            {
                var visa = new VisaDetail(new VisaModel
                {
                    Employee = SelectedEmployee,
                    VisaId = SelectedEmployee.VisaId
                });

                visa.ShowDialog();
                var dialogueResult = visa.DialogResult;
                if (dialogueResult == null || !(bool)dialogueResult) return false;
                return true;
            }
               
            //}
            //return false;
        }

        private void ExcuteVisaDetachCommand()
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to DETACH this Visa?", "Detach Visa",
                   MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes)
                    return;

                if (SelectedEmployee.Visa == null || SelectedEmployee.VisaId == 0)
                    return;

                if (SelectedEmployee.LabourProcess != null || SelectedEmployee.EmbassyProcess != null ||
                    SelectedEmployee.FlightProcess != null || SelectedEmployee.CurrentComplain != null)
                {
                    if (MessageBox.Show(
                        "There exists other processes depending on this visa, do you want to also delete them?",
                        "Detach Visa",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                    {
                        return;
                    }

                    SelectedEmployee.LabourProcess = null;
                    SelectedEmployee.EmbassyProcess = null;
                    SelectedEmployee.FlightProcess = null;
                    SelectedEmployee.CurrentComplain = null;
                }

                SelectedEmployee.Visa = null;
                SelectedEmployee.VisaId = null;
                _employeeService.InsertOrUpdate(SelectedEmployee);
                Load();
            }
            catch
            {
                MessageBox.Show("Can't detach visa, try again later...");
            }
        }

        #endregion

        #region Complains

        private ICommand _employeeCloseComplainCommand;
        private ICommand _employeeComplainDeleteCommand;
        private ICommand _employeeComplainViewCommand;
        private ICommand _printComplain;

        public ICommand EmployeeComplainViewCommand
        {
            get
            {
                return _employeeComplainViewCommand ??
                       (_employeeComplainViewCommand = new RelayCommand(ExcuteEmployeeComplainViewCommand, CanSave));
            }
        }
        public ICommand EmployeeComplainDeleteCommand
        {
            get
            {
                return _employeeComplainDeleteCommand ??
                       (_employeeComplainDeleteCommand =
                           new RelayCommand(ExcuteEmployeeComplainDeleteCommand, CanSave));
            }
        }
        public ICommand PrintComplainViewCommand
        {
            get { return _printComplain ?? (_printComplain = new RelayCommand(ExecutePrintComplain)); }
        }
        public ICommand EmployeeCloseComplainCommand
        {
            get
            {
                return _employeeCloseComplainCommand ??
                       (_employeeCloseComplainCommand = new RelayCommand(ExcuteEmployeeCloseComplainCommand, CanSave));
            }
        }
        
        private void ExcuteEmployeeCloseComplainCommand()
        {
            //if (SaveEmployee())
            //{
                var complain = new ComplainSolution(SelectedEmployee);
                complain.ShowDialog();

                bool? dialogueResult = complain.DialogResult;
                if (dialogueResult != null && (bool)dialogueResult)
                {
                    Load();
                }
            //}
        }
        private void ExcuteEmployeeComplainViewCommand()
        {
            //if (SaveEmployee())
            //{
                var complain = new ComplainDetail(SelectedEmployee);
                complain.ShowDialog();

                bool? dialogueResult = complain.DialogResult;
                if (dialogueResult != null && (bool)dialogueResult)
                {
                    Load();
                }
            //}
        }
        private void ExcuteEmployeeComplainDeleteCommand()
        {
            //var complain = EmployeeComplain.FirstOrDefault();
            //if (complain != null)
            //{
            //    if (MessageBox.Show("Are you sure you want to delete the complains data?", "Delete complains Data",
            //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
            //        MessageBoxResult.Yes)
            //    {
            //        new ComplainService(true).Delete(complain.Id.ToString(CultureInfo.InvariantCulture));

            //        SelectedEmployee.CurrentStatus = ProcessStatusTypes.OnGoodCondition;
            //        _employeeService.InsertOrUpdate(SelectedEmployee);

            //        //SetComplains();
            //    }
            //}
        }
        private void ExecutePrintComplain()
        {
            //if (SaveEmployee())
                GenerateReports.PrintComplain(SelectedEmployee);

        }

        #endregion

        #region RequiredDocuments

        private ICommand _requiredDocumentsViewCommand;

        public ICommand RequiredDocumentsViewCommand
        {
            get
            {
                return _requiredDocumentsViewCommand ??
                       (_requiredDocumentsViewCommand = new RelayCommand(ExcuteRequiredDocumentsViewCommand, CanSave));
            }
        }

        private void ExcuteRequiredDocumentsViewCommand()
        {
            var requiredDocuments = new RequiredDocuments(SelectedEmployee);
            requiredDocuments.ShowDialog();

            bool? dialogueResult = requiredDocuments.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                SaveEmployee();
            }

        }

        #endregion

        #region Discontinue Process

        private ICommand _discontinueProcessViewCommand;

        public ICommand DiscontinueProcessViewCommand
        {
            get
            {
                return _discontinueProcessViewCommand ??
                       (_discontinueProcessViewCommand = new RelayCommand(ExcuteDiscontinueProcessViewCommand, CanSave));
            }
        }

        private void ExcuteDiscontinueProcessViewCommand()
        {
            var discontinueProcess = new DiscontinueProcess(SelectedEmployee);
            discontinueProcess.ShowDialog();

            bool? dialogueResult = discontinueProcess.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (!SelectedEmployee.Discontinued)
                {
                    SelectedEmployee.DiscontinuedDate = null;
                }
                SaveEmployee();
            }
            
        }

        #endregion
        
        #region Labour

        private ICommand _labourProcessDeleteCommand, _labourProcessTestimoniesCommand;
        private ICommand _labourProcessViewCommand;

        public ICommand LabourProcessViewCommand
        {
            get
            {
                return _labourProcessViewCommand ??
                       (_labourProcessViewCommand = new RelayCommand(ExcuteLabourProcessViewCommand, CanSave));
            }
        }

        public ICommand LabourProcessTestimoniesCommand
        {
            get
            {
                return _labourProcessTestimoniesCommand ??
                       (_labourProcessTestimoniesCommand = new RelayCommand(ExcuteLabourProcessTestimoniesCommand, CanSave));
            }
        }

        public ICommand LabourProcessDeleteCommand
        {
            get
            {
                return _labourProcessDeleteCommand ??
                       (_labourProcessDeleteCommand = new RelayCommand(ExcuteLabourProcessDeleteCommand, CanSave));
            }
        }

        private void ExcuteLabourProcessViewCommand()
        {
            //if (SaveEmployee())
            //{
                ////if (EmployeeContact.Count == 0)
                ////{
                ////    if (!SaveEmployeeContactPerson())
                ////    {
                ////        MessageBox.Show("Problem on getting Emergency/Relative Person Data");
                ////        return;
                ////    }
                ////}

                ////if (EmployeeEducation.Count == 0)
                ////{
                ////    if (!SaveEmployeeEducation())
                ////    {
                ////        MessageBox.Show("Problem on getting Education Data...");
                ////        return;
                ////    }
                ////}

                ////if (AssignedVisa.Count == 0)
                ////{
                ////    if (!SaveVisa())
                ////    {
                ////        MessageBox.Show("Problem on getting Visa...");
                ////        return;
                ////    }
                ////}
                
                var labourProcess = new LabourProcess(SelectedEmployee);
                labourProcess.ShowDialog();

                bool? dialogueResult = labourProcess.DialogResult;
                if (dialogueResult != null && (bool)dialogueResult)
                {
                    Load();
                }
            //}
        }

        private void ExcuteLabourProcessDeleteCommand()
        {
            LabourProcessDTO labour = SelectedEmployee.LabourProcess;
            if (labour != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the labour data?", "Delete labour Data",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
                    MessageBoxResult.Yes)
                {
                    SelectedEmployee.LabourProcess = null;
                    SaveEmployee();

                    try
                    {
                        new LabourProcessService(true).Delete(labour.Id.ToString(CultureInfo.InvariantCulture));
                    }
                    catch { }

                    Load();

                }
            }
        }

        private void ExcuteLabourProcessTestimoniesCommand()
        {
            var employeeTestimony = new EmployeeTestimony(SelectedEmployee);
            employeeTestimony.ShowDialog();
        }
        
        #endregion

        #region Embassy

        private ICommand _embassyProcessDeleteCommand;
        private ICommand _embassyProcessViewCommand;

        public ICommand EmbassyProcessViewCommand
        {
            get
            {
                return _embassyProcessViewCommand ??
                       (_embassyProcessViewCommand = new RelayCommand(ExcuteEmbassyProcessViewCommand, CanSave));
            }
        }

        public ICommand EmbassyProcessDeleteCommand
        {
            get
            {
                return _embassyProcessDeleteCommand ??
                       (_embassyProcessDeleteCommand = new RelayCommand(ExcuteEmbassyProcessDeleteCommand, CanSave));
            }
        }

        private void ExcuteEmbassyProcessViewCommand()
        {
            //if (SaveEmployee())
            //{
                if (SelectedEmployee.Visa == null)
                {
                    if (!SaveVisa())
                    {
                        MessageBox.Show("Problem on getting Visa");
                        return;
                    }
                }

                var embassyProcess = new EmbassyProcess(SelectedEmployee);
                embassyProcess.ShowDialog();

                bool? dialogueResult = embassyProcess.DialogResult;
                if (dialogueResult != null && (bool) dialogueResult)
                {
                    Load();
                }
            //}
        }

        private void ExcuteEmbassyProcessDeleteCommand()
        {
            EmbassyProcessDTO embassy = SelectedEmployee.EmbassyProcess;
            if (embassy != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the embassy data?", "Delete embassy Data",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
                    MessageBoxResult.Yes)
                {
                    SelectedEmployee.EmbassyProcess = null;
                    SaveEmployee();

                    try
                    {
                        new EmbassyProcessService(true).Delete(embassy.Id.ToString(CultureInfo.InvariantCulture));
                    }
                    catch { }

                    Load();

                }
            }
        }

        #endregion

        #region Flight

        private ICommand _flightProcessDeleteCommand;
        private ICommand _flightProcessViewCommand;

        public ICommand FlightProcessViewCommand
        {
            get
            {
                return _flightProcessViewCommand ??
                       (_flightProcessViewCommand = new RelayCommand(ExcuteFlightProcessViewCommand, CanSave));
            }
        }

        public ICommand FlightProcessDeleteCommand
        {
            get
            {
                return _flightProcessDeleteCommand ??
                       (_flightProcessDeleteCommand = new RelayCommand(ExcuteFlightProcessDeleteCommand, CanSave));
            }
        }

        private void ExcuteFlightProcessViewCommand()
        {
            if(SaveFlightProcess())
                Load();
        }

        private bool SaveFlightProcess()
        {
            //if (SaveEmployee())
            //{
                if (SelectedEmployee.Visa == null)
                {
                    if (!SaveVisa())
                    {
                        MessageBox.Show("Problem on getting Visa");
                        return false;
                    }
                }

                var flightProcess = new FlightProcess(SelectedEmployee);
                flightProcess.ShowDialog();

                bool? dialogueResult = flightProcess.DialogResult;
                if (dialogueResult != null && (bool) dialogueResult)
                {
                    //SetFlight();
                    return true;
                }
            //}
            return false;
        }

        private void ExcuteFlightProcessDeleteCommand()
        {
            FlightProcessDTO flight = SelectedEmployee.FlightProcess;
            if (flight != null)
            {
                if (SelectedEmployee.CurrentComplain !=null)
                {
                    MessageBox.Show("Delete complain data first, it depends on complain data...",
                        "Deleting problem", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to delete the flight data?", "Delete flight Data",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
                    MessageBoxResult.Yes)
                {
                    SelectedEmployee.FlightProcess = null;
                    SaveEmployee();

                    try
                    {
                        new FlightProcessService(true).Delete(flight.Id.ToString(CultureInfo.InvariantCulture));
                    }
                    catch { }

                    Load();
                }
            }
        }

        #endregion

        #region AfterFlight Process

        private ICommand _afterFlightProcessViewCommand;

        public ICommand AfterFlightProcessViewCommand
        {
            get
            {
                return _afterFlightProcessViewCommand ??
                       (_afterFlightProcessViewCommand = new RelayCommand(ExcuteAfterFlightProcessViewCommand, CanSave));
            }
        }

        private void ExcuteAfterFlightProcessViewCommand()
        {
            var afterFlightProcess = new AfterFlightProcess(SelectedEmployee);
            afterFlightProcess.ShowDialog();

            bool? dialogueResult = afterFlightProcess.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (SelectedEmployee.AfterFlightStatus ==
                    AfterFlightStatusTypes.OnGoodCondition)
                {
                    SelectedEmployee.AfterFlightStatusDate = null;
                }

                SaveEmployee();
                Load();
            }

        }

        #endregion

        #endregion
        
        #region Validation

        public static int Errors { get; set; }

        public bool CanSave()
        {
            return Errors == 0;
        }

        #endregion

        #region Reports

        #region PrintBioDataAndCV

        public void LoadBioData()
        {
            GenerateReports.LoadBioData(SelectedEmployee);
        }

        public void LoadCv()
        {
            GenerateReports.LoadCv(SelectedEmployee);
        }

        public void LoadSummary()
        {
            GenerateReports.LoadSummary(EmployeeList, SelectedStatus, SelectedAgent);
        }
        #endregion

        #region ForensicList
        private ICommand _printForensicListCommandView;
        public ICommand PrintForensicListCommandView
        {
            get
            {
                return _printForensicListCommandView ?? (_printForensicListCommandView = new RelayCommand<Object>(PrintForensicList));
            }
        }

        private void PrintForensicList(object obj)
        {
            //SaveEmployee();
            var emps = Employees.Where(e => e.IsSelected).ToList();
            if (emps.Count == 0)
            {
                MessageBox.Show("First Choose the Employees to be included on the list!");
                return;
            }
            GenerateReports.PrintForensicList(emps, obj != null);
        }
        #endregion

        #region Labour Reports

        #region List
        private ICommand _printListCommandView;
        public ICommand PrintListCommandView
        {
            get
            {
                return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintLabourList));
            }
        }

        private void PrintLabourList(object obj)
        {
            //SaveEmployee();
            var emps = Employees.Where(e => e.IsSelected).ToList();
            if (emps.Count == 0)
            {
                MessageBox.Show("First Choose the Employees to be included on the list!");
                return;
            }
            GenerateReports.PrintLabourtList(emps, obj != null);
        }
        #endregion

        #region CoverLetter
        private ICommand _printLetterCommandView;
        public ICommand PrintLetterCommandView
        {
            get
            {
                return _printLetterCommandView ?? (_printLetterCommandView = new RelayCommand<Object>(PrintLetter));
            }
        }
        private void PrintLetter(object obj)
        {
            //SaveEmployee();
            GenerateReports.PrintCoverLetter(SelectedEmployee, obj != null);
        }
        #endregion

        #region Application
        private ICommand _printApplicationCommandView;
        public ICommand PrintApplicationCommandView
        {
            get
            {
                return _printApplicationCommandView ?? (_printApplicationCommandView = new RelayCommand<Object>(PrintApplication));
            }
        }
        private void PrintApplication(object obj)
        {
            //SaveEmployee();
            GenerateReports.PrintLabourApplication(SelectedEmployee, obj != null);
        }

        #endregion

        #region Testimonials
        private ICommand _printTestimonialsCommandView;
        public ICommand PrintTestimonialsCommandView
        {
            get
            {
                return _printTestimonialsCommandView ?? (_printTestimonialsCommandView = new RelayCommand<Object>(PrintTestimonials));
            }
        }
        private void PrintTestimonials(object obj)
        {
            //SaveEmployee();
            var employeeTestimony = new EmployeeTestimony(SelectedEmployee);
            employeeTestimony.ShowDialog();
            //GenerateReports.PrintTestimonialLetter(SelectedEmployee, obj != null);
        }

        #endregion

        //#region VisaTranslation
        //private ICommand _printVisaTranslationCommandView;
        //public ICommand PrintVisaTranslationCommandView
        //{
        //    get
        //    {
        //        return _printVisaTranslationCommandView ?? (_printVisaTranslationCommandView = new RelayCommand<Object>(PrintVisa));
        //    }
        //}
        //private void PrintVisa(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintVisaTranslation2(SelectedEmployee, obj != null);
        //}
        //#endregion

        //#region VisaTranslation English
        //private ICommand _printVisaTranslationEnglishCommandView;
        //public ICommand PrintVisaTranslationEnglishCommandView
        //{
        //    get
        //    {
        //        return _printVisaTranslationEnglishCommandView ?? (_printVisaTranslationEnglishCommandView = new RelayCommand<Object>(PrintVisaEnglish));
        //    }
        //}
        //private void PrintVisaEnglish(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintVisaTranslationEnglish2(SelectedEmployee, obj != null);
        //}
        //#endregion

        //#region WekalaTranslation
        //private ICommand _printWekalaTranslationCommandView;
        //public ICommand PrintWekalaTranslationCommandView
        //{
        //    get
        //    {
        //        return _printWekalaTranslationCommandView ?? (_printWekalaTranslationCommandView = new RelayCommand<Object>(PrintWekala));
        //    }
        //}
        //private void PrintWekala(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintWekalaTranslation2(SelectedEmployee, obj != null);
        //}
        //#endregion

        //#region NormalWekala
        //private ICommand _printNormalWekalaCommandView;
        //public ICommand PrintNormalWekalaCommandView
        //{
        //    get
        //    {
        //        return _printNormalWekalaCommandView ?? (_printNormalWekalaCommandView = new RelayCommand<Object>(PrintNormalWekala));
        //    }
        //}
        //private void PrintNormalWekala(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintNormalWekala(SelectedEmployee.Visa, obj != null);
        //}
        //#endregion

        //#region ConditionArabic
        //private ICommand _printConditionArabicCommandView;
        //public ICommand PrintConditionArabicCommandView
        //{
        //    get
        //    {
        //        return _printConditionArabicCommandView ?? (_printConditionArabicCommandView = new RelayCommand<Object>(PrintConditionArabic));
        //    }
        //}
        //private void PrintConditionArabic(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintConditionArabic(SelectedEmployee.Visa, obj != null);
        //}
        //#endregion

        //#region ConditionTranslation
        //private ICommand _printConditionTranslationCommandView;
        //public ICommand PrintConditionTranslationCommandView
        //{
        //    get
        //    {
        //        return _printConditionTranslationCommandView ?? (_printConditionTranslationCommandView = new RelayCommand<Object>(PrintCondition));
        //    }
        //}
        //private void PrintCondition(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintConditionTranslation(SelectedEmployee.Visa, obj != null);
        //}
        //#endregion

        //#region AgreementFront
        //private ICommand _printAgreementFrontCommandView;
        //public ICommand PrintAgreementFrontCommandView
        //{
        //    get
        //    {
        //        return _printAgreementFrontCommandView ?? (_printAgreementFrontCommandView = new RelayCommand<Object>(PrintAgreementFront));
        //    }
        //}
        //private void PrintAgreementFront(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintAgreementFront(SelectedEmployee, obj != null);
        //}
        //#endregion

        //#region AgreementBack
        //private ICommand _printAgreementBackCommandView;
        //public ICommand PrintAgreementBackCommandView
        //{
        //    get
        //    {
        //        return _printAgreementBackCommandView ?? (_printAgreementBackCommandView = new RelayCommand<Object>(PrintAgreementBack));
        //    }
        //}
        //private void PrintAgreementBack(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintAgreementBack(SelectedEmployee, obj != null);
        //}
        //#endregion

        //#region AgreementFull
        //private ICommand _printAgreementFullCommandView;
        //public ICommand PrintAgreementFullCommandView
        //{
        //    get
        //    {
        //        return _printAgreementFullCommandView ?? (_printAgreementFullCommandView = new RelayCommand<Object>(PrintAgreementFull));
        //    }
        //}
        //private void PrintAgreementFull(object obj)
        //{
        //    GenerateReports.PrintAgreementFull(SelectedEmployee, obj != null);
        //}
        //#endregion

        #region All In One

        private ICommand _printAllInOneCommandView;
        public ICommand PrintAllInOneCommandView
        {
            get
            {
                return _printAllInOneCommandView ?? (_printAllInOneCommandView = new RelayCommand<Object>(PrintLabourAllInOne));
            }
        }
        private void PrintLabourAllInOne(object obj)
        {
            //SaveEmployee();
            GenerateReports.PrintLabourAllInOne(SelectedEmployee, obj != null);
        }
        #endregion

        #endregion

        #region Embassy Reports

        #region List
        private ICommand _printEmbListCommandView;
        public ICommand PrintEmbassyListCommandView
        {
            get
            {
                return _printEmbListCommandView ?? (_printEmbListCommandView = new RelayCommand<Object>(PrintList));
            }
        }
        private void PrintList(object obj)
        {
            //SaveEmployee();
            var emps = Employees.Where(e => e.IsSelected).ToList();
            if (emps.Count == 0)
            {
                MessageBox.Show("First Choose the Employees to be included on the list!");
                return;
            }
            GenerateReports.PrintList(emps, obj != null);
        }
        #endregion  

        #region Application

        private ICommand _printEmbApplicationViewCommand;
        public ICommand PrintEmbApplicationViewCommand
        {
            get
            {
                return _printEmbApplicationViewCommand ?? (_printEmbApplicationViewCommand = new RelayCommand<Object>(PrintEmbassyApplication));
            }
        }
        private void PrintEmbassyApplication(object obj)
        {
            //SaveEmployee();
            GenerateReports.PrintEmbassyApplication(SelectedEmployee, obj != null);
        }

        #endregion

        #region RecruitingOrder
        private ICommand _printRecruitingOrderViewCommand;
        public ICommand PrintRecruitingOrderViewCommand
        {
            get
            {
                return _printRecruitingOrderViewCommand ?? (_printRecruitingOrderViewCommand = new RelayCommand<Object>(PrintRecruitingOrder));
            }
        }
        private void PrintRecruitingOrder(object obj)
        {
            //SaveEmployee();
            GenerateReports.PrintRecruitingOrder(SelectedEmployee, obj != null);
        }
        #endregion

        //#region Pledge
        //private ICommand _printPledgeViewCommand;
        //public ICommand PrintPledgeViewCommand
        //{
        //    get
        //    {
        //        return _printPledgeViewCommand ?? (_printPledgeViewCommand = new RelayCommand<Object>(PrintPledge));
        //    }
        //}
        //private void PrintPledge(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintPledge(SelectedEmployee, obj != null);
        //}
        //#endregion

        //#region Confirmation
        //private ICommand _printConfirmationViewCommand;
        //public ICommand PrintConfirmationViewCommand
        //{
        //    get
        //    {
        //        return _printConfirmationViewCommand ?? (_printConfirmationViewCommand = new RelayCommand<Object>(PrintConfirmation));
        //    }
        //}
        //private void PrintConfirmation(object obj)
        //{
        //    //SaveEmployee();
        //    GenerateReports.PrintConfirmation(SelectedEmployee, obj != null);
        //}
        //#endregion

        #region EmbassySelection
        private ICommand _printEmbassySelectionViewCommand;
        public ICommand PrintEmbassySelectionViewCommand
        {
            get
            {
                return _printEmbassySelectionViewCommand ?? (_printEmbassySelectionViewCommand = new RelayCommand<Object>(PrintEmbassySelection));
            }
        }
        private void PrintEmbassySelection(object obj)
        {
            //SaveEmployee();
            GenerateReports.PrintEmbassySelection(SelectedEmployee, obj != null);
        }
        #endregion

        #region All In One

        private ICommand _printEmbAllInOneCommandView;


        public ICommand PrintEmbAllInOneCommandView
        {
            get
            {
                return _printEmbAllInOneCommandView ?? (_printEmbAllInOneCommandView = new RelayCommand<Object>(PrintAllInOne));
            }
        }

        

        private void PrintAllInOne(object obj)
        {
            //SaveEmployee();
            GenerateReports.PrintAllInOne(SelectedEmployee, obj != null);
        }

        #endregion

        #endregion

        #endregion 
    }
}