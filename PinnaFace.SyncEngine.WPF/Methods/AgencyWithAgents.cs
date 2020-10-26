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
        public bool SyncAgencyWithAgents(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.Repository<AgencyAgentDTO>().Query()
                .Include(h => h.Agency, h => h.Agent)
                .Filter(a => !a.Synced && a.DateLastModified > LastServerSyncDate)
                .Get(1)
                .ToList();

            if (sourceList.Any())
            {
                _updatesFound = true;
                var destAgencyDtos =
                    destinationUnitOfWork.Repository<AgencyDTO>().Query()
                    .Filter(a => a.Id == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destAgentDtos =
                    destinationUnitOfWork.Repository<AgentDTO>().Query()
                        .Get(1)
                        .ToList();

                var destList =
                    destinationUnitOfWork.Repository<AgencyAgentDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Include(a => a.Agency, a => a.Agent)
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    var clientId = 0;
                    if (destination == null)
                        destination = new AgencyAgentDTO();
                    else
                        clientId = destination.Id;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<AgencyAgentDTO, AgencyAgentDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("Agent", option => option.Ignore())
                            .ForMember("Users", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());
                        destination = Mapper.Map(source, destination);
                        destination.Id = clientId;

                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencyWithAgents Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        #region Foreign Keys

                        var agencyDTO =
                            destAgencyDtos.FirstOrDefault(
                                c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
                        {
                            destination.Agency = agencyDTO;
                            destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?)null;
                        }

                        var agentDTO =
                            destAgentDtos.FirstOrDefault(c => source.Agent != null && c.RowGuid == source.Agent.RowGuid);
                        {
                            destination.Agent = agentDTO;
                            destination.AgentId = agentDTO != null ? agentDTO.Id : 1;
                        }

                        #endregion

                        destination.Synced = true;
                        destinationUnitOfWork.Repository<AgencyAgentDTO>().InsertUpdate(destination);
                    }
                    catch
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencyWithAgents Crud",
                            "Problem On SyncAgencyWithAgents Crud Method", UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencyWithAgents Commit",
                        "Problem Commiting SyncAgencyWithAgents Method", UserName, Agency);
                    return false;
                }
            }
            return true;
        }
    }
}
