using System.Linq;
using System.Web.Security;
//using G6Labs.Libraries.InfrastructureLib.BusinessLayer;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using WebMatrix.WebData;

namespace PinnaFace.Web.Filters
{
    public class CustomMembershipProvider : SimpleMembershipProvider
    {
        public override bool ValidateUser(string username, string password)
        {
            //var userService = new UserService(true);
            //var user = userService.Login(username, password);
            //return user != null;
            return base.ValidateUser(username, password);
        }

        //public override MembershipUser GetUser(string username, bool userIsOnline)
        //{
        //    var userService = new UserService(true);
        //    var user = userService.GetByName(username);
        //    return user;
        //    //return base.GetUser(username, userIsOnline);
        //}
        
        public override string GeneratePasswordResetToken(string userName)
        {
            return base.GeneratePasswordResetToken(userName);
        }

        public override string GetUserNameByEmail(string email)
        {
            var cri = new UserSearchCriteria<UserDTO>();
            cri.FiList.Add(u=>u.Email==email);//UserName Can't be Email Address
            var userProfile = new UserService(true).GetAll(cri).FirstOrDefault();

            if (userProfile != null)
                return userProfile.UserName;
            return email;
            //return base.GetUserNameByEmail(email);
        }
    }

    public class CustomRoleProvider : SimpleRoleProvider
    {
        public override void CreateRole(string roleName)
        {
            base.CreateRole(roleName);
        }
    }
}