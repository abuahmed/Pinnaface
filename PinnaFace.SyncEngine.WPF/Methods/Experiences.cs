﻿using System;
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
        public bool SyncExperiences(IUnitOfWork sourceUnitOfWork,
    IUnitOfWork destinationUnitOfWork)
        {
            Expression<Func<EmployeeExperienceDTO, bool>> filter =
                a => !a.Synced && a.DateLastModified > LastServerSyncDate;

            if (!ToServerSyncing)
            {
                Expression<Func<EmployeeExperienceDTO, bool>> filter2 =
                    a => a.Agency != null &&
                         a.Agency.RowGuid == Singleton.Agency.RowGuid;
                filter = filter.And(filter2);
            }

            var exprs = sourceUnitOfWork.Repository<EmployeeExperienceDTO>().Query()
                .Include(a => a.Agency)
                .Filter(filter)
                .Get(1)
                .ToList();

            var destLocalAgencies =
                destinationUnitOfWork.Repository<AgencyDTO>().Query()
                .Filter(a => a.Id == Singleton.Agency.Id)
                    .Get(1)
                    .ToList();
            foreach (var source in exprs)
            {
                _updatesFound = true;
                var adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<EmployeeExperienceDTO>().Query()
                        .Filter(i => i.RowGuid == adr1.RowGuid)
                        .Get(1)
                        .FirstOrDefault();

                var id = 0;
                if (destination == null)
                    destination = new EmployeeExperienceDTO();
                else
                    id = destination.Id;

                try
                {
                    Mapper.Reset();
                    Mapper.CreateMap<EmployeeExperienceDTO, EmployeeExperienceDTO>()
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
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncExperience Mapping",
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
                    destinationUnitOfWork.Repository<EmployeeExperienceDTO>()
                        .InsertUpdate(destination);
                }
                catch
                {
                    _errorsFound = true;
                    LogUtil.LogError(ErrorSeverity.Critical, "SyncExperience Crud",
                        "Problem On SyncExperience Crud Method", UserName, Agency);
                    return false;
                }
            }

            var changes = destinationUnitOfWork.Commit();
            if (changes < 0)
            {
                _errorsFound = true;
                LogUtil.LogError(ErrorSeverity.Critical, "SyncExperience Commit",
                    "Problem Commiting SyncExperience Method", UserName, Agency);
                return false;
            }
            return true;
        }
    }
}
