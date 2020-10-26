namespace PinnaFace.WPF
{
    public class ListDataItem
    {
        public string Display { get; set; }
        public int Value { get; set; }
    }

    public enum BrowserTarget
    {
        Enjazit=1,
        Musaned=2,
        UnitedInsurance=3,
        PinnaFace = 4
    }
}