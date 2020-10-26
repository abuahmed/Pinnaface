using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum RoleTypes
    {
        [Description("Adding/Editing Employees On Web")]
        AddEmployeeWeb,
        [Description("Adding/Editing Visas On Web")]
        AddVisaWeb,
        [Description("Opening Complains On Web")]
        OpenComplainWeb,
        
        [Description("Users Mgmt")]
        Users,

        [Description("Options")]
        Options,
        
        [Description("Agency Mgmt.")] LocalAgency,
        [Description("Foreign Agents Mgmt.")] ForeignAgentDetail,
        [Description("Backup Restore")]
        BackupRestore,

        [Description("Viewing Employees")] ViewEmployee,
        [Description("Adding Employees")] AddEmployee,
        [Description("Editing Employees")] EditEmployee,
        [Description("Deleting Employees")] DeleteEmployee,
        [Description("Employee Reports")] EmployeeReports,

        [Description("Viewing Visas")] ViewVisa,
        [Description("Assigning Visas")] AssignVisa,
        [Description("Adding Visas")] AddVisa,
        [Description("Editing Visas")] EditVisa,
        [Description("Deleting Visas")] DeleteVisa,
        [Description("Detaching Visas")] DetachVisa,

        [Description("Opening Complains")] OpenComplain,
        [Description("Editing Complains")] EditComplain,
        [Description("Closing Complains")] CloseComplain,
        [Description("ReOpening Complains")] ReOpenComplain,
        [Description("Confirm Complains")] ConfirmComplain,

        [Description("Labour Entry")] LabourEntry,
        [Description("Discontinue Process")] DiscontinueProcess,
        [Description("Delete Labour Data")] DeleteLabourData,
        [Description("Labour Reports")] LabourReports,

        [Description("Embassy Entry")] EmbassyEntry,
        [Description("Delete Embassy Data")] DeleteEmbassyData,
        [Description("Embassy Reports")] EmbassyReports,
        [Description("Enjaz Entry")] EnjazEntry,

        [Description("Flight Entry")] FlightEntry,
        [Description("Delete Flight Data")] DeleteFlightData,
        [Description("Flight Reports")] FlightReports,
        [Description("After Flight Entry")] AfterFlightEntry,

        [Description("Viewing Reports")] ViewReports
    }
}