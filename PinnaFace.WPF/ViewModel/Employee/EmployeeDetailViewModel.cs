using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
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
    public class EmployeeDetailViewModel : ViewModelBase
    {
        #region Fields

        private static IEmployeeService _employeeService;
        private string _headerText;
        private EmployeeDTO _selectedEmployee;
        private ObservableCollection<ListDataItem> _applyCountries;
        private ListDataItem _selectedAppliedCountry;
        #endregion

        #region Constructor

        public EmployeeDetailViewModel()
        {
            LoadApplyCountries();
            GetLiveAgents();
            
            Load();

            Messenger.Default.Register<Int32>(this, message => { EmployeeId = message; });

            //Messenger.Default.Register<EmployeeDTO>(this, message =>
            //{
                //EPDuplicateCheck = message.PassportNumber;
                //CodeNumberDuplicateCheck = message.CodeNumber;
            //});
        }

        public static void CleanUp()
        {
            if (_employeeService != null)
                _employeeService.Dispose();
        }

        public void Load()
        {
            CleanUp();
            _employeeService = new EmployeeService(false, true);
            HeaderText = "Employee Entry";// "የሰራተኛ መረጃ/Employee Data";//"የሰራተኛዋ/ው መሰረታዊ መረጃ/Employee's Basic Information";

        }
        #endregion

        #region Set Child Methods

        private Int32 _employeeId;

        public Int32 EmployeeId
        {
            get { return _employeeId; }
            set
            {
                _employeeId = value;
                RaisePropertyChanged<Int32>(() => EmployeeId);

                if (EmployeeId == 0)
                {
                    AddNewEmployee();
                    SelectedForeignAgentVisibility = "Visible";
                    AssignVisaVisibility = "Visible";
                    EditVisaVisibility = "Collapsed";
                    
                }
                else
                {
                    var cri = new SearchCriteria<EmployeeDTO>();
                    cri.FiList.Add(e => e.Id == EmployeeId);
                    SelectedEmployee = _employeeService.GetAll(cri).FirstOrDefault();
                    if (SelectedEmployee != null)
                    {
                        HeaderText = HeaderText + " (" + SelectedEmployee.CodeNumber + ")";
                        if (SelectedEmployee.VisaId != null && SelectedEmployee.VisaId != 0)
                            SelectedForeignAgentVisibility = "Collapsed";
                        else
                        {
                            SelectedForeignAgentVisibility = "Visible";
                            SelectedAgent = Agents.FirstOrDefault(a => a.Id == SelectedEmployee.AgentId);
                        }
                    }
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

                if (SelectedEmployee != null && SelectedEmployee.Id != 0)
                {
                    SelectedAppliedCountry =
                        ApplyCountries.FirstOrDefault(e => e.Value == (int) SelectedEmployee.AppliedCountry);

                    if (SelectedEmployee.Visa == null || SelectedEmployee.VisaId == 0)
                    {
                        AssignVisaVisibility = "Visible";
                        EditVisaVisibility = "Collapsed";
                    }
                    else
                    {
                        AssignVisaVisibility = "Collapsed";
                        EditVisaVisibility = "Visible";
                    }
                }
                ShowShortPhoto();
            }
        }

        public void AddNewEmployee()
        {
            SelectedEmployee = CommonUtility.GetNewEmployeeDTO();
        }

        #region Agents

        private ObservableCollection<AgentDTO> _agents;
        private AgentDTO _selectedAgent;
        private string _selectedForeignAgentVisibility, _assignVisaVisibility, _editVisaVisibility;

        public string AssignVisaVisibility
        {
            get { return _assignVisaVisibility; }
            set
            {
                _assignVisaVisibility = value;
                RaisePropertyChanged<string>(() => this.AssignVisaVisibility);
            }
        }
        public string EditVisaVisibility
        {
            get { return _editVisaVisibility; }
            set
            {
                _editVisaVisibility = value;
                RaisePropertyChanged<string>(() => this.EditVisaVisibility);
            }
        }
        public string SelectedForeignAgentVisibility
        {
            get { return _selectedForeignAgentVisibility; }
            set
            {
                _selectedForeignAgentVisibility = value;
                RaisePropertyChanged<string>(() => this.SelectedForeignAgentVisibility);
            }
        }
        
        public AgentDTO SelectedAgent
        {
            get { return _selectedAgent; }
            set
            {
                _selectedAgent = value;
                RaisePropertyChanged<AgentDTO>(() => this.SelectedAgent);
                if (SelectedAgent != null && SelectedAgent.Id!=-1)
                    SelectedEmployee.AgentId = SelectedAgent.Id;
            }
        }
        public ObservableCollection<AgentDTO> Agents
        {
            get { return _agents; }
            set
            {
                _agents = value;
                RaisePropertyChanged<ObservableCollection<AgentDTO>>(() => Agents);
            }
        }
       
        private void GetLiveAgents()
        {
            var agentsList = new ForeignAgentService(true, false).GetAll().ToList();
            Agents = new ObservableCollection<AgentDTO>(agentsList);

            Agents.Insert(0, new AgentDTO
            {
                AgentName = "For All Agents",
                Id = -1
            });
        }
        
        #endregion

        #endregion

        #region Open Windows

        #region Short Photo

        private BitmapImage _employeeShortImage;
        private string _employeeShortImageFileName;
        private ICommand _showEmployeeShortImageCommand;

        public BitmapImage EmployeeShortImage
        {
            get { return _employeeShortImage; }
            set
            {
                _employeeShortImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
            }
        }

        public string EmployeeShortImageFileName
        {
            get { return _employeeShortImageFileName; }
            set
            {
                _employeeShortImageFileName = value;
                RaisePropertyChanged<string>(() => EmployeeShortImageFileName);
            }
        }

        public ICommand ShowEmployeeShortImageCommand
        {
            get
            {
                return _showEmployeeShortImageCommand ??
                       (_showEmployeeShortImageCommand = new RelayCommand<Object>(ExecuteShowEmployeeShortImageViewCommand,CanSave));
            }
        }

        private void ExecuteShowEmployeeShortImageViewCommand(object obj)
        {   
            var file = new OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            bool? result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                EmployeeShortImageFileName = file.FileName;
                EmployeeShortImage = new BitmapImage(new Uri(file.FileName,true));
            }
        }

        #endregion

        #region Address

        private ICommand _employeeAddressViewCommand;

        private ICommand _employeePhotoViewCommand;

        //private BitmapImage _employeeShortImage;

        public ICommand EmployeeAddressViewCommand
        {
            get
            {
                return _employeeAddressViewCommand ??
                       (_employeeAddressViewCommand = new RelayCommand(EmployeeAddress, CanSave));
            }
        }

        public ICommand EmployeePhotoViewCommand
        {
            get { return _employeePhotoViewCommand ?? (_employeePhotoViewCommand = new RelayCommand<Object>(EmployeePhoto,CanSave)); }
        }

        //public BitmapImage EmployeeShortImage
        //{
        //    get { return _employeeShortImage; }
        //    set
        //    {
        //        _employeeShortImage = value;
        //        RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
        //    }
        //}

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

        public void EmployeePhoto(object obj)
        {   
            var employeePhoto = new EmployeePhoto(SelectedEmployee);
            employeePhoto.ShowDialog();
            bool? dialogueResult = employeePhoto.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                SaveEmployee();
                ShowShortPhoto();
            }
        }

        public void ShowShortPhoto()
        {
            AttachmentDTO shortPhotoAttachment =
                new AttachmentService().Find(SelectedEmployee.PhotoId.ToString());
            if (shortPhotoAttachment != null)
            {
                if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
                {
                    var photoPath = ImageUtil.GetPhotoPath();
                    var fiName = shortPhotoAttachment.AttachmentUrl;
                    if (fiName != null)
                    {
                        var fname = Path.Combine(photoPath, fiName);
                        EmployeeShortImage = !string.IsNullOrWhiteSpace(shortPhotoAttachment.AttachmentUrl)
                            ? new BitmapImage(new Uri(fname,true))
                            : null;
                    }
                    else
                        EmployeeShortImage = null;
                    
                }
                else
                {
                    EmployeeShortImage = ImageUtil.ToImage(shortPhotoAttachment.AttachedFile);
                }
                //
            }
            //else
            //    shortPhotoAttachment = SelectedEmployee.Photo;
        }
        
        public ObservableCollection<ListDataItem> ApplyCountries
        {
            get { return _applyCountries; }
            set
            {
                _applyCountries = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => ApplyCountries);
            }
        }

        public ListDataItem SelectedAppliedCountry
        {
            get { return _selectedAppliedCountry; }
            set
            {
                _selectedAppliedCountry = value;
                RaisePropertyChanged<ListDataItem>(() => this.SelectedAppliedCountry);

                if (SelectedAppliedCountry != null && SelectedEmployee!=null)
                    SelectedEmployee.AppliedCountry = (CountryList) SelectedAppliedCountry.Value;
            }
        }

        public void LoadApplyCountries()
        {
            var countries = Singleton.Agency.CountriesOfOpertaion;
            var applyCountry = new List<ListDataItem>();
            var idd = 0;
            foreach (var country in countries)
            {
                applyCountry.Add(new ListDataItem()
                {
                    Display = country.ToString(),
                    Value = idd
                });
                idd++;
            }
            ApplyCountries = new ObservableCollection<ListDataItem>(applyCountry);

        }
        #endregion

        #region Visa

        private ICommand _visaDetachCommand;
        private ICommand _visaViewCommand, _assignVisaViewCommand;

        public ICommand AssignVisaViewCommand
        {
            get { return _assignVisaViewCommand ?? (_assignVisaViewCommand = new RelayCommand(ExcuteAssignVisaViewCommand, CanSave)); }
        }

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
        private void ExcuteAssignVisaViewCommand()
        {
            if (!SaveEmployee()) return;

            var visa = new Visas(new VisaModel
            {
                Employee = SelectedEmployee
            });

            visa.ShowDialog();

            bool? dialogueResult = visa.DialogResult;
            if (dialogueResult != null && !(bool)dialogueResult) return;
            Load();
            EmployeeId = SelectedEmployee.Id;
            //SaveEmployee();
        }
        private void ExcuteVisaViewCommand()
        {
            if (!SaveEmployee()) return;

            var visa = new VisaDetail(new VisaModel
            {
                VisaId = SelectedEmployee.VisaId
            });
            visa.ShowDialog();

            bool? dialogueResult = visa.DialogResult;
            if (dialogueResult != null && !(bool)dialogueResult) return;
            //SaveEmployee();
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
                EmployeeId = SelectedEmployee.Id;
            }
            catch
            {
                MessageBox.Show("Can't detach visa, try again later...");
            }
        }

        #endregion

        #region Education

        private ICommand _employeeEducationDeleteCommand;
        private ICommand _employeeEducationViewCommand;

        public ICommand EmployeeEducationViewCommand
        {
            get
            {
                return _employeeEducationViewCommand ??
                       (_employeeEducationViewCommand = new RelayCommand(ExcuteEmployeeEducationViewCommand, CanSave));
            }
        }

        public ICommand EmployeeEducationDeleteCommand
        {
            get
            {
                return _employeeEducationDeleteCommand ??
                       (_employeeEducationDeleteCommand =
                           new RelayCommand(ExcuteEmployeeEducationDeleteCommand, CanSave));
            }
        }

        private void ExcuteEmployeeEducationViewCommand()
        {
            SaveEmployeeEducation();
        }

        private bool SaveEmployeeEducation()
        {
            if (SaveEmployee())
            {
                var employeeEducation = new EmployeeEducation(SelectedEmployee);
                employeeEducation.ShowDialog();

                bool? dialogueResult = employeeEducation.DialogResult;
                if (dialogueResult == null || !(bool) dialogueResult) return false;
                SaveEmployee();

                return true;
            }
            return false;
        }

        private void ExcuteEmployeeEducationDeleteCommand()
        {
            EmployeeEducationDTO empEduc = SelectedEmployee.Education;
            //if (empEduc != null)
            //{
            //    if (LabourProcess.Count > 0)
            //    {
            //        MessageBox.Show("Delete labour process data first, it depends on education data...",
            //            "Deleting problem", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        return;
            //    }

            //    if (EmbassyProcess.Count > 0)
            //    {
            //        MessageBox.Show("Delete embassy process data first, it depends on education data...",
            //           "Deleting problem", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        return;
            //    }

            //    if (MessageBox.Show("Are you sure you want to delete the education data?", "Delete Education Data",
            //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
            //        MessageBoxResult.Yes)
            //    {

            //        new EmployeeEducationService(true).Delete(empEduc.Id.ToString(CultureInfo.InvariantCulture));

            //        SetEducation();
            //    }
            //}
        }

        #endregion

        #region Contact Person

        private ICommand _empContactPersonViewCommand;
        private ICommand _employeeContactPersonDeleteCommand;

        public ICommand EmployeeContactPersonViewCommand
        {
            get
            {
                return _empContactPersonViewCommand ??
                       (_empContactPersonViewCommand = new RelayCommand(ExcuteEmployeeContactPersonViewCommand, CanSave));
            }
        }

        public ICommand EmployeeContactPersonDeleteCommand
        {
            get
            {
                return _employeeContactPersonDeleteCommand ??
                       (_employeeContactPersonDeleteCommand =
                           new RelayCommand(ExcuteEmployeeContactPersonDeleteCommand, CanSave));
            }
        }

        private void ExcuteEmployeeContactPersonViewCommand()
        {
            SaveEmployeeContactPerson();
        }

        private bool SaveEmployeeContactPerson()
        {
            if (!SaveEmployee()) return false;
            var employeeContactPerson = new EmployeeContactPerson(SelectedEmployee);
            employeeContactPerson.ShowDialog();

            bool? dialogueResult = employeeContactPerson.DialogResult;
            if (dialogueResult == null || !(bool) dialogueResult) return false;

            SaveEmployee();
            //GetContactPerson();
            return true;
        }

        private void ExcuteEmployeeContactPersonDeleteCommand()
        {
            DeleteEmployeeContactPerson();
        }

        private void DeleteEmployeeContactPerson()
        {
            //var empRelative = EmployeeContact.FirstOrDefault();
            //if (empRelative != null)
            //{
            //    if (LabourProcess.Count > 0)
            //    {
            //        MessageBox.Show("Delete labour process data first, it depends on education data...",
            //            "Deleting problem", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        return;
            //    }

            //    if (EmployeeApplication.Count > 0)
            //    {
            //        MessageBox.Show("Delete application data first, it depends on education data...",
            //           "Deleting problem", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //        return;
            //    }

            //    if (MessageBox.Show("Are you sure you want to delete the empergency person data?", "Delete contact person Data",
            //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
            //        MessageBoxResult.Yes)
            //    {
            //        new EmployeeRelativeService(true).Delete(empRelative.Id.ToString(CultureInfo.InvariantCulture));

            //        SetContactPerson();
            //    }
            //}
        }

        #endregion

        #region Experience

        private ICommand _employeeExperienceDeleteCommand;
        private ICommand _employeeExperiencenViewCommand;

        public ICommand EmployeeExperienceViewCommand
        {
            get
            {
                return _employeeExperiencenViewCommand ??
                       (_employeeExperiencenViewCommand =
                           new RelayCommand(ExcuteEmployeeExperienceViewCommand, CanSave));
            }
        }

        public ICommand EmployeeExperienceDeleteCommand
        {
            get
            {
                return _employeeExperienceDeleteCommand ??
                       (_employeeExperienceDeleteCommand =
                           new RelayCommand(ExcuteEmployeeExperienceDeleteCommand, CanSave));
            }
        }

        private void ExcuteEmployeeExperienceViewCommand()
        {
            SaveEmployeeExperience();
        }

        private void SaveEmployeeExperience()
        {
            if (!SaveEmployee()) return;

            var employeeExperience = new EmployeeExperience(SelectedEmployee);
            employeeExperience.ShowDialog();

            bool? dialogueResult = employeeExperience.DialogResult;
            if (dialogueResult != null && !(bool) dialogueResult) return;
            SaveEmployee();
        }

        private void ExcuteEmployeeExperienceDeleteCommand()
        {
            DeleteEmployeeExperience();
        }

        private void DeleteEmployeeExperience()
        {
            //var empApplication = EmployeeApplication.FirstOrDefault();
            //if (empApplication != null)
            //{
            //    if (MessageBox.Show("Are you sure you want to delete the application data?", "Delete application Data",
            //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
            //        MessageBoxResult.Yes)
            //    {
            //        new EmployeeExperienceService(true).Delete(empApplication.Id.ToString(CultureInfo.InvariantCulture));

            //        SetApplication();
            //    }
            //}
        }

        #endregion

        #region Insurance

        private ICommand _insuranceProcessDeleteCommand;
        private ICommand _insuranceProcessnViewCommand;

        public ICommand InsuranceProcessViewCommand
        {
            get
            {
                return _insuranceProcessnViewCommand ??
                       (_insuranceProcessnViewCommand =
                           new RelayCommand(ExcuteInsuranceProcessViewCommand, CanSave));
            }
        }

        public ICommand InsuranceProcessDeleteCommand
        {
            get
            {
                return _insuranceProcessDeleteCommand ??
                       (_insuranceProcessDeleteCommand =
                           new RelayCommand(ExcuteInsuranceProcessDeleteCommand, CanSave));
            }
        }

        private void ExcuteInsuranceProcessViewCommand()
        {
            SaveInsuranceProcess();
        }

        private void SaveInsuranceProcess()
        {
            if (!SaveEmployee()) return;

            var insuranceProcess = new InsuranceProcess(SelectedEmployee);
            insuranceProcess.ShowDialog();

            bool? dialogueResult = insuranceProcess.DialogResult;
            if (dialogueResult != null && !(bool) dialogueResult) return;
            SaveEmployee();
        }

        private void ExcuteInsuranceProcessDeleteCommand()
        {
            DeleteInsuranceProcess();
        }

        private void DeleteInsuranceProcess()
        {
            //var empApplication = EmployeeApplication.FirstOrDefault();
            //if (empApplication != null)
            //{
            //    if (MessageBox.Show("Are you sure you want to delete the application data?", "Delete application Data",
            //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
            //        MessageBoxResult.Yes)
            //    {
            //        new InsuranceProcessService(true).Delete(empApplication.Id.ToString(CultureInfo.InvariantCulture));

            //        SetApplication();
            //    }
            //}
        }

        #endregion

        #region Required Documents

        private ICommand _requiredDocumentsDeleteCommand;
        private ICommand _requiredDocumentsnViewCommand;

        public ICommand RequiredDocumentsViewCommand
        {
            get
            {
                return _requiredDocumentsnViewCommand ??
                       (_requiredDocumentsnViewCommand =
                           new RelayCommand(ExcuteRequiredDocumentsViewCommand, CanSave));
            }
        }

        public ICommand RequiredDocumentsDeleteCommand
        {
            get
            {
                return _requiredDocumentsDeleteCommand ??
                       (_requiredDocumentsDeleteCommand =
                           new RelayCommand(ExcuteRequiredDocumentsDeleteCommand, CanSave));
            }
        }

        private void ExcuteRequiredDocumentsViewCommand()
        {
            SaveRequiredDocuments();
        }

        private void SaveRequiredDocuments()
        {
            if (!SaveEmployee()) return;

            var requiredDocuments = new RequiredDocuments(SelectedEmployee);
            requiredDocuments.ShowDialog();

            bool? dialogueResult = requiredDocuments.DialogResult;
            if (dialogueResult != null && !(bool)dialogueResult) return;
            SaveEmployee();
        }

        private void ExcuteRequiredDocumentsDeleteCommand()
        {
            DeleteRequiredDocuments();
        }

        private void DeleteRequiredDocuments()
        {
            //var empApplication = EmployeeApplication.FirstOrDefault();
            //if (empApplication != null)
            //{
            //    if (MessageBox.Show("Are you sure you want to delete the application data?", "Delete application Data",
            //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
            //        MessageBoxResult.Yes)
            //    {
            //        new RequiredDocumentsService(true).Delete(empApplication.Id.ToString(CultureInfo.InvariantCulture));

            //        SetApplication();
            //    }
            //}
        }

        #endregion

        #region Hawala

        private ICommand _employeeHawalaDeleteCommand;
        private ICommand _employeeHawalaViewCommand;

        public ICommand EmployeeHawalaViewCommand
        {
            get
            {
                return _employeeHawalaViewCommand ??
                       (_employeeHawalaViewCommand =
                           new RelayCommand(ExcuteEmployeeHawalaViewCommand, CanSave));
            }
        }

        public ICommand EmployeeHawalaDeleteCommand
        {
            get
            {
                return _employeeHawalaDeleteCommand ??
                       (_employeeHawalaDeleteCommand =
                           new RelayCommand(ExcuteEmployeeHawalaDeleteCommand, CanSave));
            }
        }

        private void ExcuteEmployeeHawalaViewCommand()
        {
            SaveEmployeeHawala();
        }

        private void SaveEmployeeHawala()
        {
            if (!SaveEmployee()) return;

            var employeeHawala = new EmployeeHawala(SelectedEmployee);
            employeeHawala.ShowDialog();

            bool? dialogueResult = employeeHawala.DialogResult;
            if (dialogueResult != null && !(bool) dialogueResult) return;
            SaveEmployee();
        }

        private void ExcuteEmployeeHawalaDeleteCommand()
        {
            DeleteEmployeeHawala();
        }

        private void DeleteEmployeeHawala()
        {
            //var empApplication = EmployeeApplication.FirstOrDefault();
            //if (empApplication != null)
            //{
            //    if (MessageBox.Show("Are you sure you want to delete the application data?", "Delete application Data",
            //        MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) ==
            //        MessageBoxResult.Yes)
            //    {
            //        new EmployeeHawalaService(true).Delete(empApplication.Id.ToString(CultureInfo.InvariantCulture));

            //        SetApplication();
            //    }
            //}
        }

        #endregion

        #endregion

        #region Commands

        private ICommand _closeEmployeeCommand,
            _dateOfBirthViewCommand;

        private ICommand _passportExpiryDateViewCommand;

        private ICommand _passportIssueDateViewCommand,_documentReceivedDateViewCommand;
        private ICommand _saveEmployeeCommand;

        public ICommand PassportIssueDateViewCommand
        {
            get
            {
                return _passportIssueDateViewCommand ??
                       (_passportIssueDateViewCommand = new RelayCommand(ExcutePassportIssueDate));
            }
        }

        
        public ICommand PassportExpiryDateViewCommand
        {
            get
            {
                return _passportExpiryDateViewCommand ??
                       (_passportExpiryDateViewCommand = new RelayCommand(ExcutePassportExpiryDate));
            }
        }

        public ICommand DocumentReceivedDateViewCommand
        {
            get
            {
                return _documentReceivedDateViewCommand ??
                       (_documentReceivedDateViewCommand = new RelayCommand(ExcuteDocumentReceivedDate));
            }
        }

        public ICommand DateOfBirthDateViewCommand
        {
            get
            {
                return _dateOfBirthViewCommand ??
                       (_dateOfBirthViewCommand = new RelayCommand(ExcuteDateOfBirthDate));
            }
        }

        public ICommand SaveEmployeeCommand
        {
            get
            {
                return _saveEmployeeCommand ??
                       (_saveEmployeeCommand = new RelayCommand<Object>(ExcuteSaveEmployeeCommand, CanSave));
            }
        }

        public ICommand CloseEmployeeCommand
        {
            get { return _closeEmployeeCommand ?? (_closeEmployeeCommand = new RelayCommand<Object>(CloseWindow)); }
        }

        public void ExcutePassportIssueDate()
        {
            var calConv = new Calendar(SelectedEmployee.PassportIssueDate);
            calConv.ShowDialog();
            bool? dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedEmployee.PassportIssueDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public void ExcutePassportExpiryDate()
        {
            var calConv = new Calendar(SelectedEmployee.PassportExpiryDate);
            calConv.ShowDialog();
            bool? dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedEmployee.PassportExpiryDate = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        public void ExcuteDocumentReceivedDate()
        {
            if (SelectedEmployee.DocumentReceivedDate == null)
                SelectedEmployee.DocumentReceivedDate = DateTime.Now;

            var calConv = new Calendar(SelectedEmployee.DocumentReceivedDate.Value);
            calConv.ShowDialog();
            bool? dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedEmployee.DocumentReceivedDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        public void ExcuteDateOfBirthDate()
        {
            var calConv = new Calendar(SelectedEmployee.DateOfBirth);
            calConv.ShowDialog();
            bool? dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool) dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedEmployee.DateOfBirth = (DateTime) calConv.DtSelectedDate.SelectedDate;
            }
        }

        private void ExcuteSaveEmployeeCommand(object obj)
        {
            try
            {
                SaveEmployee();
                if (obj != null)
                    CloseWindow(obj);
                else
                {
                    AddNewEmployee();
                    SelectedForeignAgentVisibility = "Visible";
                    AssignVisaVisibility = "Visible";
                    EditVisaVisibility = "Collapsed";
                }
            }

            catch
            {
                MessageBox.Show("Can't Save SelectedEmployee!");
            }
        }

        public bool SaveEmployee()
        {
            //var isNew = false;
            try
            {
                if (EmployeeShortImage != null && EmployeeShortImage.UriSource != null &&
                    !string.IsNullOrWhiteSpace(EmployeeShortImageFileName))
                {
                    string photoPath = ImageUtil.GetPhotoPath();
                    // _shortPhotoAttachment.AttachedFile = ImageUtil.ToBytes(EmployeeShortImage);
                    if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
                    {
                        string fiName = Guid.NewGuid() + ".jpg"; // SelectedEmployee.RowGuid + "_Short.jpg";
                        SelectedEmployee.Photo.AttachmentUrl = fiName;
                        File.Copy(EmployeeShortImageFileName, Path.Combine(photoPath, fiName), true);
                    }
                    else
                    {
                        SelectedEmployee.Photo.AttachedFile = ImageUtil.ToBytes(EmployeeShortImage);
                        SelectedEmployee.Photo.RowGuid = Guid.NewGuid();
                    }
                    SelectedEmployee.Photo.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                    SelectedEmployee.Photo.DateLastModified = DateTime.Now;
                    //SelectedEmployee.Photo = _shortPhotoAttachment;
                }

                string msg = _employeeService.InsertOrUpdate(SelectedEmployee);
                if (string.IsNullOrEmpty(msg))
                    return true;
                MessageBox.Show(msg);
                return false;
            }
            catch
            {
                MessageBox.Show("Problem Saving SelectedEmployee Data");
                return false;
            }
        }


        public void CloseWindow(object obj)
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

        #region Duplicate Check

        //private string _codeNumberDuplicateCheck;
        //private string _ePDuplicateCheck;

        //public string EPDuplicateCheck
        //{
        //    get { return _ePDuplicateCheck; }
        //    set
        //    {
        //        _ePDuplicateCheck = value;
        //        RaisePropertyChanged<string>(() => EPDuplicateCheck);
        //        if (string.IsNullOrEmpty(EPDuplicateCheck)) return;
        //        try
        //        {
        //            var uw = new UnitOfWork(DbContextUtil.GetDbContextInstance());
        //            try
        //            {
        //                EmployeeDTO newEmployee = uw.Repository<EmployeeDTO>()
        //                    .Query()
        //                    .Filter(e => e.PassportNumber == EPDuplicateCheck && e.Id != SelectedEmployee.Id)
        //                    .Get().FirstOrDefault();
        //                if (newEmployee == null) return;
        //                MessageBox.Show("There Exists Employee with the same Passport Number: " +
        //                                EPDuplicateCheck);
        //                SelectedEmployee.PassportNumber = "";
        //                EPDuplicateCheck = "";
        //            }
        //            finally
        //            {
        //                uw.Dispose();
        //            }
        //        }
        //        catch
        //        {
        //            Console.WriteLine("");
        //        }
        //    }
        //}

        //public string CodeNumberDuplicateCheck
        //{
        //    get { return _codeNumberDuplicateCheck; }
        //    set
        //    {
        //        _codeNumberDuplicateCheck = value;
        //        RaisePropertyChanged<string>(() => CodeNumberDuplicateCheck);
        //        if (string.IsNullOrEmpty(CodeNumberDuplicateCheck)) return;
        //        try
        //        {
        //            var cr = new SearchCriteria<EmployeeDTO>();
        //            cr.FiList.Add(e => e.CodeNumber == value && e.Id != SelectedEmployee.Id);
        //            EmployeeDTO newEmployee = new EmployeeService(true, false).GetAll(cr).FirstOrDefault();
        //            if (newEmployee == null) return;
        //            MessageBox.Show("There Exists Employee with the same Code Number: " +
        //                            CodeNumberDuplicateCheck);
        //            SelectedEmployee.CodeNumber = "";
        //            CodeNumberDuplicateCheck = "";
        //        }
        //        catch
        //        {
        //            Console.WriteLine("");
        //        }
        //    }
        //}

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }

        public bool CanSave(object obj)
        {
            return Errors == 0;
        }

        public bool CanSave()
        {
            return Errors == 0;
        }

        #endregion
    }
}