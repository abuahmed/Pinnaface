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
    public class EmployeeRelativeService : IEmployeeRelativeService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<EmployeeRelativeDTO> _employeeRelativeRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public EmployeeRelativeService()
        {
            InitializeDbContext();
        }
        public EmployeeRelativeService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _employeeRelativeRepository = new Repository<EmployeeRelativeDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<EmployeeRelativeDTO> Get()
        {
            var piEmployeeRelative = _employeeRelativeRepository
                .Query()
                .Include(a => a.Address, a => a.Employee)
                .Filter(a => !string.IsNullOrEmpty(a.FullName))
                .OrderBy(q => q.OrderBy(c => c.FullName));
            return piEmployeeRelative;
        }

        public IEnumerable<EmployeeRelativeDTO> GetAll(SearchCriteria<EmployeeRelativeDTO> criteria = null)
        {
            IEnumerable<EmployeeRelativeDTO> catEmployeeRelative;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<EmployeeRelativeDTO> pdtoEmployeeRelative;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoEmployeeRelative = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoEmployeeRelative = pdto.GetList().ToList();

                    catEmployeeRelative = pdtoEmployeeRelative.ToList();
                }
                else
                {
                    catEmployeeRelative = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catEmployeeRelative;
        }

        public EmployeeRelativeDTO Find(string employeeRelativeId)
        {
            return _employeeRelativeRepository.FindById(Convert.ToInt32(employeeRelativeId));
        }

        public EmployeeRelativeDTO GetByName(string displayName)
        {
            var cat = _employeeRelativeRepository
                .Query()
                .Filter(c => c.FullName.Contains(displayName))
                .Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(EmployeeRelativeDTO employeeRelative)
        {
            try
            {
                var validate = Validate(employeeRelative);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(employeeRelative))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;
                
                employeeRelative.Synced = false;
                _employeeRelativeRepository.InsertUpdate(employeeRelative);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(EmployeeRelativeDTO employeeRelative)
        {
            if (employeeRelative == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _employeeRelativeRepository.Update(employeeRelative);
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

        public int Delete(string employeeRelativeId)
        {
            try
            {
                _employeeRelativeRepository.Delete(Convert.ToInt32(employeeRelativeId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(EmployeeRelativeDTO employeeRelative)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<EmployeeRelativeDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FullName == employeeRelative.FullName && bp.Id != employeeRelative.Id && bp.Type == employeeRelative.Type)
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

        public string Validate(EmployeeRelativeDTO employeeRelative)
        {
            if (null == employeeRelative)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(employeeRelative.FullName))
                return employeeRelative.FullName + " " + GenericMessages.StringIsNullOrEmpty;

            if (employeeRelative.FullName.Length > 255)
                return employeeRelative.FullName + " can not be more than 255 characters ";

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