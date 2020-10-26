using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum AfterFlightStatusTypes
    {
        [Description("All")]
        All = -1,
        [Description("On Good Condition (ተጉዛ በጥሩ ሁኔታ ያለ(ች))")]
        OnGoodCondition = 0,
        [Description("Lost (ተጉዛ የጠፋ(ች))")]
        Lost = 1,
        [Description("Returned (ተጉዛ የተመለሰ(ች))")]
        Returned = 2
    }
}