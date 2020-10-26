using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum ComplainTypes
    {
        [Description("Didn't Call")]
        DidNotCall = 0,
        [Description("Status Not Known")]
        StatusNotKnown = 1,
        [Description("Not Comfortable with Employers")]
        NotComfortable = 2,
        [Description("Other")]
        Other = 3
    }
}