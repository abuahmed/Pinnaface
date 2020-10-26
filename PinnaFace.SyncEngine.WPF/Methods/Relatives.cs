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
        public bool SyncRelatives(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<EmployeeRelativeDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<EmployeeRelativeDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }

            var sourceList = sourceUnitOfWork.Repository<EmployeeRelativeDTO>().Query()
                .Include(a => a.Agency, a => a.Address, a => a.Employee)
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
                var destAddresses =
                    destinationUnitOfWork.Repository<AddressDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();
                var destEmployees =
                    destinationUnitOfWork.Repository<EmployeeDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destList =
                    destinationUnitOfWork.Repository<EmployeeRelativeDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Include(a => a.Address)
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    if (destination == null)
                        destination = new EmployeeRelativeDTO();

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<EmployeeRelativeDTO, EmployeeRelativeDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("AgencyId", option => option.Ignore())
                            .ForMember("Address", option => option.Ignore())
                            .ForMember("Id", option => option.Ignore())
                            .ForMember("Employee", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());

                        destination = Mapper.Map(source, destination);
                        //clients.Id = clientId;
                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncRelatives Mapping",
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

                        var employeeDto =
                            destEmployees.FirstOrDefault(
                                c => source.Employee != null && c.RowGuid == source.Employee.RowGuid);
                        {
                            destination.Employee = employeeDto;
                            destination.EmployeeId = employeeDto != null ? employeeDto.Id : (int?)null;
                        }

                        #endregion

                        destination.Synced = true;
                        destinationUnitOfWork.Repository<EmployeeRelativeDTO>()
                            .InsertUpdate(destination);
                    }
                    catch
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncRelatives Crud",
                            "Problem On SyncRelatives Crud Method", UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncRelatives Commit",
                        "Problem Commiting SyncRelatives Method", UserName, Agency);
                    return false;
                }
            }
            return true;
        }
    }
}
