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
        public bool SyncMemberships(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.UserRepository<MembershipDTO>().Query()
                //.Filter(a => a.DateLastModified > LastServerSyncDate)
                .Get(1).ToList();
            var sourceUsers = sourceUnitOfWork.UserRepository<UserDTO>().Query()
                .Get(1).ToList();
            if (sourceList.Any())
            {
                //_updatesFound = true;
                var destUsers =
                    destinationUnitOfWork.UserRepository<UserDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                    .Get(1).ToList();
                //var destRoles = destinationUnitOfWork.UserRepository<RoleDTO>().Query().Get(1).ToList();

                //var destList =
                //    destinationUnitOfWork.UserRepository<MembershipDTO>().Query()
                //        .Get(1).ToList();

                foreach (var source in sourceList)
                {
                    //var destination =
                    //    destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);
                    MembershipDTO source1 = source;
                    var destination = destinationUnitOfWork.UserRepository<MembershipDTO>().Query()
                        .Filter(i => i.RowGuid == source1.RowGuid).Get(1).FirstOrDefault();
                    var id = 0;
                    if (destination == null)
                        destination = new MembershipDTO();
                    else
                    {
                        continue;
                        //id = destination.UserId;
                    }

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<MembershipDTO, MembershipDTO>()
                            .ForMember("Synced", option => option.Ignore());
                        destination = Mapper.Map(source, destination);
                        destination.Id = id;

                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        var userguid = sourceUsers.FirstOrDefault(c => c.UserId == source.UserId);
                        var userDto =
                            destUsers.FirstOrDefault(c => userguid != null && c.RowGuid == userguid.RowGuid);
                        {
                            //users.User = userDto;
                            destination.UserId = userDto != null ? userDto.UserId : 1;
                        }
                        if (id == 0)
                            destinationUnitOfWork.UserRepository<MembershipDTO>()
                                .Insert(destination);
                        else
                            destinationUnitOfWork.UserRepository<MembershipDTO>()
                                .Update(destination);
                    }
                    catch
                    {
                        //_errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Crud",
                            "Problem On SyncMemberships Crud Method", UserName, Agency);
                        //return false;
                    }
                }

                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Commit",
                        "Problem Commiting SyncMemberships Method", UserName, Agency);
                    return false;
                }
            }

            return true;
        }
    }
}
