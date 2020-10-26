using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {        
        #region Fields

        private UnitOfWorkServer _unitOfWorkServer;

        #endregion

        #region Constructor

        private bool _errorsFound;
        //private bool _noConnection;
        private bool _updatesFound;

        public static DateTime LastToServerSyncDate { get; set; }
        public static DateTime LastFromServerSyncDate { get; set; }

        public static DateTime LastServerSyncDate { get; set; }
        public static SettingDTO Setting { get; set; }

        public static bool ToServerSyncing { get; set; }

        public static string UpdatingText { get; set; }
        public static string UserName { get; set; }
        public static string Agency { get; set; }

        #endregion

        #region Property Methods

        public IUnitOfWork GetNewUow(IUnitOfWork uom)
        {
            uom.Dispose();
            _unitOfWorkServer = new UnitOfWorkServer(new ServerDbContextFactory().Create());
            return _unitOfWorkServer;
        }

        public IUnitOfWork GetNewUow2(IUnitOfWork uom)
        {
            uom.Dispose();
            return new UnitOfWork(new DbContextFactory().Create());
        }

        #endregion

        public void Sync() //(object sender, DoWorkEventArgs e)
        {
            Singleton.SystemVersionDate = DbCommandUtil.GetCurrentDatabaseVersion();
            
            var productionDbVersion = Convert.ToInt32(DbCommandUtil.GetCurrentDbVersion());
            var systemVersionDate = Convert.ToInt32(Singleton.SystemVersionDate);

            if (systemVersionDate<productionDbVersion)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "ValidateProduct",
                  "Higher Database Version", UserName, Agency);
                return;
            }
            
            LogUtil.LogError(ErrorSeverity.Critical, "Sync", "Started", UserName, Agency);

            AgencyWithAgentsUtility.InsertAgencyNamesonAddressesandAttachments();
            if (!AgencyWithAgentsUtility.InsertAgencyWithAgents(UserName, Agency)) return;
            if (!AgencyWithAgentsUtility.InsertUserWithAgencyWithAgents(UserName, Agency)) return;


            IUnitOfWork sourceUnitOfWork = new UnitOfWork(
                new DbContextFactory().Create());
            IUnitOfWork destinationUnitOfWork = new UnitOfWorkServer(
                new ServerDbContextFactory().Create());

            var agency= sourceUnitOfWork.Repository<AgencyDTO>()
                .Query()
                .Get()
                .FirstOrDefault();

            //Setting = sourceUnitOfWork.Repository<SettingDTO>()
            //    .Query()
            //    .Get(1)
            //    .FirstOrDefault();

            try
            {
                //if (Setting != null)
                //{
                //    LastFromServerSyncDate = Setting.LastFromServerSyncDate != null
                //        ? (DateTime) Setting.LastFromServerSyncDate
                //        : DbCommandUtil.GetCurrentSqlDate(false).AddYears(-1);//If it is for first time collect all last one year data
                //    LastToServerSyncDate = Setting.LastToServerSyncDate != null
                //        ? (DateTime) Setting.LastToServerSyncDate
                //        : DbCommandUtil.GetCurrentSqlDate(false).AddYears(-1);
                //}
                //else
                //{
                    LastFromServerSyncDate = DbCommandUtil.GetCurrentSqlDate(false).AddYears(-10);
                    LastToServerSyncDate = DbCommandUtil.GetCurrentSqlDate(false).AddYears(-10);
                //}
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Sync DbCommandUtil.GetCurrentSqlDate(true)",
                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
            }

            LogUtil.LogError(ErrorSeverity.Info, "Sync To Server",
                "Started", UserName, Agency);

            #region Sync To Server

            ToServerSyncing = true;
            LastServerSyncDate = LastToServerSyncDate; //.AddMinutes(-10);//-10 Minutes should be syncronized with MonitorTimerElapsed
            
            try
            {
                if (!SyncUsers2(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncAgencies2(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                Singleton.Agency = destinationUnitOfWork.Repository<AgencyDTO>()
                       .Query().Filter(a => a.RowGuid == agency.RowGuid)
                       .Get()
                       .FirstOrDefault();

                if (!SyncAddresses(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncAttachments(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncRequiredDocuments(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncVisaSponsors(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncVisaConditions(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncEducation(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncExperiences(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncHawala(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncInsurance(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncLabour(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncEmbassy(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncFlight(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncAgencies(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncForeignAgents(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncAgencyWithAgents(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncSettings(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncProductActivations(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncVisas(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncEmployees(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncRelatives(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncComplains(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncComplainRemarks(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncEmployees2(sourceUnitOfWork, destinationUnitOfWork, false)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!SyncUsers(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncMemberships(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncRoles(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncUsersInRoles(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!SyncUserWithAgencyWithAgentDTO(sourceUnitOfWork, destinationUnitOfWork)) return;

                //Sync Setting
                try
                {

                    if (!_errorsFound && _updatesFound)
                    {
                        var setUnitOfWork = new UnitOfWork(new DbContextFactory().Create());
                        SettingDTO set = setUnitOfWork.Repository<SettingDTO>().Query().Get(1).FirstOrDefault();

                        if (set != null)
                            set.LastToServerSyncDate = DbCommandUtil.GetCurrentSqlDate(false); //.AddHours(-1);

                        setUnitOfWork.Repository<SettingDTO>().SimpleUpdate(set);
                        setUnitOfWork.Commit();
                        setUnitOfWork.Dispose();
                    }
                    else
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "Sync To Server",
                            "No Updates Found", UserName, Agency);
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "Update Setting.LastToServerSyncDate",
                        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Sync General Method",
                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
            }
            finally
            {
                try
                {
                    sourceUnitOfWork.Dispose();
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "Sync General sourceUnitOfWork.Dispose",
                        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                }
            }

            #endregion

            LogUtil.LogError(ErrorSeverity.Info, "Sync To Server", "Completed", UserName, Agency);

            #region Dispose UoW

            try
            {
                sourceUnitOfWork.Dispose();
                destinationUnitOfWork.Dispose();
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Dispose Unit of Work Method",
                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
            }

            #endregion
            
            LogUtil.LogError(ErrorSeverity.Info, "Sync From Server", "Started", UserName, Agency);

            #region Sync From Server

            ToServerSyncing = false;
            Singleton.Agency = agency;
            try
            {
                LastServerSyncDate = LastFromServerSyncDate; //.AddMinutes(-10);
                //-10 Minutes should be syncronized with MonitorTimerElapsed


                sourceUnitOfWork = new UnitOfWorkServer(new ServerDbContextFactory().Create());
                destinationUnitOfWork = new UnitOfWork(new DbContextFactory().Create());

                if (!SyncSettings(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);
                if (!SyncProductActivations(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);

                if (!SyncAddresses(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);

                if (!SyncVisaSponsors(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);
                if (!SyncVisaConditions(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);

                if (!SyncVisas(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);

                if (!SyncEmployees(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);

                if (!SyncComplains(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);
                if (!SyncComplainRemarks(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);
                if (!SyncEmployees2(sourceUnitOfWork, destinationUnitOfWork, true)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);

                if (!SyncRoles(sourceUnitOfWork, destinationUnitOfWork)) return;
                destinationUnitOfWork = GetNewUow2(destinationUnitOfWork);
                if (!SyncUsersInRoles(sourceUnitOfWork, destinationUnitOfWork)) return;
                ////destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                //Sync Setting
                try
                {
                    if (!_errorsFound && _updatesFound)
                    {
                        var setUnitOfWork = new UnitOfWork(new DbContextFactory().Create());
                        SettingDTO set = setUnitOfWork.Repository<SettingDTO>().Query().Get(1).FirstOrDefault();

                        if (set != null)
                            set.LastFromServerSyncDate = DbCommandUtil.GetCurrentSqlDate(false); //.AddHours(-1);

                        setUnitOfWork.Repository<SettingDTO>().SimpleUpdate(set);
                        setUnitOfWork.Commit();
                        setUnitOfWork.Dispose();
                    }
                    else
                        LogUtil.LogError(ErrorSeverity.Critical, "Sync From Server",
                            "No Updates Found", UserName, Agency);
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "Update Setting.LastFromServerSyncDate",
                        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Sync From Server General Method",
                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
            }
            finally
            {
                try
                {
                    sourceUnitOfWork.Dispose();
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "Sync From Server General sourceUnitOfWork.Dispose()",
                        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                }
            }

            #endregion

            LogUtil.LogError(ErrorSeverity.Info, "Sync From Server",
                "Completed", UserName, Agency);
        }

        private static int? GetDestCreatedModifiedByUserId(int? createdModifiedByUserId, IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            try
            {
                if (createdModifiedByUserId != null)
                {
                    var sourceUser = sourceUnitOfWork.UserRepository<UserDTO>()
                        .Query().Filter(u => u.UserId == createdModifiedByUserId)
                        .Get(1).FirstOrDefault();

                    if (sourceUser != null)
                    {
                        var destUser = destinationUnitOfWork.UserRepository<UserDTO>()
                            .Query().Filter(u => u.RowGuid == sourceUser.RowGuid)
                            .Get(1).FirstOrDefault();
                        if (destUser != null)
                            return destUser.UserId;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "GetDestCreatedModifiedByUserId",
                                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
            }

            return 1;
        }

        public bool CheckInternetConnection()
        {
            try
            {
                //Check for internet connection

                if (Singleton.BuildType == BuildType.LocalDev)
                    return true;

                var stream = new WebClient().OpenWrite("http://www.google.com");
            }
            catch
            {
                //_noConnection = true;
                return false;
            }
            return true;
        }

        public bool ValidateProduct()
        {
            //var dt = DbCommandUtil.GetCurrentSqlDate(true);
            //var loc = dt.ToLocalTime();

            var activationModel = Singleton.Edition == PinnaFaceEdition.CompactEdition
                ? DbCommandUtil.ValidateProductSqlCe()
                : DbCommandUtil.ValidateProductSql();

            if (activationModel != null
                && activationModel.DatabaseVersionDate != 0
                && activationModel.MaximumSystemVersion != 0)
            {
                if (Singleton.SystemVersionDate < activationModel.DatabaseVersionDate)
                    return false;
            }

            return true ;
        }
    }
}


//Singleton.UseServerDateTime = true; //TO Handle Datetime.Now from serverornot
//Singleton.BuildType = BuildType.Dev;

/*Uncomment below when it is required to run Sync from PinnaFace.SyncEngine*/

////For Server Edition
//Singleton.Edition = PinnaFaceEdition.ServerEdition;
//Singleton.SqlceFileName = "PinnaFaceDbProd";//"PinnaFaceDb4"; //
//Singleton.PhotoStorage = PhotoStorage.Database;

//////For Compact Edition
////Singleton.Edition = PinnaFaceEdition.CompactEdition;
////Singleton.SqlceFileName = PathUtil.GetDatabasePath();
////Singleton.PhotoStorage = PhotoStorage.FileSystem;

//try
//{
//    UserName = Singleton.User.UserName;
//    Agency = new LocalAgencyService(true).GetLocalAgency().AgencyName;
//}
//catch
//{
//    UserName = "Default User";
//    Agency = "Default Agency";
//}

//private int _monitorTimerDelay = 15000; //15 second
//private Timer _monitorTimer;

////It will be greate to give each sync method its own class file for better management
//public class SyncParameters
//{
//    public bool ErrorsFound { get; set; }
//    public bool UpdatesFound { get; set; }
//    public string UserName { get; set; }
//    public string Agency { get; set; }
//    public DateTime LastServerSyncDate { get; set; }
//}

//private void Initialize()
//{
//    try
//    {
//        LogUtil.LogError(ErrorSeverity.Info, "Initialize", "Service is started at " + DateTime.Now, "", "");
//        _monitorTimer = new Timer(_monitorTimerDelay);
//        _monitorTimer.Elapsed += OnMonitorTimerElapsed;
//    }
//    catch (Exception ex)
//    {
//        LogUtil.LogError(ErrorSeverity.Critical, "Initialize Sync",
//            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
//    }
//}

//public void Start()
//{
//    //try
//    //{
//    //    UserName = Singleton.User.UserName;
//    //    Agency = Singleton.Agency.AgencyName;
//    //}
//    //catch
//    //{
//    //    UserName = "Default User";
//    //    Agency = "Default Agency";
//    //}

//    //IUnitOfWork sourceUnitOfWork = new UnitOfWork(new DbContextFactory().Create());
//    //try
//    //{
//    //    LogUtil.LogError(ErrorSeverity.Info, "Start", "Service is started at " + DateTime.Now, "", "");
//    //    Setting = sourceUnitOfWork.Repository<SettingDTO>().Query().Get(1).FirstOrDefault();
//    //    if (Setting != null && Setting.StartSync)
//    //    {
//    //        _monitorTimerDelay = Setting.SyncDuration*10000;//60000; //1 minute is equivalent to 60000
//    //        Initialize();
//    //        _monitorTimer.Enabled = true;
//    //    }
//    //}
//    //catch (Exception ex)
//    //{
//    //    LogUtil.LogError(ErrorSeverity.Critical, "Start Sync",
//    //        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
//    //}
//    //finally
//    //{
//    //    sourceUnitOfWork.Dispose();
//    //}
//}

//private void OnMonitorTimerElapsed(object source, ElapsedEventArgs e)
//{
//    LogUtil.LogError(ErrorSeverity.Info, "OnMonitorTimerElapsed", "Service is started at " + DateTime.Now, "", "");
//    _monitorTimer.Enabled = false;
//    _updatesFound = false;
//    _noConnection = false;
//    UpdatingText = "Searching for updates...";
//    try
//    {
//        var worker = new BackgroundWorker();
//        worker.DoWork += Sync;
//        worker.RunWorkerCompleted += worker_RunWorkerCompleted;
//        worker.RunWorkerAsync();
//    }
//    catch
//    {
//        UpdatingText = "Problem updating...";
//    }

//    //Sync();
//    //_monitorTimer.Enabled = true;
//}

//private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
//{
//    _monitorTimer.Enabled = true;
//    if (_noConnection)
//        UpdatingText = "There is no Internet connection...";
//    else
//    {
//        UpdatingText = _updatesFound
//            ? "There exists new updates, refresh to see the updates"
//            : "No updates found...";
//    }
//    //else if (_refreshed)
//    //{
//    //    UpdatingText = _noConnection ? "There is no Internet connection..." : "No updates found...";
//    //}
//}