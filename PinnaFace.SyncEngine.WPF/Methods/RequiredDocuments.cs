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
        public bool SyncRequiredDocuments(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<RequiredDocumentsDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<RequiredDocumentsDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }
            var sourceList = sourceUnitOfWork.Repository<RequiredDocumentsDTO>().Query()
                .Include(a => a.Agency)
                .Include(a => a.AgreementAttachment, a => a.PassportAttachment, a => a.IdCardAttachment, a => a.ContactIdCardAttachment)
                .Include(a => a.FingerPrintAttachment, a => a.MedicalAttachment, a => a.PreDepartureAttachment, a => a.GradeEightAttachment)
                .Include(a => a.CocAttachment, a => a.InsuranceAttachment)
                .Filter(filter)
                .Get(1)
                .ToList();

            var destLocalAgencies =
                destinationUnitOfWork.Repository<AgencyDTO>().Query()
                .Filter(a => a.Id == Singleton.Agency.Id)
                    .Get(1)
                    .ToList();

            var destPhotos =
             destinationUnitOfWork.Repository<AttachmentDTO>().Query()
             .Filter(a=>a.AgencyId==Singleton.Agency.Id)
                 .Get(1)
                 .ToList();

            foreach (var source in sourceList)
            {
                _updatesFound = true;

                var adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<RequiredDocumentsDTO>().Query()
                        .Filter(i => i.RowGuid == adr1.RowGuid)
                        .Get(1).FirstOrDefault();

                var clientId = 0;
                if (destination == null)
                    destination = new RequiredDocumentsDTO();
                else
                    clientId = destination.Id;


                try
                {
                    Mapper.Reset();
                    Mapper.CreateMap<RequiredDocumentsDTO, RequiredDocumentsDTO>()
                        .ForMember("Agency", option => option.Ignore())
                         .ForMember("AgreementAttachment", option => option.Ignore())
                          .ForMember("PassportAttachment", option => option.Ignore())
                           .ForMember("IdCardAttachment", option => option.Ignore())
                            .ForMember("ContactIdCardAttachment", option => option.Ignore())
                             .ForMember("FingerPrintAttachment", option => option.Ignore())
                              .ForMember("MedicalAttachment", option => option.Ignore())
                               .ForMember("PreDepartureAttachment", option => option.Ignore())
                                .ForMember("GradeEightAttachment", option => option.Ignore())
                                 .ForMember("CocAttachment", option => option.Ignore())
                                  .ForMember("InsuranceAttachment", option => option.Ignore())
                        .ForMember("AgencyId", option => option.Ignore())
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
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncRequiredDocuments Mapping",
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

                    if (source.AgreementAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.AgreementAttachment != null && c.RowGuid == source.AgreementAttachment.RowGuid);
                        {
                            destination.AgreementAttachment = attachmentDTO;
                            destination.AgreementAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }

                    if (source.PassportAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.PassportAttachment != null && c.RowGuid == source.PassportAttachment.RowGuid);
                        {
                            destination.PassportAttachment = attachmentDTO;
                            destination.PassportAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.IdCardAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.IdCardAttachment != null && c.RowGuid == source.IdCardAttachment.RowGuid);
                        {
                            destination.IdCardAttachment = attachmentDTO;
                            destination.IdCardAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.ContactIdCardAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.ContactIdCardAttachment != null && c.RowGuid == source.ContactIdCardAttachment.RowGuid);
                        {
                            destination.ContactIdCardAttachment = attachmentDTO;
                            destination.ContactIdCardAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.FingerPrintAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.FingerPrintAttachment != null && c.RowGuid == source.FingerPrintAttachment.RowGuid);
                        {
                            destination.FingerPrintAttachment = attachmentDTO;
                            destination.FingerPrintAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.MedicalAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.MedicalAttachment != null && c.RowGuid == source.MedicalAttachment.RowGuid);
                        {
                            destination.MedicalAttachment = attachmentDTO;
                            destination.MedicalAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.PreDepartureAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.PreDepartureAttachment != null && c.RowGuid == source.PreDepartureAttachment.RowGuid);
                        {
                            destination.PreDepartureAttachment = attachmentDTO;
                            destination.PreDepartureAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.GradeEightAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.GradeEightAttachment != null && c.RowGuid == source.GradeEightAttachment.RowGuid);
                        {
                            destination.GradeEightAttachment = attachmentDTO;
                            destination.GradeEightAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.CocAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.CocAttachment != null && c.RowGuid == source.CocAttachment.RowGuid);
                        {
                            destination.CocAttachment = attachmentDTO;
                            destination.CocAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    if (source.InsuranceAttachmentId != null)
                    {
                        var attachmentDTO =
                            destPhotos.FirstOrDefault(c => source.InsuranceAttachment != null && c.RowGuid == source.InsuranceAttachment.RowGuid);
                        {
                            destination.InsuranceAttachment = attachmentDTO;
                            destination.InsuranceAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
                        }
                    }
                    #endregion

                    destination.Synced = true;
                    destinationUnitOfWork.Repository<RequiredDocumentsDTO>()
                        .InsertUpdate(destination);
                }
                catch
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncRequiredDocuments Crud",
                        "Problem On SyncRequiredDocuments Crud Method", UserName, Agency);
                    return false;
                }
            }

            var changes = destinationUnitOfWork.Commit();
            if (changes < 0)
            {
                _errorsFound = true;
                LogUtil.LogError(ErrorSeverity.Critical, "SyncRequiredDocuments Commit",
                    "Problem Commiting SyncRequiredDocuments Method", UserName, Agency);
                return false;
            }

            return true;
        }
    }
}
