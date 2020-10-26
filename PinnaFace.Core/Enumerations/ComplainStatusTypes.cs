using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum ComplainStatusTypes
    {
        [Description("Opened")]
        Opened = 0,
        [Description("On Process")]
        OnProcess = 1,
        [Description("Closed")]
        Closed = 2,
        [Description("Confirmed")]
        Confirmed = 3,
        [Description("Re-Opened")]
        ReOpened = 4
    }
}