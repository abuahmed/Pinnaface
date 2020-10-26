using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum ReportTypes
    {
        [Description("Monthly Labour Report")]
        LabourMonthly = 0,
        [Description("Lost Employees Report")]
        LabourLost = 1,
        [Description("Returned Employees Report")]
        LabourReturned = 2,
        [Description("Discontinued Employees Report")]
        LabourDiscontinued = 3,
        [Description("Contract Completed Report")]
        LabourContractEnd = 4,

        [Description("Monthly Embassy Report")]
        EmbassyMonthly = 5,
        [Description("Ticket List Report")]
        TicketList = 6,
        [Description("Ticket Amount List Report")]
        TicketAmountList = 7,
        [Description("Summary Report")]
        SummaryList = 8
    }

    public enum ReportDocumentTypes
    {
        [Description("Labour Letter")]
        LabourLetter = 0,
        [Description("Aggreement Front")]
        AggreementFront = 1,
        [Description("Aggreement Back")]
        AggreementBack = 1,
    }
}