using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;

namespace PinnaFace.Core
{
    public class Singleton
    {
        private static Singleton _instance;
        private Singleton() { }

        public static Singleton Instance
        {
            get { return _instance ?? (_instance = new Singleton()); }
        }

        public static ProductActivationDTO ProductActivation { get; set; }

        public static UserDTO User { get; set; }

        public static PinnaFaceEdition Edition { get; set; }

        public static PhotoStorage PhotoStorage { get; set; }

        public static BuildType BuildType { get; set; }

        public static bool UseServerDateTime { get; set; }

        public static string SqlceFileName { get; set; }

        public static bool SeedDefaults { get; set; }

        public static string ConnectionStringName { get; set; }

        public static string ProviderName { get; set; }

        public static int SystemVersionDate { get; set; }

        public static UserRolesModel UserRoles { get; set; }

        public static AgencyDTO Agency { get; set; }

        public static SettingDTO Setting { get; set; }
    }
}
