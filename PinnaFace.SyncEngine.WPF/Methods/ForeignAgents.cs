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
        public bool SyncForeignAgents(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.Repository<AgentDTO>().Query()
                .Include(a => a.Address).Include(h => h.Header, h => h.Footer)
                .Filter(a => !a.Synced && a.DateLastModified > LastServerSyncDate)
                .Get(1)
                .ToList();

            if (sourceList.Any())
            {
                _updatesFound = true;
                var destAddresses =
                    destinationUnitOfWork.Repository<AddressDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destHeadersFooters =
                    destinationUnitOfWork.Repository<AttachmentDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destList =
                    destinationUnitOfWork.Repository<AgentDTO>().Query()
                    .Filter(a => a.Id == Singleton.Agency.Id)
                        .Include(a => a.Address)
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    var clientId = 0;
                    if (destination == null)
                        destination = new AgentDTO();
                    else
                        clientId = destination.Id;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<AgentDTO, AgentDTO>()
                            .ForMember("Address", option => option.Ignore())
                            .ForMember("Header", option => option.Ignore())
                            .ForMember("Footer", option => option.Ignore())
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
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncForeignAgents Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        #region Foreign Keys

                        var categoryDTO =
                            destAddresses.FirstOrDefault(
                                c => source.Address != null && c.RowGuid == source.Address.RowGuid);
                        {
                            destination.Address = categoryDTO;
                            destination.AddressId = categoryDTO != null ? categoryDTO.Id : (int?)null;
                        }

                        var headerDto =
                            destHeadersFooters.FirstOrDefault(
                                c => source.Header != null && c.RowGuid == source.Header.RowGuid);
                        {
                            destination.Header = headerDto;
                            destination.HeaderId = headerDto != null ? headerDto.Id : (int?)null;
                        }

                        var footerDto =
                            destHeadersFooters.FirstOrDefault(
                                c => source.Footer != null && c.RowGuid == source.Footer.RowGuid);
                        {
                            destination.Footer = footerDto;
                            destination.FooterId = footerDto != null ? footerDto.Id : (int?)null;
                        }

                        #endregion

                        destination.Synced = true;
                        destinationUnitOfWork.Repository<AgentDTO>().InsertUpdate(destination);
                    }
                    catch
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncForeignAgents Crud",
                            "Problem On SyncForeignAgents Crud Method", UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncForeignAgents Commit",
                        "Problem Commiting SyncForeignAgents Method", UserName, Agency);
                    return false;
                }
            }
            return true;
        } 
    }
}
