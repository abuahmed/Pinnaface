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
        public bool SyncEmployees(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<EmployeeDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<EmployeeDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }
            var sourceList = sourceUnitOfWork.Repository<EmployeeDTO>().Query()
                .Include(c => c.Photo, c => c.StandPhoto, c => c.Address, c => c.RequiredDocuments)
                .Include(c => c.Education, c => c.Experience, c => c.Hawala, c => c.Agency, c => c.Agent)
                .Include(c => c.Visa, c => c.InsuranceProcess, c => c.LabourProcess, c => c.EmbassyProcess,
                    c => c.FlightProcess)
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

                var destAddresses =
                    destinationUnitOfWork.Repository<AddressDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destVisas =
                    destinationUnitOfWork.Repository<VisaDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destExperiences =
                    destinationUnitOfWork.Repository<EmployeeExperienceDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destEducations =
                    destinationUnitOfWork.Repository<EmployeeEducationDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destHawalas =
                    destinationUnitOfWork.Repository<EmployeeHawalaDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destRequiredDocuments =
                    destinationUnitOfWork.Repository<RequiredDocumentsDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destFlights =
                    destinationUnitOfWork.Repository<FlightProcessDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destEmbassies =
                    destinationUnitOfWork.Repository<EmbassyProcessDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destLabours =
                    destinationUnitOfWork.Repository<LabourProcessDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();

                var destInsurances =
                    destinationUnitOfWork.Repository<InsuranceProcessDTO>().Query()
                    .Filter(a => a.AgencyId == Singleton.Agency.Id)
                        .Get(1)
                        .ToList();
                var destPhotos =
                    destinationUnitOfWork.Repository<AttachmentDTO>().Query()
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

                    if (destination == null)
                        destination = new EmployeeDTO();
                    else if (ToServerSyncing && !destination.Synced)
                        continue;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<EmployeeDTO, EmployeeDTO>()
                            .ForMember("Agency", option => option.Ignore())
                            .ForMember("Agent", option => option.Ignore())
                            .ForMember("Id", option => option.Ignore())
                            .ForMember("Address", option => option.Ignore())
                            .ForMember("Visa", option => option.Ignore())
                            .ForMember("Experience", option => option.Ignore())
                            .ForMember("Education", option => option.Ignore())
                            .ForMember("Hawala", option => option.Ignore())
                            .ForMember("FlightProcess", option => option.Ignore())
                            .ForMember("EmbassyProcess", option => option.Ignore())
                            .ForMember("LabourProcess", option => option.Ignore())
                            .ForMember("InsuranceProcess", option => option.Ignore())
                            .ForMember("EmployeeRelatives", option => option.Ignore())
                            .ForMember("ContactPerson", option => option.Ignore())
                            .ForMember("ContactPersonId", option => option.Ignore())
                            .ForMember("Complains", option => option.Ignore())
                            .ForMember("CurrentComplain", option => option.Ignore())
                            .ForMember("CurrentComplainId", option => option.Ignore())
                            .ForMember("Photo", option => option.Ignore())
                            .ForMember("RequiredDocuments", option => option.Ignore())
                            .ForMember("StandPhoto", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());
                        destination = Mapper.Map(source, destination);
                        //destEmployeeDto.Id = destEmployeeId; we don't need if this is added .ForMember("Id", option => option.Ignore())
                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                    }
                    try
                    {
                        #region Foreign Keys

                        var categoryDTO =
                            destAgents.FirstOrDefault(
                                c => source.Agent != null && c.RowGuid == source.Agent.RowGuid);
                        {
                            destination.Agent = categoryDTO;
                            destination.AgentId = categoryDTO != null ? categoryDTO.Id : (int?)null;
                        }

                        var agencyDTO =
                            destLocalAgencies.FirstOrDefault(
                                c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
                        {
                            destination.Agency = agencyDTO;
                            destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?)null;
                        }

                        if (source.AddressId != null)
                        {
                            var categoryDto =
                                destAddresses.FirstOrDefault(
                                    c => source.Address != null && c.RowGuid == source.Address.RowGuid);
                            {
                                destination.Address = categoryDto;
                                destination.AddressId = categoryDto != null ? categoryDto.Id : (int?)null;
                            }
                        }
                        if (source.VisaId != null)
                        {
                            var headerDto =
                                destVisas.FirstOrDefault(c => source.Visa != null && c.RowGuid == source.Visa.RowGuid);
                            {
                                destination.Visa = headerDto;
                                destination.VisaId = headerDto != null ? headerDto.Id : (int?)null;
                            }
                        }
                        if (source.ExperienceId != null)
                        {
                            var footerDto =
                                destExperiences.FirstOrDefault(
                                    c => source.Experience != null && c.RowGuid == source.Experience.RowGuid);
                            {
                                destination.Experience = footerDto;
                                destination.ExperienceId = footerDto != null ? footerDto.Id : (int?)null;
                            }
                        }
                        if (source.EducationId != null)
                        {
                            var footerDto1 =
                                destEducations.FirstOrDefault(
                                    c => source.Education != null && c.RowGuid == source.Education.RowGuid);
                            {
                                destination.Education = footerDto1;
                                destination.EducationId = footerDto1 != null ? footerDto1.Id : (int?)null;
                            }
                        }
                        if (source.HawalaId != null)
                        {
                            var footerDto2 =
                                destHawalas.FirstOrDefault(
                                    c => source.Hawala != null && c.RowGuid == source.Hawala.RowGuid);
                            {
                                destination.Hawala = footerDto2;
                                destination.HawalaId = footerDto2 != null ? footerDto2.Id : (int?)null;
                            }
                        }
                        if (source.RequiredDocumentsId != null)
                        {
                            var footerDto2 =
                                destRequiredDocuments.FirstOrDefault(
                                    c =>
                                        source.RequiredDocuments != null &&
                                        c.RowGuid == source.RequiredDocuments.RowGuid);
                            {
                                destination.RequiredDocuments = footerDto2;
                                destination.RequiredDocumentsId = footerDto2 != null ? footerDto2.Id : (int?)null;
                            }
                        }
                        if (source.FlightProcessId != null)
                        {
                            var flight =
                                destFlights.FirstOrDefault(
                                    c => source.FlightProcess != null && c.RowGuid == source.FlightProcess.RowGuid);
                            {
                                destination.FlightProcess = flight;
                                destination.FlightProcessId = flight != null ? flight.Id : (int?)null;
                            }
                        }
                        if (source.EmbassyProcessId != null)
                        {
                            var embassy =
                                destEmbassies.FirstOrDefault(
                                    c => source.EmbassyProcess != null && c.RowGuid == source.EmbassyProcess.RowGuid);
                            {
                                destination.EmbassyProcess = embassy;
                                destination.EmbassyProcessId = embassy != null ? embassy.Id : (int?)null;
                            }
                        }
                        if (source.LabourProcessId != null)
                        {
                            var labour =
                                destLabours.FirstOrDefault(
                                    c => source.LabourProcess != null && c.RowGuid == source.LabourProcess.RowGuid);
                            {
                                destination.LabourProcess = labour;
                                destination.LabourProcessId = labour != null ? labour.Id : (int?)null;
                            }
                        }
                        if (source.InsuranceProcessId != null)
                        {
                            var insurance =
                                destInsurances.FirstOrDefault(
                                    c => source.InsuranceProcess != null && c.RowGuid == source.InsuranceProcess.RowGuid);
                            {
                                destination.InsuranceProcess = insurance;
                                destination.InsuranceProcessId = insurance != null ? insurance.Id : (int?)null;
                            }
                        }
                        if (source.PhotoId != null)
                        {
                            var photo =
                                destPhotos.FirstOrDefault(c => source.Photo != null && c.RowGuid == source.Photo.RowGuid);
                            {
                                destination.Photo = photo;
                                destination.PhotoId = photo != null ? photo.Id : (int?)null;
                            }
                        }
                        if (source.StandPhotoId != null)
                        {
                            var standPhoto =
                                destPhotos.FirstOrDefault(
                                    c => source.StandPhoto != null && c.RowGuid == source.StandPhoto.RowGuid);
                            {
                                destination.StandPhoto = standPhoto;
                                destination.StandPhotoId = standPhoto != null ? standPhoto.Id : (int?)null;
                            }
                        }

                        #endregion

                        destination.Synced = true;
                        destinationUnitOfWork.Repository<EmployeeDTO>()
                            .InsertUpdate(destination);
                    }
                    catch (Exception ex)
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees Crud",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees Commit",
                        "Problem Commiting SyncEmployees Method", UserName, Agency);
                    return false;
                }
            }


            return true;
        }
    }
}
