using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Migrations.Model;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class EnjazitBrowserViewModel : ViewModelBase
    {
        #region Fields

        private static IUnitOfWork _unitOfWork;
        private ChromiumWebBrowser _browser;
        private BrowserTarget _browserTarget;
        private EmployeeDTO _employee;
        private ObservableCollection<EmployeeDTO> _employeeList;
        private ObservableCollection<EmployeeDTO> _employees;
        private bool _fillFormDocumentsEnability;
        private EmployeeDTO _selectedEmployee;

        #endregion

        #region Constructor

        public EnjazitBrowserViewModel()
        {
            LoadEmployees();

            Messenger.Default.Register<BrowserTarget>(this, message => { BrowserTarget = message; });
            Messenger.Default.Register<EmployeeDTO>(this,
                message => { Employee = EmployeeList.FirstOrDefault(e => e.Id == message.Id); });

            //PopulateBrowser();
            //CleanUp();
        }

        public void LoadEmployees()
        {
            CleanUp();

            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            IQueryable<EmployeeDTO> empList = _unitOfWork.Repository<EmployeeDTO>()
                .Query()
                .Include(
                    e => e.Address, e => e.ContactPerson, e => e.ContactPerson.Address,
                    e => e.Experience, e => e.Education,
                    e => e.Visa, e => e.Visa.Sponsor, e => e.Visa.Sponsor.Address)
                .Get()
                .Where(e => e.Id != 0);

            EmployeeList = new ObservableCollection<EmployeeDTO>(empList);
            CleanUp();
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Public Properties

        public BrowserTarget BrowserTarget
        {
            get { return _browserTarget; }
            set
            {
                _browserTarget = value;
                RaisePropertyChanged<BrowserTarget>(() => BrowserTarget);

                if (BrowserTarget == BrowserTarget.Enjazit)
                {
                }
                //PopulateBrowser();
            }
        }

        public ChromiumWebBrowser Browser
        {
            get { return _browser; }
            set
            {
                _browser = value;
                RaisePropertyChanged<ChromiumWebBrowser>(() => Browser);
            }
        }

        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                RaisePropertyChanged<string>(() => Url);
            }
        }

        public EmployeeDTO Employee
        {
            get { return _employee; }
            set
            {
                _employee = value;
                RaisePropertyChanged<EmployeeDTO>(() => Employee);
                if (Employee == null)
                {
                    FillFormDocumentsEnability = false;
                }
                else
                {
                    Employees = new ObservableCollection<EmployeeDTO> {Employee};
                    FillFormDocumentsEnability = true;
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
                if (SelectedEmployee == null || SelectedEmployee.Id == 0 ||
                    SelectedEmployee.EmployeeDetail == string.Empty) return;
                
                Employee = SelectedEmployee;


                try
                {
                    var cri = new SearchCriteria<AttachmentDTO>();
                    cri.FiList.Add(a => a.Id == SelectedEmployee.PhotoId);
                    var attachment = new AttachmentService(true).GetAll(cri).FirstOrDefault();

                    if (attachment != null && attachment.AttachedFile != null)
                    {
                        var tempFolder = PathUtil.GetFolderPath() + "Temp\\";
                        if (!Directory.Exists(tempFolder))
                            Directory.CreateDirectory(tempFolder);
                        var fileName = Path.Combine(tempFolder, Guid.NewGuid() + ".jpg");

                        File.WriteAllBytes(fileName, attachment.AttachedFile);

                        var stat=ImageResizingUtil.ResizeEnjazImage(fileName, SelectedEmployee.FullName);
                        
                        if (stat) NotifyUtility.ShowCustomBalloon("Success", "Photo Resized Successfully!", 4000);
                        else NotifyUtility.ShowCustomBalloon("Error","Can't Resize Enjaz Photo",4000);
                    }
                    else
                    {
                        NotifyUtility.ShowCustomBalloon("Can't Get Photo", "First Insert Photo On Employee Entry", 4000); 
                    }

                }
                catch (Exception)
                {
                    //MessageBox.Show()
                    NotifyUtility.ShowCustomBalloon("Error","Can't Resize Enjaz Photo",4000);
                }

            }
        }

        public ObservableCollection<EmployeeDTO> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged<ObservableCollection<EmployeeDTO>>(() => Employees);
            }
        }

        public ObservableCollection<EmployeeDTO> EmployeeList
        {
            get { return _employeeList; }
            set
            {
                _employeeList = value;
                RaisePropertyChanged<ObservableCollection<EmployeeDTO>>(() => EmployeeList);
            }
        }

        public bool FillFormDocumentsEnability
        {
            get { return _fillFormDocumentsEnability; }
            set
            {
                _fillFormDocumentsEnability = value;
                RaisePropertyChanged<bool>(() => FillFormDocumentsEnability);
            }
        }

        #endregion

        #region Commands

        private ICommand _embassyProcessViewCommand;

        private ICommand _fillEnjazFormCommand;

        private ICommand _fillInsuranceFormCommand;

        private ICommand _fillMusanedFormCommand;
        private ICommand _loadEmployeesViewCommand;

        public ICommand LoadEmployeesViewCommand
        {
            get
            {
                return _loadEmployeesViewCommand ??
                       (_loadEmployeesViewCommand = new RelayCommand(LoadEmployees));
            }
        }

        public ICommand EmbassyProcessViewCommand
        {
            get
            {
                return _embassyProcessViewCommand ??
                       (_embassyProcessViewCommand = new RelayCommand(ExcuteEmbassyProcessViewCommand));
            }
        }

        public ICommand FillEnjazFormCommand
        {
            get
            {
                return _fillEnjazFormCommand ??
                       (_fillEnjazFormCommand = new RelayCommand<Object>(ExcuteFillEnjazFormCommand));
            }
        }

        public ICommand FillMusanedFormCommand
        {
            get
            {
                return _fillMusanedFormCommand ??
                       (_fillMusanedFormCommand = new RelayCommand<Object>(ExcuteFillMusanedFormCommand));
            }
        }

        public ICommand FillInsuranceFormCommand
        {
            get
            {
                return _fillInsuranceFormCommand ??
                       (_fillInsuranceFormCommand = new RelayCommand<Object>(ExcuteFillInsuranceFormCommand));
            }
        }

        private void ExcuteEmbassyProcessViewCommand()
        {
            var embassyProcess = new EmbassyProcess(Employee);
            embassyProcess.ShowDialog();
        }

        private void ExcuteFillEnjazFormCommand(object obj)
        {
            BrowserTarget = BrowserTarget.Enjazit;
            ExcuteFillFormCommand(obj);
        }

        private void ExcuteFillMusanedFormCommand(object obj)
        {
            BrowserTarget = BrowserTarget.Musaned;
            ExcuteFillFormCommand(obj);
        }

        private void ExcuteFillInsuranceFormCommand(object obj)
        {
            BrowserTarget = BrowserTarget.UnitedInsurance;
            ExcuteFillFormCommand(obj);
        }

        private void ExcuteFillFormCommand(object obj)
        {
            Browser = obj as ChromiumWebBrowser;
            EmployeeDTO employee = Employee;

            string religionText = EnumUtil.GetEnumDesc(employee.Religion);
            string sexText = employee.Sex.ToString();
            string maritalText = employee.MaritalStatus.ToString();

            int religionValue = Convert.ToInt32(employee.Religion);
            int sexValue = Convert.ToInt32(employee.Sex);
            int maritalValue = Convert.ToInt32(employee.MaritalStatus);

            if (Browser != null)
            {
                switch (BrowserTarget)
                {
                    case BrowserTarget.Enjazit:
                        EnjazitFill(religionText, religionValue, sexText, sexValue, maritalText, maritalValue);
                        break;
                    case BrowserTarget.Musaned:
                        MusanedFill(religionValue, sexValue, maritalValue);
                        break;
                    case BrowserTarget.UnitedInsurance:
                        InsuranceFill(sexValue);
                        break;
                }
            }
        }

        #endregion

        #region Enjaz

        private ICommand _goToMainEnjazViewCommand;

        public ICommand GoToMainEnjazViewCommand
        {
            get
            {
                return _goToMainEnjazViewCommand ??
                       (_goToMainEnjazViewCommand = new RelayCommand(ExcuteGoToMainEnjazViewCommand));
            }
        }

        private void EnjazitFill(string religionText, int religionValue, string sexText, int sexValue,
            string maritalText, int maritalValue)
        {
            try
            {
                EmployeeDTO employee = Employee;
                if (employee.Visa == null)
                {
                    MessageBox.Show(
                        "" + employee.FullName + " doesn't have Visa Information, You have to import visa data" +
                        " from Musaned->Employment Requests->Contract or " +
                        "Assign the Visa Manually", "Empty Visa");
                    return;
                }
                if (religionText.Contains("Non"))
                    religionValue = 6;
                else religionText = "Islam";
                var passIssue = string.IsNullOrEmpty(employee.PlaceOfIssue)
                    ? EnumUtil.GetEnumDesc(CityList.AddisAbeba)
                    : employee.PlaceOfIssue;

                //Split FirstName because it is Given Name
                string fname = employee.FirstName.Split(' ')[0];
                string mname = employee.FirstName.Split(' ')[1];

                Browser.ExecuteScriptAsync("$('#SPONSER_NAME').val('" + employee.Visa.Sponsor.FullName + "')");
                Browser.ExecuteScriptAsync("$('#SPONSER_NUMBER').val('" + employee.Visa.Sponsor.PassportNumber + "')");
                Browser.ExecuteScriptAsync("$('#SPONSER_ADDRESS').val('" + employee.Visa.Sponsor.Address.City + "')");
                Browser.ExecuteScriptAsync("$('#SPONSER_PHONE').val('" +
                                           employee.Visa.Sponsor.Address.MobileWithCountryCode + "')");

                Browser.ExecuteScriptAsync("$('#EFIRSTNAME').val('" + fname + "')");
                Browser.ExecuteScriptAsync("$('#EFATHER').val('" + mname + "')");
                Browser.ExecuteScriptAsync("$('#EGRAND').val(' ')");
                Browser.ExecuteScriptAsync("$('#EFAMILY').val('" + employee.LastName + "')");

                //Browser.ExecuteScriptAsync("$('#AFIRSTNAME').val('" + fname + "')");
                //Browser.ExecuteScriptAsync("$('#AFATHER').val('" + mname + "')");
                //Browser.ExecuteScriptAsync("$('#AGRAND').val(' ')");
                //Browser.ExecuteScriptAsync("$('#AFAMILY').val('" + employee.LastName + "')");

                Browser.ExecuteScriptAsync("$('#PASSPORTnumber').val('" + employee.PassportNumber + "')");
                Browser.ExecuteScriptAsync("$('#PASSPORT_ISSUE_PLACE').val('" + passIssue + "')");
                Browser.ExecuteScriptAsync("$('#PersonId').val(' ')");
                Browser.ExecuteScriptAsync("$('#BIRTH_PLACE').val('" + employee.PlaceOfBirth + "')");

                Browser.ExecuteScriptAsync("$('#JOB_OR_RELATION').val('" +
                                           EnumUtil.GetEnumDesc(employee.AppliedProfession) + "')");
                Browser.ExecuteScriptAsync("$('#DEGREE').val('PRIMARY')");
                Browser.ExecuteScriptAsync("$('#DEGREE_SOURCE').val('SCHOOL')");
                Browser.ExecuteScriptAsync("$('#ADDRESS_HOME').val('" + employee.Address.City + "')");
                Browser.ExecuteScriptAsync("$('#BIRTH_DATE').val('" + employee.DateOfBirth.ToString("yyyy/MM/dd") + "')");
                Browser.ExecuteScriptAsync("$('#PASSPORT_ISSUE_DATE').val('" +
                                           employee.PassportIssueDate.ToString("yyyy/MM/dd") + "')");
                Browser.ExecuteScriptAsync("$('#PASSPORT_EXPIRY_DATe').val('" +
                                           employee.PassportExpiryDate.ToString("yyyy/MM/dd") + "')");
                Browser.ExecuteScriptAsync("$('#porpose').val('For Work')");

                //Select Tag
                /*
                   Or to find by index:
                   var index = 0
                   $('.second').children().eq(index);*/
                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-1').textContent = 'Normal'");
                Browser.ExecuteScriptAsync("$('#PASSPORType').val('1')");
                Browser.ExecuteScriptAsync("$('#s2id_PASSPORType').children().first().removeClass('select2-default')");

                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-2').textContent = 'Ethiopia'");
                Browser.ExecuteScriptAsync("$('#NATIONALITY').val('ETH')");
                Browser.ExecuteScriptAsync("$('#s2id_NATIONALITY').children().first().removeClass('select2-default')");

                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-3').textContent = 'Ethiopia'");
                Browser.ExecuteScriptAsync("$('#NATIONALITY_FIRST').val('ETH')");
                Browser.ExecuteScriptAsync(
                    "$('#s2id_NATIONALITY_FIRST').children().first().removeClass('select2-default')");

                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-4').textContent = '" + religionText +
                                           "'");
                Browser.ExecuteScriptAsync("$('#RELIGION').val('" + religionValue + "')");
                Browser.ExecuteScriptAsync("$('#s2id_RELIGION').children().first().removeClass('select2-default')");

                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-5').textContent = '" + maritalText +
                                           "'");
                Browser.ExecuteScriptAsync("$('#SOCIAL_STATUS').val('" + maritalValue + "')");
                Browser.ExecuteScriptAsync("$('#s2id_SOCIAL_STATUS').children().first().removeClass('select2-default')");

                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-6').textContent = '" + sexText + "'");
                Browser.ExecuteScriptAsync("$('#Sex').val('" + sexValue + "')");
                Browser.ExecuteScriptAsync("$('#s2id_Sex').children().first().removeClass('select2-default')");

                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-7').textContent = 'Work'");
                Browser.ExecuteScriptAsync("$('#VisaKind').val('1')");
                Browser.ExecuteScriptAsync("$('#s2id_VisaKind').children().first().removeClass('select2-default')");
                Browser.ExecuteScriptAsync("$('.tr_DocumentNumber').hide()");

                Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-8').textContent = 'Addis Ababa'");
                Browser.ExecuteScriptAsync("$('#EmbassyCode').val('301')");
                Browser.ExecuteScriptAsync("$('#s2id_EmbassyCode').children().first().removeClass('select2-default')");

                #region EntryPoint

                string cityText, cityValue;
                string city = Employee.Visa.Sponsor.Address.City.ToLower();

                if (city.Contains("jed"))
                {
                    cityText = "Jeddah";
                    cityValue = "2";
                }
                else if (city.Contains("dhah"))
                {
                    cityText = "Dhahran";
                    cityValue = "3";
                }
                else if (city.Contains("mad") || city.Contains("med"))
                {
                    cityText = "Al-Madinah";
                    cityValue = "4";
                }
                else if (city.Contains("dam"))
                {
                    cityText = "Dammam";
                    cityValue = "5";
                }
                else
                {
                    cityText = "Riyadh";
                    cityValue = "1";
                }
                //Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-12').textContent = '" + cityText +
                //                           "'");
                Browser.ExecuteScriptAsync(
                    "$('#s2id_ENTRY_POINT').children().first().children().first().text('" + cityText + "')");
                Browser.ExecuteScriptAsync("$('#ENTRY_POINT').val('" + cityValue + "')");
                Browser.ExecuteScriptAsync("$('#s2id_ENTRY_POINT').children().first().removeClass('select2-default')");

                #endregion

                Browser.ExecuteScriptAsync("UpdateEntryTypes();");
                var worker = new BackgroundWorker();
                worker.DoWork += DoWork1;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted1;
                worker.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.InnerException,
                    "Error Filling Enjaz");
            }
        }

        private void DoWork1(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void Worker_RunWorkerCompleted1(object sender, RunWorkerCompletedEventArgs e)
        {
            //Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-41').textContent = 'Single'");
            Browser.ExecuteScriptAsync(
                "$('#s2id_NUMBER_OF_ENTRIES').children().first().children().first().text('Single')");
            Browser.ExecuteScriptAsync("$('#NUMBER_OF_ENTRIES').val('0')");
            Browser.ExecuteScriptAsync(
                "$('#s2id_NUMBER_OF_ENTRIES').children().first().removeClass('select2-default')");

            //Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-51').textContent = 'By Air'");
            Browser.ExecuteScriptAsync(
                "$('#s2id_COMING_THROUGH').children().first().children().first().text('By Air')");
            Browser.ExecuteScriptAsync("$('#COMING_THROUGH').val('2')");
            Browser.ExecuteScriptAsync("$('#s2id_COMING_THROUGH').children().first().removeClass('select2-default')");


            Browser.ExecuteScriptAsync("UpdateEntryDurations();");
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }


        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-50').textContent = '90 Days'");
            Browser.ExecuteScriptAsync(
                "$('#s2id_Number_Entry_Day').children().first().children().first().text('90 Days')");
            Browser.ExecuteScriptAsync("document.getElementById('Number_Entry_Day').value = '90'");
            Browser.ExecuteScriptAsync("$('#s2id_Number_Entry_Day').children().first().removeClass('select2-default')");


            Browser.ExecuteScriptAsync("UpdateResidenceDurations();");
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork2;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted2;
            worker.RunWorkerAsync();
        }

        private void DoWork2(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void Worker_RunWorkerCompleted2(object sender, RunWorkerCompletedEventArgs e)
        {
            Browser.ExecuteScriptAsync("document.getElementById('RESIDENCY_IN_KSA').value = '90'");
            Browser.ExecuteScriptAsync("$('#s2id_RESIDENCY_IN_KSA').children().first().removeClass('select2-default')");
            //Browser.ExecuteScriptAsync("document.getElementById('select2-chosen-58').textContent = '90'");
            Browser.ExecuteScriptAsync(
                "$('#s2id_RESIDENCY_IN_KSA').children().first().children().first().text('90')");

            //Browser.ExecuteScriptAsync("alert($('#VisaKind').val());");
            //Browser.ExecuteScriptAsync("alert($('#NUMBER_OF_ENTRIES').val());");
            //Browser.ExecuteScriptAsync("alert($('#COMING_THROUGH').val());");
            //Browser.ExecuteScriptAsync("alert($('#Number_Entry_Day').val());");
            //Browser.ExecuteScriptAsync("alert($('#RESIDENCY_IN_KSA').val());");
        }


        private void ExcuteGoToMainEnjazViewCommand()
        {
            BrowserTarget = BrowserTarget.Enjazit;
            //PopulateBrowser();
        }

        #endregion

        #region Musaned

        private ICommand _goToMainMusanedViewCommand;
        private ICommand _importVisaViewCommand;

        public ICommand GoToMainMusanedViewCommand
        {
            get
            {
                return _goToMainMusanedViewCommand ??
                       (_goToMainMusanedViewCommand = new RelayCommand(ExcuteGoToMainMusanedViewCommand));
            }
        }

        public ICommand ImportVisaViewCommand
        {
            get
            {
                return _importVisaViewCommand ??
                       (_importVisaViewCommand = new RelayCommand<Object>(ExcuteImportVisaViewCommand));
            }
        }

        private void MusanedFill(int religionValue, int sexValue, int maritalValue)
        {
            try
            {
                EmployeeDTO employee = Employee;
                maritalValue = maritalValue - 1;
                var hij = new Dates();
                var passIssue = string.IsNullOrEmpty(employee.PlaceOfIssue)
                    ? EnumUtil.GetEnumDesc(CityList.AddisAbeba)
                    : employee.PlaceOfIssue;

                #region Experience Years

                string exphome = "0", expabroad = "0";
                try
                {
                    if (employee.Experience.HaveWorkExperienceInCountry)
                        exphome = ((int) employee.Experience.ExperiencePeriodInCountry).ToString();
                    if (employee.Experience.HaveWorkExperience)
                        expabroad = ((int) employee.Experience.ExperiencePeriod).ToString();
                }
                catch
                {
                }

                #endregion

                #region Skills

                string[] skills =
                {
                    "Private Car Driver", "Hard Worker", "Baby Sitting", "Nanny",
                    "Washing and Ironing Clothes",
                    "Dusting and Cleaning", "Washing Kitchen Dishes", "Cooking",
                    "Speaks Arabic", "Speaks English", "Worked in Saudi Arabia Before"
                };
                IList<int> valueList = new List<int>();
                EmployeeExperienceDTO exp = employee.Experience;

                if (exp.Driving) valueList.Add(0);
                if (exp.HardWorker) valueList.Add(1);
                if (exp.BabySitting) valueList.Add(2);
                if (exp.Nanny) valueList.Add(3);
                if (exp.Washing) valueList.Add(4);

                if (exp.Cleaning) valueList.Add(5);
                if (exp.WashingDishes) valueList.Add(6);
                if (exp.Cooking) valueList.Add(7);

                if (employee.Education.ArabicLanguage != LanguageExperience.Poor) valueList.Add(8);
                if (employee.Education.EnglishLanguage != LanguageExperience.Poor) valueList.Add(9);
                if (exp.HaveWorkExperience && exp.ExperienceCountry == CountryList.SaudiArabia) valueList.Add(10);

                string values = "[", texts = "";
                bool isFirst = true;
                foreach (int i in valueList)
                {
                    if (isFirst)
                    {
                        int j = i + 1;
                        values = values + "'" + j + "'";
                        texts = skills[i];
                    }
                    else
                    {
                        int j = i + 1;
                        values = values + ",'" + j + "'";
                        texts = texts + "," + skills[i];
                    }

                    isFirst = false;
                }
                values = values + "]";

                string script = @" var element=document.getElementsByName('skills[]')[0];" +
                                @" var values=" + values + "; " +
                                @" for(var i=0;i<element.options.length;i++){ " +
                                @" element.options[i].selected = values.indexOf(element.options[i].value) >= 0;}";
                Browser.ExecuteScriptAsync(script);

                string script6 =
                    "var ul = document.getElementsByClassName('chosen-choices')[3]; " +
                    "ul.innerHTML='<li>" + texts + "</li>'; ";
                Browser.ExecuteScriptAsync(script6);

                #endregion

                #region Qualifications

                string qualVal = (((int) employee.Education.QualificationType)).ToString();
                string qualScript = @" var element=document.getElementsByName('qualifications[]')[0];" +
                                    @" element.options[" + qualVal + "].selected = true;";
                Browser.ExecuteScriptAsync(qualScript);


                string qualification = EnumUtil.GetEnumDesc(employee.Education.QualificationType);
                string script7 =
                    "var ul = document.getElementsByClassName('chosen-choices')[2]; " +
                    "ul.innerHTML='<li>" + qualification + "</li>'; ";
                Browser.ExecuteScriptAsync(script7);

                #endregion

                #region More Data

                Browser.ExecuteScriptAsync("document.getElementsByName('passport_number')[0].value = '" +
                                           employee.PassportNumber + "'");
                Browser.ExecuteScriptAsync("document.getElementsByName('religion_id')[1].value = '" + religionValue +
                                           "'");
                Browser.ExecuteScriptAsync("document.getElementsByName('nationality_id')[0].value = '8'");
                //Browser.ExecuteScriptAsync("$('select[name='religion_id']').eq(1).val('" +religionValue.ToString() + "')");


                Browser.ExecuteScriptAsync("$('#nid').val('" + employee.PassportNumber + "')");
                Browser.ExecuteScriptAsync("$('#first_name').val('" + employee.FirstName + " " + employee.MiddleName +
                                           "')");
                Browser.ExecuteScriptAsync("$('#family_name').val('" + employee.LastName + "')");
                Browser.ExecuteScriptAsync("$('#passport_issue_place').val('" + passIssue + "')");

                Browser.ExecuteScriptAsync("$('#birthdate').val('" + employee.DateOfBirth.ToString("yyyy-MM-dd") + "')");
                Browser.ExecuteScriptAsync("$('#birthdate_hijri').val('" +
                                           hij.GregToHijri(employee.DateOfBirth.ToString("yyyy-MM-dd"), "yyyy-MM-dd") +
                                           "')");
                Browser.ExecuteScriptAsync("$('#passport_issue_date').val('" +
                                           employee.PassportIssueDate.ToString("yyyy-MM-dd") + "')");
                Browser.ExecuteScriptAsync("$('#passport_issue_date_hijri').val('" +
                                           hij.GregToHijri(employee.PassportIssueDate.ToString("yyyy-MM-dd"),
                                               "yyyy-MM-dd") + "')");
                Browser.ExecuteScriptAsync("$('#passport_expire_date').val('" +
                                           employee.PassportExpiryDate.ToString("yyyy-MM-dd") + "')");
                Browser.ExecuteScriptAsync("$('#passport_expire_date_hijri').val('" +
                                           hij.GregToHijri(employee.PassportExpiryDate.ToString("yyyy-MM-dd"),
                                               "yyyy-MM-dd") + "')");

                Browser.ExecuteScriptAsync("$('#address').val('" + employee.Address.City + "')");
                Browser.ExecuteScriptAsync("$('#mobile').val('" + employee.Address.MobileWithCountryCode + "')");
                Browser.ExecuteScriptAsync("$('#relative_name').val('" + employee.ContactPerson.FullName + "')");
                Browser.ExecuteScriptAsync("$('#relative_kinship').val('" + employee.ContactPerson.Kinship + "')");
                Browser.ExecuteScriptAsync("$('#relative_phone').val('" +
                                           employee.ContactPerson.Address.MobileWithCountryCode + "')");
                Browser.ExecuteScriptAsync("$('#relative_address').val('" + employee.ContactPerson.Address.City + "')");
                Browser.ExecuteScriptAsync("$('#experience_years_home').val('" + exphome + "')");
                Browser.ExecuteScriptAsync("$('#experience_years_abroad').val('" + expabroad + "')");

                if (sexValue == 1)
                    Browser.ExecuteScriptAsync("$('#male').prop('checked',true)");
                else if (sexValue == 2)
                    Browser.ExecuteScriptAsync("$('#female').prop('checked',true)");

                Browser.ExecuteScriptAsync("$('#marital_status').val('" + maritalValue + "')");
                //Browser.ExecuteScriptAsync("$('#religion_id').val('" + religionValue.ToString() + "')");

                Browser.ExecuteScriptAsync("$('#job_id').val('3')");
                Browser.ExecuteScriptAsync("$('#country_id').val('8')");
                //Browser.ExecuteScriptAsync("$('#nationality_id').val('8')"); 

                #endregion

                Browser.ExecuteScriptAsync("get_cities();");
                var worker = new BackgroundWorker();
                worker.DoWork += DoWork3;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted3;
                worker.RunWorkerAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.InnerException,
                    "Error Filling Musaned");
            }
        }

        private void Worker_RunWorkerCompleted3(object sender, RunWorkerCompletedEventArgs e)
        {
            Browser.ExecuteScriptAsync("$('#city_id').val('5020')");
            //Browser.ExecuteScriptAsync("alert($('#city_id').val());");
        }

        private void DoWork3(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
        }

        private void ExcuteGoToMainMusanedViewCommand()
        {
            BrowserTarget = BrowserTarget.Musaned;
        }

        private void ExcuteImportEmployeeOnly()
        {
            try
            {
                var empService = new EmployeeService();

                var passportNumber = GetElementValueById(Browser, "nid").ToUpper().Trim();

                if (string.IsNullOrEmpty(passportNumber))
                {
                    MessageBox.Show("Can't Get Employee Data, Close and Re-Open this window...", "No Data Found");
                    return;
                }

                bool exists = true;
                EmployeeDTO emp = empService.GetAll()
                    .FirstOrDefault(e => e.PassportNumber == passportNumber);

                if (emp == null)
                {
                    exists = false;
                    emp = CommonUtility.GetNewEmployeeDTO();
                }

                #region Employee Data

                emp.PassportNumber = passportNumber;
                //emp.FirstName = GetElementValueById(Browser, "first_name").ToUpper().Trim();
                var givenNames = GetElementValueById(Browser, "first_name").ToUpper().Trim();
                var gnames = givenNames.Split(' ').ToList();
                var names = gnames.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                emp.FirstName = names[0] + " " + names[1];

                emp.LastName = GetElementValueById(Browser, "family_name").ToUpper().Trim();
                emp.PlaceOfBirth = GetElementValueById(Browser, "address");
                // GetElementValueById(Browser, "passport_issue_place");
                emp.PlaceOfIssue = GetElementValueById(Browser, "passport_issue_place");

                emp.DateOfBirth = Convert.ToDateTime(GetElementValueById(Browser, "birthdate").Trim());
                var passIssueDtae = GetElementValueById(Browser, "passport_issue_date").Trim();
                emp.PassportIssueDate = Convert.ToDateTime(passIssueDtae);
                emp.PassportExpiryDate =
                    Convert.ToDateTime(GetElementValueById(Browser, "passport_expire_date").Trim());

                var maritalValue = Convert.ToInt32(GetElementValueById(Browser, "marital_status")) + 1;
                emp.MaritalStatus = (MaritalStatusTypes) maritalValue;
                
                const string religionText =
                    "(function() {{return document.getElementsByName('religion_id')[1].value;}})();";
                var relRes = GetEvaluateScriptAsync(Browser, religionText);//Browser.EvaluateScriptAsync(religionText).Result.Result.ToString();
                if (!string.IsNullOrEmpty(relRes))
                {
                    var religionValue = Convert.ToInt32(relRes);
                    emp.Religion = (ReligionTypes) religionValue;
                }
                else emp.Religion = ReligionTypes.Muslim;

                const string genderText =
                    "(function() {{ if(document.getElementsByName('gender')[3].checked){ return 'male' ;} else{ return 'female'; }}})();";
                var jsRes = GetEvaluateScriptAsync(Browser, genderText);// Browser.EvaluateScriptAsync(genderText).Result.Result.ToString();
                if (!string.IsNullOrEmpty(jsRes))
                    emp.Sex = jsRes.Contains("fe") ? Sex.Female : Sex.Male;
                else emp.Sex = Sex.Female;

                emp.Address.City = GetElementValueById(Browser, "address");
                var mob = GetElementValueById(Browser, "mobile");
                emp.Address.Mobile = mob.Replace("+", "");

                #endregion

                #region Contact Data

                var conName = GetElementValueById(Browser, "relative_name");
                emp.ContactPerson.FullName = string.IsNullOrEmpty(conName) ? "-" : conName.ToUpper();
                emp.ContactPerson.Kinship = GetElementValueById(Browser, "relative_kinship");
                var contMob = GetElementValueById(Browser, "relative_phone");
                emp.ContactPerson.Address.Mobile = contMob.Replace("+", "");
                emp.ContactPerson.Address.City = GetElementValueById(Browser, "relative_address");

                #endregion

                #region Work Experience

                try
                {
                    var exphome = Convert.ToInt32(GetElementValueById(Browser, "experience_years_home"));
                    var expabroad = Convert.ToInt32(GetElementValueById(Browser, "experience_years_abroad"));

                    if (exphome > 0)
                    {
                        emp.Experience.HaveWorkExperienceInCountry = true;
                        emp.Experience.ExperiencePeriodInCountry = (ContratPeriods) exphome;
                    }
                    if (expabroad > 0)
                    {
                        emp.Experience.HaveWorkExperience = true;
                        emp.Experience.ExperiencePeriodInCountry = (ContratPeriods) expabroad;
                    }
                }
                catch
                {
                }

                #endregion

                #region Skills

                string script = @" (function() {{ var element=document.getElementsByName('skills[]')[0];" +
                                @" var values=''; " +
                                @" for(var i=0;i<element.options.length;i++){ " +
                                @" if(element.options[i].selected) {values = values +'_'+ element.options[i].value;} } return values; }})();";
                var skillsRes = GetEvaluateScriptAsync(Browser, script);
                    // Browser.EvaluateScriptAsync(script).Result.Result.ToString();
                if (!string.IsNullOrEmpty(skillsRes))
                {
                    var res = skillsRes.Split('_');

                    emp.Experience.Driving = false;
                    emp.Experience.HardWorker = false;
                    emp.Experience.BabySitting = false;
                    emp.Experience.Nanny = false;
                    emp.Experience.Washing = false;
                    emp.Experience.WashingDishes = false;
                    emp.Experience.Cleaning = false;
                    emp.Experience.Cooking = false;

                    #region Skill

                    foreach (var re in res)
                    {
                        if (!string.IsNullOrEmpty(re))
                        {
                            switch (re)
                            {
                                case "2":
                                    emp.Experience.HardWorker = true;
                                    break;
                                case "3":
                                    emp.Experience.BabySitting = true;
                                    break;
                                case "4":
                                    emp.Experience.Nanny = true;
                                    break;
                                case "5":
                                    emp.Experience.Washing = true;
                                    break;
                                case "6":
                                    emp.Experience.WashingDishes = true;
                                    break;
                                case "7":
                                    emp.Experience.Cleaning = true;
                                    break;
                                case "8":
                                    emp.Experience.Cooking = true;
                                    break;
                                case "9":
                                    emp.Education.ArabicLanguage = LanguageExperience.Fluent;
                                    break;
                                case "10":
                                    emp.Education.EnglishLanguage = LanguageExperience.Fluent;
                                    break;
                                case "11":
                                    emp.Experience.ExperienceCountry = CountryList.SaudiArabia;
                                    break;
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    emp.Experience.HardWorker = true;
                    emp.Experience.Washing = true;
                    emp.Experience.WashingDishes = true;
                    emp.Experience.Cleaning = true;
                }

            #endregion

                #region Qualifications

                //values +'_'+
                string script2 = @" (function() {{ var element=document.getElementsByName('qualifications[]')[0];" +
                                 @" for(var i=0;i<element.options.length;i++){ " +
                                 @" if(element.options[i].selected) { return element.options[i].value;} } }})();";
                var qualifications = GetEvaluateScriptAsync(Browser, script2);// Browser.EvaluateScriptAsync(script2).Result.Result.ToString();
                if (!string.IsNullOrEmpty(qualifications))
                {
                    var qual = Convert.ToInt32(qualifications) - 1;
                    emp.Education.QualificationType = (QualificationTypes) qual;
                }
                else
                    emp.Education.QualificationType=QualificationTypes.Primary;
                

                #endregion

                string stat = empService.InsertOrUpdate(emp);
                if (string.IsNullOrEmpty(stat))
                {
                    if (exists)
                        NotifyUtility.ShowCustomBalloon("Successful Updated",
                            "'" + emp.FirstName + "' is successfully Updated!",
                            4000);
                    else
                        NotifyUtility.ShowCustomBalloon("Successful Added",
                            "'" + emp.FirstName + "' is successfully Added!",
                            4000);
                }
                else
                {
                    NotifyUtility.ShowCustomBalloon("Can't Import",
                        "Can't Import '" + emp.FirstName + "' data...",
                        4000);
                }
            }
            catch
            {
                MessageBox.Show("Problem while importing Employee data...", "Importing Problem");
            }
        }

        private void ExcuteImportVisaViewCommand(object obj)
        {
            Browser = obj as ChromiumWebBrowser;
            var visaNum = GetElementValueById(Browser, "visa_number");


            if (string.IsNullOrEmpty(visaNum))
            {
                ExcuteImportEmployeeOnly();
            }
            else
            {
                try
                {
                    var tempEmployee = new EmployeeDTO();
                    //Employee Name
                    bool isCompanyVisa = string.IsNullOrEmpty(GetElementValueById(Browser, "nid"));
                    var script = string.Format("document.getElementsByName('name')[2].value;");
                    if (isCompanyVisa)
                        script = string.Format("document.getElementsByName('name')[1].value;");

                    var fullName = GetEvaluateScriptAsync(Browser, script);
                        // javascriptResponse.Result.ToString().Trim(); //.ToUpper()
                    if (string.IsNullOrEmpty(fullName))
                    {
                        MessageBox.Show("Incorrect Employee Name");
                        return;
                    }
                    var fnames = fullName.Split(' ').ToList();
                    var names = fnames.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

                    var scr2 =
                        " (function() { var MyRows = $('table').find('tbody').find('tr');" +
                        " for (var i = 0; i < MyRows.length; i++) {" +
                        " var firstName = $(MyRows[i]).find('td:eq(9)').text().trim(); " +
                        " if(firstName.indexOf('" + names[0] + "') != -1 && firstName.indexOf('" + names[1] +
                        "') != -1){ " +
                        " var contractNumber = $(MyRows[i]).find('td:eq(1)').text(); " +
                        "  return contractNumber.trim()+firstName; }" +
                        " }})();";

                    var jsR2 = GetEvaluateScriptAsync(Browser, scr2); // response.Result.ToString();

                    if (!string.IsNullOrEmpty(jsR2))
                    {
                        tempEmployee.ContractNumber = jsR2.Substring(0, 10);
                    }
                    else
                    {
                        MessageBox.Show("Incorrect Contract Number");
                        return;
                    }


                    var cri = new SearchCriteria<VisaDTO>();
                    cri.FiList.Add(v => v.VisaNumber == visaNum);
                    var vis = new VisaService(true, false).GetAll(cri).FirstOrDefault();
                    if (vis != null)
                    {
                        NotifyUtility.ShowCustomBalloon("Exists", "Visa Already Exists", 4000);
                    }
                    else
                    {
                        VisaDTO visa = GetNewVisa(Browser);

                        script = string.Format("document.getElementsByName('name')[0].value;");
                        var res = GetEvaluateScriptAsync(Browser, script);// Browser.EvaluateScriptAsync(script).Result;
                        if (!string.IsNullOrEmpty(res))
                        {
                            visa.Sponsor.FullName = res.ToUpper();
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Sponsor Name");
                            return; 
                        }
                

                        script = string.Format("document.getElementsByName('name')[4].value;");
                        if (isCompanyVisa)
                        {
                            script = string.Format("document.getElementsByName('name')[3].value;");
                            visa.VisaQuantity = 200;
                        }

                        var resan = GetEvaluateScriptAsync(Browser, script);// Browser.EvaluateScriptAsync(script).Result;
                        if (!string.IsNullOrEmpty(resan))
                        {
                            visa.Sponsor.FullNameArabic = resan;
                        }
                        //else MessageBox.Show("Incorrect Arabic Sponsor Name "); 
                        
                        script = string.Format("document.getElementsByName('epro_title_ar')[0].value;");
                        var mob = GetEvaluateScriptAsync(Browser, script);
                            // Browser.EvaluateScriptAsync(script).Result.Result.ToString();
                        visa.Sponsor.Address.Mobile = mob.Replace("+", "");

                        script = string.Format("document.getElementsByName('email')[0].value;");
                        visa.Sponsor.Address.PrimaryEmail = GetEvaluateScriptAsync(Browser, script);
                            // Browser.EvaluateScriptAsync(script).Result.Result.ToString();

                        script = string.Format("document.getElementsByName('city')[0].value;");
                        visa.Sponsor.Address.City = GetEvaluateScriptAsync(Browser, script);
                            // Browser.EvaluateScriptAsync(script).Result.Result.ToString();

                        visa.VisaNumber = visaNum;

                        var scr =
                            " (function() { var MyRows = $('table').find('tbody').find('tr');" +
                            " for (var i = 0; i < MyRows.length; i++) {" +
                            " var visaNumber = $(MyRows[i]).find('td:eq(0)').text(); " +
                            " if(visaNumber==" + visa.VisaNumber + "){ " +
                            " var contractNumber = $(MyRows[i]).find('td:eq(1)').text(); var sponsorID = $(MyRows[i]).find('td:eq(2)').text(); " +
                            "  return contractNumber.trim() + sponsorID.trim(); }" +
                            " }})();";

                        var jsR = GetEvaluateScriptAsync(Browser, scr);// Browser.EvaluateScriptAsync(scr).Result.Result.ToString();
                        if (!string.IsNullOrEmpty(jsR) && jsR.Length>=20)
                        {
                            visa.ContratNumber = jsR.Substring(0, 10);
                            visa.Sponsor.PassportNumber = jsR.Substring(10, 10); // GetElementValueById(Browser, "nid");
                        }
                        else
                        {
                            MessageBox.Show("Incorrect ContractNumber/SponsorId Number");
                            return; 
                        }
                        SaveVisa(visa);
                        vis = visa;
                    }

                    tempEmployee.PassportNumber = GetElementValueById(Browser, "passport_number");

                    AttachVisa(vis, tempEmployee);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem while importing Visa Data..." + ex.Message, "Importing Problem");
                }
                //Add Visa
            }
        }

        public string GetEvaluateScriptAsync(ChromiumWebBrowser myCwb, string script)
        {
            var result = "";

            try
            {
                var res = Browser.EvaluateScriptAsync(script).Result;
                if (res != null)
                {
                    var res2 = res.Result;
                    if (res2 != null)
                        result = res2.ToString().Trim();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message , "Problem while excuting Scripts");
            }

            return result;
        }

        private VisaDTO GetNewVisa(ChromiumWebBrowser myCwb)
        {
            try
            {
                string script = string.Format("document.getElementsByName('name')[1].value;");
                string agName = myCwb.EvaluateScriptAsync(script).Result.ToString();

                script = string.Format("document.getElementsByName('license_no')[0].value;");
                string licenseNo = Browser.EvaluateScriptAsync(script).Result.Result.ToString();

                List<AgentDTO> agentsList = new ForeignAgentService(true, false).GetAll().ToList();
                AgentDTO selectedAgent =
                    agentsList.FirstOrDefault(ag => ag.AgentName.Equals(agName) || ag.LicenseNumber.Equals(licenseNo));

                if (selectedAgent == null)
                    selectedAgent = agentsList.FirstOrDefault();

                if (selectedAgent != null)
                {
                    return CommonUtility.GetNewVisaDTO(selectedAgent.Id);
                }
            }
            catch
            {
                NotifyUtility.ShowCustomBalloon("Can't Add Visa", "Problem while Getting New Visa", 4000);
            }

            return null;
        }

        private void SaveVisa(VisaDTO importedVisa)
        {
            try
            {
                var visaService = new VisaService(false, true);
                //Check For Visa Existance
                var cri = new SearchCriteria<VisaDTO>();
                cri.FiList.Add(v => v.VisaNumber == importedVisa.VisaNumber);
                var vis = visaService.GetAll(cri).FirstOrDefault();
                if (vis != null)
                {
                    MessageBox.Show("Visa Already Exists");
                }
                else
                {
                    importedVisa.DateLastModified = DateTime.Now;
                    importedVisa.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                    importedVisa.Sponsor.DateLastModified = DateTime.Now;
                    importedVisa.Sponsor.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                    importedVisa.Sponsor.Address.DateLastModified = DateTime.Now;
                    importedVisa.Sponsor.Address.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                    importedVisa.Condition.DateLastModified = DateTime.Now;
                    importedVisa.Condition.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;

                    visaService.InsertOrUpdate(importedVisa);
                }


                visaService.Dispose();

                NotifyUtility.ShowCustomBalloon("Successed",
                    " visa is successfully imported!",
                    4000);
                //return true;
            }
            catch
            {
                MessageBox.Show("Can't Save Visa");
                //return false;
            }
        }

        public void AttachVisa(VisaDTO selectedVisa, EmployeeDTO tempEmployee)
        {
            try
            {
                var empService = new EmployeeService();

                EmployeeDTO emp = empService.GetAll()
                    .FirstOrDefault(e => e.PassportNumber == tempEmployee.PassportNumber);

                if (emp != null)
                {
                    emp.ContractNumber = tempEmployee.ContractNumber;
                    emp.VisaId = selectedVisa.Id;
                    emp.AgentId = selectedVisa.ForeignAgentId;
                    var stat = empService.InsertOrUpdate(emp);
                    if (string.IsNullOrEmpty(stat))
                        NotifyUtility.ShowCustomBalloon("Successed", "Visa is successfully Attached!", 4000);
                }
                else
                {
                    MessageBox.Show(
                        "Employee doesn't exist in the system, You have to first import Employee Data from 'HR Pool and import Contract from Employment Contracts ",
                        "Import Employee From HR Pool");
                    ////if (MessageBox.Show(
                    ////    "Employee doesn't exist in the database!,Do you want to create the employee", "Create Employee",
                    ////    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
                    ////{
                    //    emp = CommonUtility.GetNewEmployeeDTO();

                    //    emp.ContractNumber = tempEmployee.ContractNumber;
                    //    emp.PassportNumber = tempEmployee.PassportNumber;

                    //    bool isCompanyVisa = string.IsNullOrEmpty(GetElementValueById(Browser, "nid"));
                    //    var script = string.Format("document.getElementsByName('name')[2].value;");
                    //    if(isCompanyVisa)
                    //        script = string.Format("document.getElementsByName('name')[1].value;");

                    //    var fullName=Browser.EvaluateScriptAsync(script).Result.Result.ToString().ToUpper();
                    //    var fnames = fullName.Split(' ').ToList();
                    //    var names = fnames.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
                    //    emp.FirstName = names[0] + " " + names[1];
                    //    emp.LastName = names[2];

                    //    emp.PlaceOfBirth = GetElementValueById(Browser, "passport_issue_place");
                    //    emp.PassportIssueDate = Convert.ToDateTime(GetElementValueById(Browser, "passport-issue-date").Trim());

                    //    script = string.Format("document.getElementsByName('address')[0].value;");
                    //    emp.Address.City = Browser.EvaluateScriptAsync(script).Result.Result.ToString().ToUpper();

                    //    script = string.Format("document.getElementsByName('mobile_no')[0].value;");
                    //    var mob = Browser.EvaluateScriptAsync(script).Result.Result.ToString();
                    //    emp.Address.Mobile = mob.Replace("+", "");

                    //    var conName = GetElementValueById(Browser, "relative_name");
                    //    emp.ContactPerson.FullName = string.IsNullOrEmpty(conName)?"-":conName.ToUpper();
                    //    emp.ContactPerson.Kinship = GetElementValueById(Browser, "relative_kinship");
                    //    var contactMob=GetElementValueById(Browser, "relative_phone");
                    //    emp.ContactPerson.Address.Mobile = contactMob.Replace("+", "" );

                    //    emp.ContactPerson.Address.City = GetElementValueById(Browser, "relative_address");

                    //    emp.VisaId = selectedVisa.Id;
                    //    emp.AgentId = selectedVisa.ForeignAgentId;
                    //    var stat=empService.InsertOrUpdate(emp);
                    //    if(string.IsNullOrEmpty(stat))
                    //        NotifyUtility.ShowCustomBalloon("Successed", "Data is imported and attached!",4000);
                    ////}
                }

                empService.Dispose();
            }
            catch
            {
                MessageBox.Show("Can't Attach Visa, Do it Manually");
            }
        }

        public void SetElementValueById(ChromiumWebBrowser myCwb, string eltId, string setValue)
        {
            string script = string.Format("(function() {{document.getElementById('{0}').value='{1}';}})()", eltId,
                setValue);
            myCwb.ExecuteScriptAsync(script);
        }

        public string GetElementValueById(ChromiumWebBrowser myCwb, string eltId)
        {
            try
            {
                string script = string.Format("(function() {{return document.getElementById('{0}').value;}})();",
                    eltId);
                JavascriptResponse jr = myCwb.EvaluateScriptAsync(script).Result;
                return jr.Result.ToString();
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region Insurance

        private ICommand _goToMainInsuranceViewCommand;
        private string _url;

        public ICommand GoToMainInsuranceViewCommand
        {
            get
            {
                return _goToMainInsuranceViewCommand ??
                       (_goToMainInsuranceViewCommand = new RelayCommand(ExcuteGoToMainInsuranceViewCommand));
            }
        }

        private void InsuranceFill(int sexValue)
        {
            try
            {
                EmployeeDTO employee = Employee;

                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtInsrName').val('" + employee.FirstName + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtInsrSurName').val('" + employee.LastName + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtPassportNo').val('" + employee.PassportNumber + "')");

                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtMobile').val('" + employee.Address.MobileWithCountryCode ??
                                           "" + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtHouseNo').val('" + employee.Address.HouseNumber ??
                                           "" + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtPolCity').val('" + employee.Address.City ?? "" + "')");

                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtSubcity').val('" + employee.Address.SubCity ?? "" + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtKebele').val('" + employee.Address.Woreda ?? "" + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtEmail').val('" + employee.Address.PrimaryEmail ??
                                           "" + "')");

                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_ftxtPolRemarks').val('" + employee.MoreNotes ?? "" + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_fdtPolDOB_fdtPolDOB_rtxtDate').val('" +
                                           employee.DateOfBirth.ToString("dd/MM/yyyy") + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_fdtPassportExpDt_fdtPassportExpDt_rtxtDate').val('" +
                                           employee.PassportExpiryDate.ToString("dd/MM/yyyy") + "')");

                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_fddlGender').val('" + sexValue + "')");
                Browser.ExecuteScriptAsync("$('#ctl00_cphGI_fddlCountry').val('196')");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + Environment.NewLine + exception.InnerException,
                    "Error Filling Insurance");
            }
        }

        private void ExcuteGoToMainInsuranceViewCommand()
        {
            BrowserTarget = BrowserTarget.UnitedInsurance;
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


    //var script2 =
    //" var element=document.getElementsByName('skills[]')[0]; element.removeAttribute('style');";// +
    ////" for(var i=0;i<element.options.length;i++){" +
    //// " if(element.options[i].selected){" +
    ////" }}";//alert(element.options[i].text);
    //Browser.ExecuteScriptAsync(script2);

    //" var li = document.createElement('li');" +
    //" li.appendChild(document.createTextNode('Hard Worker, Babby Sitting, Nanny'));" +
    //" ul.appendChild(li);";
    // //Browser.ExecuteScriptAsync("alert('<span>Hard Worker</span><a class='search-choice-close' data-option-array-index='1'></a>');");


    //var script3 =
    //    "$('.chosen-choices').eq(3).append('<li class='search-choice'><span>Hard Worker</span><a class='search-choice-close' data-option-array-index='1'></a></li>')";
    //Browser.ExecuteScriptAsync(script3);

    //var script5 =
    //@" var element=document.getElementsByName('skills[]')[0].nextElementSibling.children().first();";
    //" li.appendChild(document.createTextNode('<span>Hard Worker</span>'));" +       //getElementsByClassName('.chosen-choices')[] 
    //<a class='search-choice-close' data-option-array-index='1'></a><a class='search-choice-close'></a>
    //Browser.ExecuteScriptAsync("alert('$('[name=skills[]]').eq(0).length');"); //" li.innerHTML = 'Hard Worker'; " +
    //var script6 =
    //    "var ul = document.getElementsByClassName('chosen-choices')[3]; " +
    //" var li = document.createElement('li'); li.setAttribute('class', 'search-choice search-choice-close');" +
    //" li.appendChild(document.createTextNode('Hard Worker'));" +
    //" ul.appendChild(li);";
    ////Browser.ExecuteScriptAsync("alert('<span>Hard Worker</span><a class='search-choice-close' data-option-array-index='1'></a>');");
    //Browser.ExecuteScriptAsync(script6);

    //var script4 =
    //       "$('ul.chosen-choices').append('<li class='search-choice'><span>Hard Worker</span><a class='search-choice-close' data-option-array-index='1'></a></li>')";
    //Browser.ExecuteScriptAsync(script4);
    //for (int i = 0; i < 4; i++)
    //{
    //var sccr = "$('select[name='skills[]']').select2('val', ['2', '3', '4']).trigger('change');";
    //Browser.ExecuteScriptAsync(sccr);

    //var sccr2 = "$('select[name='skills[]']').eq(0).select2('val', ['2', '3', '4']).trigger('change');";
    //Browser.ExecuteScriptAsync("alert(1)");

    //var scrp = "var aa=$('select[name='skills[]']').children('option').length; alert(aa);";//$('select[name='skills[]']').eq(0)
    //Browser.ExecuteScriptAsync(scrp);

    //Browser.ExecuteScriptAsync("alert($('select[name='skills[]']').eq(0).children('option').length);");
    //"]);";//".val(2);$('[name='skills[]']').eq(0).select2().trigger('change')";//".trigger('change');$('[name=skills[]]').eq(0).val(2).trigger('change');";

    //var sccr1 = "$('[name='skills[]']').eq(1).val(2);$('[name='skills[]']').eq(1).select2().trigger('change')";//".trigger('change');$('[name=skills[]]').eq(0).val(2).trigger('change');";
    //Browser.ExecuteScriptAsync(sccr1);
    // var scr = "$('[name='skills[]']').eq(0).select2('data', {id: '2', text: 'Hard worker'}).trigger('change');$('[name='skills[]']').eq(1).select2('data', {id: '2', text: 'Hard worker'}).trigger('change');";
    //Browser.ExecuteScriptAsync(scr);
    //var script4 =
    //"$('[class='chosen-choices']').eq(3).append('<li class='search-choice'><span>Diploma</span><a class='search-choice-close' data-option-array-index='1'></a></li>')";
    //Browser.ExecuteScriptAsync(script4);
    //}

    /*
    //var script = " var els=document.getElementsByName('passport_number');" +
    //             " for(var i=0;i<els.length;i++){" +
    //             " els[i].value='" + employee.PassportNumber + "';}";
    //Browser.ExecuteScriptAsync(script); alert(values.length); alert(element.options[i].value);              
               

    //Browser.ExecuteScriptAsync("alert(document.getElementsByName('skills[]')[0].options.length);");
    //Browser.ExecuteScriptAsync("alert(document.getElementsByName('qualification[]')[0].options.length);");
    //getElementsByName using jquery
    */

    //private void EnjazitFill(IHTMLDocument2 doc, string religionText, int religionValue, string sexText,
    //    int sexValue, string maritalText, int maritalValue)
    //{
    //    try
    //    {
    //        EmployeeDTO employee = Employee;
    //        if (religionText.Contains("Non"))
    //            religionValue = 6;
    //        else religionText = "Islam";

    //        //Split FirstName because it is Given Name
    //        string fname = employee.FirstName.Split(' ')[0];
    //        string mname = employee.FirstName.Split(' ')[1];

    //        #region Input Tag

    //        var theElementCollection = (IHTMLElementCollection) doc.all.tags("input");
    //        foreach (IHTMLElement el in theElementCollection)
    //        {
    //            try
    //            {
    //                string controlName = el.getAttribute("id").ToString();

    //                switch (controlName)
    //                {
    //                        #region Enjazit

    //                    case "SPONSER_NAME": // "26":
    //                        el.setAttribute("value", employee.Visa.Sponsor.FullName);
    //                        break;
    //                    case "SPONSER_NUMBER":
    //                        el.setAttribute("value", employee.Visa.Sponsor.PassportNumber);
    //                        break;
    //                    case "SPONSER_ADDRESS":
    //                        el.setAttribute("value", employee.Visa.Sponsor.Address.City);
    //                        break;
    //                    case "SPONSER_PHONE":
    //                        el.setAttribute("value", employee.Visa.Sponsor.Address.Telephone);
    //                        break;
    //                    case "EFIRSTNAME":
    //                        el.setAttribute("value", fname); //employee.FirstName
    //                        break;
    //                    case "EFATHER":
    //                        el.setAttribute("value", mname); //employee.MiddleName
    //                        break;
    //                    case "EGRAND":
    //                        el.setAttribute("value", "");
    //                        break;
    //                    case "EFAMILY":
    //                        el.setAttribute("value", employee.LastName);
    //                        break;
    //                    case "AFIRSTNAME":
    //                        el.setAttribute("value", fname); //employee.FirstName
    //                        break;
    //                    case "AFATHER":
    //                        el.setAttribute("value", mname); //employee.MiddleName
    //                        break;
    //                    case "AGRAND":
    //                        el.setAttribute("value", "");
    //                        break;
    //                    case "AFAMILY":
    //                        el.setAttribute("value", employee.LastName);
    //                        break;
    //                    case "16":
    //                        el.setAttribute("value", "");
    //                        break;
    //                    case "PASSPORTnumber":
    //                        el.setAttribute("value", employee.PassportNumber);
    //                        break;
    //                    case "PASSPORT_ISSUE_PLACE": //"11":
    //                        el.setAttribute("value", "Addis Ababa");
    //                        break;
    //                    case "BIRTH_PLACE": //"14":
    //                        el.setAttribute("value", employee.PlaceOfBirth);
    //                        break;
    //                    case "JOB_OR_RELATION":
    //                        el.setAttribute("value", employee.AppliedProfession);
    //                        break;
    //                    case "DEGREE":
    //                        el.setAttribute("value", "ELEMENTARY");
    //                        break;
    //                    case "DEGREE_SOURCE":
    //                        el.setAttribute("value", "ELEMENTARY");
    //                        break;
    //                    case "ADDRESS_HOME":
    //                        el.setAttribute("value", employee.PlaceOfBirth);
    //                        break;
    //                    case "BIRTH_DATE":
    //                        el.setAttribute("value", employee.DateOfBirth.ToString("yyyy/MM/dd"));
    //                        break;
    //                    case "PASSPORT_ISSUE_DATE":
    //                        el.setAttribute("value", employee.PassportIssueDate.ToString("yyyy/MM/dd"));
    //                        break;
    //                    case "PASSPORT_EXPIRY_DATe":
    //                        el.setAttribute("value", employee.PassportExpiryDate.ToString("yyyy/MM/dd"));
    //                        break;
    //                    case "porpose":
    //                        el.setAttribute("value", "For Work");
    //                        break;

    //                        #endregion
    //                }
    //            }
    //            catch
    //            {
    //            }
    //        }

    //        #endregion

    //        #region Select Tags

    //        try
    //        {


    //            //var theElementCollectionSelect = (IHTMLElementCollection)doc.all.tags("select");

    //            //foreach (IHTMLElement el in theElementCollectionSelect)
    //            //{
    //            //    try
    //            //    {//el
    //            //        string controlName = el.getAttribute("name").ToString();

    //            //        switch (controlName)
    //            //        {
    //            //            case "PASSPORType":
    //            //                var dropdownItems = (IHTMLElementCollection)el.children;

    //            //                foreach (IHTMLElement option in dropdownItems)
    //            //                {
    //            //                    dynamic value = option.getAttribute("value").ToString();
    //            //                    if (value.Equals("1"))
    //            //                    {
    //            //                        option.setAttribute("selected", "selected");
    //            //                        break;
    //            //                    }
    //            //                }
    //            //                break;
    //            //        }
    //            //    }
    //            //    catch
    //            //    {
    //            //    }
    //            //}

    //            #region PassportType

    //            var spanItems = ((IHTMLElement)doc.all.item("select2-chosen-1"));
    //            spanItems.innerText = "Normal";

    //            //var spanItems = ((IHTMLElementCollection)doc.all.item("select2-chosen-1"));
    //            //foreach (IHTMLElement spanItem in spanItems)
    //            //{
    //            //    spanItem.innerText = "Normal";
    //            //}

    //            var passportType = ((IHTMLElement)doc.all.item("PASSPORType"));
    //            var itemspassportType = (IHTMLElementCollection)passportType.children;
    //            foreach (IHTMLElement option in itemspassportType)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("1"))
    //                {
    //                    option.setAttribute("selected", "selected");
    //                    //SendKeys.SendWait("{TAB}");
    //                    break;
    //                }

    //            }

    //            #endregion

    //            #region Nationality

    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-2"));
    //            spanItems.innerText = "Ethiopia";
    //            //spanItems = ((IHTMLElementCollection)doc.all.item("select2-chosen-2"));
    //            //foreach (IHTMLElement spanItem in spanItems)
    //            //{
    //            //    spanItem.innerText = "Ethiopia";
    //            //}

    //            var nationality = ((IHTMLElement)doc.all.item("nationality"));
    //            var itemsnationality = (IHTMLElementCollection)nationality.children;


    //            foreach (IHTMLElement option in itemsnationality)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("ETH")) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Past Nationality

    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-3"));
    //            spanItems.innerText = "Ethiopia";

    //            //spanItems = ((IHTMLElementCollection)doc.all.item("select2-chosen-3"));
    //            //foreach (IHTMLElement spanItem in spanItems)
    //            //{
    //            //    spanItem.innerText = "Ethiopia";
    //            //}

    //            var nationalityFirst = ((IHTMLElement)doc.all.item("nationality_first"));
    //            var itemsnationalityFirst = (IHTMLElementCollection)nationalityFirst.children;
    //            foreach (IHTMLElement option in itemsnationalityFirst)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("ETH")) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Religion

    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-4"));
    //            spanItems.innerText = religionText;

    //            var religion = ((IHTMLElement)doc.all.item("religion"));
    //            var itemsreligion = (IHTMLElementCollection)religion.children;
    //            foreach (IHTMLElement option in itemsreligion)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals(religionValue.ToString())) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Marital Status
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-5"));
    //            spanItems.innerText = maritalText;

    //            var marital = ((IHTMLElement)doc.all.item("social_status"));
    //            var itemsmarital = (IHTMLElementCollection)marital.children;
    //            foreach (IHTMLElement option in itemsmarital)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals(maritalValue.ToString())) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Sex
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-6"));
    //            spanItems.innerText = sexText;

    //            var sex = ((IHTMLElement)doc.all.item("sex"));
    //            var itemssex = (IHTMLElementCollection)sex.children;
    //            foreach (IHTMLElement option in itemssex)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals(sexValue.ToString())) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Visa Type
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-7"));
    //            spanItems.innerText = "Work";

    //            var visaType = ((IHTMLElement)doc.all.item("VisaKind"));
    //            var itemsvisaType = (IHTMLElementCollection)visaType.children;
    //            foreach (IHTMLElement option in itemsvisaType)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("1"))
    //                {
    //                    option.setAttribute("selected", "true");
    //                    //SendKeys.Send("{TAB}");
    //                    break;
    //                }

    //            }

    //            //spanItems = ((IHTMLElement)doc.all.item("select2-chosen-7"));
    //            //spanItems.innerText = "Resident";

    //            //var visaType2 = ((IHTMLElement)doc.all.item("VisaKind"));
    //            //var itemsvisaType2 = (IHTMLElementCollection)visaType2.children;
    //            //foreach (IHTMLElement option in itemsvisaType2)
    //            //{
    //            //    dynamic value = option.getAttribute("value").ToString();
    //            //    if (value.Equals("3"))
    //            //    {
    //            //        option.setAttribute("selected", "true");
    //            //        //SendKeys.Send("{TAB}");
    //            //        break;
    //            //    }

    //            //}
    //            #endregion

    //            #region EmbassyCode
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-8"));
    //            spanItems.innerText = "Addis Ababa";

    //            var embassyCode = ((IHTMLElement)doc.all.item("EmbassyCode"));
    //            var itemsEmbassyCode = (IHTMLElementCollection)embassyCode.children;
    //            foreach (IHTMLElement option in itemsEmbassyCode)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("301")) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Entry Point

    //            string cityText, cityValue;
    //            string city = employee.Visa.Sponsor.Address.City.ToLower();

    //            if (city.Contains("jed"))
    //            {
    //                cityText = "Jeddah";
    //                cityValue = "2";
    //            }
    //            else if (city.Contains("dhah"))
    //            {
    //                cityText = "Dhahran";
    //                cityValue = "3";
    //            }
    //            else if (city.Contains("mad") || city.Contains("med"))
    //            {
    //                cityText = "Al-Madinah";
    //                cityValue = "4";
    //            }
    //            else if (city.Contains("dam"))
    //            {
    //                cityText = "Dammam";
    //                cityValue = "5";
    //            }
    //            else
    //            {
    //                cityText = "Riyadh";
    //                cityValue = "1";
    //            }

    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-12"));
    //            spanItems.innerText = cityText;

    //            var entryPoint = ((IHTMLElement)doc.all.item("ENTRY_POINT"));
    //            var itemsentryPoint = (IHTMLElementCollection)entryPoint.children;

    //            foreach (IHTMLElement option in itemsentryPoint)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals(cityValue)) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Number of Entries
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-13"));
    //            spanItems.innerText = "Single";

    //            var numberOfEntries = ((IHTMLElement)doc.all.item("NUMBER_OF_ENTRIES"));
    //            var itemsnumberOfEntries = (IHTMLElementCollection)numberOfEntries.children;
    //            foreach (IHTMLElement option in itemsnumberOfEntries)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("0")) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Transport Mode
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-14"));
    //            spanItems.innerText = "By Air";

    //            var transportMode = ((IHTMLElement)doc.all.item("COMING_THROUGH"));
    //            var itemstransportMode = (IHTMLElementCollection)transportMode.children;
    //            foreach (IHTMLElement option in itemstransportMode)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("2")) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Visa Validity
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-15"));
    //            spanItems.innerText = "90 days";

    //            var visaValidity = ((IHTMLElement)doc.all.item("Number_Entry_Day"));
    //            var itemsvisaValidity = (IHTMLElementCollection)visaValidity.children;
    //            foreach (IHTMLElement option in itemsvisaValidity)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("90")) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            #region Duration of Stay
    //            spanItems = ((IHTMLElement)doc.all.item("select2-chosen-16"));
    //            spanItems.innerText = "90 days";

    //            var durationOfStay = ((IHTMLElement)doc.all.item("RESIDENCY_IN_KSA"));
    //            var itemsdurationOfStay = (IHTMLElementCollection)durationOfStay.children;
    //            foreach (IHTMLElement option in itemsdurationOfStay)
    //            {
    //                dynamic value = option.getAttribute("value").ToString();
    //                if (value.Equals("1")) option.setAttribute("selected", "selected");
    //            }

    //            #endregion

    //            //var theElementCollectionSelect = (IHTMLElementCollection)doc.all.tags("div");

    //            //foreach (IHTMLElement el in theElementCollectionSelect)
    //            //{
    //            //    try
    //            //    {//el
    //            //        string controlName = el.getAttribute("id").ToString();

    //            //        switch (controlName)
    //            //        {
    //            //            case "s2id_NATIONALITY":
    //            //                el.setAttribute("class", "select2-container required select2-allowclear ");//select2-container-active
    //            //                break;
    //            //        }
    //            //    }
    //            //    catch
    //            //    {
    //            //    }
    //            //}
    //        }
    //        catch
    //        {
    //            //continue;
    //        }

    //        #endregion
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException, "Error Populating Form");
    //    }
    //}

    /*private void MusanedFill(IHTMLDocument2 doc, int religionValue, int sexValue, int maritalValue)
        {
            try
            {
                EmployeeDTO employee = Employee;
                maritalValue = maritalValue - 1;

                #region Experience Years

                string exphome = "", expabroad = "";
                try
                {
                    if (employee.Experience.HaveWorkExperienceInCountry)
                        exphome = ((int) employee.Experience.ExperiencePeriodInCountry).ToString();
                    if (employee.Experience.HaveWorkExperience)
                        expabroad = ((int) employee.Experience.ExperiencePeriod).ToString();
                }
                catch
                {
                }

                #endregion

                #region Input Tag

                var theElementCollection = (IHTMLElementCollection) doc.all.tags("input");
                foreach (IHTMLElement el in theElementCollection)
                {
                    try
                    {
                        string controlName = el.getAttribute("name").ToString();
                        if (controlName.Equals("passport_number"))
                            el.setAttribute("value", employee.PassportNumber);
                        var hij = new Dates();


                        string controlId = el.getAttribute("id").ToString();

                        switch (controlId)
                        {
                                #region Musaned

                            case "nid":
                                el.setAttribute("value", employee.PassportNumber);
                                break;
                            case "first_name":
                                el.setAttribute("value", employee.FirstName + " " + employee.MiddleName);
                                break;
                            case "family_name":
                                el.setAttribute("value", employee.LastName);
                                break;

                            case "passport_issue_place":
                                el.setAttribute("value", "Addis Ababa");
                                break;
                            case "birthdate":
                                el.setAttribute("value", employee.DateOfBirth.ToString("yyyy-MM-dd"));
                                break;
                            case "birthdate_hijri":
                                el.setAttribute("value",
                                    hij.GregToHijri(employee.DateOfBirth.ToString("yyyy-MM-dd"), "yyyy-MM-dd"));
                                break;
                            case "passport_issue_date":
                                el.setAttribute("value", employee.PassportIssueDate.ToString("yyyy-MM-dd"));
                                break;
                            case "passport_issue_date_hijri":
                                el.setAttribute("value",
                                    hij.GregToHijri(employee.PassportIssueDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd"));
                                break;
                            case "passport_expire_date":
                                el.setAttribute("value", employee.PassportExpiryDate.ToString("yyyy-MM-dd"));
                                break;
                            case "passport_expire_date_hijri":
                                el.setAttribute("value",
                                    hij.GregToHijri(employee.PassportExpiryDate.ToString("yyyy-MM-dd"), "yyyy-MM-dd"));
                                break;
                            case "address":
                                el.setAttribute("value", employee.Address.City);
                                break;
                            case "mobile":
                                el.setAttribute("value", employee.Address.MobileWithCountryCode);
                                break;
                            case "relative_name":
                                el.setAttribute("value", employee.ContactPerson.FullName);
                                break;
                            case "relative_kinship":
                                el.setAttribute("value", employee.ContactPerson.Kinship);
                                break;
                            case "relative_phone":
                                el.setAttribute("value", employee.ContactPerson.Address.MobileWithCountryCode);
                                break;
                            case "relative_address":
                                el.setAttribute("value", employee.ContactPerson.Address.City);
                                break;

                            case "experience_years_home":
                                el.setAttribute("value", exphome);
                                break;
                            case "experience_years_abroad":
                                el.setAttribute("value", expabroad);
                                break;
                            case "male":
                                if (sexValue == 1) el.setAttribute("checked", "checked");
                                break;
                            case "female":
                                if (sexValue == 2) el.setAttribute("checked", "checked");
                                break;

                                #endregion
                        }
                    }
                    catch
                    {
                    }
                }

                #endregion

                //var dropdown = ((IHTMLElement)doc.all.item("gender"));
                //var genderItems = (IHTMLElementCollection)dropdown.children;
                //foreach (IHTMLElement option in genderItems)
                //{
                //    var value = option.getAttribute("value").ToString();
                //    if (value.Equals(sexValue)) option.setAttribute("checked", "checked");
                //}

                #region Select Tags

                var theElementCollectionSelect = (IHTMLElementCollection) doc.all.tags("select");

                foreach (IHTMLElement el in theElementCollectionSelect)
                {
                    try
                    {
                        string controlName = el.getAttribute("name").ToString();

                        switch (controlName)
                        {
                                #region Musaned Entry

                            case "marital_status":
                                var dropdownItems = (IHTMLElementCollection) el.children;
                                //var opt =
                                //    dropdownItems.AsQueryable()
                                //        .Cast<IHTMLElement>().ToList()
                                //        .FirstOrDefault(e => e.getAttribute("value",0)
                                //            .Equals(maritalValue.ToString()));
                                //opt.setAttribute("selected", "selected");
                                foreach (IHTMLElement option in dropdownItems)
                                {
                                    dynamic value = option.getAttribute("value").ToString();
                                    if (value.Equals(maritalValue.ToString()))
                                    {
                                        option.setAttribute("selected", "selected");
                                        break;
                                    }
                                }
                                break;
                            case "religion_id":
                                var dropdownItemsrel = (IHTMLElementCollection) el.children;
                                foreach (IHTMLElement option in dropdownItemsrel)
                                {
                                    dynamic value = option.getAttribute("value").ToString();
                                    if (value.Equals(religionValue.ToString()))
                                        option.setAttribute("selected", "selected");
                                }
                                break;
                            case "job_id":
                                var dropdownItemsjob = (IHTMLElementCollection) el.children;
                                foreach (IHTMLElement option in dropdownItemsjob)
                                {
                                    dynamic value = option.getAttribute("value").ToString();
                                    if (value.Equals("3")) option.setAttribute("selected", "selected");
                                }
                                break;
                            case "nationality_id":
                                var dropdownItemsnation = (IHTMLElementCollection) el.children;
                                foreach (IHTMLElement option in dropdownItemsnation)
                                {
                                    dynamic value = option.getAttribute("value").ToString();
                                    if (value.Equals("8")) option.setAttribute("selected", "selected");
                                }
                                break;
                            case "country_id":
                                var dropdownItemscountry = (IHTMLElementCollection) el.children;
                                foreach (IHTMLElement option in dropdownItemscountry)
                                {
                                    dynamic value = option.getAttribute("value").ToString();
                                    if (value.Equals("8")) option.setAttribute("selected", "selected");
                                }
                                break;
                            case "city_id":
                                var dropdownItemscity = (IHTMLElementCollection) el.children;
                                foreach (IHTMLElement option in dropdownItemscity)
                                {
                                    dynamic value = option.getAttribute("value").ToString();
                                    if (value.Equals("8")) option.setAttribute("selected", "selected");
                                }
                                break;

                                #endregion
                        }
                    }
                    catch
                    {
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException, "Error Populating Form");
            }
        }

     */

    /*
     private void InsuranceFill(IHTMLDocument2 doc, int sexValue)
        {
            try
            {
                EmployeeDTO employee = Employee;

                #region Input Tag

                var theElementCollection = (IHTMLElementCollection) doc.all.tags("input");
                foreach (IHTMLElement el in theElementCollection)
                {
                    try
                    {
                        string controlName = el.getAttribute("id").ToString();

                        switch (controlName)
                        {
                                #region United Insurance Portal

                            case "ctl00_cphGI_ftxtInsrName":
                                el.setAttribute("value", employee.FirstName); //FirstName=GivenName
                                break;
                            case "ctl00_cphGI_ftxtInsrSurName":
                                el.setAttribute("value", employee.LastName);
                                break;
                            case "ctl00_cphGI_ftxtPassportNo":
                                el.setAttribute("value", employee.PassportNumber);
                                break;
                            case "ctl00_cphGI_ftxtMobile":
                                el.setAttribute("value", employee.Address.Mobile ?? "");
                                break;

                            case "ctl00_cphGI_ftxtHouseNo":
                                el.setAttribute("value", employee.Address.HouseNumber ?? "");
                                break;
                            case "ctl00_cphGI_ftxtPolCity":
                                el.setAttribute("value", employee.Address.City ?? "");
                                break;
                            case "ctl00_cphGI_ftxtSubcity":
                                el.setAttribute("value", employee.Address.SubCity ?? "");
                                break;
                            case "ctl00_cphGI_ftxtKebele":
                                el.setAttribute("value", employee.Address.Woreda ?? "");
                                break;
                            case "ctl00_cphGI_ftxtEmail":
                                el.setAttribute("value", employee.Address.PrimaryEmail ?? "");
                                break;
                            case "ctl00_cphGI_ftxtPolRemarks":
                                el.setAttribute("value", employee.MoreNotes ?? "");
                                break;

                            case "ctl00_cphGI_fdtPolDOB_fdtPolDOB_rtxtDate":
                                el.setAttribute("value", employee.DateOfBirth.ToString("dd/MM/yyyy"));
                                break;
                                //case "ctl00_cphGI_ftxtAge":
                                //    el.setAttribute("value", employee.AgentName);
                                //    break;
                            case "ctl00_cphGI_fdtPassportExpDt_fdtPassportExpDt_rtxtDate":
                                el.setAttribute("value", employee.PassportExpiryDate.ToString("dd/MM/yyyy"));
                                break;

                                #endregion
                        }
                    }
                    catch
                    {
                    }
                }

                #endregion

                #region Select Tags

                var theElementCollectionSelect = (IHTMLElementCollection) doc.all.tags("select");

                foreach (IHTMLElement el in theElementCollectionSelect)
                {
                    try
                    {
                        string controlName = el.getAttribute("id").ToString();
                        //string countryinsurance = "196";
                        switch (controlName)
                        {
                            case "ctl00_cphGI_fddlGender":
                                el.setAttribute("value", sexValue.ToString());
                                break;
                            case "ctl00_cphGI_fddlCountry":
                                el.setAttribute("value", "196");
                                break;
                        }

                        //var dropdown = ((IHTMLElement)doc.all.item("ctl00_cphGI_fddlCountry"));
                        //var dropdownItems = (IHTMLElementCollection) dropdown.children;
                        //foreach (IHTMLElement option in dropdownItems)
                        //{
                        //    var value = option.getAttribute("value").ToString();
                        //    if(value.Equals("183")) option.setAttribute("selected","selected");
                        //}
                    }
                    catch
                    {
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException, "Error Populating Form");
            }
        }*/

    //private void PopulateBrowser()
    //{
    //    if(BrowserTarget==0)return;

    //    //Browser = new WebBrowser();
    //    if ((BrowserTarget)BrowserTarget == BrowserTarget.Enjazit)
    //    {
    //        Url= "file:///C:/Users/user/Desktop/Offline%20Pages/Visa%20Services%20Platform%20(Enjaz).html";
    //    }
    //    else if ((BrowserTarget)BrowserTarget == BrowserTarget.Musaned)
    //    {
    //        Url = "file:///C:/Users/user/Desktop/Offline%20Pages/MusanedEntry.html";
    //    }
    //    else if ((BrowserTarget)BrowserTarget == BrowserTarget.UnitedInsurance)
    //    {
    //        Url = "file:///C:/Users/user/Desktop/Offline%20Pages/United%20Insurance%20Company%20SC.html";
    //    }
    //    //Browser.Navigate(new Uri(Url));
    //}
}