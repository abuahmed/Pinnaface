using System;
using System.Collections.Generic;
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
        public bool SyncEmployees2(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork, bool fromServer)
        {
            Expression<Func<EmployeeDTO, bool>> filter = a =>
                !a.Synced && a.DateLastModified > LastServerSyncDate &&
                (a.CurrentComplain != null || a.ContactPerson != null);

            if (!ToServerSyncing)
            {
                Expression<Func<EmployeeDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }

            var sourceList = sourceUnitOfWork.Repository<EmployeeDTO>().Query()
                .Include(c => c.CurrentComplain, c => c.ContactPerson)
                .Filter(filter)
                .Get(1)
                .ToList();

            if (sourceList.Any())
            {
                _updatesFound = true;

                IList<EmployeeDTO> destEmployeesTemp = new List<EmployeeDTO>();

                var destComplains =
                    destinationUnitOfWork.Repository<ComplainDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destContactPersons =
                    destinationUnitOfWork.Repository<EmployeeRelativeDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destList =
                    destinationUnitOfWork.Repository<EmployeeDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                foreach (var source in sourceList)
                {
                    var destination =
                        destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

                    //To Prevent ServerData Overriding
                    if (destination == null)
                    {
                        continue;
                        //destination = new EmployeeDTO();
                    }
                    if (ToServerSyncing && !destination.Synced)
                        continue;

                    try
                    {
                        #region Mapping

                        Mapper.Reset();
                        Mapper.CreateMap<EmployeeDTO, EmployeeDTO>()
                            .ForMember("AgencyId", option => option.Ignore())
                            .ForMember("AgentId", option => option.Ignore())
                            .ForMember("Id", option => option.Ignore())
                            .ForMember("AddressId", option => option.Ignore())
                            .ForMember("RequiredDocumentsId", option => option.Ignore())
                            .ForMember("VisaId", option => option.Ignore())
                            .ForMember("ExperienceId", option => option.Ignore())
                            .ForMember("EducationId", option => option.Ignore())
                            .ForMember("HawalaId", option => option.Ignore())
                            .ForMember("FlightProcessId", option => option.Ignore())
                            .ForMember("EmbassyProcessId", option => option.Ignore())
                            .ForMember("LabourProcessId", option => option.Ignore())
                            .ForMember("InsuranceProcessId", option => option.Ignore())
                            .ForMember("ContactPerson", option => option.Ignore())
                            .ForMember("ContactPersonId", option => option.Ignore())
                            .ForMember("EmployeeRelatives", option => option.Ignore())
                            .ForMember("CurrentComplain", option => option.Ignore())
                            .ForMember("CurrentComplainId", option => option.Ignore())
                            .ForMember("Complains", option => option.Ignore())
                            .ForMember("PhotoId", option => option.Ignore())
                            .ForMember("StandPhotoId", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());

                        destination = Mapper.Map(source, destination);

                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees2 Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        #region Foreign Keys

                        if (source.CurrentComplainId != null)
                        {
                            var footerDto2 =
                                destComplains.FirstOrDefault(
                                    c =>
                                        source.CurrentComplain != null &&
                                        c.RowGuid == source.CurrentComplain.RowGuid);
                            {
                                destination.CurrentComplain = footerDto2;
                                destination.CurrentComplainId = footerDto2 != null ? footerDto2.Id : (int?)null;
                            }
                        }
                        if (source.ContactPersonId != null) //&& destEmployeeDto.ContactPerson == null
                        {
                            var footerDto2 =
                                destContactPersons.FirstOrDefault(c => source.ContactPerson != null
                                                                       &&
                                                                       c.RowGuid ==
                                                                       source.ContactPerson.RowGuid);
                            {
                                destination.ContactPerson = footerDto2;
                                destination.ContactPersonId = footerDto2 != null ? footerDto2.Id : (int?)null;
                            }
                        }

                        #endregion

                        //destination.Synced = true;
                        destEmployeesTemp.Add(destination);
                        //destinationUnitOfWork.Repository<EmployeeDTO>()
                        //    .InsertUpdate(destEmployeeDto);
                    }
                    catch
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees2 Crud",
                            "Problem On SyncEmployees2 Crud Method", UserName, Agency);
                        return false;
                    }
                }


                destinationUnitOfWork = fromServer
                    ? GetNewUow2(destinationUnitOfWork)
                    : GetNewUow(destinationUnitOfWork);

                foreach (var destEmployeeTemp in destEmployeesTemp)
                {
                    var destEmployee =
                        destinationUnitOfWork.Repository<EmployeeDTO>().FindById(destEmployeeTemp.Id);
                    if (destEmployee != null)
                    {
                        destEmployee.ContactPersonId = destEmployeeTemp.ContactPersonId;
                        destEmployee.CurrentComplainId = destEmployeeTemp.CurrentComplainId;
                        destEmployee.Synced = true;
                        destinationUnitOfWork.Repository<EmployeeDTO>().InsertUpdate(destEmployee);
                    }
                    //else
                    //{
                        
                    //}

                }

                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees2 Commit",
                        "Problem Commiting SyncEmployees2 Method", UserName, Agency);
                }
            }
            return true;
        }
    }
}
