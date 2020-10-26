using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Documents;
using AutoMapper;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {
        public bool SyncUsers2(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.UserRepository<UserDTO>().Query()
                .Filter(a => !(bool)a.Synced && a.DateLastModified > LastServerSyncDate)
                .Get(1).ToList();

            if (sourceList.Any())
            {
                _updatesFound = true;
               
                //List<UserDTO> destList;

                //if(Singleton.Agency!=null)
                //    destList = destinationUnitOfWork.UserRepository<UserDTO>()
                //        .Query().Filter(a => a.AgencyId == Singleton.Agency.Id)
                //        .Get(1)
                //        .ToList();
                //else 
                    //destList = destinationUnitOfWork.UserRepository<UserDTO>()
                    //    .Query()
                    //    .Get(1)
                    //    .ToList();

                foreach (var source in sourceList)
                {
                    UserDTO source1 = source;
                    var destination =
                         destinationUnitOfWork.UserRepository<UserDTO>()
                        .Query().Filter(i => i.RowGuid == source1.RowGuid)
                        .Get(1).FirstOrDefault();

                    if (destination == null)
                        destination = new UserDTO();
                    else
                        continue;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<UserDTO, UserDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("AgencyId", option => option.Ignore())
                            .ForMember("AgenciesWithAgents", option => option.Ignore())
                            .ForMember("Agent", option => option.Ignore())
                            .ForMember("AgentId", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());

                        destination = Mapper.Map(source, destination);
                        destination.Agency = null;
                        destination.AgencyId = null;
                        destination.CreatedByUserId = 1;
                        destination.ModifiedByUserId = 1;
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        destination.Synced = true;

                        //if (userId == 0)
                        destinationUnitOfWork.UserRepository<UserDTO>()
                            .Insert(destination);
                        //else
                        //    destinationUnitOfWork.UserRepository<UserDTO>()
                        //        .Update(destination);
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
