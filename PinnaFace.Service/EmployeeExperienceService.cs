using System;
using System.Collections.Generic;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service.Interfaces;

namespace PinnaFace.Service
{
    public class EmployeeExperienceService : IEmployeeExperienceService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<EmployeeExperienceDTO> _employeeApplicationRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public EmployeeExperienceService()
        {
            InitializeDbContext();
        }
        public EmployeeExperienceService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _employeeApplicationRepository = new Repository<EmployeeExperienceDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<EmployeeExperienceDTO> Get()
        {
            var piEmployeeApplication = _employeeApplicationRepository
                .Query();//.Include(e=>e.Employee);
            return piEmployeeApplication;
        }

        public IEnumerable<EmployeeExperienceDTO> GetAll(SearchCriteria<EmployeeExperienceDTO> criteria = null)
        {
            IEnumerable<EmployeeExperienceDTO> catEmployeeApplication;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<EmployeeExperienceDTO> pdtoEmployeeApplication;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoEmployeeApplication = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoEmployeeApplication = pdto.GetList().ToList();

                    catEmployeeApplication = pdtoEmployeeApplication.ToList();
                }
                else
                {
                    catEmployeeApplication = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catEmployeeApplication;
        }

        public EmployeeExperienceDTO Find(string employeeApplicationId)
        {
            return _employeeApplicationRepository.FindById(Convert.ToInt32(employeeApplicationId));
        }

        public EmployeeExperienceDTO GetByName(string displayName)
        {
            var cat = _employeeApplicationRepository
                .Query()
                //.Filter(c => c.Employee.FirstName.Contains(displayName))
                .Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(EmployeeExperienceDTO employeeExperience)
        {
            try
            {
                var validate = Validate(employeeExperience);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(employeeExperience))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                employeeExperience.Synced = false;
                _employeeApplicationRepository.InsertUpdate(employeeExperience);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(EmployeeExperienceDTO employeeExperience)
        {
            if (employeeExperience == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _employeeApplicationRepository.Update(employeeExperience);
                _unitOfWork.Commit();
                stat = string.Empty;
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                iDbContext.Dispose();
            }
            return stat;
        }

        public int Delete(string employeeApplicationId)
        {
            try
            {
                _employeeApplicationRepository.Delete(Convert.ToInt32(employeeApplicationId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(EmployeeExperienceDTO employeeExperience)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<EmployeeExperienceDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.DisplayName == employeeExperience.DisplayName && bp.Id != employeeExperience.Id && bp.Type == employeeExperience.Type)
            //        .Get()
            //        .FirstOrDefault();
            //    if (catExists != null)
            //        objectExists = true;
            //}
            //finally
            //{
            //    iDbContext.Dispose();
            //}

            //return objectExists;
            return false;
        }

        public string Validate(EmployeeExperienceDTO employeeExperience)
        {
            if (null == employeeExperience)
                return GenericMessages.ObjectIsNull;

            //if (String.IsNullOrEmpty(employeeExperience.ContractPeriod))
            //    return employeeExperience.ContractPeriod + " " + GenericMessages.StringIsNullOrEmpty;

            //if (employeeExperience.ContractPeriod.Length > 255)
            //    return employeeExperience.ContractPeriod + " can not be more than 255 characters ";

            return string.Empty;
        }

        #endregion

        #region Disposing
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}