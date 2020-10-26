using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using MessageBox = System.Windows.MessageBox;

namespace PinnaFace.WPF.ViewModel
{
    public class EmployeeExperienceViewModel : ViewModelBase
    {
        #region Fields
        private EmployeeDTO _selectedEmployee;
        private bool _haveWorkExperience, _haveWorkExperienceInCountry;
        private string _haveWorkExperienceVisibility, _haveWorkExperienceInCountryVisibility;
        #endregion

        #region Constructor
        public EmployeeExperienceViewModel()
        {
            CleanUp();
            
            Messenger.Default.Register<EmployeeDTO>(this, message =>
            {
                SelectedEmployee = message;
            });
        }
        public static void CleanUp()
        {
        }
        #endregion

        #region Properties

        public bool HaveWorkExperience
        {
            get { return _haveWorkExperience; }
            set
            {
                _haveWorkExperience = value;
                RaisePropertyChanged<bool>(() => HaveWorkExperience);
                HaveWorkExperienceVisibility = HaveWorkExperience ? "Visible" : "Collapsed";
            }
        }

        public string HaveWorkExperienceVisibility
        {
            get { return _haveWorkExperienceVisibility; }
            set
            {
                _haveWorkExperienceVisibility = value;
                RaisePropertyChanged<string>(() => HaveWorkExperienceVisibility);
            }
        }
        public bool HaveWorkExperienceInCountry
        {
            get { return _haveWorkExperienceInCountry; }
            set
            {
                _haveWorkExperienceInCountry = value;
                RaisePropertyChanged<bool>(() => HaveWorkExperienceInCountry);
                HaveWorkExperienceInCountryVisibility = HaveWorkExperienceInCountry ? "Visible" : "Collapsed";
            }
        }

