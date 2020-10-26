using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using WebMatrix.WebData;

namespace PinnaFace.Web.Models
{
    public static class WebUtility
    {
        public static int GetAgentIdForTheAgency(int? agencyId)
        {
            //var currentUser = new UserService(true)
            //    .GetAll(new UserSearchCriteria<UserDTO>())
            //    .FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
            var currentUser = Singleton.User;
            if (currentUser != null && currentUser.AgenciesWithAgents != null &&
                currentUser.AgenciesWithAgents.Count > 0)
            {
                var userWithAgencyWithAgentDTO = currentUser.AgenciesWithAgents.FirstOrDefault();
                if (agencyId != -1)
                    userWithAgencyWithAgentDTO = currentUser.AgenciesWithAgents.FirstOrDefault(
                        ag => ag.AgencyAgent.AgencyId == agencyId);

                if (userWithAgencyWithAgentDTO != null)
                    if (userWithAgencyWithAgentDTO.AgencyAgent != null)
                        return
                            userWithAgencyWithAgentDTO.AgencyAgent.AgentId;
            }
            return -1;
        }

        public static bool IsUserAgent()
        {
            //var currentUser = new UserService(true)
            //    .GetAll(new UserSearchCriteria<UserDTO>())
            //    .FirstOrDefault(u => u.UserId == WebSecurity.CurrentUserId);
            var currentUser = Singleton.User;

            if (currentUser != null && currentUser.AgentId!=null)
                return true;

            return false;
        }
    }
}