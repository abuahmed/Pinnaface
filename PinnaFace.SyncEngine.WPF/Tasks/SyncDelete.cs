using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.SyncEngine.WPF.Tasks
{
    #region Sync Delete
    
    public partial class SyncTask
    {
        public void DeleteSync(object sender, DoWorkEventArgs e)
        {
            if (!CheckInternetConnection())
            {
                //_noConnection = true;
                return;
            }

            IUnitOfWork sourceUnitOfWork = new UnitOfWork(new DbContextFactory().Create());
            IUnitOfWork destinationUnitOfWork = new UnitOfWorkServer(new ServerDbContextFactory().Create());

            #region Sync Delete

            try
            {

                if (!DeleteAddresses(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteAttachmentes(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteRequiredDocumentses(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!DeleteVisaSponsores(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteVisaConditiones(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!DeleteEmployeeEducationes(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteEmployeeExperiencees(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteEmployeeHawalaes(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!DeleteInsuranceProcesses(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteLabourProcesses(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteEmbassyProcesses(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteFlightProcesses(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!DeleteVisaes(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!DeleteEmployeees(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteEmployeeRelativees(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

                if (!DeleteComplaines(sourceUnitOfWork, destinationUnitOfWork)) return;
                sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                destinationUnitOfWork = GetNewUow(destinationUnitOfWork);
                if (!DeleteComplainRemarkes(sourceUnitOfWork, destinationUnitOfWork)) return;
                //sourceUnitOfWork = GetNewUow2(sourceUnitOfWork);
                //destinationUnitOfWork = GetNewUow(destinationUnitOfWork);

            }
            catch (Exception ex)
            {
                LogUtil.LogError(ErrorSeverity.Critical, "Sync General Method",
                    ex.Message + Environment.NewLine + ex.InnerException, Tasks.SyncTask.UserName, Tasks.SyncTask.Agency);
            }
            finally
            {
                try
                {
                    sourceUnitOfWork.Dispose();
                }
                catch (Exception ex)
                {
                    LogUtil.LogError(ErrorSeverity.Critical, "Sync General sourceUnitOfWork.Dispose",
                        ex.Message + Environment.NewLine + ex.InnerException, Tasks.SyncTask.UserName, Tasks.SyncTask.Agency);
                }
            }

            #endregion
        }

        public bool DeleteAddresses(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<AddressDTO> addressDtos = sourceUnitOfWork.Repository<AddressDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (AddressDTO source in addressDtos)
            {
                AddressDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<AddressDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<AddressDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<AddressDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteRequiredDocumentses(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<RequiredDocumentsDTO> addressDtos = sourceUnitOfWork.Repository<RequiredDocumentsDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (RequiredDocumentsDTO source in addressDtos)
            {
                RequiredDocumentsDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<RequiredDocumentsDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<RequiredDocumentsDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<RequiredDocumentsDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteAttachmentes(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<AttachmentDTO> addressDtos = sourceUnitOfWork.Repository<AttachmentDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (AttachmentDTO source in addressDtos)
            {
                AttachmentDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<AttachmentDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<AttachmentDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<AttachmentDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteVisaSponsores(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<VisaSponsorDTO> addressDtos = sourceUnitOfWork.Repository<VisaSponsorDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (VisaSponsorDTO source in addressDtos)
            {
                VisaSponsorDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<VisaSponsorDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<VisaSponsorDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<VisaSponsorDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteVisaConditiones(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<VisaConditionDTO> addressDtos = sourceUnitOfWork.Repository<VisaConditionDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (VisaConditionDTO source in addressDtos)
            {
                VisaConditionDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<VisaConditionDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<VisaConditionDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<VisaConditionDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteVisaes(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<VisaDTO> addressDtos = sourceUnitOfWork.Repository<VisaDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (VisaDTO source in addressDtos)
            {
                VisaDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<VisaDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<VisaDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<VisaDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteEmployeeEducationes(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<EmployeeEducationDTO> addressDtos = sourceUnitOfWork.Repository<EmployeeEducationDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (EmployeeEducationDTO source in addressDtos)
            {
                EmployeeEducationDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<EmployeeEducationDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<EmployeeEducationDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<EmployeeEducationDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteEmployeeExperiencees(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<EmployeeExperienceDTO> addressDtos = sourceUnitOfWork.Repository<EmployeeExperienceDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (EmployeeExperienceDTO source in addressDtos)
            {
                EmployeeExperienceDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<EmployeeExperienceDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<EmployeeExperienceDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<EmployeeExperienceDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteEmployeeHawalaes(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<EmployeeHawalaDTO> addressDtos = sourceUnitOfWork.Repository<EmployeeHawalaDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (EmployeeHawalaDTO source in addressDtos)
            {
                EmployeeHawalaDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<EmployeeHawalaDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<EmployeeHawalaDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<EmployeeHawalaDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteInsuranceProcesses(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<InsuranceProcessDTO> addressDtos = sourceUnitOfWork.Repository<InsuranceProcessDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (InsuranceProcessDTO source in addressDtos)
            {
                InsuranceProcessDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<InsuranceProcessDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<InsuranceProcessDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<InsuranceProcessDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteLabourProcesses(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<LabourProcessDTO> addressDtos = sourceUnitOfWork.Repository<LabourProcessDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (LabourProcessDTO source in addressDtos)
            {
                LabourProcessDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<LabourProcessDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<LabourProcessDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<LabourProcessDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteEmbassyProcesses(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<EmbassyProcessDTO> addressDtos = sourceUnitOfWork.Repository<EmbassyProcessDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (EmbassyProcessDTO source in addressDtos)
            {
                EmbassyProcessDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<EmbassyProcessDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<EmbassyProcessDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<EmbassyProcessDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteFlightProcesses(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<FlightProcessDTO> addressDtos = sourceUnitOfWork.Repository<FlightProcessDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (FlightProcessDTO source in addressDtos)
            {
                FlightProcessDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<FlightProcessDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<FlightProcessDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<FlightProcessDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteEmployeeRelativees(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<EmployeeRelativeDTO> addressDtos = sourceUnitOfWork.Repository<EmployeeRelativeDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (EmployeeRelativeDTO source in addressDtos)
            {
                EmployeeRelativeDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<EmployeeRelativeDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<EmployeeRelativeDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<EmployeeRelativeDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteComplaines(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<ComplainDTO> addressDtos = sourceUnitOfWork.Repository<ComplainDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (ComplainDTO source in addressDtos)
            {
                ComplainDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<ComplainDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<ComplainDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<ComplainDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteComplainRemarkes(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<ComplainRemarkDTO> addressDtos = sourceUnitOfWork.Repository<ComplainRemarkDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (ComplainRemarkDTO source in addressDtos)
            {
                ComplainRemarkDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<ComplainRemarkDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<ComplainRemarkDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<ComplainRemarkDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

        public bool DeleteEmployeees(IUnitOfWork sourceUnitOfWork, IUnitOfWork destinationUnitOfWork)
        {
            List<EmployeeDTO> addressDtos = sourceUnitOfWork.Repository<EmployeeDTO>()
                .Query()
                .Get(-1)
                .ToList();

            foreach (EmployeeDTO source in addressDtos)
            {
                EmployeeDTO adr1 = source;
                var destination =
                    destinationUnitOfWork.Repository<EmployeeDTO>()
                    .Query()
                    .Filter(i => i.RowGuid == adr1.RowGuid)
                    .Get(-1)//don't use .Get() to make sure both sides of data are disabled
                    .FirstOrDefault();

                if (destination != null)
                {
                    sourceUnitOfWork.Repository<EmployeeDTO>().Delete(source.Id);
                    destinationUnitOfWork.Repository<EmployeeDTO>().Delete(destination.Id);

                    sourceUnitOfWork.Commit();
                    destinationUnitOfWork.Commit();
                }
            }

            return true;
        }

    }
        #endregion
}
