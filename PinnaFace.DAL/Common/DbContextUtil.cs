using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL.Interfaces;
using PinnaFace.DAL.Mappings;

namespace PinnaFace.DAL
{
    public static class DbContextUtil
    {
        public static DbModelBuilder OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProductActivationMap());

            modelBuilder.Configurations.Add(new SettingMap());

            modelBuilder.Configurations.Add(new ListMap());

            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new RequiredDocumentsMap());
            modelBuilder.Configurations.Add(new AttachmentMap());

            modelBuilder.Configurations.Add(new AgencyMap());
            modelBuilder.Configurations.Add(new AgencyAgentsMap());
            modelBuilder.Configurations.Add(new UserAgencyAgentsMap());
            modelBuilder.Configurations.Add(new AgentMap());

            modelBuilder.Configurations.Add(new VisaMap());
            modelBuilder.Configurations.Add(new VisaConditionMap());
            modelBuilder.Configurations.Add(new VisaSponsorMap());

            modelBuilder.Configurations.Add(new EmployeeMap());
            modelBuilder.Configurations.Add(new EmployeeExperienceMap());
            modelBuilder.Configurations.Add(new EmployeeRelativeMap());
            modelBuilder.Configurations.Add(new EmployeeHawalaMap());
            modelBuilder.Configurations.Add(new EmployeeEducationMap());

            modelBuilder.Configurations.Add(new ComplainMap());
            modelBuilder.Configurations.Add(new ComplainRemarkMap());

            modelBuilder.Configurations.Add(new InsuranceProcessMap());
            modelBuilder.Configurations.Add(new LabourProcessMap());
            modelBuilder.Configurations.Add(new EmbassyProcessMap());
            modelBuilder.Configurations.Add(new FlightProcessMap());

            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new MembershipMap());
            modelBuilder.Configurations.Add(new RoleMap());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            return modelBuilder;
        }

        public static IDbContext Seed(IDbContext context)
        {
            var list = context.Set<ListDTO>().ToList();
            
            if (!list.Any())
            {
                #region List Seeds

                #region Professions
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.Profession, DisplayName = "HOUSE MAID" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.Profession, DisplayName = "DRIVER" });

                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.ProfessionAmharic, DisplayName = "የቤት ውስጥ ሠራተኛ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.ProfessionAmharic, DisplayName = "ሹፌር" });

                #endregion

                #region Cities

                #region English
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "RIYADH" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "MAKKAH" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "MADINA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "ABHA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "GASSIM" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "QASSIM" });

                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "JEDDAH" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "DAMMAM" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "JIZAN" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "TAYIF" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "NAJRAN" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "TABOUK" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.City, DisplayName = "DUBA" });
                #endregion

                #region Amharic
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ሪያድ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "መካ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "መዲና" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "አብሃ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ጋሲም" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ቃሲም" });

                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ጅዳ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ደማም" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ጂዛን" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ጣይፍ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ነጅራን" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ተቡክ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CityAmharic, DisplayName = "ዱባ" });
                #endregion

                #endregion

                #region Countries

                #region Amharic
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CountryAmharic, DisplayName = "ሳውዲ አረቢያ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CountryAmharic, DisplayName = "ኩዌት" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CountryAmharic, DisplayName = "ዱባይ" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.CountryAmharic, DisplayName = "ቌታር" });
                #endregion

                #endregion

                #region LocalCity
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "ADDIS ABABA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "ADAMA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "DEBREZEIT" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "GONDAR" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "BAHAR DAR" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "DESSE" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "MEKELE" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "ARSI" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "WELLEGA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "AMBO" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "WOLLO" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "D/WOLLO" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "KOMBOLCHA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "SILTE" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "GURAGE" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "BUTAJIRA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.LocalCity, DisplayName = "WOLKITE" });
                #endregion

                #region Subcity
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "ARADA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "ADDIS KETEMA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "KOLFE KERANIYO" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "BOLE" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "YEKA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "NIFAS SILK" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "KIRKOS" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "AKAKI KALITI" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "LIDETA" });
                context.Set<ListDTO>().Add(new ListDTO { Type = ListTypes.SubCity, DisplayName = "GULELE" });
                #endregion

                #endregion
            }
            return context;
        }

        public static IDbContext GetDbContextInstance()
        {
            switch (Singleton.Edition)
            {
                case PinnaFaceEdition.CompactEdition:
                    return new DbContextFactory().Create();
                case PinnaFaceEdition.ServerEdition:
                    return new DbContextFactory().Create();
                case PinnaFaceEdition.WebEdition:
                    return new ServerDbContextFactory().Create();
            }
            return new DbContextFactory().Create();
        }

        //public static IDbContext GetDbContextInstanceFake()
        //{
        //    switch (Singleton.Edition)
        //    {
        //        case PinnaFaceEdition.CompactEdition:
        //            return new DbContextFactoryFake().Create();
        //        case PinnaFaceEdition.ServerEdition:
        //            return new DbContextFactoryFake().Create();
        //    }
        //    return new DbContextFactoryFake().Create();
        //}
    }
}