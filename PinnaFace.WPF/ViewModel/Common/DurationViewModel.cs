using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Reports.Flight;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class DurationViewModel : ViewModelBase
    {
        #region Fields
        private ReportTypes _reportType;

        private ICommand _durationStartDateViewCommand,
            _durationEndDateViewCommand,
            _printSummaryListCommandView,
            _closeItemViewCommand;

        private string _startDateText, _endDateText, _headerText;

        #endregion

        #region Constructor

        public DurationViewModel()
        {
            CheckRoles();

            FilterStartDate = DateTime.Now;
            FilterEndDate = DateTime.Now;

            Agency = Singleton.Agency;// new LocalAgencyService(true).GetLocalAgency();

            Messenger.Default.Register<ReportTypes>(this, message =>
            {
                ReportType = message;
            });
        }

        #endregion

        #region Public Properties

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }

        public ReportTypes ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                RaisePropertyChanged<ReportTypes>(() => ReportType);

                HeaderText = EnumUtil.GetEnumDesc(ReportType);// + " ሪፖርት ማውጫ";
            }
        }

        public DateTime FilterStartDate
        {
            get { return _filterStartDate; }
            set
            {
                _filterStartDate = value;
                RaisePropertyChanged<DateTime>(() => FilterStartDate);
                if (FilterStartDate.Year > 2000)
                    StartDateText = CalendarUtil.GetEthCalendar(FilterStartDate, true) +
                                    " (" + FilterStartDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        public DateTime FilterEndDate
        {
            get { return _filterEndDate; }
            set
            {
                _filterEndDate = value;
                RaisePropertyChanged<DateTime>(() => FilterEndDate);

                if (FilterEndDate.Year > 2000)
                    EndDateText = CalendarUtil.GetEthCalendar(FilterEndDate, true) + " (" +
                                  FilterEndDate.ToString("dd-MM-yyyy") + ")";
            }
        }

        public string StartDateText
        {
            get { return _startDateText; }
            set
            {
                _startDateText = value;
                RaisePropertyChanged<string>(() => StartDateText);
            }
        }

        public string EndDateText
        {
            get { return _endDateText; }
            set
            {
                _endDateText = value;
                RaisePropertyChanged<string>(() => EndDateText);
            }
        }
        public AgencyDTO Agency
        {
            get { return _agency; }
            set
            {
                _agency = value;
                RaisePropertyChanged<AgencyDTO>(() => Agency);
            }
        }
        #endregion

        #region Commands

        public ICommand DurationStartDateViewCommand
        {
            get
            {
                return _durationStartDateViewCommand ??
                       (_durationStartDateViewCommand = new RelayCommand(DurationStartDate));
            }
        }

        public void DurationStartDate()
        {
            var calConv = new Calendar(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterStartDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand DurationEndDateViewCommand
        {
            get
            {
                return _durationEndDateViewCommand ??
                       (_durationEndDateViewCommand = new RelayCommand(DurationEndDate));
            }
        }

        public void DurationEndDate()
        {
            var calConv = new Calendar(DateTime.Now);
            calConv.ShowDialog();
            var dialogueResult = calConv.DialogResult;
            if (dialogueResult != null && (bool)dialogueResult)
            {
                if (calConv.DtSelectedDate.SelectedDate != null)
                    FilterEndDate = (DateTime)calConv.DtSelectedDate.SelectedDate;
            }
        }

        public ICommand PrintSummaryListCommandView
        {
            get
            {
                return _printSummaryListCommandView ??
                       (_printSummaryListCommandView = new RelayCommand<Object>(PrintSummaryList));
            }
        }

        public void PrintSummaryList(object obj)
        {
            switch (ReportType)
            {
                case ReportTypes.LabourMonthly:
                    GenerateReports.PrintMonthlyList(FilterStartDate,FilterEndDate);
                    break;
                case ReportTypes.LabourLost:
                    GenerateReports.PrintLost(FilterStartDate, FilterEndDate);
                    break;
                case ReportTypes.LabourReturned:
                    GenerateReports.PrintReturned(FilterStartDate, FilterEndDate);
                    break;
                case ReportTypes.LabourContractEnd:
                    GenerateReports.PrintContratCompleted(FilterStartDate, FilterEndDate);
                    break;
                case ReportTypes.LabourDiscontinued:
                    GenerateReports.PrintDiscontinued(FilterStartDate, FilterEndDate);
                    break;

                case ReportTypes.EmbassyMonthly:
                    GenerateReports.PrintEmbassyMonthly(FilterStartDate, FilterEndDate);
                    break;

                case ReportTypes.TicketList:
                    GenerateReports.PrintFlightList(FilterStartDate, FilterEndDate, new GetTicketList(), ReportTypes.TicketList);
                    break;
                case ReportTypes.TicketAmountList:
                    GenerateReports.PrintFlightList(FilterStartDate, FilterEndDate, new TicketAmountList(), ReportTypes.TicketAmountList);
                    break;
            }
        }

        //private void PrintMonthlyList()
        //{
        //    var myDataSet = new ReportsDataSet2();
        //    string[] alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", 
        //                           "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", 
        //                           "Z1","Z2","Z3","Z4","Z5","Z6","Z7","Z8","Z9","Z91","Z92","Z93","Z94","Z95","Z96","Z97","Z98","Z99" };
                  
        //    var myReport = new LabourMonthlyLetter();

        //    var cri = new SearchCriteria<EmployeeDTO>
        //    {
        //        BeginingDate = FilterStartDate, 
        //        EndingDate = FilterEndDate,
        //        ReportType = ReportTypes.LabourMonthly
        //    };
        //    cri.FiList.Add(s => s.FlightProcess != null && s.FlightProcess.Departured);

        //    ////If Filtered By Country or City
        //    //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

        //    var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
        //    if (employeeList.Count == 0)
        //    {
        //        MessageBox.Show("No Data is found on the given Duration!!");
        //        return;
        //    }

        //    var serNo = 1;
        //    bool found = false;

        //    int dif = FilterEndDate.Subtract(FilterStartDate).Days;
        //    //MessageBox.Show(dif.ToString());
        //    //int dif = (toDate.DayOfYear - frDate.DayOfYear) + (toDate.Year-frDate.Year) * 365;
        //    dif = (dif - 1) / 30;

        //    var fromdateAmh = CalendarUtil.GetEthCalendar(FilterStartDate, true);
        //    var todateAmh = CalendarUtil.GetEthCalendar(FilterEndDate, true);

        //    foreach (var employ in employeeList)
        //    {
        //        //found = true;
        //        var vi = employ.Visa;
        //        if(vi==null) continue;

        //        int x = (serNo - 1) / 15;
                
        //        myDataSet.LabourMonthly.Rows.Add(
        //            serNo.ToString(), 
        //            employ.FullNameAmharic, 
        //            employ.PassportNumber,
        //            CalendarUtil.GetEthCalendarFormated(employ.FlightProcess.SubmitDate, "/"), 
        //            employ.SponsorFullNameShort,
        //            vi.Sponsor.Address.Mobile, 
        //            vi.Sponsor.Address.CountryAmharic,
        //            vi.Sponsor.Address.CityAmharic, 
        //            "በጥሩ ሁኔታ", 
        //            alphabet[x], 
        //            Agency.AgencyNameAmharic, 
        //            fromdateAmh + " -  " + todateAmh + "  የ " + Monthsreturn(dif) + " ");

        //        myDataSet.LetterHeads.Rows.Add(serNo.ToString(), null, null, null,
        //            CalendarUtil.GetEthCalendarFormated(employ.FlightProcess.SubmitDate.AddYears(2), "/"), "");

        //        serNo++;
        //    }

        //    myReport.SetDataSource(myDataSet);
        //    var report = new ReportViewerCommon(myReport);
        //    report.ShowDialog();
            
        //}
        
        //private void PrintLost()
        //{
        //    var myDataSet = new ReportsDataSet();
        //    var myReport = new LabourMonthlyReports();

        //    var cri = new SearchCriteria<EmployeeDTO>
        //    {
        //        BeginingDate = FilterStartDate,
        //        EndingDate = FilterEndDate,
        //        ReportType = ReportTypes.LabourLost
        //    };
        //    cri.FiList.Add(s => s.FlightProcess != null && s.FlightProcess.Departured && s.FlightProcess.AfterFlightStatus==AfterFlightStatusTypes.Lost);

        //    ////If Filtered By Country or City
        //    //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

        //    var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
        //    if (employeeList.Count == 0)
        //    {
        //        MessageBox.Show("No Data is found on the given Duration!!");
        //        return;
        //    }

        //    var serNo = 1;
        //    //bool found = false;

        //    var fromdateAmh = CalendarUtil.GetEthCalendar(FilterStartDate, true);
        //    var todateAmh = CalendarUtil.GetEthCalendar(FilterEndDate, true);

        //    foreach (var employ in employeeList)
        //    {
        //        //found = true;
        //        myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(), 
        //            employ.FullNameAmharic,
        //            employ.PassportNumber, "", "", 
        //            null, "");

        //        serNo++;
        //    }
        //    myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", Agency.AgencyNameAmharic, 
        //        " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " የጠፉ");
        //    myDataSet.LetterHeads.Rows.Add("", Agency.Header.AttachedFile, Agency.Footer.AttachedFile, null, "", "");

        //    myReport.SetDataSource(myDataSet);
            
        //    var report = new ReportViewerCommon(myReport);
        //    report.ShowDialog();
          
        //}

        //private void PrintReturned()
        //{
        //    var myDataSet = new ReportsDataSet();

        //    var myReport = new LabourMonthlyReports();

        //    var cri = new SearchCriteria<EmployeeDTO>
        //    {
        //        BeginingDate = FilterStartDate,
        //        EndingDate = FilterEndDate,
        //        ReportType = ReportTypes.LabourReturned
        //    };
        //    cri.FiList.Add(s => s.FlightProcess != null && s.FlightProcess.Departured && s.FlightProcess.AfterFlightStatus == AfterFlightStatusTypes.Returned);

        //    ////If Filtered By Country or City
        //    //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

        //    var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
        //    if (employeeList.Count == 0)
        //    {
        //        MessageBox.Show("No Data is found on the given Duration!!");
        //        return;
        //    }

        //    var serNo = 1;
        //    //bool found = false;

        //    var fromdateAmh = CalendarUtil.GetEthCalendar(FilterStartDate, true);
        //    var todateAmh = CalendarUtil.GetEthCalendar(FilterEndDate, true);

        //    foreach (var employ in employeeList)
        //    {
        //        //found = true;
        //        myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
        //            employ.FullNameAmharic,
        //            employ.PassportNumber, "", "",
        //            null, "");

        //        serNo++;
        //    }
        //    myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", Agency.AgencyNameAmharic,
        //        " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " የተመለሱ");
        //    myDataSet.LetterHeads.Rows.Add("", Agency.Header.AttachedFile, Agency.Footer.AttachedFile, null, "", "");

        //    myReport.SetDataSource(myDataSet);

        //    var report = new ReportViewerCommon(myReport);
        //    report.ShowDialog();
           
        //}

        //private void PrintContratCompleted()
        //{
        //    var myDataSet = new ReportsDataSet();

        //    var myReport = new LabourMonthlyReports();

        //    var cri = new SearchCriteria<EmployeeDTO>
        //    {
        //        BeginingDate = FilterStartDate,
        //        EndingDate = FilterEndDate,
        //        ReportType = ReportTypes.LabourContractEnd
        //    };
        //    cri.FiList.Add(s => s.FlightProcess != null && s.FlightProcess.Departured && s.FlightProcess.AfterFlightStatus == AfterFlightStatusTypes.OnGoodCondition);

        //    ////If Filtered By Country or City
        //    //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

        //    var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
        //    if (employeeList.Count == 0)
        //    {
        //        MessageBox.Show("No Data is found on the given Duration!!");
        //        return;
        //    }

        //    var serNo = 1;
        //    //bool found = false;

        //    var fromdateAmh = CalendarUtil.GetEthCalendar(FilterStartDate, true);
        //    var todateAmh = CalendarUtil.GetEthCalendar(FilterEndDate, true);

        //    foreach (var employ in employeeList)
        //    {
        //        //found = true;
        //        myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
        //            employ.FullNameAmharic,
        //            employ.PassportNumber, "", "",
        //            null, "");

        //        serNo++;
        //    }

        //    if (FilterEndDate < DateTime.Now)
        //    {
        //        myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", Agency.AgencyNameAmharic, " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " ኮንትራታቸው የተጠናቀቀ");
        //    }
        //    else
        //    {
        //        myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", Agency.AgencyNameAmharic, " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " ኮንትራታቸው የሚጠናቀቅ");
        //    }
        //    myDataSet.LetterHeads.Rows.Add("", Agency.Header.AttachedFile, Agency.Footer.AttachedFile, null, "", "");

        //    myReport.SetDataSource(myDataSet);
        //    var report = new ReportViewerCommon(myReport);
        //    report.ShowDialog();
            
        //}

        //private void PrintDiscontinued()
        //{
        //    var myDataSet = new ReportsDataSet();

        //    var myReport = new LabourMonthlyReports();

        //    var cri = new SearchCriteria<EmployeeDTO>
        //    {
        //        BeginingDate = FilterStartDate,
        //        EndingDate = FilterEndDate,
        //        ReportType = ReportTypes.LabourDiscontinued
        //    };
        //    cri.FiList.Add(s => s.LabourProcess != null && s.LabourProcess.Discontinued && s.CurrentStatus == ProcessStatusTypes.Discontinued);

        //    ////If Filtered By Country or City
        //    //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

        //    var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();

        //    if (employeeList.Count == 0)
        //    {
        //        MessageBox.Show("No Data is found on the given Duration!!");
        //        return;
        //    }
        //    var serNo = 1;
        //    //bool found = false;

        //    var fromdateAmh = CalendarUtil.GetEthCalendar(FilterStartDate, true);
        //    var todateAmh = CalendarUtil.GetEthCalendar(FilterEndDate, true);

        //    foreach (var employ in employeeList)
        //    {
        //        //found = true;
        //        myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
        //            employ.FullNameAmharic,
        //            employ.PassportNumber, "", "",
        //            null, "");

        //        serNo++;
        //    }

        //    myDataSet.EmployeeListForEmbassyHeader.Rows.Add("", 
        //        Agency.AgencyNameAmharic, 
        //        " ለሥራ ወደ ውጭ ሀገር ተልከው " + "ከ" + fromdateAmh + " እስከ  " + todateAmh + " ማህበራዊ ከተማሩ በኋላ ያቋረጡ");
         
        //    myDataSet.LetterHeads.Rows.Add("", Agency.Header.AttachedFile, Agency.Footer.AttachedFile, null, "", "");

        //    myReport.SetDataSource(myDataSet);
        //    var report = new ReportViewerCommon(myReport);
        //    report.ShowDialog();
            
        //}

        //private void PrintEmbassyMonthly()
        //{
        //    var myDataSet = new ReportsDataSet();

        //    var myReport = new EmbassyListForEmail();

        //    var cri = new SearchCriteria<EmployeeDTO>
        //    {
        //        BeginingDate = FilterStartDate,
        //        EndingDate = FilterEndDate,
        //        ReportType = ReportTypes.EmbassyMonthly
        //    };
        //    cri.FiList.Add(s => s.EmbassyProcess != null);// && s.LabourProcess.Discontinued && s.CurrentStatus == ProcessStatusTypes.OnProcess);

        //    ////If Filtered By Country or City
        //    //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

        //    var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();

        //    if (employeeList.Count == 0)
        //    {
        //        MessageBox.Show("No Data is found on the given Duration!!");
        //        return;
        //    }
        //    var serNo = 1;
        //    //bool found = false;
            
        //    foreach (var employ in employeeList)
        //    {
                
        //        myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(), 
        //            employ.FullName, 
        //            employ.PassportNumber, 
        //            "ETHIOPIA", employ.EmbassyProcess.EnjazNumber, null, "");
        //        myDataSet.EmployeeListForEmbassyHeader.Rows.Add(serNo.ToString(), Agency.AgencyName, DateTime.Now.ToShortDateString());
        //        myDataSet.EmployeeListForEmbassyHeader2.Rows.Add(serNo.ToString(), FilterStartDate.ToShortDateString(), FilterEndDate.ToShortDateString(), "", "", "", "");
        //        myDataSet.LetterHeads.Rows.Add(serNo.ToString(), Agency.Header.AttachedFile, null, null, "", "");
        //        serNo++;

        //        serNo++;
        //    }
            
        //    myReport.SetDataSource(myDataSet);
        //    var report = new ReportViewerCommon(myReport);
        //    report.ShowDialog();

        //}

        //private void PrintFlightList(ReportDocument myReport, ReportTypes reportType)
        //{
        //    var myDataSet = new ReportsDataSet();

        //    //var myReport = new TicketAmountList();
        //    //var myReport = new GetTicketList();

        //    var cri = new SearchCriteria<EmployeeDTO>
        //    {
        //        BeginingDate = FilterStartDate,
        //        EndingDate = FilterEndDate,
        //        ReportType = reportType
        //    };
        //    cri.FiList.Add(s => s.FlightProcess != null && !s.FlightProcess.Departured);

        //    ////If Filtered By Country or City
        //    //cri.FiList.Add(s=>s.Visa!=null && s.Visa.Sponsor!=null && s.Visa.Sponsor.Address!=null);

        //    var employeeList = new EmployeeService(true, true).GetAll(cri).ToList();
        //    if (employeeList.Count == 0)
        //    {
        //        MessageBox.Show("No Data is found on the given Duration!!");
        //        return;
        //    }

        //    var serNo = 1;
        //    //bool found = false;
            
        //    foreach (var employ in employeeList)
        //    {
        //        //found = true;
        //        var flight = employ.FlightProcess;

        //        myDataSet.FlightTicket.Rows.Add(flight.SubmitDateString,
        //                            serNo.ToString(),
        //                            employ.FullName,
        //                            employ.PassportNumber,
        //                            flight.TicketNumber,
        //                            flight.TicketAmount,
        //                            flight.TicketCity.Trim().ToUpper(), "", "", "", 0.0);

        //        serNo++;
        //    }

        //    myDataSet.LetterHeads.Rows.Add("", Agency.Header.AttachedFile, Agency.Footer.AttachedFile, null, "", "");

        //    myReport.SetDataSource(myDataSet);
        //    var report = new ReportViewerCommon(myReport);
        //    report.ShowDialog();  
        //}

        public ICommand CloseItemViewCommand
        {
            get { return _closeItemViewCommand ?? (_closeItemViewCommand = new RelayCommand<Object>(CloseWindow)); }
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

        public string Monthsreturn(int ind)
        {
            if (ind < 0) ind = 0;
            if (ind > 2) ind = 2;
            string[] months = { "አንድ", "ሁለት", "ሦስት" };
            return months[ind];
        }
        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }

        #endregion

        #region Previlege Visibility

        private UserRolesModel _userRoles;
        private DateTime _filterStartDate;
        private DateTime _filterEndDate;
        private AgencyDTO _agency;


        public UserRolesModel UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                RaisePropertyChanged<UserRolesModel>(() => UserRoles);
            }
        }

        private void CheckRoles()
        {
            UserRoles = Singleton.UserRoles;
        }

        #endregion
    }
}
