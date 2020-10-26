using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Reports;
using PinnaFace.Reports.Labour;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class LabourProcessViewModel : ViewModelBase
    {
        #region Fields
        private static IEmployeeService _employeeService;
        private EmployeeDTO _employeeee;
        private DateTime _maxDate;
        private bool _expanderEnability;
        private string _headerText;
        private LabourProcessDTO _selectedLabourProcess;
        private ICommand _saveLabourProcessViewCommand;
        #endregion

        #region Constructor
        public LabourProcessViewModel()
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

        #region Properties
   
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

                    SelectedLabourProcess = Employee.LabourProcess ?? new LabourProcessDTO
                    {
                        AgencyId = Singleton.Agency.Id,
                        SubmitDate = DateTime.Now,
                        ContratBeginDate = DateTime.Now,
                        ContratEndDate = DateTime.Now.AddYears(2).AddDays(45),
                        //Status = ProcessStatusTypes.OnProcess
                    };
                    
                    MaxDate = Singleton.ProductActivation.ExpiryDate;

                }
            }
        }
        public LabourProcessDTO SelectedLabourProcess
        {
            get { return _selectedLabourProcess; }
            set
            {
                _selectedLabourProcess = value;
                RaisePropertyChanged<LabourProcessDTO>(() => SelectedLabourProcess);
                if (SelectedLabourProcess != null && SelectedLabourProcess.AgreementReturned)
                    ExpanderEnability = true;
                else
                    ExpanderEnability = false;
            }
        }
        public bool ExpanderEnability
        {
            get { return _expanderEnability; }
            set
            {
                _expanderEnability = value;
                RaisePropertyChanged<bool>(() => ExpanderEnability);
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

        public ICommand SaveLabourProcessViewCommand
        {
            get { return _saveLabourProcessViewCommand ?? (_saveLabourProcessViewCommand = new RelayCommand<Object>(ExecuteSaveLabourProcessViewCommand, CanSave)); }
        }
        private void ExecuteSaveLabourProcessViewCommand(object obj)
        {
            SaveLabourProcess();
            CloseWindow(obj);
        }
        private void SaveLabourProcess()
        {
            try
            {
                if (SelectedLabourProcess != null)
                {
                    if (Employee != null)
                    {   
                        //SelectedLabourProcess.AgreementFileName = PrintAgreementFull();

                        SelectedLabourProcess.DateLastModified = DateTime.Now;
                        Employee.LabourProcess = SelectedLabourProcess;
                        _employeeService.InsertOrUpdate(Employee);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Problem saving labour process data");
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
            var calConv = new Calendar(SelectedLabourProcess.SubmitDate);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    SelectedLabourProcess.SubmitDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        private ICommand _closeLabourProcessViewCommand;
        public ICommand CloseLabourProcessViewCommand
        {
            get
            {
                return _closeLabourProcessViewCommand ?? (_closeLabourProcessViewCommand = new RelayCommand<Object>(CloseWindow));
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
            if (Errors == 0)
                return true;
            return false;
        }

        #endregion

        #region Reports

        #region List
        //private ICommand _printListCommandView;
        //public ICommand PrintListCommandView
        //{
        //    get
        //    {
        //        return _printListCommandView ?? (_printListCommandView = new RelayCommand<Object>(PrintList));
        //    }
        //}

        //private void PrintList(object obj)
        //{
        //    SaveLabourProcess();
        //    GenerateReports.PrintLabourtList(Employee, obj != null);
        //}
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
            SaveLabourProcess();
            GenerateReports.PrintCoverLetter(Employee, obj != null);
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
            SaveLabourProcess();
            GenerateReports.PrintLabourApplication(Employee, obj != null);
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
            var employeeTestimony = new EmployeeTestimony(Employee);
            employeeTestimony.ShowDialog();
            //GenerateReports.PrintTestimonialLetter(Employee, obj != null);
        }

        #endregion
        
        #region VisaTranslation
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
        //    SaveLabourProcess();
        //    GenerateReports.PrintVisaTranslation2(Employee, obj != null);
        //}
        #endregion

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
        //    SaveLabourProcess();
        //    GenerateReports.PrintVisaTranslationEnglish2(Employee, obj != null);
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
        //    SaveLabourProcess();
        //    GenerateReports.PrintWekalaTranslation2(Employee, obj != null);
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
        //    SaveLabourProcess();
        //    GenerateReports.PrintNormalWekala(Employee.Visa, obj != null);
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
        //    SaveLabourProcess();
        //    GenerateReports.PrintConditionArabic(Employee.Visa, obj != null);
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
        //    SaveLabourProcess();
        //    GenerateReports.PrintConditionTranslation(Employee.Visa, obj != null);
        //}
        //#endregion

        //private string PrintAgreementFull()
        //{
        //    var fileName = "";

        //    try
        //    {
        //        ReportClass myReport = new AgreementFull();
        //        var myDataSetAgreementFront = GenerateReportDatasets.GetAgreementFrontDataSet(Employee);
        //        var myDataSetAgreementBack = GenerateReportDatasets.GetAgreementBackDataSet(Employee);
        //        myReport.Subreports["AgreementFront.rpt"].SetDataSource(myDataSetAgreementFront);
        //        myReport.Subreports["AgreementBack.rpt"].SetDataSource(myDataSetAgreementBack);

        //        var crDiskFileDestinationOptions = new DiskFileDestinationOptions();
        //        var crFormatTypeOptions = new PdfRtfWordFormatOptions();
        //        var path = PathUtil.GetAgreementPath();
        //        fileName = Guid.NewGuid() + ".pdf";
        //        var agreeFile = Path.Combine(path, fileName);

        //        crDiskFileDestinationOptions.DiskFileName = agreeFile;

        //        ExportOptions crExportOptions = myReport.ExportOptions;
        //        {
        //            crExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
        //            crExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
        //            crExportOptions.DestinationOptions = crDiskFileDestinationOptions;
        //            crExportOptions.FormatOptions = crFormatTypeOptions;
        //        }
        //        myReport.Export();

        //        return fileName;
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show("Can't Generate Agreement File " + Environment.NewLine + exception.Message);
        //        return fileName;
        //    }

            
        //}
       

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
        //    SaveLabourProcess();
        //    GenerateReports.PrintAgreementFront(Employee, obj != null);
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
        //    SaveLabourProcess();
        //    GenerateReports.PrintAgreementBack(Employee, obj != null);
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
            SaveLabourProcess();
            GenerateReports.PrintLabourAllInOne(Employee, obj != null);
        }
        #endregion

        #endregion

        
    }
}
