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
        public bool SyncComplainRemarks(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<ComplainRemarkDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<ComplainRemarkDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }
            var sourceList = sourceUnitOfWork.Repository<ComplainRemarkDTO>().Query()
                .Include(a => a.Complain, a => a.Agency)
                .Filter(filter)
                .Get(1)
                .ToList();

            var destLocalAgencies =
                destinationUnitOfWork.Repository<AgencyDTO>().Query()
                .Filter(a => a.Id == Singleton.Agency.Id)
                    .Get(1)
                    .ToList();
            if (sourceList.Any())
            {
                _updatesFound = true;
                var destEmployees =
                    destinationUnitOfWork.Repository<ComplainDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destList =
                    destinationUnitOfWork.Repository<ComplainRemarkDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Include(a => a.Complain)
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    //To Prevent ServerData Overriding
                    if (destination == null)
                        destination = new ComplainRemarkDTO();
                    else if (ToServerSyncing && !destination.Synced)
                        continue;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<ComplainRemarkDTO, ComplainRemarkDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("AgencyId", option => option.Ignore())
                            .ForMember("Id", option => option.Ignore())
                            .ForMember("Complain", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());

                        destination = Mapper.Map(source, destination);

                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncComplainRemarks Mapping",
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


                        var employeeDto =
                            destEmployees.FirstOrDefault(
                                c => source.Complain != null && c.RowGuid == source.Complain.RowGuid);
                        {
                            destination.Complain = employeeDto;
                            destination.ComplainId = employeeDto != null ? employeeDto.Id : 1;
                        }

                        #endregion

                        destination.Synced = true;
                        destinationUnitOfWork.Repository<ComplainRemarkDTO>()
                            .InsertUpdate(destination);
                    }
                    catch
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncComplainRemarks Crud",
                            "Problem On SyncComplainRemarks Crud Method", UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncComplainRemarks Commit",
                        "Problem Commiting SyncComplainRemarks Method", UserName, Agency);
                    return false;
                }
            }
            return true;
        }
    }
}
