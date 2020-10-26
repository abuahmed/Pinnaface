using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum LabourListTypes
    {
        [Description("With No Introduction")]
        WithNoIntroduction = 0,
        [Description("With Introduction")]
        WithIntroduction = 1
    }
    public enum CoverLetterTypes
    {
        [Description("With No CC")]
        Format1 = 0,
        [Description("With CC")]
        Format2 = 1
    }
    public enum CvHeaderFormats
    {
        [Description("Agency Header")]
        Agency = 0,
        [Description("Agent Header")]
        Agent = 1
    }
    public enum DocumentOrientationTypes
    {
        Portrait = 0,
        Landscape = 1
    }

    public enum EmbassyApplicationTypes
    {
        [Description("In Arabic")]
        SponsorNameOnTopArabic = 0,
        [Description("In English")]
        SponsorNameOnTop = 1,
    }
    public enum EmbassyApplicationFormats
    {
        [Description("Format 1")]
        Format1 = 0,
        [Description("Format 2")]
        Format2 = 1,
    }
    public enum LabourApplicationContractTypes
    {
        [Description("Don't Show Contract Duration on Application")]
        NoContract = 0,
        [Description("Show Contract Duration on Application")]
        ShowContract = 1
    }
}
