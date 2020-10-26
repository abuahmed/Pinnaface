using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.WPF.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static Bootstrapper _bootStrapper;

        public ViewModelLocator()
        {
            //Add Code to choose the server/database the user wants to connect to, the line below depends on it
            Singleton.Edition = PinnaFaceEdition.ServerEdition;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (_bootStrapper == null)
                _bootStrapper = new Bootstrapper();
        }

        public MainViewModel Main
        {
            get
            {
                return _bootStrapper.Container.Resolve<MainViewModel>();
            }
        }
        public AddressViewModel AddressVm
        {
            get
            {
                return _bootStrapper.Container.Resolve<AddressViewModel>();
            }
        }
        public LocalAgencyViewModel LocalAgency
        {
            get
            {
                return _bootStrapper.Container.Resolve<LocalAgencyViewModel>();
            }
        }
        public ForeignAgentViewModel ForeignAgent
        {
            get
            {
                return _bootStrapper.Container.Resolve<ForeignAgentViewModel>();
            }
        }
        public EmployeeViewModel Employee
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeViewModel>();
            }
        }
        public RequiredDocumentsViewModel RequiredDocuments
        {
            get
            {
                return _bootStrapper.Container.Resolve<RequiredDocumentsViewModel>();
            }
        }
        public EmployeeExperienceViewModel EmployeeExperience
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeExperienceViewModel>();
            }
        }
        public EmployeeDetailViewModel EmployeeDetail
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeDetailViewModel>();
            }
        }
        public EmployeePhotoViewModel EmployeePhoto
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeePhotoViewModel>();
            }
        }
        public EmployeeContactPersonViewModel EmployeeRelative
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeContactPersonViewModel>();
            }
        }
        public EmployeeEducationViewModel EmployeeEducation
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeEducationViewModel>();
            }
        }
        public EmployeeHawalaViewModel EmployeeHawala
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeHawalaViewModel>();
            }
        }
        public EmployeeTestimonyViewModel EmployeeTestimony
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeTestimonyViewModel>();
            }
        }
        public VisaViewModel Visa
        {
            get
            {
                return _bootStrapper.Container.Resolve<VisaViewModel>();
            }
        }
        public VisaDetailViewModel VisaDetail
        {
            get
            {
                return _bootStrapper.Container.Resolve<VisaDetailViewModel>();
            }
        }
        public VisaConditionViewModel VisaCondition
        {
            get
            {
                return _bootStrapper.Container.Resolve<VisaConditionViewModel>();
            }
        }
        public ReportsViewModel Reports
        {
            get
            {
                return _bootStrapper.Container.Resolve<ReportsViewModel>();
            }
        }
        public ReportViewerViewModel ReportViewerCommon
        {
            get
            {
                return _bootStrapper.Container.Resolve<ReportViewerViewModel>();
            }
        }
        public EnjazitBrowserViewModel EnjazitBrowser
        {
            get
            {
                return _bootStrapper.Container.Resolve<EnjazitBrowserViewModel>();
            }
        }
        public EmployeeMedicalInsuranceViewModel EmployeeMedicalInsurance
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmployeeMedicalInsuranceViewModel>();
            }
        }
        public LabourProcessViewModel LabourProcess
        {
            get
            {
                return _bootStrapper.Container.Resolve<LabourProcessViewModel>();
            }
        }
        public DiscontinueProcessViewModel DiscontinueProcess
        {
            get
            {
                return _bootStrapper.Container.Resolve<DiscontinueProcessViewModel>();
            }
        }
        public EmbassyProcessViewModel EmbassyProcess
        {
            get
            {
                return _bootStrapper.Container.Resolve<EmbassyProcessViewModel>();
            }
        }
        public FlightProcessViewModel FlightProcess
        {
            get
            {
                return _bootStrapper.Container.Resolve<FlightProcessViewModel>();
            }
        }
        public AfterFlightProcessViewModel AfterFlightProcess
        {
            get
            {   
                return _bootStrapper.Container.Resolve<AfterFlightProcessViewModel>();
            }
        }
        public DurationViewModel Duration
        {
            get
            {
                return _bootStrapper.Container.Resolve<DurationViewModel>();
            }
        }
        public CalendarConvertorViewModel Convertor
        {
            get
            {
                return _bootStrapper.Container.Resolve<CalendarConvertorViewModel>();
            }
        }
        public CalendarViewModel Calendar
        {
            get
            {
                return _bootStrapper.Container.Resolve<CalendarViewModel>();
            }
        }
        public SendEmailViewModel SendEmail
        {
            get
            {
                return _bootStrapper.Container.Resolve<SendEmailViewModel>();
            }
        }
        public ComplainViewModel Complain
        {
            get
            {
                return _bootStrapper.Container.Resolve<ComplainViewModel>();
            }
        }
        public ComplainDetailViewModel ComplainDetail
        {
            get
            {
                return _bootStrapper.Container.Resolve<ComplainDetailViewModel>();
            }
        }
        public ComplainSolutionViewModel ComplainSolution
        {
            get
            {
                return _bootStrapper.Container.Resolve<ComplainSolutionViewModel>();
            }
        }
        public UserViewModel User
        {
            get
            {
                return _bootStrapper.Container.Resolve<UserViewModel>();
            }
        }
        public LoginViewModel Login
        {
            get
            {
                return _bootStrapper.Container.Resolve<LoginViewModel>();
            }
        }
        public ChangePasswordViewModel ChangePassword
        {
            get
            {
                return _bootStrapper.Container.Resolve<ChangePasswordViewModel>();
            }
        }
        public SplashScreenViewModel Splash
        {
            get
            {
                return _bootStrapper.Container.Resolve<SplashScreenViewModel>();
            }
        }
        public AboutBoxViewModel AboutBox
        {
            get 
            {
                return _bootStrapper.Container.Resolve<AboutBoxViewModel>();
            }
        }
        public ActivationViewModel Activation
        {
            get
            {
                return _bootStrapper.Container.Resolve<ActivationViewModel>();
            }
        }
        public SendReportViewModel SendReport
        {
            get
            {
                return _bootStrapper.Container.Resolve<SendReportViewModel>();
            }
        }
        public SettingViewModel Setting
        {
            get
            {
                return _bootStrapper.Container.Resolve<SettingViewModel>();
            }
        }
        public ListViewModel List
        {
            get
            {
                return _bootStrapper.Container.Resolve<ListViewModel>();
            }
        }
        public BackupRestoreViewModel BackupRestore
        {
            get
            {
                return _bootStrapper.Container.Resolve<BackupRestoreViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}