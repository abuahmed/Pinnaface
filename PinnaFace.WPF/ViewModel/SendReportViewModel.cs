using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.WPF.Views;
using PinnaKeys.OA;

namespace PinnaFace.WPF.ViewModel
{
    public class SendReportViewModel : ViewModelBase
    {
        #region Fields

        private string _sent;
        private readonly string _sourceLogFile;
        private ICommand _activateCommand;
        private string _progressBarVisibility, _fileLocation;
        private bool _commandsEnability;

        #endregion

        #region Constructor

        public SendReportViewModel()
        {
            ProgressBarVisibility = "Collapsed";
            CommandsEnability = true;

            var sourceFile = PathUtil.GetLogPath();
            var sourceFiName = DateTime.Now.Date.ToString("dd-MM-yy") + "_Log.txt";
            _sourceLogFile = Path.Combine(sourceFile, sourceFiName);

            if (File.Exists(_sourceLogFile))
            {
                FileLocation = sourceFiName;
            }
            else
            {
                FileLocation = "Can't Find Error Log File";
                CommandsEnability = false;
            }
        }

        #endregion

        #region Properties

        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }

        public bool CommandsEnability
        {
            get { return _commandsEnability; }
            set
            {
                _commandsEnability = value;
                RaisePropertyChanged<bool>(() => this.CommandsEnability);
            }
        }

        public string FileLocation
        {
            get { return _fileLocation; }
            set
            {
                _fileLocation = value;
                RaisePropertyChanged<string>(() => FileLocation);
            }
        }
        #endregion

        #region Commands

        private object _obj;

        public ICommand SendCommand
        {
            get { return _activateCommand ?? (_activateCommand = new RelayCommand<object>(ExcuteActivateCommand)); }
        }

        private void ExcuteActivateCommand(object windowObject)
        {
            _obj = windowObject;
            ProgressBarVisibility = "Visible";
            CommandsEnability = false;
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarVisibility = "Collapsed";
            CommandsEnability = true;
            if (string.IsNullOrEmpty(_sent))
            {
                NotifyUtility.ShowCustomBalloon("Sent", "Report Successfully Sent", 4000);
                CloseWindow(_obj);
            }
            else
            {
                NotifyUtility.ShowCustomBalloon("Failed Sending", _sent, 4000);
            }
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            _sent = "";
            try
            {
                var agencyName = "NoAgencyName";
                var biosSn = "00000";
                try
                {
                    agencyName = Singleton.Agency.AgencyName;
                    biosSn = Singleton.ProductActivation.BiosSn;
                    agencyName = agencyName.Substring(0, agencyName.IndexOf(' '));
                }
                catch
                {
                }

                var destination = PathUtil.GetServerLogPath();
                
                var destFiName = agencyName + "_" + biosSn + "_" + DateTime.Now.Date.ToString("dd-MM-yy") + "_Log.txt";
                
                var destpa = Path.Combine(destination, destFiName);

                using (var client = new WebClient())
                {
                    client.Credentials = DbCommandUtil.GetNetworkCredential();

                    client.UploadFile(destpa, _sourceLogFile);
                }
                
            }
            catch(Exception exception)
            {
                _sent = exception.Message;
            }
        }

        private ICommand _closeWindowCommand;

        public ICommand CloseWindowCommand
        {
            get { return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand<object>(CloseWindow)); }
        }

        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }

        #endregion
    }
}