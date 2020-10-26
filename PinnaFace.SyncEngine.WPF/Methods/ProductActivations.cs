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
        public bool SyncProductActivations(IUnitOfWork sourceUnitOfWork,
            IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<ProductActivationDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<ProductActivationDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;

                filter = filter.And(filter2);
            }

            var productActivationDtos =
                sourceUnitOfWork.Repository<ProductActivationDTO>().Query()
                    .Include(a => a.Agency)
                    .Filter(filter)
                    .Get(1)
                    .ToList();
            var destLocalAgencies =
                destinationUnitOfWork.Repository<AgencyDTO>().Query()
                .Filter(a => a.Id == Singleton.Agency.Id)
                    .Get(1)
                    .ToList();

            foreach (var source in productActivationDtos)
            {
                _updatesFound = true;


                var adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<ProductActivationDTO>().Query()
                        .Filter(i => i.RowGuid == adr1.RowGuid)
                        .Get(1)
                        .FirstOrDefault();

                //To Prevent ServerData Overriding
                if (destination != null && (ToServerSyncing && !destination.Synced))
                    continue;

                var clientId = 0;
                if (destination == null)
                    destination = new ProductActivationDTO();
                else
                    clientId = destination.Id;

                try
                {
                    Mapper.Reset();
                    Mapper.CreateMap<ProductActivationDTO, ProductActivationDTO>()
                        .ForMember("Agency", option => option.Ignore())
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
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncProductActivations Mapping",
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
                    destinationUnitOfWork.Repository<ProductActivationDTO>()
                        .InsertUpdate(destination);
                }
                catch
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncProductActivations Crud",
                        "Problem On SyncProductActivations Crud Method", UserName, Agency);
                    return false;
                }
            }
            var changes = destinationUnitOfWork.Commit();
            if (changes < 0)
            {
                _errorsFound = true;
                LogUtil.LogError(ErrorSeverity.Critical, "SyncProductActivations Commit",
                    "Problem Commiting SyncProductActivations Method", UserName, Agency);
                return false;
            }

            return true;
        }
    }
}