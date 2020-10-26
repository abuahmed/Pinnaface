using System;
using System.Collections.Generic;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Repository;
using PinnaFace.Service;

namespace PinnaFace.SyncEngine
{
    public static class AgencyWithAgentsUtility
    {
        public static bool InsertAgencyWithAgents( string userName, string agencyName)
        {
           
            //try
            //{
            //    userName = Singleton.User.UserName;
            //    agencyName = Singleton.Agency.AgencyName;
            //}
            //catch
            //{
            //    userName = "Default User";
            //    agencyName = "Default Agency";
            //}

            try
            {
                IDbContext iDbContext = DbContextUtil.GetDbContextInstance();
                var unitOfWork = new UnitOfWork(iDbContext);

               
                AgencyDTO agency = new LocalAgencyService(true).GetLocalAgency();
                IEnumerable<AgentDTO> agents = new ForeignAgentService(true, false).GetAll();

                foreach (AgentDTO foreignAgentDTO in agents)
                {
                    AgentDTO dto = foreignAgentDTO;
                    var agencyWithAgents = unitOfWork.Repository<AgencyAgentDTO>()
                        .Query()
                        .FilterList(f => f.AgentId == dto.Id && f.AgencyId == agency.Id)
                        .Get()
                        .FirstOrDefault();

                    if (agencyWithAgents != null) continue;

                    agencyWithAgents = new AgencyAgentDTO
                    {
                        AgencyId = agency.Id,
                        AgentId = foreignAgentDTO.Id
                    };

                    unitOfWork.Repository<AgencyAgentDTO>().Insert(agencyWithAgents);
                }
                unitOfWork.Commit();

                unitOfWork.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Insert Agency With Agents",
                       ex.Message + Environment.NewLine + ex.InnerException, userName, agencyName);
                return false;
            }

        }

        public static bool InsertUserWithAgencyWithAgents(string userName, string agencyName)
        {
            //string userName, agencyName;
            //try
            //{
            //    userName = Singleton.User.UserName;
            //    agencyName = Singleton.Agency.AgencyName;
            //}
            //catch
            //{
            //    userName = "Default User";
            //    agencyName = "Default Agency";
            //}

            try
            {
                IDbContext iDbContext = DbContextUtil.GetDbContextInstance();
                var unitOfWork = new UnitOfWork(iDbContext);

                var users = new UserService().GetAll().ToList();
                var agenctWithAgents = unitOfWork.Repository<AgencyAgentDTO>()
                    .Query()
                    .Get()
                    .ToList();

                foreach (var userDTO in users)
                {
                    foreach (var agencyWithAgentDTO in agenctWithAgents)
                    {
                        UserDTO dto = userDTO;
                        AgencyAgentDTO agentDTO = agencyWithAgentDTO;

                        var userWithAgencyWithAgents = unitOfWork.UserRepository<UserAgencyAgentDTO>()
                            .Query()
                            .FilterList(f => f.UserId == dto.UserId && f.AgencyWithAgentId == agentDTO.Id)
                            .Get()
                            .FirstOrDefault();

                        if (userWithAgencyWithAgents != null) continue;

                        userWithAgencyWithAgents = new UserAgencyAgentDTO()
                        {
                            UserId = dto.UserId,
                            AgencyWithAgentId = agentDTO.Id
                        };

                        unitOfWork.UserRepository<UserAgencyAgentDTO>().Insert(userWithAgencyWithAgents);
                    }
                }

                unitOfWork.Commit();

                unitOfWork.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Insert User With Agency With Agents",
                       ex.Message + Environment.NewLine + ex.InnerException, userName, agencyName);
                return false;
            }

        }

        public static bool InsertAgencyNamesonAddressesandAttachments()
        {
            #region Update Addresses
            var localAgency = new LocalAgencyService(true).GetLocalAgency();

            try
            {
                var unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

                var addresses = unitOfWork.Repository<AddressDTO>()
                    .Query().Filter(a => a.AgencyId == null).Get().ToList();
                if (addresses.Count > 0)
                {
                    foreach (var addressDTO in addresses)
                    {
                        addressDTO.AgencyId = localAgency.Id;
                        unitOfWork.Repository<AddressDTO>().Update(addressDTO);
                    }
                    unitOfWork.Commit();
                }


                unitOfWork.Dispose();
            }
            catch (Exception exception)
            {
                LogUtil.LogError(ErrorSeverity.Fatal,
                    "InsertAgencyNamesonAddressesandAttachments",
                    exception.Message + Environment.NewLine + exception.InnerException, "", "");
            }
            #endregion

            #region Update Attachments
            try
            {
                var unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());


                var addresses = unitOfWork.Repository<AttachmentDTO>()
                    .Query().Filter(a => a.AgencyId == null).Get().ToList();

                if (addresses.Count > 0)
                {
                    foreach (var addressDTO in addresses)
                    {
                        addressDTO.AgencyId = localAgency.Id;
                        unitOfWork.Repository<AttachmentDTO>().Update(addressDTO);
                    }
                    unitOfWork.Commit();
                }
                unitOfWork.Dispose();
            }
            catch (Exception exception)
            {
                LogUtil.LogError(ErrorSeverity.Fatal,
                    "InsertAgencyNamesonAddressesandAttachments",
                    exception.Message + Environment.NewLine + exception.InnerException, "", "");
            }
            #endregion

            return true;
        }
    }
}