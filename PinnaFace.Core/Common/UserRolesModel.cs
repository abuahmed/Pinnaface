using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core.Common
{
    public class UserRolesModel : EntityBase
    {
        public UserRolesModel()
        {
            #region Role Visibilities
            
            Options = CommonUtility.UserHasRole((RoleTypes.Options)) ? "Visible" : "Collapsed";
            UsersMgmt = CommonUtility.UserHasRole((RoleTypes.Users)) ? "Visible" : "Collapsed";
            BackupRestore = CommonUtility.UserHasRole((RoleTypes.BackupRestore)) ? "Visible" : "Collapsed";

            AgencyDetail = CommonUtility.UserHasRole((RoleTypes.LocalAgency)) ? "Visible" : "Collapsed";
            ForeignAgentDetail = CommonUtility.UserHasRole((RoleTypes.ForeignAgentDetail)) ? "Visible" : "Collapsed";


            ViewEmployee = CommonUtility.UserHasRole(RoleTypes.ViewEmployee) ? "Visible" : "Collapsed";
            AddEmployee = CommonUtility.UserHasRole(RoleTypes.AddEmployee) ? "Visible" : "Collapsed";
            EditEmployee = CommonUtility.UserHasRole(RoleTypes.EditEmployee) ? "Visible" : "Collapsed";
            EmployeeEntry = CommonUtility.UserHasRole(RoleTypes.AddEmployeeWeb) ? "Visible" : "Collapsed";
            DeleteEmployee = CommonUtility.UserHasRole(RoleTypes.DeleteEmployee) ? "Visible" : "Collapsed";
            EmployeeReports = CommonUtility.UserHasRole(RoleTypes.EmployeeReports) ? "Visible" : "Collapsed";

            ViewVisa = CommonUtility.UserHasRole(RoleTypes.ViewVisa) ? "Visible" : "Collapsed";
            AssignVisa = CommonUtility.UserHasRole(RoleTypes.AssignVisa) ? "Visible" : "Collapsed";
            AddVisa = CommonUtility.UserHasRole(RoleTypes.AddVisa) ? "Visible" : "Collapsed";
            EditVisa = CommonUtility.UserHasRole(RoleTypes.EditVisa) ? "Visible" : "Collapsed";
            VisaEntry = CommonUtility.UserHasRole(RoleTypes.AddVisaWeb) ? "Visible" : "Collapsed";
            DeleteVisa = CommonUtility.UserHasRole(RoleTypes.DeleteVisa) ? "Visible" : "Collapsed";
            DetachVisa = CommonUtility.UserHasRole(RoleTypes.DetachVisa) ? "Visible" : "Collapsed";

            //ManageComplain = CommonUtility.UserHasRole(RoleTypes.ManageComplain) ? "Visible" : "Collapsed";
            OpenComplain = CommonUtility.UserHasRole(RoleTypes.OpenComplain) ? "Visible" : "Collapsed";
            EditComplain = CommonUtility.UserHasRole(RoleTypes.EditComplain) ? "Visible" : "Collapsed";
            ComplainEntry = CommonUtility.UserHasRole(RoleTypes.OpenComplainWeb) ? "Visible" : "Collapsed";
            CloseComplain = CommonUtility.UserHasRole(RoleTypes.CloseComplain) ? "Visible" : "Collapsed";
            ReOpenComplain = CommonUtility.UserHasRole(RoleTypes.ReOpenComplain) ? "Visible" : "Collapsed";
            ConfirmComplain = CommonUtility.UserHasRole(RoleTypes.ConfirmComplain) ? "Visible" : "Collapsed";
            //Labour
            LabourEntry = CommonUtility.UserHasRole(RoleTypes.LabourEntry) ? "Visible" : "Collapsed";
            DiscontinueProcess = CommonUtility.UserHasRole(RoleTypes.DiscontinueProcess) ? "Visible" : "Collapsed";
            DeleteLabourData = CommonUtility.UserHasRole(RoleTypes.DeleteLabourData) ? "Visible" : "Collapsed";
            LabourReports = CommonUtility.UserHasRole(RoleTypes.LabourReports) ? "Visible" : "Collapsed";
            //EMbassy
            EmbassyEntry = CommonUtility.UserHasRole(RoleTypes.EmbassyEntry) ? "Visible" : "Collapsed";
            DeleteEmbassyData = CommonUtility.UserHasRole(RoleTypes.DeleteEmbassyData) ? "Visible" : "Collapsed";
            EmbassyReports = CommonUtility.UserHasRole(RoleTypes.EmbassyReports) ? "Visible" : "Collapsed";
            EnjazEntry = CommonUtility.UserHasRole(RoleTypes.EnjazEntry) ? "Visible" : "Collapsed";
            //Flight
            FlightEntry = CommonUtility.UserHasRole(RoleTypes.FlightEntry) ? "Visible" : "Collapsed";
            DeleteFlightData = CommonUtility.UserHasRole(RoleTypes.DeleteFlightData) ? "Visible" : "Collapsed";
            FlightReports = CommonUtility.UserHasRole(RoleTypes.FlightReports) ? "Visible" : "Collapsed";
            AfterFlightEntry = CommonUtility.UserHasRole(RoleTypes.AfterFlightEntry) ? "Visible" : "Collapsed";
            
            #endregion
        }

        #region Public Properties
        
        public string Admin
        {
            get { return GetValue(() => Admin); }
            set { SetValue(() => Admin, value); }
        }
        public string AgencyDetail
        {
            get { return GetValue(() => AgencyDetail); }
            set { SetValue(() => AgencyDetail, value); }
        }
        public string ForeignAgentDetail
        {
            get { return GetValue(() => ForeignAgentDetail); }
            set { SetValue(() => ForeignAgentDetail, value); }
        }
        public string Options
        {
            get { return GetValue(() => Options); }
            set { SetValue(() => Options, value); }
        }
        public string UsersMgmt
        {
            get { return GetValue(() => UsersMgmt); }
            set { SetValue(() => UsersMgmt, value); }
        }
        public string BackupRestore
        {
            get { return GetValue(() => BackupRestore); }
            set { SetValue(() => BackupRestore, value); }
        }

        public string ViewEmployee
        {
            get { return GetValue(() => ViewEmployee); }
            set { SetValue(() => ViewEmployee, value); }
        }
        public string AddEmployee
        {
            get { return GetValue(() => AddEmployee); }
            set { SetValue(() => AddEmployee, value); }
        }
        public string EditEmployee
        {
            get { return GetValue(() => EditEmployee); }
            set { SetValue(() => EditEmployee, value); }
        }
        public string EmployeeEntry
        {
            get { return GetValue(() => EmployeeEntry); }
            set { SetValue(() => EmployeeEntry, value); }
        }
        public string DeleteEmployee
        {
            get { return GetValue(() => DeleteEmployee); }
            set { SetValue(() => DeleteEmployee, value); }
        }
        public string EmployeeReports
        {
            get { return GetValue(() => EmployeeReports); }
            set { SetValue(() => EmployeeReports, value); }
        }
        
        public string ViewVisa
        {
            get { return GetValue(() => ViewVisa); }
            set { SetValue(() => ViewVisa, value); }
        }
        public string AssignVisa
        {
            get { return GetValue(() => AssignVisa); }
            set { SetValue(() => AssignVisa, value); }
        }
        public string AddVisa
        {
            get { return GetValue(() => AddVisa); }
            set { SetValue(() => AddVisa, value); }
        }
        public string EditVisa
        {
            get { return GetValue(() => EditVisa); }
            set { SetValue(() => EditVisa, value); }
        }
        public string VisaEntry
        {
            get { return GetValue(() => VisaEntry); }
            set { SetValue(() => VisaEntry, value); }
        }
        public string DeleteVisa
        {
            get { return GetValue(() => DeleteVisa); }
            set { SetValue(() => DeleteVisa, value); }
        }
        public string DetachVisa
        {
            get { return GetValue(() => DetachVisa); }
            set { SetValue(() => DetachVisa, value); }
        }

        //public string ManageComplain
        //{
        //    get { return GetValue(() => ManageComplain); }
        //    set { SetValue(() => ManageComplain, value); }
        //}
        public string OpenComplain
        {
            get { return GetValue(() => OpenComplain); }
            set { SetValue(() => OpenComplain, value); }
        }
        public string EditComplain
        {
            get { return GetValue(() => EditComplain); }
            set { SetValue(() => EditComplain, value); }
        }
        public string ComplainEntry
        {
            get { return GetValue(() => ComplainEntry); }
            set { SetValue(() => ComplainEntry, value); }
        }
        public string CloseComplain
        {
            get { return GetValue(() => CloseComplain); }
            set { SetValue(() => CloseComplain, value); }
        }
        public string ReOpenComplain
        {
            get { return GetValue(() => ReOpenComplain); }
            set { SetValue(() => ReOpenComplain, value); }
        }
        public string ConfirmComplain
        {
            get { return GetValue(() => ConfirmComplain); }
            set { SetValue(() => ConfirmComplain, value); }
        }

        public string LabourEntry
        {
            get { return GetValue(() => LabourEntry); }
            set { SetValue(() => LabourEntry, value); }
        }
        public string DiscontinueProcess
        {
            get { return GetValue(() => DiscontinueProcess); }
            set { SetValue(() => DiscontinueProcess, value); }
        }
        public string DeleteLabourData
        {
            get { return GetValue(() => DeleteLabourData); }
            set { SetValue(() => DeleteLabourData, value); }
        }
        public string LabourReports
        {
            get { return GetValue(() => LabourReports); }
            set { SetValue(() => LabourReports, value); }
        }
        
        public string EmbassyEntry
        {
            get { return GetValue(() => EmbassyEntry); }
            set { SetValue(() => EmbassyEntry, value); }
        }
        public string DeleteEmbassyData
        {
            get { return GetValue(() => DeleteEmbassyData); }
            set { SetValue(() => DeleteEmbassyData, value); }
        }
        public string EmbassyReports
        {
            get { return GetValue(() => EmbassyReports); }
            set { SetValue(() => EmbassyReports, value); }
        }
        public string EnjazEntry
        {
            get { return GetValue(() => EnjazEntry); }
            set { SetValue(() => EnjazEntry, value); }
        }
        
        public string FlightEntry
        {
            get { return GetValue(() => FlightEntry); }
            set { SetValue(() => FlightEntry, value); }
        }
        public string DeleteFlightData
        {
            get { return GetValue(() => DeleteFlightData); }
            set { SetValue(() => DeleteFlightData, value); }
        }
        public string FlightReports
        {
            get { return GetValue(() => FlightReports); }
            set { SetValue(() => FlightReports, value); }
        }
        public string AfterFlightEntry
        {
            get { return GetValue(() => AfterFlightEntry); }
            set { SetValue(() => AfterFlightEntry, value); }
        }
        
        #endregion
    }
}