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
        public bool SyncRoles(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            var roles = sourceUnitOfWork.UserRepository<RoleDTO>().Query()
                .Filter(a => !(bool)a.Synced && a.DateLastModified > LastServerSyncDate)
                .Get(1).ToList();

            //var destList =
            //    destinationUnitOfWork.UserRepository<RoleDTO>().Query()
            //        .Get(1)
            //        .ToList();

            foreach (var source in roles)
            {
                _updatesFound = true;
                //var destination = destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);
                RoleDTO source1 = source;
                var destination = destinationUnitOfWork.UserRepository<RoleDTO>().Query()
                    .Filter(i => i.RowGuid == source1.RowGuid)
                    .Get(1).FirstOrDefault();

                var id = 0;
                if (destination == null)
                    destination = new RoleDTO();
                else
                    id = destination.RoleId;

                try
                {
                    Mapper.Reset();
                    Mapper.CreateMap<RoleDTO, RoleDTO>()
                        .ForMember("RoleId", option => option.Ignore())
                        .ForMember("Users", option => option.Ignore())
                        .ForMember("Synced", option => option.Ignore());
                    destination = Mapper.Map(source, destination);
                    destination.RoleId = id;

                    //destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId, sourceUnitOfWork, destinationUnitOfWork);
                    //destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId, sourceUnitOfWork, destinationUnitOfWork);
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncRoles Mapping",
                        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                }
                try
                {
                    destination.Synced = true;
                    if (id == 0)
                        destinationUnitOfWork.UserRepository<RoleDTO>()
                            .Insert(destination);
                    else
                        destinationUnitOfWork.UserRepository<RoleDTO>()
                            .Update(destination);
                }
                catch
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncRoles Crud",
                        "Problem On SyncRoles Crud Method", UserName, Agency);
                    return false;
                }
            }
            var changes = destinationUnitOfWork.Commit();
            if (changes < 0)
            {
                _errorsFound = true;
                LogUtil.LogError(ErrorSeverity.Critical, "SyncRoles Commit",
                    "Problem Commiting SyncRoles Method", UserName, Agency);
                return false;
            }

            return true;
        }
    }
}
