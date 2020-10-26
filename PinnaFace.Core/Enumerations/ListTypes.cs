using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum ListTypes
    {
        [Description("Cities")]
        City = 0,
        [Description("Countries")]
        Country = 1,
        [Description("Cities(Amharic)")]
        CityAmharic = 2,
        [Description("Countries(Amharic)")]
        CountryAmharic = 3,
        [Description("Local Cities")]
        LocalCity = 4,
        [Description("Sub Cities")]
        SubCity = 5,
        [Description("Professions")]
        Profession = 6,
        [Description("Professions(Amharic)")]
        ProfessionAmharic = 7,
        [Description("Local Cities(Amharic)")]
        LocalCityAmharic = 8,
    }
}
