using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum VisaAssignedTypes
    {
        [Description("All")]
        All = 0,
        [Description("Not Assgned Visas")]
        NotAssgnedVisa = 1,
        [Description("Assigned Visas")]
        AssignedVisa = 2
    }
}