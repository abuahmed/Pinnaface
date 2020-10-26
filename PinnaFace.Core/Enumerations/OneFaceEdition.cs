using System.ComponentModel;

namespace PinnaFace.Core.Enumerations
{
    public enum PinnaFaceEdition
    {
        ServerEdition = 0,
        CompactEdition = 1,
        WebEdition = 2
    }

    public enum PinnaFaceFeatures
    {
        [Description("Full Feature")] Full = 0,
        [Description("Without Application")] WithoutApp = 1
    }

    public enum PhotoStorage
    {
        FileSystem = 0,
        Database = 1
    }

    public enum BuildType
    {
        LocalDev = 0,
        Dev = 1,
        Qa = 2,
        Sandbox = 3,
        Production = 4,
        //LocalDev = 4,
    }

    //public enum RuntimeType
    //{
    //    Wpf = 0,
    //    SyncEngine = 1,
    //    Web = 2
    //}
}