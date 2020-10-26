using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    public partial class SyncTask
    {
        public bool SyncAgencies2(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            var sourceList = sourceUnitOfWork.Repository<AgencyDTO>().Query()
                .Filter(a => !a.Synced && a.DateLastModified > LastServerSyncDate)
                .Get(1)
                .ToList();

            if (sourceList.Any())
            {
                _updatesFound = true;

                //List<AgencyDTO> destList;

                //if(Singleton.Agency!=null)
                // destList =
                //    destinationUnitOfWork.Repository<AgencyDTO>().Query()
                //        .Include(a => a.Address).Filter(a => a.Id == Singleton.Agency.Id)
                //        .Get(1)
                //        .ToList();
                //else destList =
                //   destinationUnitOfWork.Repository<AgencyDTO>().Query()
                //       .Include(a => a.Address)
                //       .Get(1)
                //       .ToList();

                foreach (var source in sourceList)
                {
                    AgencyDTO source1 = source;
                    var destination =
                        destinationUnitOfWork.Repository<AgencyDTO>().Query()
                        .Filter(i => i.RowGuid == source1.RowGuid)
                        .Include(a => a.Address)
                        .Get(1).FirstOrDefault();

                    const int clientId = 0;
                    if (destination == null)
                    {
                        destination = new AgencyDTO();
                    }
                    else
                        continue;//clientId = destination.Id;

                    try
                    {
                        Mapper.Reset();
                        Mapper.CreateMap<AgencyDTO, AgencyDTO>()
                            .ForMember("Address", option => option.Ignore())
                            .ForMember("Header", option => option.Ignore())
                            .ForMember("Footer", option => option.Ignore())
                            .ForMember("AddressId", option => option.Ignore())
                            .ForMember("HeaderId", option => option.Ignore())
                            .ForMember("FooterId", option => option.Ignore())
                            .ForMember("Synced", option => option.Ignore());
                        destination = Mapper.Map(source, destination);
                        destination.Id = clientId;

                        destination.Address = null;
                        destination.AddressId = null;
                        destination.Header = null;
                        destination.HeaderId = null;
                        destination.Footer = null;
                        destination.FooterId = null;

                        destination.CreatedByUserId = GetDestCreatedModifiedByUserId(source.CreatedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                        destination.ModifiedByUserId = GetDestCreatedModifiedByUserId(source.ModifiedByUserId,
                            sourceUnitOfWork, destinationUnitOfWork);
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Mapping",
                            ex.Message + Environment.NewLine + ex.InnerException, UserName, Agency);
                        //UpdatingText = "Problem mapping AgencyDTO";
                    }
                    try
                    {
                        destination.Synced = true;
                        destinationUnitOfWork.Repository<AgencyDTO>()
                            .InsertUpdate(destination);
                    }
                    catch
                    {
                        _errorsFound = true;
                        LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Crud",
                            "Problem On SyncAgencies Crud Method", UserName, Agency);
                        return false;
                    }
                }
                var changes = destinationUnitOfWork.Commit();
                if (changes < 0)
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncAgencies Commit",
                        "Problem Commiting SyncAgencies Method", UserName, Agency);
                    return false;
                }
            }
            return true;
        }
    }
}
