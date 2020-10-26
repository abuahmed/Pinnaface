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
    public class EmployeeEducationService : IEmployeeEducationService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<EmployeeEducationDTO> _employeeEducationRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public EmployeeEducationService()
        {
            InitializeDbContext();
        }
        public EmployeeEducationService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _employeeEducationRepository = new Repository<EmployeeEducationDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<EmployeeEducationDTO> Get()
        {
            var piEmployeeEducation = _employeeEducationRepository
                .Query();//.Include(e => e.Employee);
            return piEmployeeEducation;
        }

        public IEnumerable<EmployeeEducationDTO> GetAll(SearchCriteria<EmployeeEducationDTO> criteria = null)
        {
            IEnumerable<EmployeeEducationDTO> catEmployeeEducation;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<EmployeeEducationDTO> pdtoEmployeeEducation;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoEmployeeEducation = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoEmployeeEducation = pdto.GetList().ToList();

                    catEmployeeEducation = pdtoEmployeeEducation.ToList();
                }
                else
                {
                    catEmployeeEducation = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catEmployeeEducation;
        }

        public EmployeeEducationDTO Find(string employeeEducationId)
        {
            return _employeeEducationRepository.FindById(Convert.ToInt32(employeeEducationId));
        }

        public EmployeeEducationDTO GetByName(string displayName)
        {
            var cat = _employeeEducationRepository
                .Query()
                //.Filter(c => c.Employee.FirstName.Contains(displayName))
                .Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(EmployeeEducationDTO employeeEducation)
        {
            try
            {
                var validate = Validate(employeeEducation);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(employeeEducation))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;
                
                employeeEducation.Synced = false;
                _employeeEducationRepository.InsertUpdate(employeeEducation);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(EmployeeEducationDTO employeeEducation)
        {
            if (employeeEducation == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _employeeEducationRepository.Update(employeeEducation);
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

        public int Delete(string employeeEducationId)
        {
            try
            {
                _employeeEducationRepository.Delete(Convert.ToInt32(employeeEducationId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(EmployeeEducationDTO employeeEducation)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<EmployeeEducationDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.DisplayName == employeeEducation.DisplayName && bp.Id != employeeEducation.Id && bp.Type == employeeEducation.Type)
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

        public string Validate(EmployeeEducationDTO employeeEducation)
        {
            if (null == employeeEducation)
                return GenericMessages.ObjectIsNull;

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