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

namespace PinnaFace.WPF.ViewModel
{
    public class EmployeeTestimonyViewModel : ViewModelBase
    {
        #region Fields

        private static IEmployeeRelativeService _employeeRelativeService;
        private ICommand _addNewEmployeeTestimonyViewCommand;
        private ICommand _closeEmployeeTestimonyViewCommand;
        private ICommand _deleteEmployeeTestimonyViewCommand;
        private bool _editCommandVisibility; int _noOfTestimonial;
        private EmployeeDTO _employee;
        private ObservableCollection<EmployeeRelativeDTO> _filteredEmployeeTestimonies;
        private ICommand _saveEmployeeTestimonyViewCommand, _printTestimonyViewCommand;
        private EmployeeRelativeDTO _selectedEmployeeTestimony;
        private EmployeeRelativeDTO _selectedEmployeeTestimonyForSearch;
        private ObservableCollection<EmployeeRelativeDTO> _testimonyPersons;

        #endregion

        #region Constructor

        public EmployeeTestimonyViewModel()
        {
            CleanUp();
           

            var noOfTestimonials = (int)new SettingService(true).GetSetting().NumberOfTestimonials;
            NoOfTestimonials = noOfTestimonials == 1 ? 3 : 2;

            LoadTestimonyPersons();

            Messenger.Default.Register<EmployeeDTO>(this, message => { Employee = message; });

            EditCommandVisibility = false;
        }

        public static void CleanUp()
        {
            if (_employeeRelativeService != null)
                _employeeRelativeService.Dispose();
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
                if (Employee == null) return;

                CheckTestimoniesAndAddNew();

                _employeeRelativeService = new EmployeeRelativeService();
                List<EmployeeRelativeDTO> testimonies = _employeeRelativeService
                    .GetAll()
                    .Where(ag => ag.EmployeeId == Employee.Id && ag.Type == RelativeTypes.Testimony).ToList();

                FilteredEmployeeTestimonies = new ObservableCollection<EmployeeRelativeDTO>(testimonies);

                if (testimonies.Any())
                {
                    SelectedEmployeeTestimony = testimonies.FirstOrDefault();
                    EditCommandVisibility = FilteredEmployeeTestimonies.Count != NoOfTestimonials;
                }
                else
                {
                    ////Select Two Random Testimonials

                    //FilteredEmployeeTestimonies = new ObservableCollection<EmployeeRelativeDTO>(TestimonyPersons.Take(NoOfTestimonials));

                    ////Save the Random Ones To Db
                    //foreach (var filteredEmployeeTestimony in FilteredEmployeeTestimonies)
                    //{
                    //    var feTestimony = AddNewRelative();
                    //    feTestimony.FullName = filteredEmployeeTestimony.FullName;
                    //    feTestimony.Address.SubCity = filteredEmployeeTestimony.Address.SubCity;
                    //    feTestimony.Address.Woreda = filteredEmployeeTestimony.Address.Woreda;
                    //    feTestimony.Address.Kebele = filteredEmployeeTestimony.Address.Kebele;
                    //    feTestimony.Address.HouseNumber = filteredEmployeeTestimony.Address.HouseNumber;
                    //    feTestimony.Address.Mobile = filteredEmployeeTestimony.Address.Mobile;
                    //    _employeeRelativeService.InsertOrUpdate(feTestimony);
                    //}

                    //SelectedEmployeeTestimony = FilteredEmployeeTestimonies.FirstOrDefault();
                    SelectedEmployeeTestimony = AddNewRelative();//SelectedEmployeeTestimony ??
                }
            }
        }

        private void CheckTestimoniesAndAddNew()
        {
            List<EmployeeRelativeDTO> testimonies = new EmployeeRelativeService(true)
                   .GetAll()
                   .Where(ag => ag.EmployeeId == Employee.Id && ag.Type == RelativeTypes.Testimony).ToList();

            if (!testimonies.Any())
            {
                var empRelativeService = new EmployeeRelativeService();
                var fEmployeeTestimonies = new ObservableCollection<EmployeeRelativeDTO>(TestimonyPersons.Take(NoOfTestimonials));

                foreach (var filteredEmployeeTestimony in fEmployeeTestimonies)
                {
                    var feTestimony = AddNewRelative();
                    feTestimony.FullName = filteredEmployeeTestimony.FullName;
                    feTestimony.Address.SubCity = filteredEmployeeTestimony.Address.SubCity;
                    feTestimony.Address.Woreda = filteredEmployeeTestimony.Address.Woreda;
                    feTestimony.Address.Kebele = filteredEmployeeTestimony.Address.Kebele;
                    feTestimony.Address.HouseNumber = filteredEmployeeTestimony.Address.HouseNumber;
                    feTestimony.Address.Mobile = filteredEmployeeTestimony.Address.Mobile;
                    empRelativeService.InsertOrUpdate(feTestimony);
                }
                empRelativeService.Dispose();

            }
        }

