using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace PinnaFace.Admin.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
                
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            CurrentViewModel = DashboardViewModel;
            HeaderText = "PinnaFace DashBoard";
            
            //ProductKeyViewCommand = new RelayCommand(ExecuteProductKeyViewCommand); 
        }
        private ViewModelBase _currentViewModel;

        /// <summary>
        /// Static instance of one of the ViewModels.
        /// </summary>        
        readonly static DashboardViewModel DashboardViewModel = new ViewModelLocator().Dashboard;
        /// <summary>
        /// The CurrentView property.  The setter is private since only this 
        /// class can change the view via a command. If the View is changed,
        /// we need to raise a property changed event (via INPC).
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                if (_currentViewModel == value)
                    return;
                _currentViewModel = value;
                RaisePropertyChanged("CurrentViewModel");
            }
        }

        public RelayCommand ProductKeyViewCommand { get; private set; }

        /// <summary>
        /// Set the CurrentViewModel to 'FirstViewModel'
        /// </summary>
        private void ExecuteProductKeyViewCommand()
        {
            HeaderText = "PinnaFace Dashboard";
            CurrentViewModel = DashboardViewModel;
        }

        string _headerText;

        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                if (_headerText == value)
                    return;
                _headerText = value;
                RaisePropertyChanged("HeaderText");
            }
        }
       
        
    }
}