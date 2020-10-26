using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.WPF.Reports;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class ReportViewerViewModel : ViewModelBase
    {
        #region Fields

        private static IUnitOfWork _unitOfWork;
        private EmailDTO _emailDetail;
        private ICommand _printCommand;
        private ReportTypes _reportType;
        private ICommand _sendEmailCommand, _exportReportToPdfCommand;
        private string _sendEmailCommandVisibility;
        private string _textChanged;

        #endregion

        #region Constructor

        public ReportViewerViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

            Messenger.Default.Register<ReportDocument>(this, message => { ReportToView = message; });
            Messenger.Default.Register<ReportTypes>(this, message => { ReportType = message; });
            Messenger.Default.Register<EmailDTO>(this, message => { EmailDetail = message; });
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Public Properties

        public EmailDTO EmailDetail
        {
            get { return _emailDetail; }
            set
            {
                _emailDetail = value;
                RaisePropertyChanged<EmailDTO>(() => EmailDetail);
            }
        }

        public static ReportDocument ReportToView { get; set; }

        //public static ReportDocument ReportToView
        //{
        //    get { return _reportToView; }
        //    set
        //    {
        //        _reportToView = value;
        //        //RaisePropertyChanged<ReportDocument>(() => ReportToView);
        //    }
        //}
        public ReportTypes ReportType
        {
            get { return _reportType; }
            set
            {
                _reportType = value;
                RaisePropertyChanged<ReportTypes>(() => ReportType);
                SendEmailCommandVisibility = ReportType == ReportTypes.EmbassyMonthly ? "Visible" : "Hidden";
            }
        }

        public string SendEmailCommandVisibility
        {
            get { return _sendEmailCommandVisibility; }
            set
            {
                _sendEmailCommandVisibility = value;
                RaisePropertyChanged<string>(() => SendEmailCommandVisibility);
            }
        }

        public string TextChanged
        {
            get { return _textChanged; }
            set
            {
                _textChanged = value;
                RaisePropertyChanged<string>(() => TextChanged);
            }
        }

        #endregion

        #region Commands

        public ICommand PrintCommand
        {
            get { return _printCommand ?? (_printCommand = new RelayCommand(ExcutePrintCommand)); }
        }

        public ICommand SendEmailCommand
        {
            get { return _sendEmailCommand ?? (_sendEmailCommand = new RelayCommand(ExcuteSendEmailCommand)); }
        }
        public ICommand ExportReportToPdfCommand
        {
            get { return _exportReportToPdfCommand ?? (_exportReportToPdfCommand = new RelayCommand(ExcuteExportReportToPDFCommand)); }
        }

        private void ExcutePrintCommand()
        {
            var repUtil = new ReportUtility();
            bool printed = repUtil.DirectPrinter(ReportToView);

            //Increment letter references
            if (printed)
            {
                try
                {
                    if (ReportToView != null)
                    {
                        string reportName = ReportToView.GetType().Name;

                        const string letersWithReferenceNum =
                            "LabourList_LabourListAmh_LabourListNew_LabourSingleLetterCustom1_" +
                            "LabourSingleLetterCustom4_TestimonialLetter_TestimonialLetterFor2_ForensicList";

                        if (letersWithReferenceNum.Contains(reportName))
                        {
                            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
                            SettingDTO setting = _unitOfWork.Repository<SettingDTO>().Query().Get().FirstOrDefault();
                            if (setting != null)
                            {
                                setting.CurrentLetterReferenceNumber++;
                                _unitOfWork.Repository<SettingDTO>().InsertUpdate(setting);
                                _unitOfWork.Commit();
                            }
                            _unitOfWork.Dispose();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Can't Increment Letter Number");
                }
            }
        }

        private void ExcuteSendEmailCommand()
        {
            var sendEmail = new SendEmail(ReportToView, EmailDetail);
            sendEmail.ShowDialog();


            //var myDataSet = new ReportsDataSet();
            //var myReport = new EmbassyListPortrait();

            //var Agency = _unitOfWork.Repository<AgencyDTO>()
            //    .Query()
            //    .Get()
            //    .FirstOrDefault();
            //if (Agency == null) return;

            //var serNo = 1;
            //IEnumerable<EmbassyProcessDTO> embassyProcessList = _unitOfWork.Repository<EmbassyProcessDTO>()
            //    .Query()
            //    .Include(e => e.SelectedEmployee)
            //    .Get();


            //foreach (var embassyProcess in embassyProcessList)
            //{
            //    var employ = embassyProcess.SelectedEmployee;
            //    var brCode = new BarcodeProcess();
            //    myDataSet.EmployeeListForEmbassy.Rows.Add(serNo.ToString(),
            //        employ.FullName,
            //        employ.PassportNumber, "ETHIOPIA",
            //        embassyProcess.EnjazNumber,
            //        null, "");
            //    myDataSet.EmployeeListForEmbassyHeader.Rows.Add(serNo.ToString(),
            //        Agency.AgencyName,
            //        embassyProcess.SubmitDate.ToShortDateString());

            //    serNo++;
            //}
            //myDataSet.LetterHeads.Rows.Add(
            //        "",
            //        Agency.Header,
            //        Agency.Footer, null,
            //        "", "aa");

            //myReport.SetDataSource(myDataSet);

            //ReportToView = myReport;
            //TextChanged = Guid.NewGuid().ToString().Substring(0, 2);
        }

        private void ExcuteExportReportToPDFCommand()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PinnaFace\\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var doc = ReportToView;

                var crDiskFileDestinationOptions = new DiskFileDestinationOptions();
                var crFormatTypeOptions = new PdfRtfWordFormatOptions();
                var filePath = path + "\\SampleReport.pdf";
                crDiskFileDestinationOptions.DiskFileName = filePath;
                var crExportOptions = doc.ExportOptions;
                {
                    crExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
                    crExportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
                    crExportOptions.DestinationOptions = crDiskFileDestinationOptions;
                    crExportOptions.FormatOptions = crFormatTypeOptions;
                }
                doc.Export();

                System.Diagnostics.Process.Start(@filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.InnerException, "Can't Export Document");
            }
        }
        #endregion
    }
}