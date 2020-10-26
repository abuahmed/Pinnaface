using System;
using System.Linq;
using AutoMapper;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {
        public bool SyncUsers(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.UserRepository<UserDTO>().Query()
                .Include(h => h.Agency)
                .Filter(a => !(bool)a.Synced && a.DateLastModified > LastServerSyncDate)
                .Get(1).ToList();

            if (sourceList.Any())
            {
                _updatesFound = true;
                var destLocalAgencies =
                    destinationUnitOfWork.Repository<AgencyDTO>().Query()
                    .Filter(a => a.Id == Singleton.Agency.Id)
                    .Get(1)
                    .ToList();

                //var destList =
                //    destinationUnitOfWork.UserRepository<UserDTO>().Query()
                //    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                //    .Get(1)
                //    .ToList();

                foreach (var source in sourceList)
                {
                    UserDTO source1 = source;
                    var destination =
                        destinationUnitOfWork.UserRepository<UserDTO>().Query()
                        .Filter(i => i.RowGuid == source1.RowGuid)
                        .Get(1)
                        .FirstOrDefault();

                    var userId = 0;
                    if (destination == null)
                        destination = new UserDTO();
                    else
                        userId = destination.UserId;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<UserDTO, UserDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("AgenciesWithAgents", option => option.Ignore())
                            .ForMember("Agent", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());

                        destination = Mapper.Map(source, destination);
                        destination.UserId = userId;
                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        #region Foreign Keys

                        var agencyDTO =
                            destLocalAgencies.FirstOrDefault(
                                c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
                        {
                            destination.Agency = agencyDTO;
                            destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?)null;
                        }

                        #endregion

                        destination.Synced = true;

                        if (userId == 0)
                            destinationUnitOfWork.UserRepository<UserDTO>()
                                .Insert(destination);
                        else
                            destinationUnitOfWork.UserRepository<UserDTO>()
                                .Update(destination);
                    }
                    catch (Exception ex)
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Crud",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Commit",
                        "Problem Commiting SyncUsers Method", UserName, Agency);
                    return false;
                }
            }

            return true;
        }
    }
}
