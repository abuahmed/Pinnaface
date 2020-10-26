using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum Numbers
    {
        [Description("0")] Zero = 0,
        [Description("1")] One = 1,
        [Description("2")] Two = 2,
        [Description("3")] Three = 3,
        [Description("4")] Four = 4,
        [Description("5")] Five = 5,
    }
    public enum TestimonialNumbers
    {
        [Description("2 Persons")]
        Two = 0,
        [Description("3 Persons")]
        Three = 1,
    }
    public enum TestimonialFormats
    {
        [Description("No Letter")]
        NoLetter = 0,
        [Description("With Letter")]
        WithLetter = 1,
        
    }
    public enum Complexion
    {
        [Description("Brown")] Brown = 0,
        [Description("Black")] Black = 1,
        [Description("Blue")] Blue = 2,
        [Description("Acceptable")]
        Acceptable = 3,
    }
    public enum ContratPeriods
    {
        [Description("1 Year")]
        One = 1,
        [Description("2 Years")]
        Two = 2,
        [Description("3 Years")]
        Three = 3,
        [Description("4 Years")]
        Four = 4,
        [Description("5 Years")]
        Five = 5,
        [Description("6 Years")]
        Six = 6,
        [Description("7 Years")]
        Seven = 7,
        [Description("8 Years")]
        Eight = 8,
        [Description("9 Years")]
        Nine = 9,
        [Description("10 Years")]
        Ten = 10,
    }
}