        public string HaveWorkExperienceInCountryVisibility
        {
            get { return _haveWorkExperienceInCountryVisibility; }
            set
            {
                _haveWorkExperienceInCountryVisibility = value;
                RaisePropertyChanged<string>(() => HaveWorkExperienceInCountryVisibility);
            }
        }
        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);
                if (SelectedEmployee != null)
                {
                    if (SelectedEmployee.Experience == null)
                    {
                        SelectedEmployee.Experience = new EmployeeExperienceDTO
                        {
                            HaveWorkExperience = false,
                            HaveWorkExperienceInCountry = false
                        };
                        
                    }
                    HaveWorkExperience = SelectedEmployee.Experience.HaveWorkExperience;
                    HaveWorkExperienceInCountry = SelectedEmployee.Experience.HaveWorkExperienceInCountry;
                    
                }
            }
        }
        
        #endregion


        #region Commands

        private ICommand _saveApplicationCommand, _closeApplicationCommand;

        public ICommand SaveEmployeeExperienceCommand
        {
            get
            {
                return _saveApplicationCommand ?? (_saveApplicationCommand = new RelayCommand<Object>(ExcuteSaveEmployeeExperienceCommand,CanSave));
            }
        }
        private void ExcuteSaveEmployeeExperienceCommand(object obj)
        {
            try
            {
                SelectedEmployee.Experience.HaveWorkExperience = HaveWorkExperience;
                SelectedEmployee.Experience.HaveWorkExperienceInCountry = HaveWorkExperienceInCountry;
                SelectedEmployee.Experience.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.Experience.DateLastModified = DateTime.Now;
                CloseWindow(obj);
            }

            catch
            {
                MessageBox.Show("Can't Save Experience!");
            }
        }

        public ICommand CloseEmployeeApplicationCommand
        {
            get
            {
                return _closeApplicationCommand ?? (_closeApplicationCommand = new RelayCommand<Object>(CloseWindow));
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

        //public ICommand BioDataCommand
        //{
        //    get
        //    {
        //        return _bioDataCommand ?? (_bioDataCommand = new RelayCommand(ExcuteBioDataCommand));
        //    }
        //}
        //public void ExcuteBioDataCommand()
        //{
        //    var myDataSet = new ReportsDataSet2();
        //    var myReport = new BioData2();
        //    var Agency = new LocalAgencyService(true).GetLocalAgency();

        //    var employee = SelectedEmployee;
        //    if (Agency != null)
        //        myDataSet.LetterHeads.Rows.Add("1", Agency.Header.AttachedFile, Agency.Footer.AttachedFile, null, "", "");

        //    if (employee != null)
        //    {
        //        var cri = new SearchCriteria<EmployeeRelativeDTO>();
        //        cri.FiList.Add(e => e.EmployeeId == employee.Id && e.Type == RelativeTypes.ContactPerson);
        //        var cont = new EmployeeRelativeService(true)
        //                            .GetAll(cri)
        //                            .FirstOrDefault();

        //        var criApp = new SearchCriteria<EmployeeExperienceDTO>();
        //        criApp.FiList.Add(e => e.EmployeeId == employee.Id);
        //        var app = new EmployeeExperienceService(true)
        //                            .GetAll(criApp)
        //                            .FirstOrDefault();

        //        double age = DateTime.Now.Subtract(employee.DateOfBirth).Days;
        //        age = age / 365.25;
        //        string agee;
        //        try
        //        {
        //            agee = age.ToString().Substring(0, 4);
        //        }
        //        catch
        //        {
        //            agee = age.ToString();
        //        }
        //        #region Language Conditions
        //        string poor1 = "NO", poor2 = "NO", fair1 = "NO", fair2 = "NO", fluent1 = "NO", fluent2 = "NO";
        //        if (app != null)
        //        {
        //            switch (app.EnglishLanguage)
        //            {
        //                case LanguageExperience.Poor:
        //                    poor1 = "YES";
        //                    break;
        //                case LanguageExperience.Fair:
        //                    fair1 = "YES";
        //                    break;
        //                case LanguageExperience.Fluent:
        //                    fluent1 = "YES";
        //                    break;
        //            }

        //            switch (app.ArabicLanguage)
        //            {
        //                case LanguageExperience.Poor:
        //                    poor2 = "YES";
        //                    break;
        //                case LanguageExperience.Fair:
        //                    fair2 = "YES";
        //                    break;
        //                case LanguageExperience.Fluent:
        //                    fluent2 = "YES";
        //                    break;
        //            }
        //        }

        //        #endregion

        //        #region Skill Conditions
        //        string baby = "NO", clean = "NO", cook = "NO",
        //            wash = "NO", decor = "NO", drive = "NO", sew = "NO";
        //        if (app != null)
        //        {
        //            if (app.BabySitting)
        //                baby = "YES";
        //            if (app.Cleaning)
        //                clean = "YES";
        //            if (app.Cooking)
        //                cook = "YES";
        //            if (app.Washing)
        //                wash = "YES";
        //            if (app.Decorating)
        //                decor = "YES";
        //            if (app.Driving)
        //                drive = "YES";
        //            if (app.Sewing)
        //                sew = "YES";
        //        }

        //        #endregion

        //        if (cont != null)
        //            if (app != null)
        //                myDataSet.KuwaitApp.Rows.Add("1",
        //                    employee.CodeNumber,
        //                    employee.Address.HouseNumber,
        //                    employee.Address.Telephone,
        //                    app.Salary,
        //                    app.ContractPeriod.ToUpper(),
        //                    employee.FullName,
        //                    employee.PassportNumber,
        //                    employee.PassportIssueDate.ToShortDateString(), "ADDIS ABABA",
        //                    employee.PassportExpiryDate.ToShortDateString(),
        //                    cont.FirstName + " " + cont.MiddleName,
        //                    "Subcity/Zone:- " + cont.Address.SubCity,
        //                    "Kebele:- " + cont.Address.Kebele,
        //                    "House No:- " + cont.Address.HouseNumber,
        //                    "Tele:- " + cont.Address.Telephone,
        //                    employee.Address.City,
        //                    employee.Address.SubCity,
        //                    employee.Address.Kebele);

        //        if (app != null)
        //            myDataSet.KuwaitAppDetail.Rows.Add("1",
        //                employee.Sex.ToString().ToUpper(),
        //                employee.Religion.ToString().ToUpper(),
        //                employee.DateOfBirth.ToShortDateString(),
        //                agee + " YEARS",
        //                employee.PlaceOfBirth,
        //                employee.Address.City,
        //                employee.MaritalStatus.ToString().ToUpper(),
        //                app.NumberOfChildren,
        //                app.Weight, app.Height, app.Complexion, app.EducateQG,
        //                poor1, fair1, fluent1, poor2, fair2, fluent2,
        //                app.ExperienceCountry,
        //                app.ExperiencePeriod, decor, drive, sew,
        //                baby, clean, cook, wash, "", "", "");


        //        myReport.SetDataSource(myDataSet);

        //        var report = new ReportViewerCommon(myReport);
        //        report.ShowDialog();
        //    }

        //}

        //public ICommand KuwaitAppForm
        //{
        //    get
        //    {
        //        return _kuwaitAppFormCommand ?? (_kuwaitAppFormCommand = new RelayCommand(ExcuteKuwatAppForm));
        //    }
        //}
        //public void ExcuteKuwatAppForm()
        //{
        //    var myDataSet = new ReportsDataSet2();

        //    var myReport = new KuwaitApp();

        //    try
        //    {
        //        var employee = SelectedEmployee;
        //        if (employee != null)
        //        {

        //            var cri = new SearchCriteria<EmployeeRelativeDTO>();
        //            cri.FiList.Add(e => e.EmployeeId == employee.Id && e.Type == RelativeTypes.ContactPerson);
        //            var contactPerson = new EmployeeRelativeService(true)
        //                                .GetAll(cri)
        //                                .FirstOrDefault();

        //            var criApp = new SearchCriteria<EmployeeExperienceDTO>();
        //            criApp.FiList.Add(e => e.EmployeeId == employee.Id);
        //            var application = new EmployeeExperienceService(true)
        //                                .GetAll(criApp)
        //                                .FirstOrDefault();


        //            #region Conditional Agent Header

        //            //byte[] agentHeader = null;
        //            //try
        //            //{
        //            //    var Agent = _unitOfWork.Repository<AgentDTO>().GetAll().FirstOrDefault(f => f.Id == application.VisaAgentId);
        //            //    if (Agent != null)
        //            //        agentHeader = Agent.Header;
        //            //}
        //            //catch { }

        //            #endregion

        //            #region Age Calculation

        //            double age = DateTime.Now.Subtract(employee.DateOfBirth).Days;
        //            age = age / 365.25;
        //            string agee;
        //            try
        //            {
        //                agee = age.ToString().Substring(0, 4);
        //            }
        //            catch
        //            {
        //                agee = age.ToString();
        //            }

        //            #endregion

        //            #region Language Conditions

        //            string poor1 = "NO", poor2 = "NO", fair1 = "NO", fair2 = "NO", fluent1 = "NO", fluent2 = "NO";
        //            if (application != null && application.EnglishLanguage == LanguageExperience.Poor)
        //            {
        //                poor1 = "YES";
        //            }
        //            else if (application != null && application.EnglishLanguage == LanguageExperience.Fair)
        //            {
        //                fair1 = "YES";
        //            }
        //            else if (application != null && application.EnglishLanguage == LanguageExperience.Fluent)
        //            {
        //                fluent1 = "YES";
        //            }

        //            if (application != null && application.ArabicLanguage == LanguageExperience.Poor)
        //            {
        //                poor2 = "YES";
        //            }
        //            else if (application != null && application.ArabicLanguage == LanguageExperience.Fair)
        //            {
        //                fair2 = "YES";
        //            }
        //            else if (application != null && application.ArabicLanguage == LanguageExperience.Fluent)
        //            {
        //                fluent2 = "YES";
        //            }

        //            #endregion

        //            #region Skill Conditions

        //            string baby = "NO",
        //                clean = "NO",
        //                cook = "NO",
        //                wash = "NO",
        //                decor = "NO",
        //                drive = "NO",
        //                sew = "NO";
        //            if (application != null)
        //            {
        //                if (application.BabySitting)
        //                    baby = "YES";
        //                if (application.Cleaning)
        //                    clean = "YES";
        //                if (application.Cooking)
        //                    cook = "YES";
        //                if (application.Washing)
        //                    wash = "YES";
        //                if (application.Decorating)
        //                    decor = "YES";
        //                if (application.Driving)
        //                    drive = "YES";
        //                if (application.Sewing)
        //                    sew = "YES";
        //            }

        //            #endregion


        //            var Agency = new LocalAgencyService(true).GetLocalAgency();

        //            //if (Agency.AgencyName.ToLower().Contains("o2beka"))
        //            //{
        //            //    myDataSet.LetterHeads.Rows.Add("1", agentHeader, employee.Photo, application.StandPhoto, "", "");
        //            //}
        //            //else
        //            //{
        //            if (Agency != null)
        //                if (application != null)
        //                    myDataSet.LetterHeads.Rows.Add("1", Agency.Header, employee.Photo, application.StandPhoto, "", "");
        //            //}

        //            if (contactPerson != null)
        //                if (application != null)
        //                    myDataSet.KuwaitApp.Rows.Add("1",
        //                        employee.CodeNumber,
        //                        DateTime.Now.ToString("dd MMM yyyy"),
        //                        employee.AppliedProfession,
        //                        application.Salary,
        //                        application.ContractPeriod,
        //                        employee.FirstName + " " + employee.MiddleName + " " + employee.LastName,
        //                        employee.PassportNumber, employee.PassportIssueDate.ToString("dd MMM yyyy"), "ADDIS ABABA",
        //                        employee.PassportExpiryDate.ToString("dd MMM yyyy"),
        //                        contactPerson.FullName, "" +
        //                        contactPerson.Address.SubCity, "" + contactPerson.Address.Kebele, "" + contactPerson.Address.HouseNumber, "" + contactPerson.Address.Telephone,
        //                        "", "", "");
        //            if (application != null)
        //                myDataSet.KuwaitAppDetail.Rows.Add("1", "ETHIOPIA",
        //                    employee.Religion.ToString().ToUpper(),
        //                    employee.DateOfBirth.ToString("dd MMM yyyy"), agee + " YEARS",
        //                    employee.PlaceOfBirth, employee.Address.City,
        //                    employee.MaritalStatus.ToString().ToUpper(),
        //                    application.NumberOfChildren, application.Weight, application.Height, application.Complexion, application.EducateQG,
        //                    poor1, fair1, fluent1, poor2, fair2, fluent2,
        //                    application.ExperienceCountry, application.ExperiencePeriod, decor, drive, sew,
        //                    baby, clean, cook, wash, "", "", "");


        //            myReport.SetDataSource(myDataSet);


        //            var report = new ReportViewerCommon(myReport);
        //            report.ShowDialog();
        //        }
        //    }
        //    catch
        //    {
        //    }

        //}
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
