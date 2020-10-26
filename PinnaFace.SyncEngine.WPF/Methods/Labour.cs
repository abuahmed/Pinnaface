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
        public bool SyncLabour(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<LabourProcessDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<LabourProcessDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }

            var labourProcessDtos = sourceUnitOfWork.Repository<LabourProcessDTO>().Query()
                .Include(a => a.Agency)
                .Filter(filter)
                .Get(1)
                .ToList();

            var destLocalAgencies =
                destinationUnitOfWork.Repository<AgencyDTO>().Query()
                .Filter(a => a.Id == Singleton.Agency.Id)
                    .Get(1)
                    .ToList();
            foreach (var source in labourProcessDtos)
            {
                _updatesFound = true;
                var adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<LabourProcessDTO>().Query()
                        .Filter(i => i.RowGuid == adr1.RowGuid)
                        .Get(1)
                        .FirstOrDefault();

                var id = 0;
                if (destination == null)
                    destination = new LabourProcessDTO();
                else
                    id = destination.Id;
                try
                {
                    Mapper.Reset();
                    Mapper.CreateMap<LabourProcessDTO, LabourProcessDTO>()
                        .ForMember("Agency", option => option.Ignore())
                        .ForMember("AgencyId", option => option.Ignore())
                        .ForMember("Synced", option => option.Ignore());
                    destination = Mapper.Map(source, destination);
                    destination.Id = id;

                    destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                        sourceUnitOfWork, destinationUnitOfWork);
                    destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                        sourceUnitOfWork, destinationUnitOfWork);
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncLabour Mapping",
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
                    destinationUnitOfWork.Repository<LabourProcessDTO>()
                        .InsertUpdate(destination);
                }
                catch
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncLabour Crud",
                        "Problem On SyncLabour Crud Method", UserName, Agency);
                    return false;
                }

                //if (!string.IsNullOrEmpty(source.AgreementFileName))
                //{
                //    var dest = PathUtil.GetDestinationAgreementsPath();
                //    var agreementPath = PathUtil.GetAgreementPath();
                //    var fiName = source.AgreementFileName;

                //    if (Singleton.BuildType == BuildType.LocalDev)
                //        File.Copy(Path.Combine(agreementPath, fiName), Path.Combine(dest, fiName), true);
                //    else
                //    {
                //        using (var client = new WebClient())
                //        {
                //            client.Credentials = DbCommandUtil.GetNetworkCredential();
                //            client.UploadFile(Path.Combine(dest, fiName), WebRequestMethods.Ftp.UploadFile,
                //                Path.Combine(agreementPath, fiName));
                //        }
                //    }
                //}
            }
            var changes = destinationUnitOfWork.Commit();
            if (changes < 0)
            {
                _errorsFound = true;
                LogUtil.LogError(ErrorSeverity.Critical, "SyncLabour Commit",
                    "Problem Commiting SyncLabour Method", UserName, Agency);
                return false;
            }
            return true;
        }
    }
}
