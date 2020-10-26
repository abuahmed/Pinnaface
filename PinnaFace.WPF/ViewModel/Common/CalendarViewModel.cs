using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;

namespace PinnaFace.WPF.ViewModel
{
    public class CalendarViewModel : ViewModelBase
    {
        #region Fields

        private string _durationHeader;
        private string _selectedDateDay;
        private string _selectedDateMonth;
        private string _selectedDateYear;
        private bool _toEthio;

        #endregion

        #region Constructors

        public CalendarViewModel()
        {
            DurationHeader = "Select Date"; //Convertor

            #region Initialize

            //_ethioDays = new ObservableCollection<ListDataItem>();
            //_ethioMonths = new ObservableCollection<ListDataItem>();
            //_ethioYears = new ObservableCollection<ListDataItem>();
            //_selectedEthioDay = new ListDataItem();
            //_selectedEthioMonth = new ListDataItem();
            //_selectedEthioYear = new ListDataItem();

            #endregion

            #region Load Properties

            //for (int i = 1; i <= 30; i++)
            //{
            //    EthioDays.Add(new ListDataItem { Display = i.ToString(), Value = i });
            //}

            //for (int i = 1; i <= 12; i++)
            //{
            //    string monthNo = " (" + i + ")";
            //    EthioMonths.Add(new ListDataItem { Display = CalendarUtil.GetAmhMonth(i - 1) + monthNo, Value = i });
            //}
            //EthioMonths.Add(new ListDataItem { Display = CalendarUtil.GetAmhMonth(12) + " (13)", Value = 13 });

            //for (int i = 1900; i <= 2020; i++)
            //{
            //    EthioYears.Add(new ListDataItem { Display = i.ToString(), Value = i });
            //}

            #endregion

            //Messenger.Default.Register<DateTime>(this, message => { SelectedDate = message; });
        }

        #endregion

        #region Properties

        public string DurationHeader
        {
            get { return _durationHeader; }
            set
            {
                _durationHeader = value;
                RaisePropertyChanged(() => DurationHeader);
            }
        }

        public string SelectedDateMonth
        {
            get { return _selectedDateMonth; }
            set
            {
                _selectedDateMonth = value;
                RaisePropertyChanged(() => SelectedDateMonth);
            }
        }

        public string SelectedDateDay
        {
            get { return _selectedDateDay; }
            set
            {
                _selectedDateDay = value;
                RaisePropertyChanged(() => SelectedDateDay);
            }
        }

        public string SelectedDateYear
        {
            get { return _selectedDateYear; }
            set
            {
                _selectedDateYear = value;
                RaisePropertyChanged(() => SelectedDateYear);
            }
        }

        #endregion

        #region Ethio Properties

        //private ObservableCollection<ListDataItem> _ethioDays, _ethioMonths, _ethioYears;
        private DateTime _selectedDate;//, _selectedDateParam;
        //private ListDataItem _selectedEthioDay, _selectedEthioMonth, _selectedEthioYear;

        //public ObservableCollection<ListDataItem> EthioDays
        //{
        //    get { return _ethioDays; }
        //    set
        //    {
        //        _ethioDays = value;
        //        RaisePropertyChanged(() => EthioDays);
        //    }
        //}

        //public ListDataItem SelectedEthioDay
        //{
        //    get { return _selectedEthioDay; }
        //    set
        //    {
        //        _selectedEthioDay = value;
        //        RaisePropertyChanged(() => SelectedEthioDay);
        //        if (!_toEthio)
        //            SetGregorValues();
        //    }
        //}

        //public ObservableCollection<ListDataItem> EthioMonths
        //{
        //    get { return _ethioMonths; }
        //    set
        //    {
        //        _ethioMonths = value;
        //        RaisePropertyChanged(() => EthioMonths);
        //    }
        //}

        //public ListDataItem SelectedEthioMonth
        //{
        //    get { return _selectedEthioMonth; }
        //    set
        //    {
        //        _selectedEthioMonth = value;
        //        RaisePropertyChanged(() => SelectedEthioMonth);
        //        if (!_toEthio)
        //            SetGregorValues();
        //    }
        //}

        //public ObservableCollection<ListDataItem> EthioYears
        //{
        //    get { return _ethioYears; }
        //    set
        //    {
        //        _ethioYears = value;
        //        RaisePropertyChanged(() => EthioYears);
        //    }
        //}

        //public ListDataItem SelectedEthioYear
        //{
        //    get { return _selectedEthioYear; }
        //    set
        //    {
        //        _selectedEthioYear = value;
        //        RaisePropertyChanged(() => SelectedEthioYear);
        //        if (!_toEthio)
        //            SetGregorValues();
        //    }
        //}

        //public DateTime SelectedDate
        //{
        //    get { return _selectedDate; }
        //    set
        //    {
        //        _selectedDate = value;
        //        RaisePropertyChanged(() => SelectedDate);
        //        //SetEthioValues(true);
        //    }
        //}

        //public DateTime SelectedDateParam
        //{
        //    get { return _selectedDateParam; }
        //    set
        //    {
        //        _selectedDateParam = value;
        //        RaisePropertyChanged(() => SelectedDateParam);

        //        SelectedDate = SelectedDateParam;
        //    }
        //}

        #endregion

        #region Commands

        private ICommand _okCommand, _closeCommand;

        public ICommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new RelayCommand<Object>(ExcuteOkCommand)); }
        }

        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand<Object>(ExcuteCloseCommand)); }
        }

        public void SetEthioValues(bool toEthio)
        {
            try
            {
                _toEthio = toEthio;
                //DateTime gregorDayFrom = SelectedDate;
                //string ethioDayFrom = CalendarUtil.GetEthCalendar(gregorDayFrom, false);
                //int dayf = Convert.ToInt32(ethioDayFrom.Substring(0, 2)),
                //    monthf = Convert.ToInt32(ethioDayFrom.Substring(2, 2)),
                //    yearf = Convert.ToInt32(ethioDayFrom.Substring(4, 4));
                //SelectedEthioDay = EthioDays[dayf - 1];
                //SelectedEthioMonth = EthioMonths[monthf - 1];
                //SelectedEthioYear = EthioYears[yearf - 1900];

                //SelectedDateMonth = SelectedDate.ToString("MMMM").ToUpper() + " (" + SelectedDate.Month + ")";
                //SelectedDateDay = SelectedDate.Day.ToString();
                //SelectedDateYear = SelectedDate.Year.ToString();

                _toEthio = false;
            }
            catch
            {
                //MessageBox.Show("Can't convert, may be out side of the scope!");
            }
        }

        public void SetGregorValues()
        {
            try
            {
                //DateTime gregorDayFrom = CalendarUtil.GetGregorCalendar(SelectedEthioYear.Value,
                //    SelectedEthioMonth.Value,
                //    SelectedEthioDay.Value);
                //SelectedDate = gregorDayFrom;
            }
            catch
            {
                //MessageBox.Show("Can't convert, may be out side of the scope!");
            }
        }


        public void ExcuteOkCommand(object obj)
        {
            //SelectedDateParam = SelectedDate;
            CloseWindow(obj, true);
        }

        public void ExcuteCloseCommand(object obj)
        {
            CloseWindow(obj, false);
        }

        public void CloseWindow(object obj, bool diagRes)
        {
            try
            {
                if (obj == null) return;
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = diagRes;
                    window.Close();
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}