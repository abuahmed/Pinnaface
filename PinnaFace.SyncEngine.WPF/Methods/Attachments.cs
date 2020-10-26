using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {

        public bool SyncAttachments(IUnitOfWork sourceUnitOfWork,
            IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<AttachmentDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<AttachmentDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }
            var attachmentDtos = sourceUnitOfWork.Repository<AttachmentDTO>().Query()
                .Include(a => a.Agency)
                .Filter(filter)
                .Get(1)
                .ToList();

            var destLocalAgencies =
                destinationUnitOfWork.Repository<AgencyDTO>().Query()
                .Filter(a => a.Id == Singleton.Agency.Id)
                    .Get(1)
                    .ToList();

            foreach (var source in attachmentDtos)
            {
                _updatesFound = true;

                var adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<AttachmentDTO>().Query()
                        .Filter(i => i.RowGuid == adr1.RowGuid)
                        .Get(1)
                        .FirstOrDefault();

                var clientId = 0;
                if (destination == null)
                    destination = new AttachmentDTO();
                else
                    clientId = destination.Id;

                try
                {
                    Mapper.Reset();
                    Mapper.CreateMap<AttachmentDTO, AttachmentDTO>()
                        .ForMember("Agency", option => option.Ignore())
                        .ForMember("AgencyId", option => option.Ignore())
                        .ForMember("Synced", option => option.Ignore())
                        .ForMember("AttachedFile", option => option.Ignore());
                    destination = Mapper.Map(source, destination);
                    destination.Id = clientId;

                    destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                        sourceUnitOfWork, destinationUnitOfWork);
                    destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                        sourceUnitOfWork, destinationUnitOfWork);
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments Mapping",
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

                    #endregion

                    destination.Synced = true;
                    destinationUnitOfWork.Repository<AttachmentDTO>().InsertUpdate(destination);
                }
                catch
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments Crud",
                        "Problem On SyncAttachments Crud Method", UserName, Agency);
                    return false;
                }
                try
                {
                    if (!string.IsNullOrEmpty(source.AttachmentUrl) ||
                        (Singleton.PhotoStorage == PhotoStorage.Database && source.AttachedFile != null))
                    {
                        //var dest = PathUtil.GetDestinationPhotoPath();
                        var photoPath = PathUtil.GetLocalPhotoPath();
                        //var fiName = source.AttachmentUrl;

                        if (Singleton.PhotoStorage == PhotoStorage.Database && source.AttachedFile != null)
                        {
                            var fiName = source.RowGuid + ".jpg";
                            File.WriteAllBytes(Path.Combine(photoPath, fiName), source.AttachedFile);
                        }

                        //if (Singleton.BuildType == BuildType.LocalDev)
                        //    ////fileNames.Add(fiName);
                        //    //File.Copy(Path.Combine(photoPath, fiName), Path.Combine(dest, fiName), true);
                        //else
                        //{
                        //    ////Collect Filenames here
                        //    //fileNames.Add(fiName);

                        //    //using (var client = new WebClient())
                        //    //{
                        //    //    client.Credentials = DbCommandUtil.GetNetworkCredential();
                        //    //    //var desti = Path.Combine(dest, fiName);
                        //    //    //var photopa = Path.Combine(photoPath, fiName);
                        //    //    //client.UploadFile(desti, photopa);
                        //    //    client.UploadFileAsync(new Uri(Path.Combine(dest, fiName)), WebRequestMethods.Ftp.UploadFile,
                        //    //        Path.Combine(photoPath, fiName));
                        //    //    //client.UploadFileAsync();
                        //    //}
                        //}
                    }
                    else if (source.AttachedFile != null)
                    {
                        var photoPath = PathUtil.GetPhotoPath();
                        var fiName = source.RowGuid + "_File.jpg";

                        File.WriteAllBytes(Path.Combine(photoPath, fiName), source.AttachedFile);
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments upload photo problem",
                        ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                }

            }

            var changes = destinationUnitOfWork.Commit();
            if (changes < 0)
            {
                _errorsFound = true;
                LogUtil.LogError(ErrorSeverity.Critical, "SyncAttachments Commit",
                    "Problem Commiting SyncAttachments Method", UserName, Agency);
                return false;
            }

            return true;
        }

    }
}
