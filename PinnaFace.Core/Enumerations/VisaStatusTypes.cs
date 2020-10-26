using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum VisaStatusTypes
    {
        [Description("New")]
        New = 0,
        [Description("Keep Ready")]
        KeepReady = 1,
        [Description("Visa Ready")]
        VisaReady = 2,
        [Description("On Medical")]
        OnMedical = 3,
        [Description("On Foreign Affairs")]
        OnForeignAffairs = 4
    }
}