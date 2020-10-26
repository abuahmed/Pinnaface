using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;

namespace PinnaFace.Core
{
    public static class CommonUtility
    {
        public static IList<RoleDTO> GetRolesList()
        {
            return Enum.GetNames(typeof (RoleTypes))
                .Select(name => (RoleTypes) Enum.Parse(typeof (RoleTypes), name))
                .Select(GetRoleDTO).ToList();
        }

        public static RoleDTO GetRoleDTO(RoleTypes roleType)
        {
            var role = new RoleDTO
            {
                RoleName = roleType.ToString(),
                RoleDescription = EnumUtil.GetEnumDesc(roleType),
                RoleDescriptionShort = roleType.ToString()
            };
            return role;
        }

        public static bool UserHasRole(RoleTypes role)
        {
            return Singleton.User.Roles.Any(u => u.Role.RoleName == role.ToString());
        }

        public static IList<string> GetRolesGuidList()
        {
            IList<String> guidList = new List<string>();
            int sno = 11;
            while (sno < 49)
            {
                guidList.Add("94B5ACBE-17A9-426A-94A2-D2D4277A3A" + sno.ToString());
                sno++;
            }

            return guidList;
        }

        public static VisaDTO GetNewVisaDTO(int selectedAgentId)
        {
            try
            {
                var agencyId = Singleton.Agency.Id;
                return new VisaDTO
                {
                    VisaQuantity = 1,
                    ForeignAgentId = selectedAgentId,
                    AgencyId = agencyId,
                    Sponsor = new VisaSponsorDTO
                    {
                        AgencyId = agencyId,
                        Address = new AddressDTO
                        {
                            AgencyId = agencyId,
                            AddressType = AddressTypes.Foreign,
                            Country = CountryList.SaudiArabia,
                            CountryAmharic = CountryListAmharic.ሳውዲአረቢያ,
                            City = EnumUtil.GetEnumDesc(CityList.Riyadh),
                            CityAmharic = EnumUtil.GetEnumDesc(CityListAmharic.ሪያድ)
                        }
                    },
                    Condition = new VisaConditionDTO
                    {
                        AgencyId = agencyId,
                        Age = AgeCategory.Bet1825,
                        Religion = ReligionTypes.Muslim,
                        Salary = 1000,
                        CurrencyType = CurrencyTypes.SaudiArabia,
                        Profession = ProffesionTypes.Housemaid,
                        ProfessionAmharic = ProffesionTypesAmharic.Housemaid,
                        ContratPeriod = ContratPeriods.Two,
                        FirstTime = true,
                        GoodLooking = true
                    },
                };
            }
            catch
            {
                LogUtil.LogError(ErrorSeverity.Fatal, "CommonUtility.GetNewVisaDTO", "Problem while Getting New Visa",
                    "", "");
            }
            return null;
        }

        public static EmployeeDTO GetNewEmployeeDTO()
        {
            try
            {
                var agencyId = Singleton.Agency.Id;
                return new EmployeeDTO
                {
                    AgencyId = agencyId,
                    PassportIssueDate = DateTime.Now,
                    PassportExpiryDate = DateTime.Now.AddYears(5),
                    PlaceOfIssue = EnumUtil.GetEnumDesc(CityList.AddisAbeba),
                    DateOfBirth = DateTime.Now,
                    MaritalStatus = MaritalStatusTypes.Single,
                    Sex = Sex.Female,
                    Religion = ReligionTypes.Muslim,
                    CurrentStatus = ProcessStatusTypes.New,
                    CurrentStatusDate = DateTime.Now,
                    //AppliedCountry = CountryList.SaudiArabia,
                    AppliedProfession = ProffesionTypes.Housemaid,
                    NumberOfChildren = Numbers.Zero,
                    Complexion = Complexion.Brown,
                    Address = new AddressDTO
                    {
                        AgencyId = agencyId,
                        AddressType = AddressTypes.Local,
                        Country = CountryList.Ethiopia,
                        City = EnumUtil.GetEnumDesc(CityList.AddisAbeba)
                    },
                    ContactPerson = new EmployeeRelativeDTO
                    {
                        AgencyId = agencyId,
                        FullName = "-",
                        Type = RelativeTypes.ContactPerson,
                        Sex = Sex.Male,
                        //Employee = SelectedEmployee,
                        Address = new AddressDTO
                        {
                            AgencyId = agencyId,
                            AddressType = AddressTypes.Local,
                            Country = CountryList.Ethiopia,
                            City = EnumUtil.GetEnumDesc(CityList.AddisAbeba)
                        }
                    },
                    Education = new EmployeeEducationDTO
                    {
                        AgencyId = agencyId,
                        ArabicLanguage = LanguageExperience.Poor,
                        EnglishLanguage = LanguageExperience.Poor,
                        QualificationType = QualificationTypes.Primary,
                        EducateQG = "8th"
                    },
                    Experience = new EmployeeExperienceDTO
                    {
                        AgencyId = agencyId,
                        HaveWorkExperience = false,
                        ExperiencePeriod = ContratPeriods.Two,
                        ExperiencePeriodInCountry = ContratPeriods.Two,
                        HardWorker = true,
                        BabySitting = true,
                        Nanny = true,
                        Washing = true,
                        WashingDishes = true,
                        Cleaning = true,
                        Cooking = true
                    },
                    Hawala = new EmployeeHawalaDTO
                    {
                        AgencyId = agencyId,
                        BankName = BankList.Cbe,
                        SwiftCode = SwiftCodeList.Cbe,
                        AccountNumber = "0000000000000"
                    },
                    Photo = new AttachmentDTO()
                    {
                        AgencyId = agencyId,
                    },
                    StandPhoto = new AttachmentDTO()
                    {
                        AgencyId = agencyId,
                    },
                    RequiredDocuments = new RequiredDocumentsDTO()
                    {
                        AgencyId = agencyId,
                    },
                    InsuranceProcess = new InsuranceProcessDTO
                    {
                        AgencyId = agencyId,
                        SubmitDate = DateTime.Now,
                        BeginingDate = DateTime.Now,
                        EndDate = DateTime.Now,
                        InsuranceCompany = InsuranceList.United
                        //Status = ProcessStatusTypes.OnProcess
                    },
                    Salary = 1000,
                    CurrencyType = CurrencyTypes.SaudiArabia,
                    ContratPeriod = ContratPeriods.Two,
                    Synced = false,
                    DocumentReceivedDate = DateTime.Now,
                };
            }
            catch
            {
                LogUtil.LogError(ErrorSeverity.Fatal, "CommonUtility.GetNewEmployeeDTO",
                    "Problem while Getting New Employee DTO",
                    "", "");
            }
            return null;
        }
    }
}