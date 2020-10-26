using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum MaritalStatusTypes
    {
        [Description("Single")]
        Single = 1,
        [Description("Married")]
        Married = 2,
        [Description("Divorced")]
        Divorced = 3,
        [Description("Widow")]
        Widow = 4,
        [Description("Separated")]
        Separated = 5
    }
}