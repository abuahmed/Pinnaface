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
        public bool SyncVisaSponsors(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<VisaSponsorDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<VisaSponsorDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }
            var sourceList = sourceUnitOfWork.Repository<VisaSponsorDTO>().Query()
                .Include(a => a.Address, a => a.Agency)
                .Filter(filter)
                .Get(1)
                .ToList();

            if (sourceList.Any())
            {
                var destLocalAgencies =
                    destinationUnitOfWork.Repository<AgencyDTO>().Query()
                    .Filter(a => a.Id == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();
                _updatesFound = true;

                var destAddresses =
                    destinationUnitOfWork.Repository<AddressDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var dto = source;
                    var destination =
                        destinationUnitOfWork.Repository<VisaSponsorDTO>().Query()
                            .Include(a => a.Address).Filter(i => i.RowGuid == dto.RowGuid)
                            .Get(1)
                            .FirstOrDefault();

                    //To Prevent ServerData Overriding
                    if (destination != null && (ToServerSyncing && !destination.Synced))
                        continue;

                    var clientId = 0;
                    if (destination == null)
                        destination = new VisaSponsorDTO();
                    else
                        clientId = destination.Id;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<VisaSponsorDTO, VisaSponsorDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("AgencyId", option => option.Ignore())
                            .ForMember("Address", option => option.Ignore())
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
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaSponsors Mapping",
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


                        var categoryDTO =
                            destAddresses.FirstOrDefault(
                                c => source.Address != null && c.RowGuid == source.Address.RowGuid);
                        {
                            destination.Address = categoryDTO;
                            destination.AddressId = categoryDTO != null ? categoryDTO.Id : (int?)null;
                        }

                        #endregion

                        destination.Synced = true;
                        destinationUnitOfWork.Repository<VisaSponsorDTO>().InsertUpdate(destination);
                    }
                    catch
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaSponsors Crud",
                            "Problem On SyncVisaSponsors Crud Method", UserName, Agency);
                        return false;
                    }
                }

                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaSponsors Commit",
                        "Problem Commiting SyncVisaSponsors Method", UserName, Agency);
                    return false;
                }
            }
            return true;
        }
    }
}
