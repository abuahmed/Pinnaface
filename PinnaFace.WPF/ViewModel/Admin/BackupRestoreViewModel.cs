using System;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using System.Windows;
using PinnaFace.Service;
using PinnaFace.WPF.Properties;
using MessageBox = System.Windows.MessageBox;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace PinnaFace.WPF.ViewModel
{
    public class BackupRestoreViewModel : ViewModelBase
    {
        #region Fields
        private static Server _server;
        #endregion

        #region Constructor
        public BackupRestoreViewModel()
        {
            if (Singleton.Edition == PinnaFaceEdition.ServerEdition)
            {
                _server = BackUpRestoreUtil.GetServer();
            }
            ProgressBarVisibility = "Collapsed";
            CommandsEnability = true;
            Messenger.Default.Register<object>(this, message =>
            {
                MainWindow = message;
            });
        }
        #endregion

        #region Properties

        private object _mainWindow;
        public object MainWindow
        {
            get { return _mainWindow; }
            set
            {
                _mainWindow = value;
                RaisePropertyChanged<object>(() => MainWindow);
            }
        }

        private string _progressBarVisibility;
        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }


        private bool _commandsEnability;
        public bool CommandsEnability
        {
            get { return _commandsEnability; }
            set
            {
                _commandsEnability = value;
                RaisePropertyChanged<bool>(() => this.CommandsEnability);
            }
        }

        private string _fileLocation;
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
        private ICommand _backupCommand, _restoreCommand, _closeWindowCommand;

        public ICommand BackupCommand
        {
            get
            {
                return _backupCommand ?? (_backupCommand = new RelayCommand(BackUp));
            }
        }
        public ICommand RestoreCommand
        {
            get
            {
                return _restoreCommand ?? (_restoreCommand = new RelayCommand<Object>(Restore));
            }
        }

        public ICommand CloseWindowCommand
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand<Object>(CloseWindow));
            }
        }
        
        private void BackUp()
        {
            ProgressBarVisibility = "Visible";
            if (Singleton.Edition == PinnaFaceEdition.ServerEdition)
            {
                BackupSqlServerData();
            }
            else
            {
                BackupCompactData();
            }
            ProgressBarVisibility = "Collapsed";
        }
        private void Restore(object obj)
        {
            ProgressBarVisibility = "Visible";
            if (Singleton.Edition == PinnaFaceEdition.ServerEdition)
            {
                RestoreSqlServerData();
            }
            else
            {
                RestoreCompactData(obj);
            }
            ProgressBarVisibility = "Collapsed";
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                //window.DialogResult = true;
                window.Close();
            }
        }
        #endregion

        #region SQLCE
        public void BackupCompactData()
        {
            try
            {
                var folder = new FolderBrowserDialog();
                if (folder.ShowDialog() != DialogResult.OK) return;
                FileLocation = folder.SelectedPath;
                try
                {
                    var sourcefileName = Singleton.SqlceFileName;
                    var sourceFile = new FileInfo(sourcefileName);
                    var agencyName = Singleton.Agency.AgencyName;
                    var biosSn = Singleton.ProductActivation.BiosSn;
                    agencyName = agencyName.Substring(0, agencyName.IndexOf(' '));

                    var destFileName = DateTime.Now.ToString("dd-MM-yyyy") + "_" + agencyName + "_" + biosSn + "_PFsqlCe.sdf";
                    var destinationFile = Path.Combine(folder.SelectedPath, destFileName);

                    var destFile = new FileInfo(destinationFile);
                    if (destFile.Exists)
                        File.Delete(destFile.FullName);

                    if (sourceFile.Exists)
                    {
                        File.Copy(sourceFile.FullName, destinationFile);
                        MessageBox.Show(Resources.Database_Backup_taken_Successfully + Environment.NewLine + "You can get file here:" + destinationFile);
                    }
                    else
                        MessageBox.Show("Can't Find the source file to copy to!" + Environment.NewLine + Resources.Try_Agian_Later);
                }
                catch
                {
                    MessageBox.Show(Resources.Cant_Backup_File + Environment.NewLine + Resources.Try_Agian_Later);
                }
            }
            catch
            {
                MessageBox.Show(Resources.Cant_Backup_File + Environment.NewLine + Resources.Try_Agian_Later);
            }
        }
        public void RestoreCompactData(object obj)
        {
            try
            {
                var open = new OpenFileDialog { Filter = Resources.Backup_Files___sdf_____sdf };
                if (open.ShowDialog() == DialogResult.OK)
                {
                    FileLocation = open.FileName;
                    try
                    {
                        var destinationFile = Singleton.SqlceFileName;
                        var destFile = new FileInfo(destinationFile);
                        if (destFile.Exists)//If file exists we have to first move it, because replace doesnt work
                        {
                            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\PinnaFaceCEBackUp" + "\\";
                            if (!Directory.Exists(path))
                                Directory.CreateDirectory(path);
                            var pathfile = Path.Combine(path, "PinnaFaceDb_" + DateTime.Now.Ticks.ToString() + ".sdf");

                            File.Move(destFile.FullName, pathfile);
                        }

                        var sourcefilePath = open.FileName;
                        var fi = new FileInfo(sourcefilePath);

                        if (!fi.Exists) return;
                        File.Copy(sourcefilePath, destinationFile);
                        MessageBox.Show("Database Restored Successfully!" + Environment.NewLine +
                                        "The System will close you have to reopen the system to refresh the new restored data!",
                            "Restore Success", MessageBoxButton.OK, MessageBoxImage.Error);
                        CloseWindow(obj);
                        CloseWindow(MainWindow);
                    }
                    catch { }
                }
            }
            catch { }
        }
        #endregion

        #region SQLServer
        public void BackupSqlServerData()
        {
            try
            {
                var folder = new FolderBrowserDialog();
                if (folder.ShowDialog() == DialogResult.OK)
                {
                    FileLocation = folder.SelectedPath;
                
                    if (_server != null)
                    {
                        var status=BackUpRestoreUtil.BackUpServerDatabase(_server,FileLocation);
                        if(string.IsNullOrEmpty(status))
                            NotifyUtility.ShowCustomBalloon("Backup Successfull", "Bakup of Database " + " successfully created",4000);
                        else
                        {
                            MessageBox.Show("ERROR: An error ocurred while backing up DataBase" + status, "Backup Error");
                        }
                    }
                }
            }
            catch (Exception x)
            {
                MessageBox.Show("ERROR: An error ocurred while backing up DataBase" + x.Message, "Server Error");
            }

        }
        public void RestoreSqlServerData()
        {
            try
            {
                var open = new OpenFileDialog { Filter = "Backup Files(*.bak;)|*.bak" };
                if (open.ShowDialog() != DialogResult.OK) return;
                FileLocation = open.FileName;
                if (_server != null)
                {
                    try
                    {
                        /**
                         Exclusive access could not be obtained because the database is in use.
                            RESTORE DATABASE is terminating abnormally.
                         */
                        var rstDatabase = new Restore { Action = RestoreActionType.Database, Database = "PinnaFaceDbProd" };
                        var bkpDevice = new BackupDeviceItem(open.FileName, DeviceType.File);
                        rstDatabase.Devices.Add(bkpDevice);
                        rstDatabase.ReplaceDatabase = true;
                        rstDatabase.Restart = true;
                        rstDatabase.SqlRestore(_server);
                        //MessageBox.Show("The Database is" + " succefully restored", "Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //Close Main Window
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message + Environment.NewLine + exception.InnerException, "Restore Error");
                    }
                 
                }
                else
                {
                    //MessageBox.Show("ERROR: A connection to a SQL server was not established.", "Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //this.Cursor = Cursors.Arrow;
                }
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed loading backup file");
            }
        }
        #endregion

        #region Restore PinnaFace 1

        //public ICommand RestorePinnaFace1Command
        //{
        //    get
        //    {
        //        return _restorePinnaFace1Command ?? (_restorePinnaFace1Command = new RelayCommand(ExcuteRestorePinnaFace1Command));
        //    }
        //}

        //private void ExcuteRestorePinnaFace1Command()
        //{
        //    ProgressBarVisibility = "Visible";
        //    CommandsEnability = false;
        //    var worker = new BackgroundWorker();
        //    worker.DoWork += DoWork;
        //    worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        //    worker.RunWorkerAsync();
        //}

        //private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    ProgressBarVisibility = "Collapsed";
        //    CommandsEnability = true;
        //    MessageBox.Show("Restore Completed");
        //}

        //private void DoWork(object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        var dbContext = new OneFEMPDbContext(ConnectionStringName);
        //        IEnumerable<string> existingEPs = _unitOfWork.Repository<EmployeeDTO>().GetAll().Select(em => em.PassportNumber).ToList();

        //        #region Local Agency
        //        var agency = dbContext.Agencies.FirstOrDefault();
        //        if (agency != null)
        //        {
        //            var Agency = _unitOfWork.Repository<AgencyDTO>().GetAllIncludingChilds(a => a.Address).FirstOrDefault();
        //            if (Agency != null)
        //            {
        //                Agency.Header = agency.Header;
        //                Agency.Footer = agency.Footer;
        //                Agency.AgencyNameAmharic = agency.Amh_name;
        //                Agency.ManagerName = agency.GeneralManager;
        //                Agency.ManagerNameAmharic = agency.Amh_manager;
        //                Agency.Managertype = agency.Managertype;
        //                Agency.DepositAmount = agency.DepositAmount;
        //                Agency.LicenceNumber = agency.LicenceNumber;
        //                if (Agency.Address != null)
        //                {
        //                    Agency.Address.Region = agency.Region;
        //                    Agency.Address.City = agency.City;
        //                    Agency.Address.SubCity = agency.SubCity;
        //                    Agency.Address.Telephone = agency.Tel;
        //                    Agency.Address.PrimaryEmail = agency.Email;
        //                    Agency.Address.Kebele = agency.Kebele;
        //                    Agency.Address.PoBox = agency.POBox;
        //                    Agency.Address.Fax = agency.Fax;
        //                }

        //                _unitOfWork.Repository<AgencyDTO>().Update(Agency);
        //            }
        //            _unitOfWork.Commit();
        //        }
        //        #endregion

        //        var foreignAgents = _unitOfWork.Repository<AgentDTO>().GetAll().ToList();

        //        #region Foreign Agents
        //        try
        //        {
        //            var foreignAgentsOld = dbContext.ForeignAgents;
        //            var commit = false;
        //            foreach (var Agent in foreignAgentsOld)
        //            {
        //                if (foreignAgents.FirstOrDefault(fa => fa.AgentName == Agent.FullName) != null)
        //                    continue;
        //                var foreignAgentDto = new AgentDTO
        //                {
        //                    #region Agent Body
        //                    Header = Agent.Header,
        //                    Footer = Agent.Footer,
        //                    AgentName = Agent.FullName,
        //                    AgentNameAmharic =
        //                        string.IsNullOrEmpty(Agent.FullNameAmh)
        //                            ? Agent.FullName
        //                            : Agent.FullNameAmh,
        //                    LicenseNumber = Agent.LicenseNumber,
        //                    PassportNumber = Agent.PassportNum,
        //                    Address = new AddressDTO
        //                    {
        //                        Country =
        //                            Agent.Country.ToLower().Contains("ku")
        //                                ? CountryList.Kuwait
        //                                : CountryList.SaudiArabia,
        //                        City =
        //                            string.IsNullOrEmpty(Agent.City)
        //                                ? "RIYADH"
        //                                : Agent.City,
        //                        Telephone =
        //                            Agent.Tel.Trim().Length < 8
        //                                ? null
        //                                : Agent.Tel.Trim().Replace(" ", ""),
        //                        AlternateTelephone =
        //                            Agent.Tel2.Trim().Length < 8
        //                                ? null
        //                                : Agent.Tel2.Trim(),
        //                        PrimaryEmail = Agent.Email.Trim(),
        //                        PoBox = Agent.POBox,
        //                        Fax = Agent.Fax
        //                    }
        //                    #endregion

        //                };
        //                _unitOfWork.Repository<AgentDTO>().Insert(foreignAgentDto);
        //                commit = true;
        //            }
        //            if (foreignAgentsOld.Any() && commit)
        //                _unitOfWork.Commit();

        //            foreignAgents = _unitOfWork.Repository<AgentDTO>().GetAll().ToList();
        //        }
        //        catch
        //        {
        //            MessageBox.Show("Problem Importing foreign agents Data");
        //            return;
        //        }

        //        #endregion

        //        var employees = (from v in dbContext.Employees where v.VisaId != null && v.VisaId != 0 orderby v.EmployeeId ascending select v).ToList();
        //        foreach (var employ in employees)
        //        {
        //            try
        //            {
        //                if (existingEPs.Contains(employ.PassportNum))
        //                    continue;
        //                var emp = new EmployeeDTO
        //                {
        //                    #region SelectedEmployee Profile
        //                    CurrentStatus = ProcessStatusTypes.OnGoodCondition,
        //                    FirstName = employ.FirstName,
        //                    MiddleName = employ.MiddleName,
        //                    LastName = employ.LastName,
        //                    PassportNumber = employ.PassportNum,
        //                    DateOfBirth = employ.DateOfBirth,
        //                    Sex = employ.Sex == "Male" ? Sex.Male : Sex.Female,
        //                    MaritalStatus = MaritalStatusTypes.Single,
        //                    Religion = ReligionTypes.Muslim,
        //                    AppliedProfession = employ.Sex == "Male" ? "DRIVER" : "HOUSE MAID",
        //                    PlaceOfBirth =
        //                        string.IsNullOrEmpty(employ.PlaceOfBirth) ? "Addis Abeba" : employ.PlaceOfBirth,
        //                    CodeNumber = string.IsNullOrEmpty(employ.CodeNo) ? "000" + employ.EmployeeId : employ.CodeNo,
        //                    FirstNameAmharic = employ.AmharicFirstName,
        //                    MiddleNameAmharic = employ.AmharicMiddleName,
        //                    LastNameAmharic = employ.AmharicLastName,
        //                    PassportIssueDate = employ.PassportIssueDate != null
        //                        ? (DateTime)employ.PassportIssueDate
        //                        : DateTime.Now,
        //                    PassportExpiryDate = employ.PassportExpiryDate != null
        //                        ? (DateTime)employ.PassportExpiryDate
        //                        : DateTime.Now,
        //                    Photo = employ.Photo,


        //                    #endregion

        //                    #region SelectedEmployee Address
        //                    Address = new AddressDTO
        //                    {
        //                        Region = employ.Region,
        //                        City = string.IsNullOrEmpty(employ.City) ? "Addis Abeba" : employ.City,
        //                        SubCity = employ.SubCity,
        //                        Telephone =
        //                            employ.Tel.Trim().Length < 8 || employ.Tel.Trim().Length > 14
        //                                ? null
        //                                : employ.Tel.Trim(),
        //                        PrimaryEmail = employ.Email.Trim(),
        //                        Kebele = employ.Kebele,
        //                        PoBox = employ.POBox,
        //                        Fax = employ.Fax,
        //                        HouseNumber = employ.HouseNumber
        //                    }
        //                    #endregion
        //                };

        //                #region Details

        //                #region Contact Person
        //                var contact = employ.EmergencyPeople.FirstOrDefault();
        //                if (contact != null)
        //                {
        //                    string _ageOrBirthDate = contact.AgeOrBirthDate;
        //                    try
        //                    {
        //                        _ageOrBirthDate = Convert.ToDateTime(contact.AgeOrBirthDate).Year.ToString();
        //                    }
        //                    catch
        //                    {
        //                        _ageOrBirthDate = contact.AgeOrBirthDate;
        //                    }

        //                    var contactDto = new EmployeeRelativeDTO
        //                    {
        //                        FullName = (string.IsNullOrEmpty(contact.FirstName) ? "AAA" : contact.FirstName) + " " +
        //                        (string.IsNullOrEmpty(contact.MiddleName) ? "BBB" : contact.MiddleName) + " " +
        //                        (string.IsNullOrEmpty(contact.LastName) ? "" : contact.LastName),
        //                        Sex = contact.Sex == "Male" ? Sex.Male : Sex.Female,
        //                        DateOfBirth = contact.DateOfBirth,
        //                        AgeOrBirthDate = _ageOrBirthDate,
        //                        Address = new AddressDTO
        //                          {
        //                              Region = contact.Region,
        //                              City = string.IsNullOrEmpty(contact.City) ? "Addis Abeba" : contact.City,
        //                              SubCity = contact.SubCity.Trim(),
        //                              Telephone = contact.Tel.Trim().Length < 8 || contact.Tel.Trim().Length > 14 ? null : contact.Tel.Trim().Replace(" ", ""),
        //                              PrimaryEmail = contact.Email.Trim(),
        //                              Kebele = contact.Kebele,
        //                              PoBox = contact.POBox,
        //                              Fax = contact.Fax,
        //                              HouseNumber = employ.HouseNumber
        //                          }
        //                    };
        //                    emp.EmployeeRelatives.Add(contactDto);
        //                }
        //                #endregion

        //                #region Education
        //                var education = employ.EducationData.FirstOrDefault();
        //                if (education != null)
        //                {
        //                    emp.EmployeeEducations.Add(new EmployeeEducationDTO
        //                    {
        //                        LevelOfQualification = LevelOfQualificationTypes.Elementary,
        //                        Award = AwardTypes.Certificate,
        //                        QualificationType = QualificationTypes.Certificate
        //                    });
        //                }
        //                #endregion

        //                #region Visa
        //                var visa = employ.Visa;
        //                if (visa != null && visa.Agent != null && visa.ForeignAgentId != 0)
        //                {
        //                    int visQty;
        //                    DateTime? visaDatee = null;
        //                    if (visa.VisaDate != null) visaDatee = Convert.ToDateTime(visa.VisaDate);
        //                    if (!int.TryParse(visa.VisaQuantity, out visQty)) visQty = 1;

        //                    emp.Visa = new VisaDTO
        //                    {
        //                        #region Visa Detail
        //                        VisaNumber = visa.VisaNum,
        //                        City = string.IsNullOrEmpty(visa.City) ? "RIYADH" : visa.City.Trim(),
        //                        CityAmharic = string.IsNullOrEmpty(visa.CityAmh) ? "ሪያድ" : visa.CityAmh,
        //                        Country = visa.Country.ToLower().Contains("ku") ? CountryList.Kuwait : CountryList.SaudiArabia,
        //                        CountryAmharic = string.IsNullOrEmpty(visa.CountryAmh) ? "ሳውዲ" : visa.CountryAmh,
        //                        ContractNumber = visa.ContractNum,
        //                        VisaQuantity = visQty,
        //                        VisaDate = visaDatee,
        //                        VisaDateArabic = visa.VisaDateArabic.Trim(),
        //                        FileNumber = visa.Filenumber,
        //                        BankNumber = visa.BankNumber,
        //                        WekalaDate = visa.WekalaDate,
        //                        WekalaNumber = visa.WekalaNumber,
        //                        Notes = employ.Visa.Notes,
        //                        #endregion

        //                        #region Conditions
        //                        Condition = new VisaConditionDTO
        //                                            {
        //                                                AgeFrom = (int)visa.AgeFrom,
        //                                                AgeTo = visa.AgeTo,
        //                                                FirstTime = visa.FirstTime.ToLower() == "true",
        //                                                GoodLooking = visa.GoodLooking.ToLower() == "true",
        //                                                WriteRead = visa.WriteRead.ToLower() == "true",
        //                                                Notes = visa.Notes,
        //                                                Salary = visa.Salary,
        //                                                Profession = visa.Profession,
        //                                                ProfessionAmharic = string.IsNullOrEmpty(visa.ProfessionAmh) ? "የቤት ውስጥ ሠራተኛ" : visa.ProfessionAmh,
        //                                                Religion = ReligionTypes.Muslim
        //                                            },
        //                        #endregion

        //                        #region Sponsor

        //                        Sponsor = new VisaSponsorDTO
        //                                            {
        //                                                FirstName = visa.SponsorFirstName,
        //                                                MiddleName = visa.SponsorMiddleName,
        //                                                LastName = visa.SponsorLastName,
        //                                                FullName = visa.SponsorFirstName.Trim().Replace(" ", "") + " " + visa.SponsorMiddleName.Trim().Replace(" ", "") + " " + visa.SponsorLastName.Trim().Replace(" ", ""),
        //                                                FirstNameAmharic = string.IsNullOrEmpty(visa.SponsorFirstNameAmh) ? visa.SponsorFirstName : visa.SponsorFirstNameAmh,
        //                                                MiddleNameAmharic = string.IsNullOrEmpty(visa.SponsorMiddleNameAmh) ? visa.SponsorMiddleName : visa.SponsorMiddleNameAmh,
        //                                                LastNameAmharic = string.IsNullOrEmpty(visa.SponsorLastNameAmh) ? visa.SponsorLastName : visa.SponsorLastNameAmh,

        //                                                FullNameAmharic = (string.IsNullOrEmpty(visa.SponsorFirstNameAmh) ? visa.SponsorFirstName : visa.SponsorFirstNameAmh)
        //                                                + " " + (string.IsNullOrEmpty(visa.SponsorMiddleNameAmh) ? visa.SponsorMiddleName : visa.SponsorMiddleNameAmh)
        //                                                + " " + (string.IsNullOrEmpty(visa.SponsorLastNameAmh) ? visa.SponsorLastName : visa.SponsorLastNameAmh),

        //                                                FullNameArabic = string.IsNullOrWhiteSpace(visa.FullNameAr) ? "" : visa.FullNameAr,
        //                                                PassportNumber = visa.PassportNum,
        //                                                CityArabic = visa.CityAr,
        //                                                Address = new AddressDTO
        //                                                {
        //                                                    Country =
        //                                                        visa.Country.ToLower().Contains("ku")
        //                                                            ? CountryList.Kuwait
        //                                                            : CountryList.SaudiArabia,
        //                                                    City = string.IsNullOrEmpty(visa.City) ? "RIYADH" : visa.City,
        //                                                    Telephone = visa.Tel.Trim().Length < 8 || visa.Tel.Trim().Length > 14 ? null : visa.Tel.Trim().Replace(" ", ""),
        //                                                    AlternateTelephone = visa.Tel2.Trim().Length < 8 ? null : visa.Tel2.Trim(),
        //                                                    PrimaryEmail = visa.Email.Trim(),
        //                                                    PoBox = visa.POBox,
        //                                                    Fax = visa.Fax
        //                                                }

        //                                            },
        //                        #endregion

        //                        #region Foreign Agent
        //                        ForeignAgentId = foreignAgents.FirstOrDefault(f => f.AgentName == visa.Agent.FullName).Id
        //                        #endregion
        //                    };
        //                }
        //                #endregion

        //                #region Insurance
        //                var insurance = employ.InsuranceProcesses.FirstOrDefault();
        //                if (insurance != null)
        //                {
        //                    emp.InsuranceProcesses.Add(new InsuranceProcessDTO()
        //                    {
        //                        SubmitDate = insurance.SubmDate.Value,
        //                        PolicyNumber = "000",
        //                        BeginingDate = insurance.SubmDate,
        //                        EndDate = insurance.SubmDate.Value.AddYears(2).AddDays(45),
        //                        InsuranceCompany = "Medin Insurance"
        //                    });
        //                }
        //                #endregion

        //                #region Labour
        //                var labour = employ.LabourProcesses.FirstOrDefault();
        //                if (labour != null)
        //                {
        //                    emp.LabourProcesses.Add(new LabourProcessDTO()
        //                    {
        //                        SubmitDate = labour.SubmDate.Value,
        //                        Discontinued = labour.Discontinued.ToLower() == "true",
        //                        DiscontinuedDate = labour.SubmDate,
        //                        ContractBeginDate = labour.SubmDate.Value,
        //                        ContractEndDate = labour.SubmDate.Value.AddDays(45).AddYears(2)
        //                    });
        //                }
        //                #endregion

        //                #region Embassy
        //                var embassy = employ.EmbassyProcesses.FirstOrDefault();
        //                if (embassy != null)
        //                {
        //                    emp.EmbassyProcesses.Add(new EmbassyProcessDTO
        //                    {
        //                        SubmitDate = embassy.SubmDate.Value,
        //                        EnjazNumber = string.IsNullOrEmpty(embassy.EnjazNum) || embassy.EnjazNum.Trim().Replace(" ", "").Length != 9 ? "E00000000" : embassy.EnjazNum.Trim().Replace(" ", ""),
        //                        Stammped = embassy.Stamped.ToLower() == "true",
        //                    });
        //                }
        //                #endregion

        //                #region Flight
        //                var flight = employ.FlightProcesses.FirstOrDefault();
        //                if (flight != null)
        //                {
        //                    var amt = (decimal)0.0;
        //                    if (flight.TicketAmount != null)
        //                        amt = (decimal)flight.TicketAmount;
        //                    var afterStatus = AfterFlightStatusTypes.OnGoodCondition;
        //                    if (!string.IsNullOrEmpty(flight.AfterFlightStatus))
        //                    {
        //                        switch (flight.AfterFlightStatus)
        //                        {
        //                            case "የጠፋች":
        //                                emp.CurrentStatus = ProcessStatusTypes.Lost;
        //                                afterStatus = AfterFlightStatusTypes.Lost;
        //                                break;
        //                            case "የተመለሰች":
        //                                emp.CurrentStatus = ProcessStatusTypes.Returned;
        //                                afterStatus = AfterFlightStatusTypes.Returned;
        //                                break;
        //                        }
        //                    }
        //                    emp.FlightProcesses.Add(new FlightProcessDTO
        //                    {
        //                        SubmitDate = flight.HasGoneDate.Value,
        //                        Departured = flight.HasGone.ToLower() == "true",
        //                        TicketCity = string.IsNullOrEmpty(flight.TicketCity) ? "RIYADH" : flight.TicketCity,
        //                        TicketNumber = flight.TicketNumber.Replace(" ", ""),
        //                        AfterFlightStatus = afterStatus,
        //                        AfterFlightStatusDate = flight.AfterFlightStatusDate,
        //                        TicketAmount = amt
        //                    });

        //                }
        //                #endregion

        //                #endregion

        //                _unitOfWork.Repository<EmployeeDTO>().Insert(emp);
        //            }

        //            catch
        //            {
        //            }
        //        }
        //        try
        //        {
        //            _unitOfWork.Commit();
        //        }
        //        catch (DbEntityValidationException exception)
        //        {
        //            foreach (var eve in exception.EntityValidationErrors)
        //            {
        //                /*Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //                foreach (var ve in eve.ValidationErrors)
        //                {
        //                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
        //                        ve.PropertyName, ve.ErrorMessage);
        //                }*/
        //            }
        //            throw;
        //        }
        //    }
        //    catch //(Exception ex)
        //    {
        //    }
        //}

        #endregion
    }
}
