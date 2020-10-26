using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {
        public bool SyncVisas(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<VisaDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<VisaDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }

            var sourceList = sourceUnitOfWork.Repository<VisaDTO>().Query()
                .Include(h => h.Agent, h => h.Sponsor, h => h.Condition, h => h.Agency)
                .Filter(filter)
                .Get(1)
                .ToList();

            if (sourceList.Any())
            {
                _updatesFound = true;
                var destAgents =
                    destinationUnitOfWork.Repository<AgentDTO>().Query()
                        .Get(1)
                        .ToList();
                var destLocalAgencies =
                    destinationUnitOfWork.Repository<AgencyDTO>().Query()
                    .Filter(a => a.Id == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destHeaders =
                    destinationUnitOfWork.Repository<VisaSponsorDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destFooters =
                    destinationUnitOfWork.Repository<VisaConditionDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destList =
                    destinationUnitOfWork.Repository<VisaDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Include(h => h.Agent, h => h.Sponsor, h => h.Condition, h => h.Agency)
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    //To Prevent ServerData Overriding
                    if (destination == null)
                        destination = new VisaDTO();
                    else if (ToServerSyncing && !destination.Synced)
                        continue;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<VisaDTO, VisaDTO>()
                            .ForMember("Agent", option => option.Ignore())
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("Sponsor", option => option.Ignore())
                            .ForMember("Condition", option => option.Ignore())
                            .ForMember("Id", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());
                        destination = Mapper.Map(source, destination);

                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        //destVisaDto.Id = destVisaId;
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncVisas Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        #region Foreign Keys

                        var agentDTO =
                            destAgents.FirstOrDefault(c => source.Agent != null && c.RowGuid == source.Agent.RowGuid);
                        {
                            destination.Agent = agentDTO;
                            destination.ForeignAgentId = agentDTO != null ? agentDTO.Id : 1;
                        }

                        var agencyDTO =
                            destLocalAgencies.FirstOrDefault(
                                c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
                        {
                            destination.Agency = agencyDTO;
                            destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?)null;
                        }

                        var headerDto =
                            destHeaders.FirstOrDefault(
                                c => source.Sponsor != null && c.RowGuid == source.Sponsor.RowGuid);
                        {
                            destination.Sponsor = headerDto;
                            destination.SponsorId = headerDto != null ? headerDto.Id : 1;
                        }

                        var footerDto =
                            destFooters.FirstOrDefault(
                                c => source.Condition != null && c.RowGuid == source.Condition.RowGuid);
                        {
                            destination.Condition = footerDto;
                            destination.ConditionId = footerDto != null ? footerDto.Id : 1;
                        }

                        #endregion

                        destination.Synced = true;
                        destinationUnitOfWork.Repository<VisaDTO>().InsertUpdate(destination);
                    }
                    catch (Exception ex)
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncVisas Crud",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncVisas Commit",
                        "Problem Commiting SyncVisas Method", UserName, Agency);
                    return false;
                }
            }
            return true;
        }
    }
}
