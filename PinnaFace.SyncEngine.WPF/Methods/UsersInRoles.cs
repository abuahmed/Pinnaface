using System;
using System.Linq;
using System.Linq.Expressions;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {
        public bool SyncUsersInRoles(IUnitOfWork sourceUnitOfWork,IUnitOfWork destinationUnitOfWork)
        {
            try
            {
                #region Sync UsersInRoles

                Expression<Func<UsersInRoles, bool>> filter =
                    a => !(bool)a.Synced && a.DateLastModified > LastServerSyncDate;


                if (!ToServerSyncing)
                {
                    filter =
                        a => !(bool)a.Synced &&
                             a.DateLastModified > LastServerSyncDate &&
                             a.User.Agency != null &&
                             a.User.Agency.RowGuid == Singleton.Agency.RowGuid;
                }

                var sourceList = sourceUnitOfWork.UserRepository<UsersInRoles>()
                    .Query()
                    .Include(i => i.User, i => i.User.Agency, i => i.Role)
                    .Filter(filter)
                    .Get(1)
                    .ToList();

                if (sourceList.Any())
                {
                    _updatesFound = true;
                    var destUsers =
                        destinationUnitOfWork.UserRepository<UserDTO>().Query()
                        .Filter(i => i.AgencyId == Singleton.Agency.Id)
                        .Get(1).ToList();

                    var destRoles =
                        destinationUnitOfWork.UserRepository<RoleDTO>().Query().Get(1).ToList();

                    var destList =
                        destinationUnitOfWork.UserRepository<UsersInRoles>().Query()
                         
                            .Include(i => i.User, i => i.Role).Get(1).ToList();

                    foreach (var source in sourceList)
                    {
                        var usersInRoles =
                            destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                        if (usersInRoles == null)
                        {
                            usersInRoles = new UsersInRoles
                            {
                                RowGuid = source.RowGuid
                            };

                            try
                            {
                                #region Foreign Keys

                                var userDto =
                                    destUsers.FirstOrDefault(
                                        c => source.User != null && c.RowGuid == source.User.RowGuid);
                                {
                                    //users.User = userDto;
                                    usersInRoles.UserId = userDto != null ? userDto.UserId : 1;
                                }
                                var roleDto =
                                    destRoles.FirstOrDefault(
                                        c => source.Role != null && c.RowGuid == source.Role.RowGuid);
                                {
                                    //users.Role = roleDto;
                                    usersInRoles.RoleId = roleDto != null ? roleDto.RoleId : 1;
                                }

                                #endregion

                                var isFound = false;
                                var destination =
                                    destList.FirstOrDefault(
                                        i => i.UserId == usersInRoles.UserId && i.RoleId == usersInRoles.RoleId);

                                if (destination == null)
                                    destination = usersInRoles;
                                else
                                {
                                    isFound = true;
                                    destination.RowGuid = source.RowGuid;
                                }

                                destination.Synced = true;

                                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                                    sourceUnitOfWork, destinationUnitOfWork);
                                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                                    sourceUnitOfWork, destinationUnitOfWork);

                                if (isFound)
                                    destinationUnitOfWork.UserRepository<UsersInRoles>()
                                        .Update(destination);
                                else
                                    destinationUnitOfWork.UserRepository<UsersInRoles>()
                                        .Insert(destination);
                            }
                            catch
                            {
                                _errorsFound = true;
                                LogUtil.LogError(ErrorSeverity.Critical, "SyncUsersInRoles Crud",
                                    "Problem On SyncUsersInRoles Crud Method", UserName, Agency);
                                return false;
                            }
                        }
                    }

                    var changes = destinationUnitOfWork.Commit();
                    if (changes < 0)
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsersInRoles Commit",
                            "Problem Commiting SyncUsersInRoles Method", UserName, Agency);
                        return false;
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                _errorsFound = true;
                LogUtil.LogError(ErrorSeverity.Critical, "SyncUsersInRoles",
                    ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine +
                    "Problem On SyncUsersInRoles", UserName, Agency);
                return false;
            }
            return true;
        }
    }
}
