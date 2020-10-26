using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum ProcessStatusTypes
    {
        [Description("All")]
        All = -1,

        [Description("New")]
        New = 0,
        [Description("Visa Assigned")]
        VisaAssigned = 11,

        [Description("On Process")]
        OnProcess = 1,

        [Description("On Labour")]
        LabourProcess = 2,
        [Description("On Embassy")]
        EmbassyProcess = 3,
        [Description("Stammped")]//& Flight Processing
        FlightProcess = 4,
        [Description("Flight Booked")]// or Departured
        BookedDepartured = 44,

        [Description("Discontinued")]
        Discontinued = 5,
        [Description("Visa Canceled")]
        Canceled = 55,

        [Description("Working Well")]//Arrived & On Good Condition
        OnGoodCondition = 6,

        [Description("Lost")]
        Lost = 7,
        [Description("Returned")]
        Returned = 8,

        [Description("Has Complain")]
        WithComplain = 9
    }
    public enum ProcessStatusTypesForDisplay
    {
        [Description("All")]
        All = 0,

        [Description("New")]
        New = 1,
        [Description("Visa Assigned")]
        VisaAssigned = 2,

        [Description("On Process")]
        OnProcess = 3,

        [Description("On Labour Process")]
        LabourProcess = 4,
        [Description("On Embassy Process")]
        EmbassyProcess = 5,
        [Description("Stammped")]//& Flight Processing
        FlightProcess = 6,
        [Description("Flight Booked")]//or Departured
        BookedDepartured = 7,

        [Description("Working Well")]//Arrived & On Good Condition
        OnGoodCondition = 8,

        [Description("Discontinued")]
        Discontinued = 9,
        [Description("Visa Canceled")]
        Canceled = 10,

        [Description("Lost")]
        Lost = 11,
        [Description("Returned")]
        Returned = 12,

        [Description("Employees with Complain")]
        WithComplain = 13
    }
    public enum ProcessStatusTypesAmharic
    {
        [Description("All")]
        All = 0,
        [Description("On Process (ፕሮሰስ ላይ ያለ(ች))")]
        OnProcess = 1,
        [Description("On Labour (ማህበራዊ ፕሮሰስ ላይ ያለ(ች))")]
        LabourProcess = 2,
        [Description("On Embassy (ኢምባሲ ፕሮሰስ ላይ ያለ(ች))")]
        EmbassyProcess = 3,
        [Description("On Flight (በረራ ፕሮሰስ ላይ ያለ(ች))")]
        FlightProcess = 4,
        [Description("Discontinued (ከተማረ(ች)ና ውል ከፀደቀ በኋላ ያቋረጠ(ች))")]
        Discontinued = 5,

        [Description("On Good Condition (ተጉዞ(ዛ) በጥሩ ሁኔታ ያለ(ች))")]
        OnGoodCondition = 6,
        [Description("Lost (ተጉዞ(ዛ) የጠፋ(ች))")]
        Lost = 7,
        [Description("Returned (ተጉዞ(ዛ) የተመለሰ(ች))")]
        Returned = 8,
        [Description("Has Complain (ቅሬታ ያለበት(ባት))")]
        WithComplain = 9
    }
}