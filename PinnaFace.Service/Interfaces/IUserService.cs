using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IUserService : IDisposable
    {
        IEnumerable<UserDTO> GetAll(UserSearchCriteria<UserDTO> criteria = null);
        IEnumerable<RoleDTO> GetAllRoles();
        UserDTO Find(string employeeApplicationId);
        UserDTO GetByName(string displayName);
        UserDTO GetUser(int userId);
        string InsertOrUpdate(UserDTO user);
        string Disable(UserDTO user);
        int Delete(string employeeApplicationId);
    }
    //public interface IUserService : IDisposable
    //{
    //    IEnumerable<UserDTO> GetAll();
    //    IEnumerable<RoleDTO> GetAllRoles();
    //    UserDTO GetUser(int userId);
    //    UserDTO GetByName(string displayName);
    //    string InsertOrUpdate(UserDTO user);
    //    string Disable(UserDTO user);
    //    int Delete(UserDTO user);
    //}
}