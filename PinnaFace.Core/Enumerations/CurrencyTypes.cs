using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum CurrencyTypes
    {
        [Description("S/R")]
        SaudiArabia,
        [Description("AED")]
        UAE,
        [Description("KD")]
        Kuwait,
        [Description("Qatar Riyal")]
        Qatar,
        [Description("Lebanon Riyal")]
        Lebanon,
        [Description("Yemen Riyal")]
        Yemen,
        [Description("Bahrain Riyal")]
        Bahrain,
        [Description("Birr")]
        Ethiopia,
    }

    public enum CurrencyTypesAmharic
    {
        [Description("ሳውዲ ሪያል")]
        ሳውዲአረቢያ,
        [Description("ዱባይ ዲርሀም")]
        ዱባይ,
        [Description("ኩዌት ዲርሀም")]
        ኩዌት,
        [Description("ቋታር ሪያል")]
        ቋታር,
        [Description("ሊባኖስ ሪያል")]
        ሊባኖስ,
        [Description("የመን ሪያል")]
        የመን,
        [Description("ባህሬን ሪያል")]
        ባህሬን,
        [Description("ኢትዮጲያ ብር")]
        ኢትዮጲያ,
    }
}