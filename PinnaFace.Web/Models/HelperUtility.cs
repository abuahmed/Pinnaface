using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Service;

namespace PinnaFace.Web.Models
{
    public static class HelperUtility
    {
        public static int GetPages(int totalCount, int pageSize)
        {
            var pages = (int)totalCount / pageSize;
            if (totalCount % pageSize > 0)
                pages++;
            return pages;
        }
        public static string GetDestinationPhotoPath()
        {
            var photoPath = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer) +
                             "E:\\Dev\\Photo\\";
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);
            return photoPath;
        }

        #region Religion Types
        public static SelectList ReligionEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(ReligionTypes))
                .Select(name => (ReligionTypes)Enum.Parse(typeof(ReligionTypes), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(ReligionTypes arg)
        {
            return GetSelectListItem(arg);
        } 
        #endregion

        #region Profession Types
        public static SelectList ProfessionEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(ProffesionTypes))
                .Select(name => (ProffesionTypes)Enum.Parse(typeof(ProffesionTypes), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(ProffesionTypes arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region Sex Types
        public static SelectList SexEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(Sex))
                .Select(name => (Sex)Enum.Parse(typeof(Sex), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(Sex arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region MaritalStatusTypes
        public static SelectList MaritalStatusTypesEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(MaritalStatusTypes))
                .Select(name => (MaritalStatusTypes)Enum.Parse(typeof(MaritalStatusTypes), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(MaritalStatusTypes arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region AgeCategory Types
        public static SelectList AgeCategoryEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(AgeCategory))
                .Select(name => (AgeCategory)Enum.Parse(typeof(AgeCategory), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(AgeCategory arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region ContratPeriods Types
        public static SelectList ContratPeriodsEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(ContratPeriods))
                .Select(name => (ContratPeriods)Enum.Parse(typeof(ContratPeriods), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(ContratPeriods arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region Complexion Types
        public static SelectList ComplexionEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(Complexion))
                .Select(name => (Complexion)Enum.Parse(typeof(Complexion), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(Complexion arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region ComplainPrority Types
        public static SelectList ComplainProrityTypesEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(ComplainProrityTypes))
                .Select(name => (ComplainProrityTypes)Enum.Parse(typeof(ComplainProrityTypes), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(ComplainProrityTypes arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region Complain Types
        public static SelectList ComplainTypesEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(ComplainTypes))
                .Select(name => (ComplainTypes)Enum.Parse(typeof(ComplainTypes), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(ComplainTypes arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region ComplainStatus Types
        public static SelectList ComplainStatusTypesEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(ComplainStatusTypes))
                .Select(name => (ComplainStatusTypes)Enum.Parse(typeof(ComplainStatusTypes), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(ComplainStatusTypes arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region CountryList
        public static SelectList CountryListEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(CountryList))
                .Select(name => (CountryList)Enum.Parse(typeof(CountryList), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(CountryList arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        #region LanguageExperience
        public static SelectList LanguageExperienceEnums()
        {
            var relTypes =
                Enum.GetNames(typeof(LanguageExperience))
                .Select(name => (LanguageExperience)Enum.Parse(typeof(LanguageExperience), name))
                .Select(GetSLI).ToList();
            return new SelectList(relTypes, "value", "text");
        }

        private static SelectListItem GetSLI(LanguageExperience arg)
        {
            return GetSelectListItem(arg);
        }
        #endregion

        public static SelectList GetAgencies()
        {
            
            try
            {
                var currentUser = new UserService(true)
                        .GetAll(new UserSearchCriteria<UserDTO>())
                        .FirstOrDefault(u => u.UserId == Singleton.User.UserId);

                if (currentUser != null && currentUser.AgenciesWithAgents != null &&
                                currentUser.AgenciesWithAgents.Count > 0)
                {
                    var agencies = new List<AgencyDTO>();
                    var agencyAgents = currentUser.AgenciesWithAgents;

                    foreach (var agencyAgentsDto in agencyAgents)
                    {
                        var agen = agencyAgentsDto.AgencyAgent.Agency;
                        agencies.Add(agen);
                    }

                    //var agencies = currentUser.AgenciesWithAgents.ToList();//new LocalAgencyService(true).GetAll().ToList();
                    return new SelectList(agencies, "Id", "AgencyName");
                }
            }
            catch 
            {

            }
            return null;
        }

        public static SelectList GetAgents()
        {
            try
            {
                var currentUser = new UserService(true)
                        .GetAll(new UserSearchCriteria<UserDTO>())
                        .FirstOrDefault(u => u.UserId == Singleton.User.UserId);

                if (currentUser != null && currentUser.AgenciesWithAgents != null &&
                                currentUser.AgenciesWithAgents.Count > 0)
                {
                    var agents = new List<AgentDTO>();
                    var agencyAgents = currentUser.AgenciesWithAgents;

                    foreach (var agencyAgentsDto in agencyAgents)
                    {
                        var agen = agencyAgentsDto.AgencyAgent.Agent;
                        agents.Add(agen);
                    }

                    //var agencies = currentUser.AgenciesWithAgents.ToList();//new LocalAgencyService(true).GetAll().ToList();
                    return new SelectList(agents, "Id", "AgentName");
                }
            }
            catch
            {

            }
            return null;
            //var agents = new ForeignAgentService(true,false).GetAll().ToList();
            //return new SelectList(agents, "Id", "AgentName");
        }

        public static SelectListItem GetSelectListItem(Enum religionTypes)
        {
            var role = new SelectListItem
            {
                Value = Convert.ToInt32(religionTypes).ToString(),
                Text = EnumUtil.GetEnumDesc(religionTypes)
            };
            return role;
        }

        /** Automatically Insert this StoredProcedure in the Database
         *  USE [OneFaceWebDB]
            GO
            SET ANSI_NULLS ON
            GO
            SET QUOTED_IDENTIFIER ON
            GO
            CREATE PROCEDURE [dbo].[g6sp_GetUTCDate]
            AS
            BEGIN
	            SELECT GETUTCDATE() AS SQLUTCDateTime
            END
            GO
         
        public static DateTime GetCurrentSqlutcDate()
        {
             
             var sQlServConString = DbCommandUtil.GetActivationConnectionString();
            //const string dbConnectionString = "";

            var db = new DbUtil(sQlServConString);
            var sqlDate = DateTime.UtcNow; // Set the default to the server time
            //Now try and get the actual sql server time.
            try
            {
                db.Command.CommandText = "g6sp_GetUTCDate";
                SqlDataReader row = db.Command.ExecuteReader();

                if (row.Read())
                    sqlDate = DbUtil.GetDateTimeValue(ref row, "SQLUTCDateTime");

                row.Close();
            }
            catch
            {

            }

            finally
            {
                db.Dispose();
            }
            return sqlDate;
        }
*/
        
    }
}