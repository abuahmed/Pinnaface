using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum LanguageExperience
    {
        [Description("No")]
        Poor = 0,
        [Description("Fair")]
        Fair = 1,
        //[Description("Good")]
        //Good = 2,
        [Description("Fluent")]
        Fluent = 2
    }
}