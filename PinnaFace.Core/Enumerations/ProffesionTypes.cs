using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum ProffesionTypes
    {
        [Description("HOUSEMAID")]
        Housemaid = 0,
        [Description("HOUSE COOK")]
        Housecook = 1,
        [Description("BABY SITTER")]
        Babysitter = 2,
        [Description("DRIVER")]
        Driver = 3,
        [Description("OTHER")]
        Other = 4

    }
    public enum ProffesionTypesAmharic
    {
        [Description("የቤት ውስጥ ሠራተኛ")]
        Housemaid = 0,
        [Description("ምግብ አብሳይ")]
        Housecook = 1,
        [Description("ህጻናትን ተንከባካቢ")]
        Babysitter = 2,
        [Description("ሹፌር")]
        Driver = 3,
        [Description("ሌላ")]
        Other = 4
 
    }
}