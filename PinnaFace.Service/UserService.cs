using System;
using System.Collections.Generic;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service.Interfaces;

namespace PinnaFace.Service
{
    public class UserService : IUserService
    {
        #region Fields

        private IUnitOfWork _unitOfWork;
        private IUserRepository<UserDTO> _userRepository;
        private IUserRepository<RoleDTO> _roleRepository;
        private IUserRepository<MembershipDTO> _memberRepository;
        private IUserRepository<UsersInRoles> _usersInRolesRepository;
        private IUserRepository<UserAgencyAgentDTO> _userWithAgencyWithAgentDTO;
        private readonly bool _disposeWhenDone;

        #endregion

        #region Constructor

        public UserService()
        {
            InitializeDbContext();
        }

        public UserService(IDbContext iDbContext2)
        {
            var iDbContext = iDbContext2;
            // new ServerDbContextFactory().Create();// DbContextUtil.GetDbContextInstance();
            _userRepository = new UserRepository<UserDTO>(iDbContext);
            _roleRepository = new UserRepository<RoleDTO>(iDbContext);
            _memberRepository = new UserRepository<MembershipDTO>(iDbContext);
            _usersInRolesRepository = new UserRepository<UsersInRoles>(iDbContext);
            _userWithAgencyWithAgentDTO = new UserRepository<UserAgencyAgentDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        public UserService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _userRepository = new UserRepository<UserDTO>(iDbContext);
            _roleRepository = new UserRepository<RoleDTO>(iDbContext);
            _memberRepository = new UserRepository<MembershipDTO>(iDbContext);
            _usersInRolesRepository = new UserRepository<UsersInRoles>(iDbContext);
            _userWithAgencyWithAgentDTO = new UserRepository<UserAgencyAgentDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods

        public IUserRepositoryQuery<UserDTO> Get()
        {
            var uR = _userRepository
                .Query()
                .Include(
                            u => u.Roles, u => u.AgenciesWithAgents, 
                            u => u.Agency, u => u.Agent, 
                            u => u.Agency.Header, u => u.Agent.Header);

            return uR;
        }

        public IEnumerable<UserDTO> GetAll(UserSearchCriteria<UserDTO> criteria = null)
        {
            IEnumerable<UserDTO> userDtos;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<UserDTO> pdtoUser;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoUser =
                            pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoUser = pdto.GetList().ToList();

                    userDtos = pdtoUser.ToList();

                    foreach (var userDto in userDtos)
                    {
                        userDto.AgenciesWithAgents = _userWithAgencyWithAgentDTO.Query()
                            .Include(u => u.AgencyAgent, u => u.AgencyAgent.Agency,
                                u => u.AgencyAgent.Agent).Get().Where(u => u.UserId == userDto.UserId).ToList();
                    }
                }
                else
                {
                    userDtos = Get().Get().ToList();
                }
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }


            return userDtos;
        }

        public UserDTO GetUser(int userId)
        {
            var user = Get().Include(r => r.Roles).Get().FirstOrDefault(u => u.UserId == userId && u.Enabled == true);
            var role = GetAllUsersInRoles().Where(u => u.UserId == userId).ToList();
            //var user = _iDbContext.Set<UserDTO>().Include("Roles").FirstOrDefault(u => u.UserId == userId && u.Enabled == true);
            //var role = _iDbContext.Set<UsersInRoles>().Include("User").Include("Role").Where(u => u.UserId == user.UserId).ToList();
            if (_disposeWhenDone)
                Dispose();
            return user;
        }

        public IEnumerable<RoleDTO> GetAllRoles()
        {
            return _roleRepository.Query().Include(r => r.Users).Get();
        }

        public IEnumerable<MembershipDTO> GetAllMemberShips()
        {
            return _memberRepository.Query().Get();
        }

        public IEnumerable<UsersInRoles> GetAllUsersInRoles()
        {
            return _usersInRolesRepository.Query().Include(u => u.User, u => u.Role).Get();
        }

        public UserDTO Find(string userId)
        {
            return _userRepository.FindById(Convert.ToInt32(userId));
        }

        public UserDTO GetByName(string displayName)
        {
            var cat = _userRepository
                .Query()
                .Get().FirstOrDefault(u => u.UserName == displayName);
            return cat;
        }

