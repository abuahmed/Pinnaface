using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum KeyStatus
    {
        [Description("Waiting")]
        Waiting = -1,
        [Description("Active")]
        Active = 0,
        [Description("Blocked")]
        Blocked = 1,
        [Description("Expired")]
        Expired = 2
    }
}