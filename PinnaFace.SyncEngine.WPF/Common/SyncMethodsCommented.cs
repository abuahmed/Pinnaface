using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {
        //public bool SyncSettings(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<SettingDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<SettingDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }

        //    var settingDtos = sourceUnitOfWork.Repository<SettingDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();

        //    foreach (var source in settingDtos)
        //    {
        //        _updatesFound = true;


        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<SettingDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        //To Prevent ServerData Overriding
        //        if (destination != null && (ToServerSyncing && !destination.Synced))
        //            continue;

        //        var clientId = 0;
        //        if (destination == null)
        //            destination = new SettingDTO();
        //        else
        //            clientId = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<SettingDTO, SettingDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = clientId;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncSettings Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }

        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<SettingDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncSettings Crud",
        //                "Problem On SyncSettings Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncSettings Commit",
        //            "Problem Commiting SyncSettings Method", UserName, Agency);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool SyncProductActivations(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<ProductActivationDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<ProductActivationDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;

        //        filter = filter.And(filter2);
        //    }

        //    var productActivationDtos =
        //        sourceUnitOfWork.Repository<ProductActivationDTO>().Query()
        //            .Include(a => a.Agency)
        //            .Filter(filter)
        //            .Get(1)
        //            .ToList();
        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();

        //    foreach (var source in productActivationDtos)
        //    {
        //        _updatesFound = true;


        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<ProductActivationDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        //To Prevent ServerData Overriding
        //        if (destination != null && (ToServerSyncing && !destination.Synced))
        //            continue;

        //        var clientId = 0;
        //        if (destination == null)
        //            destination = new ProductActivationDTO();
        //        else
        //            clientId = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<ProductActivationDTO, ProductActivationDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());

        //            destination = Mapper.Map(source, destination);
        //            destination.Id = clientId;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncProductActivations Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }

        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<ProductActivationDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncProductActivations Crud",
        //                "Problem On SyncProductActivations Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncProductActivations Commit",
        //            "Problem Commiting SyncProductActivations Method", UserName, Agency);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool SyncAddresses(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<AddressDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<AddressDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var addressDtos = sourceUnitOfWork.Repository<AddressDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in addressDtos)
        //    {
        //        _updatesFound = true;

        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<AddressDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        //To Prevent ServerData Overriding
        //        if (destination != null && (ToServerSyncing && !destination.Synced))
        //            continue;

        //        var clientId = 0;
        //        if (destination == null)
        //            destination = new AddressDTO();
        //        else
        //            clientId = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<AddressDTO, AddressDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            //.ForMember("CreatedByUserId", option => option.Ignore()) Will Be Overided by the line below destination.CreatedByUserId...
        //            //.ForMember("ModifiedByUserId", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = clientId;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAddresses Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }

        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<AddressDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAddresses Crud",
        //                "Problem On SyncAddresses Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncAddresses Commit",
        //            "Problem Commiting SyncAddresses Method", UserName, Agency);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool SyncRequiredDocuments(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<RequiredDocumentsDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<RequiredDocumentsDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var sourceList = sourceUnitOfWork.Repository<RequiredDocumentsDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Include(a => a.AgreementAttachment,a=>a.PassportAttachment,a=>a.IdCardAttachment,a=>a.ContactIdCardAttachment)
        //        .Include(a => a.FingerPrintAttachment, a => a.MedicalAttachment, a => a.PreDepartureAttachment, a => a.GradeEightAttachment)
        //        .Include(a => a.CocAttachment, a => a.InsuranceAttachment)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();

        //    var destPhotos =
        //     destinationUnitOfWork.Repository<AttachmentDTO>().Query()
        //         .Get(1)
        //         .ToList();

        //    foreach (var source in sourceList)
        //    {
        //        _updatesFound = true;

        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<RequiredDocumentsDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1).FirstOrDefault();

        //        var clientId = 0;
        //        if (destination == null)
        //            destination = new RequiredDocumentsDTO();
        //        else
        //            clientId = destination.Id;


        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<RequiredDocumentsDTO, RequiredDocumentsDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                 .ForMember("AgreementAttachment", option => option.Ignore())
        //                  .ForMember("PassportAttachment", option => option.Ignore())
        //                   .ForMember("IdCardAttachment", option => option.Ignore())
        //                    .ForMember("ContactIdCardAttachment", option => option.Ignore())
        //                     .ForMember("FingerPrintAttachment", option => option.Ignore())
        //                      .ForMember("MedicalAttachment", option => option.Ignore())
        //                       .ForMember("PreDepartureAttachment", option => option.Ignore())
        //                        .ForMember("GradeEightAttachment", option => option.Ignore())
        //                         .ForMember("CocAttachment", option => option.Ignore())
        //                          .ForMember("InsuranceAttachment", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = clientId;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncRequiredDocuments Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            if (source.AgreementAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.AgreementAttachment != null && c.RowGuid == source.AgreementAttachment.RowGuid);
        //                {
        //                    destination.AgreementAttachment = attachmentDTO;
        //                    destination.AgreementAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }

        //            if (source.PassportAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.PassportAttachment != null && c.RowGuid == source.PassportAttachment.RowGuid);
        //                {
        //                    destination.PassportAttachment = attachmentDTO;
        //                    destination.PassportAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.IdCardAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.IdCardAttachment != null && c.RowGuid == source.IdCardAttachment.RowGuid);
        //                {
        //                    destination.IdCardAttachment = attachmentDTO;
        //                    destination.IdCardAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.ContactIdCardAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.ContactIdCardAttachment != null && c.RowGuid == source.ContactIdCardAttachment.RowGuid);
        //                {
        //                    destination.ContactIdCardAttachment = attachmentDTO;
        //                    destination.ContactIdCardAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.FingerPrintAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.FingerPrintAttachment != null && c.RowGuid == source.FingerPrintAttachment.RowGuid);
        //                {
        //                    destination.FingerPrintAttachment = attachmentDTO;
        //                    destination.FingerPrintAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.MedicalAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.MedicalAttachment != null && c.RowGuid == source.MedicalAttachment.RowGuid);
        //                {
        //                    destination.MedicalAttachment = attachmentDTO;
        //                    destination.MedicalAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.PreDepartureAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.PreDepartureAttachment != null && c.RowGuid == source.PreDepartureAttachment.RowGuid);
        //                {
        //                    destination.PreDepartureAttachment = attachmentDTO;
        //                    destination.PreDepartureAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.GradeEightAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.GradeEightAttachment != null && c.RowGuid == source.GradeEightAttachment.RowGuid);
        //                {
        //                    destination.GradeEightAttachment = attachmentDTO;
        //                    destination.GradeEightAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.CocAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.CocAttachment != null && c.RowGuid == source.CocAttachment.RowGuid);
        //                {
        //                    destination.CocAttachment = attachmentDTO;
        //                    destination.CocAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            if (source.InsuranceAttachmentId != null)
        //            {
        //                var attachmentDTO =
        //                    destPhotos.FirstOrDefault(c => source.InsuranceAttachment != null && c.RowGuid == source.InsuranceAttachment.RowGuid);
        //                {
        //                    destination.InsuranceAttachment = attachmentDTO;
        //                    destination.InsuranceAttachmentId = attachmentDTO != null ? attachmentDTO.Id : (int?)null;
        //                }
        //            }
        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<RequiredDocumentsDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncRequiredDocuments Crud",
        //                "Problem On SyncRequiredDocuments Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }

        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncRequiredDocuments Commit",
        //            "Problem Commiting SyncRequiredDocuments Method", UserName, Agency);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool SyncAttachments(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<AttachmentDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<AttachmentDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var adrs = sourceUnitOfWork.Repository<AttachmentDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
       
        //    foreach (var source in adrs)
        //    {
        //        _updatesFound = true;

        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<AttachmentDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var clientId = 0;
        //        if (destination == null)
        //            destination = new AttachmentDTO();
        //        else
        //            clientId = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<AttachmentDTO, AttachmentDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore())
        //                .ForMember("AttachedFile", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = clientId;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }

        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<AttachmentDTO>().InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments Crud",
        //                "Problem On SyncAttachments Crud Method", UserName, Agency);
        //            return false;
        //        }
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(source.AttachmentUrl) ||
        //                (Singleton.PhotoStorage == PhotoStorage.Database && source.AttachedFile != null))
        //            {
        //                //var dest = PathUtil.GetDestinationPhotoPath();
        //                var photoPath = PathUtil.GetLocalPhotoPath();
        //                //var fiName = source.AttachmentUrl;
                        
        //                if (Singleton.PhotoStorage == PhotoStorage.Database && source.AttachedFile != null)
        //                {
        //                    var fiName = source.RowGuid + ".jpg";
        //                    File.WriteAllBytes(Path.Combine(photoPath, fiName), source.AttachedFile);
        //                }

        //                //if (Singleton.BuildType == BuildType.LocalDev)
        //                //    ////fileNames.Add(fiName);
        //                //    //File.Copy(Path.Combine(photoPath, fiName), Path.Combine(dest, fiName), true);
        //                //else
        //                //{
        //                //    ////Collect Filenames here
        //                //    //fileNames.Add(fiName);

        //                //    //using (var client = new WebClient())
        //                //    //{
        //                //    //    client.Credentials = DbCommandUtil.GetNetworkCredential();
        //                //    //    //var desti = Path.Combine(dest, fiName);
        //                //    //    //var photopa = Path.Combine(photoPath, fiName);
        //                //    //    //client.UploadFile(desti, photopa);
        //                //    //    client.UploadFileAsync(new Uri(Path.Combine(dest, fiName)), WebRequestMethods.Ftp.UploadFile,
        //                //    //        Path.Combine(photoPath, fiName));
        //                //    //    //client.UploadFileAsync();
        //                //    //}
        //                //}
        //            }
        //            else if (source.AttachedFile != null)
        //            {
        //                var photoPath = PathUtil.GetPhotoPath();
        //                var fiName = source.RowGuid + "_File.jpg";

        //                File.WriteAllBytes(Path.Combine(photoPath, fiName), source.AttachedFile);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments upload photo problem",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }

        //    }
            
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments Commit",
        //            "Problem Commiting SyncAttachments Method", UserName, Agency);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool SyncVisaSponsors(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<VisaSponsorDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<VisaSponsorDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var sourceList = sourceUnitOfWork.Repository<VisaSponsorDTO>().Query()
        //        .Include(a => a.Address, a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        var destLocalAgencies =
        //            destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //                .Get(1)
        //                .ToList();
        //        _updatesFound = true;
        //        var destAddresses =
        //            destinationUnitOfWork.Repository<AddressDTO>().Query()
        //                .Get(1)
        //                .ToList();
        //        foreach (var source in sourceList)
        //        {
        //            var dto = source;
        //            var destination =
        //                destinationUnitOfWork.Repository<VisaSponsorDTO>().Query()
        //                    .Include(a => a.Address).Filter(i => i.RowGuid == dto.RowGuid)
        //                    .Get(1)
        //                    .FirstOrDefault();

        //            //To Prevent ServerData Overriding
        //            if (destination != null && (ToServerSyncing && !destination.Synced))
        //                continue;

        //            var clientId = 0;
        //            if (destination == null)
        //                destination = new VisaSponsorDTO();
        //            else
        //                clientId = destination.Id;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<VisaSponsorDTO, VisaSponsorDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("AgencyId", option => option.Ignore())
        //                    .ForMember("Address", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);
        //                destination.Id = clientId;

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaSponsors Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }

        //            try
        //            {
        //                #region Foreign Keys

        //                var agencyDTO =
        //                    destLocalAgencies.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }


        //                var categoryDTO =
        //                    destAddresses.FirstOrDefault(
        //                        c => source.Address != null && c.RowGuid == source.Address.RowGuid);
        //                {
        //                    destination.Address = categoryDTO;
        //                    destination.AddressId = categoryDTO != null ? categoryDTO.Id : (int?) null;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<VisaSponsorDTO>().InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaSponsors Crud",
        //                    "Problem On SyncVisaSponsors Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }

        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaSponsors Commit",
        //                "Problem Commiting SyncVisaSponsors Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncVisaConditions(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<VisaConditionDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<VisaConditionDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }

        //    var sources = sourceUnitOfWork.Repository<VisaConditionDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in sources)
        //    {
        //        _updatesFound = true;

        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<VisaConditionDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        //To Prevent ServerData Overriding
        //        if (destination != null && (ToServerSyncing && !destination.Synced))
        //            continue;

        //        var clientId = 0;
        //        if (destination == null)
        //            destination = new VisaConditionDTO();
        //        else
        //            clientId = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<VisaConditionDTO, VisaConditionDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = clientId;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaConditions Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }

        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<VisaConditionDTO>().InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaConditions Crud",
        //                "Problem On SyncVisaConditions Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncVisaConditions Commit",
        //            "Problem Commiting SyncVisaConditions Method", UserName, Agency);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool SyncAgencies(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList = sourceUnitOfWork.Repository<AgencyDTO>().Query()
        //        .Include(h => h.Header, h => h.Footer, h => h.Address)
        //        .Filter(a => !a.Synced && a.DateLastModified > LastServerSyncDate)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destAddresses =
        //            destinationUnitOfWork.Repository<AddressDTO>().Query()
        //                .Get(1)
        //                .ToList();
        //        var destHeadersFooters =
        //            destinationUnitOfWork.Repository<AttachmentDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //                .Include(a => a.Address)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            var clientId = 0;
        //            if (destination == null)
        //            {
        //                destination = new AgencyDTO();
        //            }
        //            else
        //                clientId = destination.Id;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<AgencyDTO, AgencyDTO>()
        //                    .ForMember("Address", option => option.Ignore())
        //                    .ForMember("Header", option => option.Ignore())
        //                    .ForMember("Footer", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);
        //                destination.Id = clientId;

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //                //UpdatingText = "Problem mapping AgencyDTO";
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var categoryDto =
        //                    destAddresses.FirstOrDefault(
        //                        c => source.Address != null && c.RowGuid == source.Address.RowGuid);
        //                {
        //                    destination.Address = categoryDto;
        //                    destination.AddressId = categoryDto != null ? categoryDto.Id : (int?) null;
        //                }

        //                var headerDto =
        //                    destHeadersFooters.FirstOrDefault(
        //                        c => source.Header != null && c.RowGuid == source.Header.RowGuid);
        //                {
        //                    destination.Header = headerDto;
        //                    destination.HeaderId = headerDto != null ? headerDto.Id : (int?) null;
        //                }

        //                var footerDto =
        //                    destHeadersFooters.FirstOrDefault(
        //                        c => source.Footer != null && c.RowGuid == source.Footer.RowGuid);
        //                {
        //                    destination.Footer = footerDto;
        //                    destination.FooterId = footerDto != null ? footerDto.Id : (int?) null;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<AgencyDTO>()
        //                    .InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Crud",
        //                    "Problem On SyncAgencies Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Commit",
        //                "Problem Commiting SyncAgencies Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncAgencies2(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList = sourceUnitOfWork.Repository<AgencyDTO>().Query()
        //        .Filter(a => !a.Synced && a.DateLastModified > LastServerSyncDate)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;


        //        var destList =
        //            destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //                .Include(a => a.Address)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            var clientId = 0;
        //            if (destination == null)
        //            {
        //                destination = new AgencyDTO();
        //            }
        //            else
        //                clientId = destination.Id;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<AgencyDTO, AgencyDTO>()
        //                    .ForMember("Address", option => option.Ignore())
        //                    .ForMember("Header", option => option.Ignore())
        //                    .ForMember("Footer", option => option.Ignore())
        //                    .ForMember("AddressId", option => option.Ignore())
        //                    .ForMember("HeaderId", option => option.Ignore())
        //                    .ForMember("FooterId", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);
        //                destination.Id = clientId;

        //                destination.Address = null;
        //                destination.AddressId = null;
        //                destination.Header = null;
        //                destination.HeaderId = null;
        //                destination.Footer = null;
        //                destination.FooterId = null;

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //                //UpdatingText = "Problem mapping AgencyDTO";
        //            }
        //            try
        //            {
        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<AgencyDTO>()
        //                    .InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Crud",
        //                    "Problem On SyncAgencies Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Commit",
        //                "Problem Commiting SyncAgencies Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncForeignAgents(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList = sourceUnitOfWork.Repository<AgentDTO>().Query()
        //        .Include(a => a.Address).Include(h => h.Header, h => h.Footer)
        //        .Filter(a => !a.Synced && a.DateLastModified > LastServerSyncDate)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destAddresses =
        //            destinationUnitOfWork.Repository<AddressDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destHeadersFooters =
        //            destinationUnitOfWork.Repository<AttachmentDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<AgentDTO>().Query()
        //                .Include(a => a.Address)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            var clientId = 0;
        //            if (destination == null)
        //                destination = new AgentDTO();
        //            else
        //                clientId = destination.Id;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<AgentDTO, AgentDTO>()
        //                    .ForMember("Address", option => option.Ignore())
        //                    .ForMember("Header", option => option.Ignore())
        //                    .ForMember("Footer", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);
        //                destination.Id = clientId;

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncForeignAgents Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var categoryDTO =
        //                    destAddresses.FirstOrDefault(
        //                        c => source.Address != null && c.RowGuid == source.Address.RowGuid);
        //                {
        //                    destination.Address = categoryDTO;
        //                    destination.AddressId = categoryDTO != null ? categoryDTO.Id : (int?) null;
        //                }

        //                var headerDto =
        //                    destHeadersFooters.FirstOrDefault(
        //                        c => source.Header != null && c.RowGuid == source.Header.RowGuid);
        //                {
        //                    destination.Header = headerDto;
        //                    destination.HeaderId = headerDto != null ? headerDto.Id : (int?) null;
        //                }

        //                var footerDto =
        //                    destHeadersFooters.FirstOrDefault(
        //                        c => source.Footer != null && c.RowGuid == source.Footer.RowGuid);
        //                {
        //                    destination.Footer = footerDto;
        //                    destination.FooterId = footerDto != null ? footerDto.Id : (int?) null;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<AgentDTO>().InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncForeignAgents Crud",
        //                    "Problem On SyncForeignAgents Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncForeignAgents Commit",
        //                "Problem Commiting SyncForeignAgents Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncAgencyWithAgents(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList = sourceUnitOfWork.Repository<AgencyAgentDTO>().Query()
        //        .Include(h => h.Agency, h => h.Agent)
        //        .Filter(a => !a.Synced && a.DateLastModified > LastServerSyncDate)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destAgencyDtos =
        //            destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destAgentDtos =
        //            destinationUnitOfWork.Repository<AgentDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<AgencyAgentDTO>().Query()
        //                .Include(a => a.Agency, a => a.Agent)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            var clientId = 0;
        //            if (destination == null)
        //                destination = new AgencyAgentDTO();
        //            else
        //                clientId = destination.Id;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<AgencyAgentDTO, AgencyAgentDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("Agent", option => option.Ignore())
        //                    .ForMember("Users", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);
        //                destination.Id = clientId;

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencyWithAgents Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var agencyDTO =
        //                    destAgencyDtos.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }

        //                var agentDTO =
        //                    destAgentDtos.FirstOrDefault(c => source.Agent != null && c.RowGuid == source.Agent.RowGuid);
        //                {
        //                    destination.Agent = agentDTO;
        //                    destination.AgentId = agentDTO != null ? agentDTO.Id : 1;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<AgencyAgentDTO>().InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencyWithAgents Crud",
        //                    "Problem On SyncAgencyWithAgents Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencyWithAgents Commit",
        //                "Problem Commiting SyncAgencyWithAgents Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncVisas(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<VisaDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<VisaDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }

        //    var sourceList = sourceUnitOfWork.Repository<VisaDTO>().Query()
        //        .Include(h => h.Agent, h => h.Sponsor, h => h.Condition, h => h.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destAgents =
        //            destinationUnitOfWork.Repository<AgentDTO>().Query()
        //                .Get(1)
        //                .ToList();
        //        var destLocalAgencies =
        //            destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destHeaders =
        //            destinationUnitOfWork.Repository<VisaSponsorDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destFooters =
        //            destinationUnitOfWork.Repository<VisaConditionDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<VisaDTO>().Query()
        //                .Include(h => h.Agent, h => h.Sponsor, h => h.Condition, h => h.Agency)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            //To Prevent ServerData Overriding
        //            if (destination == null)
        //                destination = new VisaDTO();
        //            else if (ToServerSyncing && !destination.Synced)
        //                continue;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<VisaDTO, VisaDTO>()
        //                    .ForMember("Agent", option => option.Ignore())
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("Sponsor", option => option.Ignore())
        //                    .ForMember("Condition", option => option.Ignore())
        //                    .ForMember("Id", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                //destVisaDto.Id = destVisaId;
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncVisas Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var agentDTO =
        //                    destAgents.FirstOrDefault(c => source.Agent != null && c.RowGuid == source.Agent.RowGuid);
        //                {
        //                    destination.Agent = agentDTO;
        //                    destination.ForeignAgentId = agentDTO != null ? agentDTO.Id : 1;
        //                }

        //                var agencyDTO =
        //                    destLocalAgencies.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }

        //                var headerDto =
        //                    destHeaders.FirstOrDefault(
        //                        c => source.Sponsor != null && c.RowGuid == source.Sponsor.RowGuid);
        //                {
        //                    destination.Sponsor = headerDto;
        //                    destination.SponsorId = headerDto != null ? headerDto.Id : 1;
        //                }

        //                var footerDto =
        //                    destFooters.FirstOrDefault(
        //                        c => source.Condition != null && c.RowGuid == source.Condition.RowGuid);
        //                {
        //                    destination.Condition = footerDto;
        //                    destination.ConditionId = footerDto != null ? footerDto.Id : 1;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<VisaDTO>().InsertUpdate(destination);
        //            }
        //            catch (Exception ex)
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncVisas Crud",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncVisas Commit",
        //                "Problem Commiting SyncVisas Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncEducation(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<EmployeeEducationDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<EmployeeEducationDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var exprs = sourceUnitOfWork.Repository<EmployeeEducationDTO>()
        //        .Query().Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in exprs)
        //    {
        //        _updatesFound = true;

        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<EmployeeEducationDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var id = 0;
        //        if (destination == null)
        //            destination = new EmployeeEducationDTO();
        //        else
        //            id = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<EmployeeEducationDTO, EmployeeEducationDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = id;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncEducation Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<EmployeeEducationDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncEducation Crud",
        //                "Problem On SyncEducation Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncEducation Commit",
        //            "Problem Commiting SyncEducation Method", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncExperiences(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<EmployeeExperienceDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<EmployeeExperienceDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }

        //    var exprs = sourceUnitOfWork.Repository<EmployeeExperienceDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in exprs)
        //    {
        //        _updatesFound = true;
        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<EmployeeExperienceDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var id = 0;
        //        if (destination == null)
        //            destination = new EmployeeExperienceDTO();
        //        else
        //            id = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<EmployeeExperienceDTO, EmployeeExperienceDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = id;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncExperience Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<EmployeeExperienceDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncExperience Crud",
        //                "Problem On SyncExperience Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }

        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncExperience Commit",
        //            "Problem Commiting SyncExperience Method", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncHawala(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<EmployeeHawalaDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<EmployeeHawalaDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var exprs = sourceUnitOfWork.Repository<EmployeeHawalaDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in exprs)
        //    {
        //        _updatesFound = true;
        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<EmployeeHawalaDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var id = 0;
        //        if (destination == null)
        //            destination = new EmployeeHawalaDTO();
        //        else
        //            id = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<EmployeeHawalaDTO, EmployeeHawalaDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = id;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncHawala Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<EmployeeHawalaDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncHawala Crud",
        //                "Problem On SyncHawala Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncHawala Commit",
        //            "Problem Commiting SyncHawala Method", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncInsurance(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<InsuranceProcessDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<InsuranceProcessDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var exprs = sourceUnitOfWork.Repository<InsuranceProcessDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in exprs)
        //    {
        //        _updatesFound = true;
        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<InsuranceProcessDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var id = 0;
        //        if (destination == null)
        //            destination = new InsuranceProcessDTO();
        //        else
        //            id = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<InsuranceProcessDTO, InsuranceProcessDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = id;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncInsurance Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<InsuranceProcessDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncInsurance Crud",
        //                "Problem On SyncInsurance Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncInsurance Commit",
        //            "Problem Commiting SyncInsurance Method", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncLabour(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<LabourProcessDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<LabourProcessDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }

        //    var labourProcessDtos = sourceUnitOfWork.Repository<LabourProcessDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in labourProcessDtos)
        //    {
        //        _updatesFound = true;
        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<LabourProcessDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var id = 0;
        //        if (destination == null)
        //            destination = new LabourProcessDTO();
        //        else
        //            id = destination.Id;
        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<LabourProcessDTO, LabourProcessDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = id;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncLabour Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<LabourProcessDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncLabour Crud",
        //                "Problem On SyncLabour Crud Method", UserName, Agency);
        //            return false;
        //        }

        //        if (!string.IsNullOrEmpty(source.AgreementFileName))
        //        {
        //            var dest = PathUtil.GetDestinationAgreementsPath();
        //            var agreementPath = PathUtil.GetAgreementPath();
        //            var fiName = source.AgreementFileName;

        //            if (Singleton.BuildType == BuildType.LocalDev)
        //                File.Copy(Path.Combine(agreementPath, fiName), Path.Combine(dest, fiName), true);
        //            else
        //            {
        //                using (var client = new WebClient())
        //                {
        //                    client.Credentials = DbCommandUtil.GetNetworkCredential();
        //                    client.UploadFile(Path.Combine(dest, fiName), WebRequestMethods.Ftp.UploadFile,
        //                        Path.Combine(agreementPath, fiName));
        //                }
        //            }
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncLabour Commit",
        //            "Problem Commiting SyncLabour Method", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncEmbassy(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<EmbassyProcessDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<EmbassyProcessDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var embassyProcessDtos = sourceUnitOfWork.Repository<EmbassyProcessDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in embassyProcessDtos)
        //    {
        //        _updatesFound = true;
        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<EmbassyProcessDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var id = 0;
        //        if (destination == null)
        //            destination = new EmbassyProcessDTO();
        //        else
        //            id = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<EmbassyProcessDTO, EmbassyProcessDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = id;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncEmbassy Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<EmbassyProcessDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncEmbassy Crud",
        //                "Problem On SyncEmbassy Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncEmbassy Commit",
        //            "Problem Commiting SyncEmbassy Method", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncFlight(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<FlightProcessDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<FlightProcessDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var flightProcessDtos = sourceUnitOfWork.Repository<FlightProcessDTO>().Query()
        //        .Include(a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    foreach (var source in flightProcessDtos)
        //    {
        //        _updatesFound = true;
        //        var adr1 = source;
        //        var destination =
        //            destinationUnitOfWork.Repository<FlightProcessDTO>().Query()
        //                .Filter(i => i.RowGuid == adr1.RowGuid)
        //                .Get(1)
        //                .FirstOrDefault();

        //        var id = 0;
        //        if (destination == null)
        //            destination = new FlightProcessDTO();
        //        else
        //            id = destination.Id;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<FlightProcessDTO, FlightProcessDTO>()
        //                .ForMember("Agency", option => option.Ignore())
        //                .ForMember("AgencyId", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.Id = id;

        //            destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //            destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncFlight Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            #region Foreign Keys

        //            var agencyDTO =
        //                destLocalAgencies.FirstOrDefault(
        //                    c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //            {
        //                destination.Agency = agencyDTO;
        //                destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //            }

        //            #endregion

        //            destination.Synced = true;
        //            destinationUnitOfWork.Repository<FlightProcessDTO>()
        //                .InsertUpdate(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncFlight Crud",
        //                "Problem On SyncFlight Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncFlight Commit",
        //            "Problem Commiting SyncFlight Method", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncEmployees(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<EmployeeDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<EmployeeDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var sourceList = sourceUnitOfWork.Repository<EmployeeDTO>().Query()
        //        .Include(c => c.Photo, c => c.StandPhoto, c => c.Address, c => c.RequiredDocuments)
        //        .Include(c => c.Education, c => c.Experience, c => c.Hawala, c => c.Agency, c => c.Agent)
        //        .Include(c => c.Visa, c => c.InsuranceProcess, c => c.LabourProcess, c => c.EmbassyProcess,
        //            c => c.FlightProcess)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destAgents =
        //            destinationUnitOfWork.Repository<AgentDTO>().Query()
        //                .Get(1)
        //                .ToList();
        //        var destLocalAgencies =
        //            destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destAddresses =
        //            destinationUnitOfWork.Repository<AddressDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destVisas =
        //            destinationUnitOfWork.Repository<VisaDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destExperiences =
        //            destinationUnitOfWork.Repository<EmployeeExperienceDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destEducations =
        //            destinationUnitOfWork.Repository<EmployeeEducationDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destHawalas =
        //            destinationUnitOfWork.Repository<EmployeeHawalaDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destRequiredDocuments =
        //            destinationUnitOfWork.Repository<RequiredDocumentsDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destFlights =
        //            destinationUnitOfWork.Repository<FlightProcessDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destEmbassies =
        //            destinationUnitOfWork.Repository<EmbassyProcessDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destLabours =
        //            destinationUnitOfWork.Repository<LabourProcessDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destInsurances =
        //            destinationUnitOfWork.Repository<InsuranceProcessDTO>().Query()
        //                .Get(1)
        //                .ToList();
        //        var destPhotos =
        //            destinationUnitOfWork.Repository<AttachmentDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<EmployeeDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            if (destination == null)
        //                destination = new EmployeeDTO();
        //            else if (ToServerSyncing && !destination.Synced)
        //                continue;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<EmployeeDTO, EmployeeDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("Agent", option => option.Ignore())
        //                    .ForMember("Id", option => option.Ignore())
        //                    .ForMember("Address", option => option.Ignore())
        //                    .ForMember("Visa", option => option.Ignore())
        //                    .ForMember("Experience", option => option.Ignore())
        //                    .ForMember("Education", option => option.Ignore())
        //                    .ForMember("Hawala", option => option.Ignore())
        //                    .ForMember("FlightProcess", option => option.Ignore())
        //                    .ForMember("EmbassyProcess", option => option.Ignore())
        //                    .ForMember("LabourProcess", option => option.Ignore())
        //                    .ForMember("InsuranceProcess", option => option.Ignore())
        //                    .ForMember("EmployeeRelatives", option => option.Ignore())
        //                    .ForMember("ContactPerson", option => option.Ignore())
        //                    .ForMember("ContactPersonId", option => option.Ignore())
        //                    .ForMember("Complains", option => option.Ignore())
        //                    .ForMember("CurrentComplain", option => option.Ignore())
        //                    .ForMember("CurrentComplainId", option => option.Ignore())
        //                    .ForMember("Photo", option => option.Ignore())
        //                    .ForMember("RequiredDocuments", option => option.Ignore())
        //                    .ForMember("StandPhoto", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);
        //                //destEmployeeDto.Id = destEmployeeId; we don't need if this is added .ForMember("Id", option => option.Ignore())
        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var categoryDTO =
        //                    destAgents.FirstOrDefault(
        //                        c => source.Agent != null && c.RowGuid == source.Agent.RowGuid);
        //                {
        //                    destination.Agent = categoryDTO;
        //                    destination.AgentId = categoryDTO != null ? categoryDTO.Id : (int?) null;
        //                }

        //                var agencyDTO =
        //                    destLocalAgencies.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }

        //                if (source.AddressId != null)
        //                {
        //                    var categoryDto =
        //                        destAddresses.FirstOrDefault(
        //                            c => source.Address != null && c.RowGuid == source.Address.RowGuid);
        //                    {
        //                        destination.Address = categoryDto;
        //                        destination.AddressId = categoryDto != null ? categoryDto.Id : (int?) null;
        //                    }
        //                }
        //                if (source.VisaId != null)
        //                {
        //                    var headerDto =
        //                        destVisas.FirstOrDefault(c => source.Visa != null && c.RowGuid == source.Visa.RowGuid);
        //                    {
        //                        destination.Visa = headerDto;
        //                        destination.VisaId = headerDto != null ? headerDto.Id : (int?) null;
        //                    }
        //                }
        //                if (source.ExperienceId != null)
        //                {
        //                    var footerDto =
        //                        destExperiences.FirstOrDefault(
        //                            c => source.Experience != null && c.RowGuid == source.Experience.RowGuid);
        //                    {
        //                        destination.Experience = footerDto;
        //                        destination.ExperienceId = footerDto != null ? footerDto.Id : (int?) null;
        //                    }
        //                }
        //                if (source.EducationId != null)
        //                {
        //                    var footerDto1 =
        //                        destEducations.FirstOrDefault(
        //                            c => source.Education != null && c.RowGuid == source.Education.RowGuid);
        //                    {
        //                        destination.Education = footerDto1;
        //                        destination.EducationId = footerDto1 != null ? footerDto1.Id : (int?) null;
        //                    }
        //                }
        //                if (source.HawalaId != null)
        //                {
        //                    var footerDto2 =
        //                        destHawalas.FirstOrDefault(
        //                            c => source.Hawala != null && c.RowGuid == source.Hawala.RowGuid);
        //                    {
        //                        destination.Hawala = footerDto2;
        //                        destination.HawalaId = footerDto2 != null ? footerDto2.Id : (int?) null;
        //                    }
        //                }
        //                if (source.RequiredDocumentsId != null)
        //                {
        //                    var footerDto2 =
        //                        destRequiredDocuments.FirstOrDefault(
        //                            c =>
        //                                source.RequiredDocuments != null &&
        //                                c.RowGuid == source.RequiredDocuments.RowGuid);
        //                    {
        //                        destination.RequiredDocuments = footerDto2;
        //                        destination.RequiredDocumentsId = footerDto2 != null ? footerDto2.Id : (int?) null;
        //                    }
        //                }
        //                if (source.FlightProcessId != null)
        //                {
        //                    var flight =
        //                        destFlights.FirstOrDefault(
        //                            c => source.FlightProcess != null && c.RowGuid == source.FlightProcess.RowGuid);
        //                    {
        //                        destination.FlightProcess = flight;
        //                        destination.FlightProcessId = flight != null ? flight.Id : (int?) null;
        //                    }
        //                }
        //                if (source.EmbassyProcessId != null)
        //                {
        //                    var embassy =
        //                        destEmbassies.FirstOrDefault(
        //                            c => source.EmbassyProcess != null && c.RowGuid == source.EmbassyProcess.RowGuid);
        //                    {
        //                        destination.EmbassyProcess = embassy;
        //                        destination.EmbassyProcessId = embassy != null ? embassy.Id : (int?) null;
        //                    }
        //                }
        //                if (source.LabourProcessId != null)
        //                {
        //                    var labour =
        //                        destLabours.FirstOrDefault(
        //                            c => source.LabourProcess != null && c.RowGuid == source.LabourProcess.RowGuid);
        //                    {
        //                        destination.LabourProcess = labour;
        //                        destination.LabourProcessId = labour != null ? labour.Id : (int?) null;
        //                    }
        //                }
        //                if (source.InsuranceProcessId != null)
        //                {
        //                    var insurance =
        //                        destInsurances.FirstOrDefault(
        //                            c => source.InsuranceProcess != null && c.RowGuid == source.InsuranceProcess.RowGuid);
        //                    {
        //                        destination.InsuranceProcess = insurance;
        //                        destination.InsuranceProcessId = insurance != null ? insurance.Id : (int?) null;
        //                    }
        //                }
        //                if (source.PhotoId != null)
        //                {
        //                    var photo =
        //                        destPhotos.FirstOrDefault(c => source.Photo != null && c.RowGuid == source.Photo.RowGuid);
        //                    {
        //                        destination.Photo = photo;
        //                        destination.PhotoId = photo != null ? photo.Id : (int?) null;
        //                    }
        //                }
        //                if (source.StandPhotoId != null)
        //                {
        //                    var standPhoto =
        //                        destPhotos.FirstOrDefault(
        //                            c => source.StandPhoto != null && c.RowGuid == source.StandPhoto.RowGuid);
        //                    {
        //                        destination.StandPhoto = standPhoto;
        //                        destination.StandPhotoId = standPhoto != null ? standPhoto.Id : (int?) null;
        //                    }
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<EmployeeDTO>()
        //                    .InsertUpdate(destination);
        //            }
        //            catch (Exception ex)
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees Crud",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees Commit",
        //                "Problem Commiting SyncEmployees Method", UserName, Agency);
        //            return false;
        //        }
        //    }


        //    return true;
        //}

        //public bool SyncRelatives(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<EmployeeRelativeDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<EmployeeRelativeDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var sourceList = sourceUnitOfWork.Repository<EmployeeRelativeDTO>().Query()
        //        .Include(a => a.Agency, a => a.Address, a => a.Employee)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destAddresses =
        //            destinationUnitOfWork.Repository<AddressDTO>().Query()
        //                .Get(1)
        //                .ToList();
        //        var destEmployees =
        //            destinationUnitOfWork.Repository<EmployeeDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<EmployeeRelativeDTO>().Query()
        //                .Include(a => a.Address)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            if (destination == null)
        //                destination = new EmployeeRelativeDTO();

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<EmployeeRelativeDTO, EmployeeRelativeDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("AgencyId", option => option.Ignore())
        //                    .ForMember("Address", option => option.Ignore())
        //                    .ForMember("Id", option => option.Ignore())
        //                    .ForMember("Employee", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());

        //                destination = Mapper.Map(source, destination);
        //                //clients.Id = clientId;
        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncRelatives Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var agencyDTO =
        //                    destLocalAgencies.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }


        //                var categoryDTO =
        //                    destAddresses.FirstOrDefault(
        //                        c => source.Address != null && c.RowGuid == source.Address.RowGuid);
        //                {
        //                    destination.Address = categoryDTO;
        //                    destination.AddressId = categoryDTO != null ? categoryDTO.Id : (int?) null;
        //                }

        //                var employeeDto =
        //                    destEmployees.FirstOrDefault(
        //                        c => source.Employee != null && c.RowGuid == source.Employee.RowGuid);
        //                {
        //                    destination.Employee = employeeDto;
        //                    destination.EmployeeId = employeeDto != null ? employeeDto.Id : (int?) null;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<EmployeeRelativeDTO>()
        //                    .InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncRelatives Crud",
        //                    "Problem On SyncRelatives Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncRelatives Commit",
        //                "Problem Commiting SyncRelatives Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncComplains(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<ComplainDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<ComplainDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var sourceList = sourceUnitOfWork.Repository<ComplainDTO>().Query()
        //        .Include(a => a.Employee, a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destEmployees =
        //            destinationUnitOfWork.Repository<EmployeeDTO>().Query()
        //                .Get(1).ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<ComplainDTO>().Query()
        //                .Include(a => a.Employee)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            //To Prevent ServerData Overriding
        //            if (destination == null)
        //                destination = new ComplainDTO();
        //            else if (ToServerSyncing && !destination.Synced)
        //                continue;
        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<ComplainDTO, ComplainDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("AgencyId", option => option.Ignore())
        //                    .ForMember("Id", option => option.Ignore())
        //                    .ForMember("RowVersion", option => option.Ignore())
        //                    .ForMember("Employee", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());

        //                destination = Mapper.Map(source, destination);

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncComplains Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var agencyDTO =
        //                    destLocalAgencies.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }


        //                var employeeDto =
        //                    destEmployees.FirstOrDefault(
        //                        c => source.Employee != null && c.RowGuid == source.Employee.RowGuid);
        //                {
        //                    destination.Employee = employeeDto;
        //                    destination.EmployeeId = employeeDto != null ? employeeDto.Id : 1;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<ComplainDTO>()
        //                    .InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncComplains Crud",
        //                    "Problem On SyncComplains Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncComplains Commit",
        //                "Problem Commiting SyncComplains Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncComplainRemarks(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    Expression<Func<ComplainRemarkDTO, bool>> filter =
        //        a => !a.Synced && a.DateLastModified > LastServerSyncDate;

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<ComplainRemarkDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }
        //    var sourceList = sourceUnitOfWork.Repository<ComplainRemarkDTO>().Query()
        //        .Include(a => a.Complain, a => a.Agency)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    var destLocalAgencies =
        //        destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //            .Get(1)
        //            .ToList();
        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destEmployees =
        //            destinationUnitOfWork.Repository<ComplainDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<ComplainRemarkDTO>().Query()
        //                .Include(a => a.Complain)
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            //To Prevent ServerData Overriding
        //            if (destination == null)
        //                destination = new ComplainRemarkDTO();
        //            else if (ToServerSyncing && !destination.Synced)
        //                continue;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<ComplainRemarkDTO, ComplainRemarkDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("AgencyId", option => option.Ignore())
        //                    .ForMember("Id", option => option.Ignore())
        //                    .ForMember("Complain", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());

        //                destination = Mapper.Map(source, destination);

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncComplainRemarks Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var agencyDTO =
        //                    destLocalAgencies.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }


        //                var employeeDto =
        //                    destEmployees.FirstOrDefault(
        //                        c => source.Complain != null && c.RowGuid == source.Complain.RowGuid);
        //                {
        //                    destination.Complain = employeeDto;
        //                    destination.ComplainId = employeeDto != null ? employeeDto.Id : 1;
        //                }

        //                #endregion

        //                destination.Synced = true;
        //                destinationUnitOfWork.Repository<ComplainRemarkDTO>()
        //                    .InsertUpdate(destination);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncComplainRemarks Crud",
        //                    "Problem On SyncComplainRemarks Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncComplainRemarks Commit",
        //                "Problem Commiting SyncComplainRemarks Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncEmployees2(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork, bool fromServer)
        //{
        //    Expression<Func<EmployeeDTO, bool>> filter = a =>
        //        !a.Synced && a.DateLastModified > LastServerSyncDate &&
        //        (a.CurrentComplain != null || a.ContactPerson != null);

        //    if (!ToServerSyncing)
        //    {
        //        Expression<Func<EmployeeDTO, bool>> filter2 =
        //            a => a.Agency != null &&
        //                 a.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        filter = filter.And(filter2);
        //    }

        //    var sourceList = sourceUnitOfWork.Repository<EmployeeDTO>().Query()
        //        .Include(c => c.CurrentComplain, c => c.ContactPerson)
        //        .Filter(filter)
        //        .Get(1)
        //        .ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;

        //        IList<EmployeeDTO> destEmployeesTemp = new List<EmployeeDTO>();

        //        var destComplains =
        //            destinationUnitOfWork.Repository<ComplainDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destContactPersons =
        //            destinationUnitOfWork.Repository<EmployeeRelativeDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.Repository<EmployeeDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            //To Prevent ServerData Overriding
        //            if (destination == null)
        //            {
        //                continue;
        //                //destination = new EmployeeDTO();
        //            }
        //            if (ToServerSyncing && !destination.Synced)
        //                continue;

        //            try
        //            {
        //                #region Mapping

        //                Mapper.Reset();
        //                Mapper.CreateMap<EmployeeDTO, EmployeeDTO>()
        //                    .ForMember("AgencyId", option => option.Ignore())
        //                    .ForMember("AgentId", option => option.Ignore())
        //                    .ForMember("Id", option => option.Ignore())
        //                    .ForMember("AddressId", option => option.Ignore())
        //                    .ForMember("RequiredDocumentsId", option => option.Ignore())
        //                    .ForMember("VisaId", option => option.Ignore())
        //                    .ForMember("ExperienceId", option => option.Ignore())
        //                    .ForMember("EducationId", option => option.Ignore())
        //                    .ForMember("HawalaId", option => option.Ignore())
        //                    .ForMember("FlightProcessId", option => option.Ignore())
        //                    .ForMember("EmbassyProcessId", option => option.Ignore())
        //                    .ForMember("LabourProcessId", option => option.Ignore())
        //                    .ForMember("InsuranceProcessId", option => option.Ignore())
        //                    .ForMember("ContactPerson", option => option.Ignore())
        //                    .ForMember("ContactPersonId", option => option.Ignore())
        //                    .ForMember("EmployeeRelatives", option => option.Ignore())
        //                    .ForMember("CurrentComplain", option => option.Ignore())
        //                    .ForMember("CurrentComplainId", option => option.Ignore())
        //                    .ForMember("Complains", option => option.Ignore())
        //                    .ForMember("PhotoId", option => option.Ignore())
        //                    .ForMember("StandPhotoId", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());

        //                destination = Mapper.Map(source, destination);

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);

        //                #endregion
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees2 Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                if (source.CurrentComplainId != null)
        //                {
        //                    var footerDto2 =
        //                        destComplains.FirstOrDefault(
        //                            c =>
        //                                source.CurrentComplain != null &&
        //                                c.RowGuid == source.CurrentComplain.RowGuid);
        //                    {
        //                        destination.CurrentComplain = footerDto2;
        //                        destination.CurrentComplainId = footerDto2 != null ? footerDto2.Id : (int?) null;
        //                    }
        //                }
        //                if (source.ContactPersonId != null) //&& destEmployeeDto.ContactPerson == null
        //                {
        //                    var footerDto2 =
        //                        destContactPersons.FirstOrDefault(c => source.ContactPerson != null
        //                                                               &&
        //                                                               c.RowGuid ==
        //                                                               source.ContactPerson.RowGuid);
        //                    {
        //                        destination.ContactPerson = footerDto2;
        //                        destination.ContactPersonId = footerDto2 != null ? footerDto2.Id : (int?) null;
        //                    }
        //                }

        //                #endregion

        //                //destination.Synced = true;
        //                destEmployeesTemp.Add(destination);
        //                //destinationUnitOfWork.Repository<EmployeeDTO>()
        //                //    .InsertUpdate(destEmployeeDto);
        //            }
        //            catch
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees2 Crud",
        //                    "Problem On SyncEmployees2 Crud Method", UserName, Agency);
        //                return false;
        //            }
        //        }


        //        destinationUnitOfWork = fromServer
        //            ? GetNewUow2(destinationUnitOfWork)
        //            : GetNewUow(destinationUnitOfWork);

        //        foreach (var destEmployeeTemp in destEmployeesTemp)
        //        {
        //            var destEmployee =
        //                destinationUnitOfWork.Repository<EmployeeDTO>().FindById(destEmployeeTemp.Id);
        //            destEmployee.ContactPersonId = destEmployeeTemp.ContactPersonId;
        //            destEmployee.CurrentComplainId = destEmployeeTemp.CurrentComplainId;
        //            destEmployee.Synced = true;
        //            destinationUnitOfWork.Repository<EmployeeDTO>().InsertUpdate(destEmployee);
        //        }

        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncEmployees2 Commit",
        //                "Problem Commiting SyncEmployees2 Method", UserName, Agency);
        //        }
        //    }
        //    return true;
        //}

        //public bool SyncUsers(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList = sourceUnitOfWork.UserRepository<UserDTO>().Query()
        //        .Include(h => h.Agency)
        //        .Filter(a => !(bool) a.Synced && a.DateLastModified > LastServerSyncDate)
        //        .Get(1).ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destLocalAgencies =
        //            destinationUnitOfWork.Repository<AgencyDTO>().Query()
        //                .Get(1)
        //                .ToList();

        //        var destList =
        //            destinationUnitOfWork.UserRepository<UserDTO>()
        //                .Query()
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            var userId = 0;
        //            if (destination == null)
        //                destination = new UserDTO();
        //            else
        //                userId = destination.UserId;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<UserDTO, UserDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("AgenciesWithAgents", option => option.Ignore())
        //                    .ForMember("Agent", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());

        //                destination = Mapper.Map(source, destination);
        //                destination.UserId = userId;
        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                #region Foreign Keys

        //                var agencyDTO =
        //                    destLocalAgencies.FirstOrDefault(
        //                        c => source.Agency != null && c.RowGuid == source.Agency.RowGuid);
        //                {
        //                    destination.Agency = agencyDTO;
        //                    destination.AgencyId = agencyDTO != null ? agencyDTO.Id : (int?) null;
        //                }

        //                #endregion

        //                destination.Synced = true;

        //                if (userId == 0)
        //                    destinationUnitOfWork.UserRepository<UserDTO>()
        //                        .Insert(destination);
        //                else
        //                    destinationUnitOfWork.UserRepository<UserDTO>()
        //                        .Update(destination);
        //            }
        //            catch (Exception ex)
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Crud",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Commit",
        //                "Problem Commiting SyncUsers Method", UserName, Agency);
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public bool SyncUsers2(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList = sourceUnitOfWork.UserRepository<UserDTO>().Query()
        //        .Filter(a => !(bool) a.Synced && a.DateLastModified > LastServerSyncDate)
        //        .Get(1).ToList();

        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;

        //        var destList =
        //            destinationUnitOfWork.UserRepository<UserDTO>()
        //                .Query()
        //                .Get(1)
        //                .ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            if (destination == null)
        //                destination = new UserDTO();
        //            else
        //                continue;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<UserDTO, UserDTO>()
        //                    .ForMember("Agency", option => option.Ignore())
        //                    .ForMember("AgencyId", option => option.Ignore())
        //                    .ForMember("AgenciesWithAgents", option => option.Ignore())
        //                    .ForMember("Agent", option => option.Ignore())
        //                    .ForMember("AgentId", option => option.Ignore())
        //                    .ForMember("Synced", option => option.Ignore());

        //                destination = Mapper.Map(source, destination);
        //                destination.Agency = null;
        //                destination.AgencyId = null;
        //                destination.CreatedByUserId = 1;
        //                destination.ModifiedByUserId = 1;
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                destination.Synced = true;

        //                //if (userId == 0)
        //                destinationUnitOfWork.UserRepository<UserDTO>()
        //                    .Insert(destination);
        //                //else
        //                //    destinationUnitOfWork.UserRepository<UserDTO>()
        //                //        .Update(destination);
        //            }
        //            catch (Exception ex)
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Crud",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //                return false;
        //            }
        //        }
        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncUsers Commit",
        //                "Problem Commiting SyncUsers Method", UserName, Agency);
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public bool SyncMemberships(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList = sourceUnitOfWork.UserRepository<MembershipDTO>().Query()
        //        //.Filter(a => a.DateLastModified > LastServerSyncDate)
        //        .Get(1).ToList();
        //    var sourceUsers = sourceUnitOfWork.UserRepository<UserDTO>().Query()
        //        .Get(1).ToList();
        //    if (sourceList.Any())
        //    {
        //        //_updatesFound = true;
        //        var destUsers =
        //            destinationUnitOfWork.UserRepository<UserDTO>().Query().Get(1).ToList();
        //        //var destRoles = destinationUnitOfWork.UserRepository<RoleDTO>().Query().Get(1).ToList();

        //        var destList =
        //            destinationUnitOfWork.UserRepository<MembershipDTO>().Query()
        //                .Get(1).ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var destination =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            var id = 0;
        //            if (destination == null)
        //                destination = new MembershipDTO();
        //            else
        //                id = destination.UserId;

        //            try
        //            {
        //                Mapper.Reset();
        //                Mapper.CreateMap<MembershipDTO, MembershipDTO>()
        //                    .ForMember("Synced", option => option.Ignore());
        //                destination = Mapper.Map(source, destination);
        //                destination.Id = id;

        //                destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //                destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                    sourceUnitOfWork, destinationUnitOfWork);
        //            }
        //            catch (Exception ex)
        //            {
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Mapping",
        //                    ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //            }
        //            try
        //            {
        //                var userguid = sourceUsers.FirstOrDefault(c => c.UserId == source.UserId);
        //                var userDto =
        //                    destUsers.FirstOrDefault(c => userguid != null && c.RowGuid == userguid.RowGuid);
        //                {
        //                    //users.User = userDto;
        //                    destination.UserId = userDto != null ? userDto.UserId : 1;
        //                }
        //                if (id == 0)
        //                    destinationUnitOfWork.UserRepository<MembershipDTO>()
        //                        .Insert(destination);
        //                else
        //                    destinationUnitOfWork.UserRepository<MembershipDTO>()
        //                        .Update(destination);
        //            }
        //            catch
        //            {
        //                //_errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Crud",
        //                    "Problem On SyncMemberships Crud Method", UserName, Agency);
        //                //return false;
        //            }
        //        }

        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncMemberships Commit",
        //                "Problem Commiting SyncMemberships Method", UserName, Agency);
        //            return false;
        //        }
        //    }

        //    return true;
        //}

        //public bool SyncRoles(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var roles = sourceUnitOfWork.UserRepository<RoleDTO>().Query()
        //        .Filter(a => !(bool) a.Synced && a.DateLastModified > LastServerSyncDate)
        //        .Get(1).ToList();

        //    var destList =
        //        destinationUnitOfWork.UserRepository<RoleDTO>().Query()
        //            .Get(1)
        //            .ToList();

        //    foreach (var source in roles)
        //    {
        //        _updatesFound = true;
        //        var destination = destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //        var id = 0;
        //        if (destination == null)
        //            destination = new RoleDTO();
        //        else
        //            id = destination.RoleId;

        //        try
        //        {
        //            Mapper.Reset();
        //            Mapper.CreateMap<RoleDTO, RoleDTO>()
        //                .ForMember("RoleId", option => option.Ignore())
        //                .ForMember("Users", option => option.Ignore())
        //                .ForMember("Synced", option => option.Ignore());
        //            destination = Mapper.Map(source, destination);
        //            destination.RoleId = id;

        //            //destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId, sourceUnitOfWork, destinationUnitOfWork);
        //            //destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId, sourceUnitOfWork, destinationUnitOfWork);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncRoles Mapping",
        //                ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //        }
        //        try
        //        {
        //            destination.Synced = true;
        //            if (id == 0)
        //                destinationUnitOfWork.UserRepository<RoleDTO>()
        //                    .Insert(destination);
        //            else
        //                destinationUnitOfWork.UserRepository<RoleDTO>()
        //                    .Update(destination);
        //        }
        //        catch
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncRoles Crud",
        //                "Problem On SyncRoles Crud Method", UserName, Agency);
        //            return false;
        //        }
        //    }
        //    var changes = destinationUnitOfWork.Commit();
        //    if (changes < 0)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncRoles Commit",
        //            "Problem Commiting SyncRoles Method", UserName, Agency);
        //        return false;
        //    }

        //    return true;
        //}

        //public bool SyncUsersInRoles(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    try
        //    {
        //        #region Sync UsersInRoles

        //        Expression<Func<UsersInRoles, bool>> filter =
        //            a => !(bool) a.Synced && a.DateLastModified > LastServerSyncDate;


        //        if (!ToServerSyncing)
        //        {
        //            filter =
        //                a => !(bool) a.Synced &&
        //                     a.DateLastModified > LastServerSyncDate &&
        //                     a.User.Agency != null &&
        //                     a.User.Agency.RowGuid == Singleton.Agency.RowGuid;
        //        }

        //        var sourceList = sourceUnitOfWork.UserRepository<UsersInRoles>()
        //            .Query()
        //            .Include(i => i.User, i => i.User.Agency, i => i.Role)
        //            .Filter(filter)
        //            .Get(1)
        //            .ToList();

        //        if (sourceList.Any())
        //        {
        //            _updatesFound = true;
        //            var destUsers =
        //                destinationUnitOfWork.UserRepository<UserDTO>().Query().Get(1).ToList();

        //            var destRoles =
        //                destinationUnitOfWork.UserRepository<RoleDTO>().Query().Get(1).ToList();

        //            var destList =
        //                destinationUnitOfWork.UserRepository<UsersInRoles>().Query()
        //                    .Include(i => i.User, i => i.Role).Get(1).ToList();

        //            foreach (var source in sourceList)
        //            {
        //                var usersInRoles =
        //                    destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //                if (usersInRoles == null)
        //                {
        //                    usersInRoles = new UsersInRoles
        //                    {
        //                        RowGuid = source.RowGuid
        //                    };

        //                    try
        //                    {
        //                        #region Foreign Keys

        //                        var userDto =
        //                            destUsers.FirstOrDefault(
        //                                c => source.User != null && c.RowGuid == source.User.RowGuid);
        //                        {
        //                            //users.User = userDto;
        //                            usersInRoles.UserId = userDto != null ? userDto.UserId : 1;
        //                        }
        //                        var roleDto =
        //                            destRoles.FirstOrDefault(
        //                                c => source.Role != null && c.RowGuid == source.Role.RowGuid);
        //                        {
        //                            //users.Role = roleDto;
        //                            usersInRoles.RoleId = roleDto != null ? roleDto.RoleId : 1;
        //                        }

        //                        #endregion

        //                        var isFound = false;
        //                        var destination =
        //                            destList.FirstOrDefault(
        //                                i => i.UserId == usersInRoles.UserId && i.RoleId == usersInRoles.RoleId);

        //                        if (destination == null)
        //                            destination = usersInRoles;
        //                        else
        //                        {
        //                            isFound = true;
        //                            destination.RowGuid = source.RowGuid;
        //                        }

        //                        destination.Synced = true;

        //                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                            sourceUnitOfWork, destinationUnitOfWork);
        //                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                            sourceUnitOfWork, destinationUnitOfWork);

        //                        if (isFound)
        //                            destinationUnitOfWork.UserRepository<UsersInRoles>()
        //                                .Update(destination);
        //                        else
        //                            destinationUnitOfWork.UserRepository<UsersInRoles>()
        //                                .Insert(destination);
        //                    }
        //                    catch
        //                    {
        //                        _errorsFound = true;
        //                        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsersInRoles Crud",
        //                            "Problem On SyncUsersInRoles Crud Method", UserName, Agency);
        //                        return false;
        //                    }
        //                }
        //            }

        //            var changes = destinationUnitOfWork.Commit();
        //            if (changes < 0)
        //            {
        //                _errorsFound = true;
        //                LogUtil.LogError(ErrorSeverity.Critical, "SyncUsersInRoles Commit",
        //                    "Problem Commiting SyncUsersInRoles Method", UserName, Agency);
        //                return false;
        //            }
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorsFound = true;
        //        LogUtil.LogError(ErrorSeverity.Critical, "SyncUsersInRoles",
        //            ex.Message + Environment.NewLine + ex.InnerException + Environment.NewLine +
        //            "Problem On SyncUsersInRoles", UserName, Agency);
        //        return false;
        //    }
        //    return true;
        //}

        //public bool SyncUserWithAgencyWithAgentDTO(IUnitOfWork sourceUnitOfWork,
        //    IUnitOfWork destinationUnitOfWork)
        //{
        //    var sourceList =
        //        sourceUnitOfWork.UserRepository<UserAgencyAgentDTO>()
        //            .Query()
        //            .Include(i => i.User, i => i.AgencyAgent)
        //            .Filter(a => !(bool) a.Synced && a.DateLastModified > LastServerSyncDate)
        //            .Get(1).ToList();
        //    if (sourceList.Any())
        //    {
        //        _updatesFound = true;
        //        var destUsers =
        //            destinationUnitOfWork.UserRepository<UserDTO>().Query().Get(1).ToList();

        //        var destAgencyWithAgent =
        //            destinationUnitOfWork.Repository<AgencyAgentDTO>().Query().Get(1).ToList();

        //        var destList =
        //            destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>().Query()
        //                .Include(i => i.User, i => i.AgencyAgent).Get(1).ToList();

        //        foreach (var source in sourceList)
        //        {
        //            var usersInRoles =
        //                destList.FirstOrDefault(i => i.RowGuid == source.RowGuid);

        //            if (usersInRoles == null)
        //            {
        //                usersInRoles = new UserAgencyAgentDTO
        //                {
        //                    RowGuid = source.RowGuid
        //                };

        //                try
        //                {
        //                    #region Foreign Keys

        //                    var userDto =
        //                        destUsers.FirstOrDefault(c => source.User != null && c.RowGuid == source.User.RowGuid);
        //                    {
        //                        usersInRoles.UserId = userDto != null ? userDto.UserId : 1;
        //                    }
        //                    var roleDto =
        //                        destAgencyWithAgent.FirstOrDefault(
        //                            c => source.AgencyAgent != null && c.RowGuid == source.AgencyAgent.RowGuid);
        //                    {
        //                        usersInRoles.AgencyWithAgentId = roleDto != null ? roleDto.Id : 1;
        //                    }

        //                    #endregion

        //                    var isFound = false;
        //                    var destination =
        //                        destList.FirstOrDefault(
        //                            i =>
        //                                i.UserId == usersInRoles.UserId &&
        //                                i.AgencyWithAgentId == usersInRoles.AgencyWithAgentId);
        //                    if (destination == null)
        //                        destination = usersInRoles;
        //                    else
        //                    {
        //                        isFound = true;
        //                        destination.RowGuid = source.RowGuid;
        //                    }

        //                    destination.Synced = true;
        //                    destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
        //                        sourceUnitOfWork, destinationUnitOfWork);
        //                    destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
        //                        sourceUnitOfWork, destinationUnitOfWork);


        //                    if (isFound)
        //                        destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>()
        //                            .Update(destination);
        //                    else
        //                        destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>()
        //                            .Insert(destination);

        //                    //destinationUnitOfWork.UserRepository<UserAgencyAgentDTO>()
        //                    //    .CrudByRowGuid(destination);
        //                }
        //                catch (Exception ex)
        //                {
        //                    _errorsFound = true;
        //                    LogUtil.LogError(ErrorSeverity.Critical, "SyncUserWithAgencyWithAgentDTO Crud",
        //                        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
        //                    return false;
        //                }
        //            }
        //        }

        //        var changes = destinationUnitOfWork.Commit();
        //        if (changes < 0)
        //        {
        //            _errorsFound = true;
        //            LogUtil.LogError(ErrorSeverity.Critical, "SyncUserWithAgencyWithAgentDTO Commit",
        //                "Problem Commiting SyncUserWithAgencyWithAgentDTO Method", UserName, Agency);
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}