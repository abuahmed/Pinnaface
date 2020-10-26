using System;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {
        public bool SyncUserWithAgencyWithAgentDTO(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            var sourceList =
                sourceUnitOfWork.UserRepository<UserAgencyAgentDTO>()
                    .Query()
                    .Include(i => i.User, i => i.AgencyAgent)
                    .Filter(a => !(bool)a.Synced && a.DateLastModified > LastServerSyncDate)
                    .Get(1).ToList();
            if (sourceList.Any())
            {
                _updatesFound = true;
                var destUsers =
                    destinationUnitOfWork.UserRepository<UserDTO>().Query()
                     .Filter(i => i.AgencyId == Singleton.Agency.Id).Get(1).ToList();

                var destAgencyWithAgent =
                    destinationUnitOfWork.Repository<AgencyAgentDTO>().Query()
                     .Filter(i => i.AgencyId == Singleton.Agency.Id).Get(1).ToList();

                var destList =
                    destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>().Query()

                        .Include(i => i.User, i => i.AgencyAgent).Get(1).ToList();

                foreach (var source in sourceList)
                {
                    var usersInRoles =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    if (usersInRoles == null)
                    {
                        usersInRoles = new UserAgencyAgentDTO
                        {
                            RowGuid = source.RowGuid
                        };

                        try
                        {
                            #region Foreign Keys

                            var userDto =
                                destUsers.FirstOrDefault(c => source.User != null && c.RowGuid == source.User.RowGuid);
                            {
                                usersInRoles.UserId = userDto != null ? userDto.UserId : 1;
                            }
                            var roleDto =
                                destAgencyWithAgent.FirstOrDefault(
                                    c => source.AgencyAgent != null && c.RowGuid == source.AgencyAgent.RowGuid);
                            {
                                usersInRoles.AgencyWithAgentId = roleDto != null ? roleDto.Id : 1;
                            }

                            #endregion

                            var isFound = false;
                            var destination =
                                destList.FirstOrDefault(
                                    i =>
                                        i.UserId == usersInRoles.UserId &&
                                        i.AgencyWithAgentId == usersInRoles.AgencyWithAgentId);
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
                                destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>()
                                    .Update(destination);
                            else
                                destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>()
                                    .Insert(destination);

                            //destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>()
                            //    .CrudByRowGuid(destination);
                        }
                        catch (Exception ex)
                        {
                            _errorsFound = true;
                            LogUtil.LogError(ErrorSeverity.Critical, "SyncUserWithAgencyWithAgentDTO Crud",
                                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                            return false;
                        }
                    }
                }

                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncUserWithAgencyWithAgentDTO Commit",
                        "Problem Commiting SyncUserWithAgencyWithAgentDTO Method", UserName, Agency);
                    return false;
                }
            }

            return true;
        }
    }
}
