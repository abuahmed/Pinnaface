using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace PinnaFace.Admin.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private static Bootstrapper _bootStrapper;

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (_bootStrapper == null)
                _bootStrapper = new Bootstrapper();
        }
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>
        public MainViewModel Main
        {
            get
            {
                return _bootStrapper.Container.Resolve<MainViewModel>();
            }
        }
        /// <summary>
        /// Gets the view's ViewModel.
        /// </summary>

        public SendEmailViewModel SendEmail
        {
            get
            {
                return _bootStrapper.Container.Resolve<SendEmailViewModel>();
            }
        }
        public UserAgencyAgentViewModel UserAgencyAgent
        {
            get
            {
                return _bootStrapper.Container.Resolve<UserAgencyAgentViewModel>();
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
        public DashboardViewModel Dashboard
        {
            get
            {
                return _bootStrapper.Container.Resolve<DashboardViewModel>();
            }
        }
        public ServerUserViewModel ServerUser
        {
            get
            {
                return _bootStrapper.Container.Resolve<ServerUserViewModel>();
            }
        }
        public ServerSettingViewModel ServerSetting
        {
            get
            {
                return _bootStrapper.Container.Resolve<ServerSettingViewModel>();
            }
        }
        public ServerAgentViewModel ServerAgent
        {
            get
            {
                return _bootStrapper.Container.Resolve<ServerAgentViewModel>();
            }
        }
        public ServerAgencyViewModel ServerAgency
        {
            get
            {
                return _bootStrapper.Container.Resolve<ServerAgencyViewModel>();
            }
        }
        public ServerProductActivationViewModel ServerProductActivation
        {
            get
            {
                return _bootStrapper.Container.Resolve<ServerProductActivationViewModel>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

     
    
    }
}