        public int NoOfTestimonials
        {
            get { return _noOfTestimonial; }
            set
            {
                _noOfTestimonial = value;
                RaisePropertyChanged<int>(() => NoOfTestimonials);
            }
        }

        private string _printVisibility;
        public string PrintVisibility
        {
            get { return _printVisibility; }
            set
            {
                _printVisibility = value;
                RaisePropertyChanged<string>(() => PrintVisibility);
            }
        }
        private string _addNewVisibility;
        public string AddNewVisibility
        {
            get { return _addNewVisibility; }
            set
            {
                _addNewVisibility = value;
                RaisePropertyChanged<string>(() => AddNewVisibility);
            }
        }
        public bool EditCommandVisibility
        {
            get { return _editCommandVisibility; }
            set
            {
                _editCommandVisibility = value;
                RaisePropertyChanged<bool>(() => EditCommandVisibility);
                if (EditCommandVisibility)
                {
                    PrintVisibility = "Collapsed";
                    AddNewVisibility = "Visible";
                }
                else
                {
                    PrintVisibility = "Visible";
                    AddNewVisibility = "Collapsed";
                }
            }
        }

        public EmployeeRelativeDTO SelectedEmployeeTestimony
        {
            get { return _selectedEmployeeTestimony; }
            set
            {
                _selectedEmployeeTestimony = value;
                RaisePropertyChanged<EmployeeRelativeDTO>(() => SelectedEmployeeTestimony);
                //EditCommandVisibility = SelectedEmployeeTestimony != null;
            }
        }

        public ObservableCollection<EmployeeRelativeDTO> FilteredEmployeeTestimonies
        {
            get { return _filteredEmployeeTestimonies; }
            set
            {
                _filteredEmployeeTestimonies = value;
                RaisePropertyChanged<ObservableCollection<EmployeeRelativeDTO>>(() => FilteredEmployeeTestimonies);
            }
        }

        public EmployeeRelativeDTO SelectedEmployeeTestimonyForSearch
        {
            get { return _selectedEmployeeTestimonyForSearch; }
            set
            {
                _selectedEmployeeTestimonyForSearch = value;
                RaisePropertyChanged<EmployeeRelativeDTO>(() => SelectedEmployeeTestimonyForSearch);

                try
                {
                    if (SelectedEmployeeTestimonyForSearch != null &&
                        !string.IsNullOrEmpty(SelectedEmployeeTestimonyForSearch.FullName))
                    {
                        if (SelectedEmployeeTestimony == null)
                            SelectedEmployeeTestimony = AddNewRelative();
                        SelectedEmployeeTestimony.FullName = SelectedEmployeeTestimonyForSearch.FullName;
                        SelectedEmployeeTestimony.Address.SubCity = SelectedEmployeeTestimonyForSearch.Address.SubCity;
                        SelectedEmployeeTestimony.Address.Woreda = SelectedEmployeeTestimonyForSearch.Address.Woreda;
                        SelectedEmployeeTestimony.Address.Kebele = SelectedEmployeeTestimonyForSearch.Address.Kebele;
                        SelectedEmployeeTestimony.Address.HouseNumber = SelectedEmployeeTestimonyForSearch.Address.HouseNumber;
                        SelectedEmployeeTestimony.Address.Mobile = SelectedEmployeeTestimonyForSearch.Address.Mobile;
                    }
                }
                catch
                {
                }
            }
        }

        public ObservableCollection<EmployeeRelativeDTO> TestimonyPersons
        {
            get { return _testimonyPersons; }
            set
            {
                _testimonyPersons = value;
                RaisePropertyChanged<ObservableCollection<EmployeeRelativeDTO>>(() => TestimonyPersons);
            }
        }

        public EmployeeRelativeDTO AddNewRelative()
        {
            if (Employee != null)
                return new EmployeeRelativeDTO
                {
                    AgencyId = Singleton.Agency.Id,
                    Type = RelativeTypes.Testimony,
                    EmployeeId = Employee.Id,
                    Sex = Sex.Male,
                    Address = new AddressDTO
                    {
                        AddressType = AddressTypes.Local,
                        Country = CountryList.Ethiopia,
                        City = EnumUtil.GetEnumDesc(CityListAmharic.አዲስአበባ)
                    }
                };
            return null;
        }