        public string InsertOrUpdate(UserDTO user)
        {
            try
            {
                var validate = Validate(user);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(user))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                user.Synced = false;

                if(user.UserId==0)
                _userRepository.Insert(user);
                else _userRepository.Update(user);

                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(UserDTO user)
        {
            if (user == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _userRepository.Update(user);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                iDbContext.Dispose();
            }
            return stat;
        }

        public int Delete(string userId)
        {
            try
            {
                _userRepository.Delete(Convert.ToInt32(userId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(UserDTO user)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<UserDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.DisplayName == user.DisplayName && bp.Id != user.Id && bp.Type == user.Type)
            //        .Get()
            //        .FirstOrDefault();
            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            //return objectExists;
            return false;
        }

        public string Validate(UserDTO user)
        {
            if (null == user)
                return GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(user.ContractPeriod))
            //    return user.ContractPeriod + " " + GenericMessages.StringIsNullOrEmpty;

            //if (user.ContractPeriod.Length > 255)
            //    return user.ContractPeriod + " can not be more than 255 characters ";

            return string.Empty;
        }

        #endregion

        #region Disposing

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}

//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using PinnaFace.Core;
//using PinnaFace.Core.Models;
//using PinnaFace.Core.Models.Interfaces;
//using PinnaFace.DAL;
//using PinnaFace.DAL.Interfaces;
//using PinnaFace.Service.Interfaces;

//namespace PinnaFace.Service
//{
//    public class UserService : IUserService
//    {
//        #region Fields
//        private IDbContext _iDbContext;
//        private readonly bool _disposeWhenDone;
//        #endregion

//        #region Constructor
//        public UserService()
//        {
//            InitializeDbContext();
//        }
//        public UserService(bool disposeWhenDone)
//        {
//            _disposeWhenDone = disposeWhenDone;
//            InitializeDbContext();
//        }
//        public void InitializeDbContext()
//        {
//            _iDbContext = DbContextUtil.GetDbContextInstance();
//        }
//        #endregion

//        #region Common Methods
//        public UserDTO GetUser(int userId)
//        {
//            var user = _iDbContext.Set<UserDTO>().Include("Roles").FirstOrDefault(u => u.UserId == userId && u.Enabled == true);
//            var role = _iDbContext.Set<UsersInRoles>().Include("User").Include("Role").Where(u => u.UserId == user.UserId).ToList();
//            if (_disposeWhenDone)
//                Dispose();
//            return user;
//        }

//        public UserDTO GetByName(string displayName)
//        {
//            var user = _iDbContext.Set<UserDTO>().Include("Roles").FirstOrDefault(u => u.UserName == displayName && u.Enabled == true);
//            var role = _iDbContext.Set<UsersInRoles>().Include("User").Include("Role").Where(u => u.UserId == user.UserId).ToList();
//            if (_disposeWhenDone)
//                Dispose();
//            return user;
//        }

//        public IEnumerable<UserDTO> GetAll()
//        {
//            IEnumerable<UserDTO> piList;
//            try
//            {
//                piList = _iDbContext.Set<UserDTO>().Include("Roles")
//                    .Where(u => u.Enabled == true).ToList();

//                #region For Eager Loading
//                foreach (var userDTO in piList)
//                {
//                    var role = _iDbContext.Set<UsersInRoles>()
//                        .Include("User")
//                        .Include("Role")
//                        .Where(u => u.UserId == userDTO.UserId)
//                        .ToList();
//                }
//                #endregion
//            }
//            finally
//            {
//                Dispose(_disposeWhenDone);
//            }
//            return piList;
//        }

//        public IEnumerable<RoleDTO> GetAllRoles()
//        {
//            return _iDbContext.Set<RoleDTO>().Include("Users").ToList();
//        }
//        public IEnumerable<MembershipDTO> GetAllMemberShips()
//        {
//            return _iDbContext.Set<MembershipDTO>().ToList();
//        }
//        public IEnumerable<UsersInRoles> GetAllUsersInRoles()
//        {
//            return _iDbContext.Set<UsersInRoles>().ToList();
//        }
//        public string InsertOrUpdate(UserDTO user)
//        {
//            try
//            {
//                //WebSecurity.CreateUserAndAccount("medina", "pa12345", new { Status = 0, Enabled = true });

//                var validate = Validate(user);
//                if (!string.IsNullOrEmpty(validate))
//                    return validate;

//                if (ObjectExists(user))
//                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

//                if (user.UserId == 0)
//                {
//                    _iDbContext.Set<UserDTO>().Add(user);
//                }
//                else
//                {
//                    _iDbContext.Set<UserDTO>().Add(user);
//                    _iDbContext.Entry(user).State = EntityState.Modified;
//                }
//                _iDbContext.SaveChanges();

//                return string.Empty;
//            }
//            catch (Exception exception)
//            {
//                return exception.Message;
//            }
//        }

//        public string Disable(UserDTO user)
//        {
//            if (user == null)
//                return GenericMessages.ObjectIsNull;

//            string stat;
//            try
//            {
//                _iDbContext.Set<UserDTO>().Add(user);
//                _iDbContext.Entry(user).State = EntityState.Modified;
//                _iDbContext.SaveChanges();

//                stat = string.Empty;
//            }
//            catch (Exception exception)
//            {
//                stat = exception.Message;
//            }
//            return stat;
//        }

//        public int Delete(UserDTO user)
//        {
//            try
//            {
//                ((IObjectState)user).ObjectState = ObjectState.Deleted;
//                _iDbContext.Set<UserDTO>().Attach(user);
//                _iDbContext.Set<UserDTO>().Remove(user);
//                _iDbContext.SaveChanges();
//                return 0;
//            }
//            catch 
//            {
//                return -1;
//            }
//        }

//        public bool ObjectExists(UserDTO user)
//        {
//            var objectExists = false;
//            var dbContextInstance = DbContextUtil.GetDbContextInstance();
//            try
//            {
//                var catExists = dbContextInstance.Set<UserDTO>().Include("Roles")
//                    .FirstOrDefault(bp => bp.UserName == user.UserName && bp.UserId != user.UserId);
//                if (catExists != null)
//                    objectExists = true;
//            }
//            finally
//            {
//                dbContextInstance.Dispose();
//            }

//            return objectExists;
//        }

//        public string Validate(UserDTO user)
//        {
//            if (null == user)
//                return GenericMessages.ObjectIsNull;

//            //if (user.Warehouse == null)
//            //    return "Warehouse " + GenericMessages.ObjectIsNull;

//            //if (user.OrganizationId == 0 || new OrganizationService(true).Find(user.OrganizationId.ToString()) == null)
//            //    return "Organization is null/disabled ";

//            if (String.IsNullOrEmpty(user.UserName))
//                return user.UserName + " " + GenericMessages.StringIsNullOrEmpty;

//            if (user.UserName.Length > 50)
//                return user.UserName + " can not be more than 50 characters ";

//            return string.Empty;
//        }
//        #endregion

//        #region Disposing
//        protected void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                _iDbContext.Dispose();
//            }
//        }
//        public void Dispose()
//        {
//            Dispose(true);
//        }
//        #endregion
//    }
//}