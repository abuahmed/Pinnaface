using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Core;
using System.Collections.ObjectModel;
using PinnaFace.Service;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class EmployeeContactPersonViewModel : ViewModelBase
    {
        #region Fields
        private EmployeeDTO _selectedEmployee;
        private ICommand _saveEmployeeRelativeViewCommand;
        private EmployeeRelativeDTO _currentEmployeeRelativeForSearch;
        private ObservableCollection<AddressDTO> _localAgencyAddressDetail;
        private ICommand _localAgencyAddressViewCommand;
        #endregion

        #region Constructor
        public EmployeeContactPersonViewModel()
        {
            CleanUp();

            LoadContactPersons();

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
                    if (SelectedEmployee.ContactPerson == null)
                        SelectedEmployee.ContactPerson = new EmployeeRelativeDTO
                        {
                            Type = RelativeTypes.ContactPerson,
                            Sex = Sex.Male,
                            EmployeeId = SelectedEmployee.Id,
                            Address = new AddressDTO
                            {
                                AddressType = AddressTypes.Local,
                                Country = CountryList.Ethiopia,
                                City = EnumUtil.GetEnumDesc(CityList.AddisAbeba)
                            }
                        };
                }
            }
        }


        public ICommand SaveEmployeeRelativeCommand
        {
            get { return _saveEmployeeRelativeViewCommand ?? (_saveEmployeeRelativeViewCommand = new RelayCommand<Object>(ExecuteSaveEmployeeRelativeViewCommand, CanSave)); }
        }
        private void ExecuteSaveEmployeeRelativeViewCommand(object obj)
        {
            try
            {
                SelectedEmployee.ContactPerson.EmployeeId = SelectedEmployee.Id;

                SelectedEmployee.ContactPerson.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.ContactPerson.DateLastModified = DateTime.Now;
                SelectedEmployee.ContactPerson.Address.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.ContactPerson.Address.DateLastModified = DateTime.Now;
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
        #region
        public EmployeeRelativeDTO SelectedEmployeeContactPersonForSearch
            {
                get { return _currentEmployeeRelativeForSearch; }
                set
                {
                    _currentEmployeeRelativeForSearch = value;
                    RaisePropertyChanged<EmployeeRelativeDTO>(() => SelectedEmployee.ContactPerson);
                    try
                    {
                        if (SelectedEmployeeContactPersonForSearch != null && !string.IsNullOrEmpty(SelectedEmployeeContactPersonForSearch.FullName))
                        {
                            SelectedEmployee.ContactPerson.FullName = SelectedEmployeeContactPersonForSearch.FullName;
                            SelectedEmployee.ContactPerson.Sex = SelectedEmployeeContactPersonForSearch.Sex;
                            SelectedEmployee.ContactPerson.AgeOrBirthDate = SelectedEmployeeContactPersonForSearch.AgeOrBirthDate;
                        }
                    }
                    catch { }

                }
            }


            private ObservableCollection<EmployeeRelativeDTO> _contactPersons;
            public ObservableCollection<EmployeeRelativeDTO> ContactPersons
            {
                get { return _contactPersons; }
                set
                {
                    _contactPersons = value;
                    RaisePropertyChanged<ObservableCollection<EmployeeRelativeDTO>>(() => this.ContactPersons);
                }
            }
            public ObservableCollection<AddressDTO> ContactPersonAdressDetail
            {
                get { return _localAgencyAddressDetail; }
                set
                {
                    _localAgencyAddressDetail = value;
                    RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ContactPersonAdressDetail);
                }
            }
            #endregion

            private void LoadContactPersons()
            {
                var cri = new SearchCriteria<EmployeeRelativeDTO>();
                cri.FiList.Add(e => e.Type == RelativeTypes.ContactPerson);
                var emergencyPrs = new EmployeeRelativeService(true).GetAll(cri).ToList().Distinct();
                ContactPersons = new ObservableCollection<EmployeeRelativeDTO>(emergencyPrs);
            }

            #region Commands
            public ICommand ContactPersonAddressViewCommand
            {
                get { return _localAgencyAddressViewCommand ?? (_localAgencyAddressViewCommand = new RelayCommand(ContactPersonAddress)); }
            }
            public void ContactPersonAddress()
            {
                new AddressEntry(SelectedEmployee.ContactPerson.Address).ShowDialog();
            }
            #endregion

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

    //public class EmployeeContactPersonViewModel : ViewModelBase
    //{
    //    #region Fields
    //    private static IEmployeeRelativeService _employeeRelativeService;
    //    private EmployeeRelativeDTO _currentEmployeeRelative, _currentEmployeeRelativeForSearch;
    //    private EmployeeDTO _employee;
    //    private ICommand _saveEmployeeRelativeViewCommand;
    //    private ObservableCollection<AddressDTO> _localAgencyAddressDetail;
    //    private ICommand _localAgencyAddressViewCommand;
    //    #endregion

    //    #region Constructor
    //    public EmployeeContactPersonViewModel()
    //    {
    //        CleanUp();
    //        _employeeRelativeService = new EmployeeRelativeService();
            
    //        LoadContactPersons();

    //        Messenger.Default.Register<EmployeeDTO>(this, message =>
    //        {
    //            Employee = message;
    //        });

    //    }

    //    public static void CleanUp()
    //    {
    //        if (_employeeRelativeService != null)
    //            _employeeRelativeService.Dispose();
    //    }
    //    #endregion

    //    #region Properties

    //    public EmployeeDTO Employee
    //    {
    //        get { return _employee; }
    //        set
    //        {
    //            _employee = value;
    //            RaisePropertyChanged<EmployeeDTO>(() => Employee);
    //            if (Employee == null) return;

    //            var contactpersons = _employeeRelativeService
    //                .GetAll()
    //                .Where(ag => ag.EmployeeId == Employee.Id && ag.Type==RelativeTypes.ContactPerson).ToList();

    //            if (contactpersons.Any())
    //            {
    //                SelectedEmployeeContactPerson = contactpersons.FirstOrDefault();
    //            }
    //            else
    //            {
    //                SelectedEmployeeContactPerson = new EmployeeRelativeDTO
    //                {
    //                    Type = RelativeTypes.ContactPerson,
    //                    EmployeeId = Employee.Id,
    //                    Sex = Sex.Male,
    //                    Address = new AddressDTO
    //                    {
    //                        AddressType = AddressTypes.Local,
    //                        Country = CountryList.Ethiopia,
    //                        City = EnumUtil.GetEnumDesc(CityList.AddisAbeba)
    //                    }
    //                };
    //            }
    //        }
    //    }
    //    public EmployeeRelativeDTO SelectedEmployeeContactPerson
    //    {
    //        get { return _currentEmployeeRelative; }
    //        set
    //        {
    //            _currentEmployeeRelative = value;
    //            RaisePropertyChanged<EmployeeRelativeDTO>(() => SelectedEmployeeContactPerson);

    //            if (SelectedEmployeeContactPerson != null)
    //            {
    //                ContactPersonAdressDetail = new ObservableCollection<AddressDTO>
    //                {
    //                    SelectedEmployeeContactPerson.Address
    //                };
    //            }
    //        }
    //    }
    //    public EmployeeRelativeDTO SelectedEmployeeContactPersonForSearch
    //    {
    //        get { return _currentEmployeeRelativeForSearch; }
    //        set
    //        {
    //            _currentEmployeeRelativeForSearch = value;
    //            RaisePropertyChanged<EmployeeRelativeDTO>(() => SelectedEmployeeContactPerson);
    //            try
    //            {
    //                if (SelectedEmployeeContactPersonForSearch != null && !string.IsNullOrEmpty(SelectedEmployeeContactPersonForSearch.FullName))
    //                {
    //                    SelectedEmployeeContactPerson.FullName = SelectedEmployeeContactPersonForSearch.FullName;
    //                    SelectedEmployeeContactPerson.Sex = SelectedEmployeeContactPersonForSearch.Sex;
    //                    SelectedEmployeeContactPerson.AgeOrBirthDate = SelectedEmployeeContactPersonForSearch.AgeOrBirthDate;
    //                }
    //            }
    //            catch { }

    //        }
    //    }
        

    //    private ObservableCollection<EmployeeRelativeDTO> _contactPersons;
    //    public ObservableCollection<EmployeeRelativeDTO> ContactPersons
    //    {
    //        get { return _contactPersons; }
    //        set
    //        {
    //            _contactPersons = value;
    //            RaisePropertyChanged<ObservableCollection<EmployeeRelativeDTO>>(() => this.ContactPersons);
    //        }
    //    }
    //    public ObservableCollection<AddressDTO> ContactPersonAdressDetail
    //    {
    //        get { return _localAgencyAddressDetail; }
    //        set
    //        {
    //            _localAgencyAddressDetail = value;
    //            RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ContactPersonAdressDetail);
    //        }
    //    }
    //    #endregion

    //    private void LoadContactPersons()
    //    {
    //        var cri = new SearchCriteria<EmployeeRelativeDTO>();
    //        cri.FiList.Add(e => e.Type == RelativeTypes.ContactPerson);
    //        var emergencyPrs = new EmployeeRelativeService(true).GetAll(cri).ToList().Distinct();
    //        ContactPersons = new ObservableCollection<EmployeeRelativeDTO>(emergencyPrs);
    //    }

    //    #region Commands
    //    public ICommand ContactPersonAddressViewCommand
    //    {
    //        get { return _localAgencyAddressViewCommand ?? (_localAgencyAddressViewCommand = new RelayCommand(ContactPersonAddress)); }
    //    }
    //    public void ContactPersonAddress()
    //    {
    //        new AddressEntry(SelectedEmployeeContactPerson.Address).ShowDialog();
    //    }

    //    public ICommand SaveEmployeeRelativeViewCommand
    //    {
    //        get { return _saveEmployeeRelativeViewCommand ?? (_saveEmployeeRelativeViewCommand = new RelayCommand<Object>(ExecuteSaveEmployeeRelativeViewCommand, CanSave)); }
    //    }
    //    private void ExecuteSaveEmployeeRelativeViewCommand(object obj)
    //    {

    //        try
    //        {
    //            _employeeRelativeService.InsertOrUpdate(SelectedEmployeeContactPerson);
    //            CloseWindow(obj);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine(e.Message);
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
    //    #endregion

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