        private void LoadTestimonyPersons()
        {
            try
            {
                var cri = new SearchCriteria<EmployeeRelativeDTO>();
                cri.FiList.Add(t => t.Type == RelativeTypes.Testimony);
                IEnumerable<EmployeeRelativeDTO> testimonies = new EmployeeRelativeService(true)
                    .GetAll(cri)
                    .GroupBy(o => o.FullName).Select(g => g.FirstOrDefault());//.Distinct();
                TestimonyPersons = new ObservableCollection<EmployeeRelativeDTO>(testimonies);
            }
            catch
            {
                NotifyUtility.ShowCustomBalloon("Can't Load","Can't Load Previously added testimonial!",4000);
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewEmployeeTestimonyViewCommand
        {
            get
            {
                return _addNewEmployeeTestimonyViewCommand ??
                       (_addNewEmployeeTestimonyViewCommand =
                           new RelayCommand(ExecuteAddNewEmployeeTestimonyViewCommand));
            }
        }

        public ICommand SaveEmployeeTestimonyViewCommand
        {
            get
            {
                return _saveEmployeeTestimonyViewCommand ??
                       (_saveEmployeeTestimonyViewCommand =
                           new RelayCommand<Object>(ExecuteSaveEmployeeTestimonyViewCommand, CanSave));
            }
        }
        
        public ICommand PrintTestimonyViewCommand
        {
            get
            {
                return _printTestimonyViewCommand ??
                       (_printTestimonyViewCommand =
                           new RelayCommand<Object>(ExecutePrintTestimonyViewCommand));
            }
        }

        private void ExecutePrintTestimonyViewCommand(object obj)
        {
            GenerateReports.PrintTestimonialLetter(Employee, false);
            CloseWindow(obj);
        }

        public ICommand DeleteEmployeeTestimonyViewCommand
        {
            get
            {
                return _deleteEmployeeTestimonyViewCommand ??
                       (_deleteEmployeeTestimonyViewCommand =
                           new RelayCommand(ExecuteDeleteEmployeeTestimonyViewCommand));
            }
        }

        public ICommand CloseEmployeeTestimonyViewCommand
        {
            get
            {
                return _closeEmployeeTestimonyViewCommand ??
                       (_closeEmployeeTestimonyViewCommand =
                           new RelayCommand<Object>(ExecuteCloseEmployeeTestimonyViewCommand));
            }
        }

        private void ExecuteAddNewEmployeeTestimonyViewCommand()
        {
            SelectedEmployeeTestimony = AddNewRelative();
        }

        private void ExecuteSaveEmployeeTestimonyViewCommand(object obj)
        {
            try
            {
                _employeeRelativeService.InsertOrUpdate(SelectedEmployeeTestimony);
                GetLiveEmployeeTestimonies();
                //if (!EditCommandVisibility)
                //    CloseWindow(obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ExecuteDeleteEmployeeTestimonyViewCommand()
        {
            try
            {
                if (
                    MessageBox.Show("Are you Sure You want to Delete this?", "Delete Testimony",
                        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    SelectedEmployeeTestimony.Enabled = false;
                    _employeeRelativeService.InsertOrUpdate(SelectedEmployeeTestimony);
                    GetLiveEmployeeTestimonies();
                }
            }
            catch
            {
            }
        }

        private void ExecuteCloseEmployeeTestimonyViewCommand(object obj)
        {
            CloseWindow(obj);
        }

        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null || FilteredEmployeeTestimonies.Count != NoOfTestimonials) return;
            window.DialogResult = true;
            window.Close();
        }

        #endregion

        private void GetLiveEmployeeTestimonies()
        {
            try
            {
                var cri = new SearchCriteria<EmployeeRelativeDTO>();
                cri.FiList.Add(t => t.Type == RelativeTypes.Testimony && t.EmployeeId == Employee.Id);
                IEnumerable<EmployeeRelativeDTO> testimonies = _employeeRelativeService.GetAll(cri).Distinct();

                FilteredEmployeeTestimonies = new ObservableCollection<EmployeeRelativeDTO>(testimonies);

                EditCommandVisibility = FilteredEmployeeTestimonies.Count != NoOfTestimonials;
            }
            catch
            {
                var testimonies = new List<EmployeeRelativeDTO>();
                FilteredEmployeeTestimonies = new ObservableCollection<EmployeeRelativeDTO>(testimonies);
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