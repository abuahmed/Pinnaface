using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    //public enum QualificationTypes
    //{
    //    Certificate = 0,
    //    Diploma = 1,
    //    Degree = 2,
    //    Others = 3
    //}

    public enum QualificationTypes
    {
        [Description("Bachelor")]
        Bachelor = 0,
        [Description("Diploma")]
        Diploma = 1,
        [Description("Doctrate")]
        Doctrate = 2,
        [Description("High School")]
        High = 3,
        [Description("Preparatory School")]
        Preparatory = 4,
        [Description("Illiterate")]
        Illiterate = 5,
        [Description("Master")]
        Master = 6,
        [Description("Primary School")]
        Primary = 7,
        [Description("Driving Cource")]
        Driving = 8,

    }
    public enum LevelOfQualificationTypes
    {
        [Description("Elementary")]
        Elementary = 0,
        [Description("Junior Secondary")]
        JuniorSecondary = 1,
        [Description("Secondary Level")]
        SecondaryLevel = 2,
        [Description("Secondary Complete")]
        SecondaryComplete = 3,
        [Description("Vocational Level")]
        VocationalLevel = 4,
        [Description("Vocational Complete")]
        VocationalComplete = 5,
        [Description("College Level")]
        CollegeLevel = 6,
        [Description("College Complete")]
        CollegeComplete = 7,
        [Description("Post Graduate Level")]
        PostGraduateLevel = 8,
        [Description("Post Graduate")]
        PostGraduate = 9,
        [Description("Non Formal Education")]
        NonFormalEducation = 10,
        [Description("Others")]
        Others = 11,
    }
   
    public enum AwardTypes
    {
        [Description("Certificate")]
        Certificate = 0,
        [Description("Diploma")]
        Diploma = 1,
        [Description("BA/BSC")]
        BABSC = 2,
        [Description("MA/MSC")]
        MAMSC = 3,
        [Description("Phd")]
        Phd = 4,
        [Description("Others")]
        Others = 5
    }
}