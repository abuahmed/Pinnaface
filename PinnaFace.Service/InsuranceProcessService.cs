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
    public class InsuranceProcessService : IInsuranceProcessService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<InsuranceProcessDTO> _insuranceProcessRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public InsuranceProcessService()
        {
            InitializeDbContext();
        }
        public InsuranceProcessService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _insuranceProcessRepository = new Repository<InsuranceProcessDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<InsuranceProcessDTO> Get()
        {
            var piInsuranceProcess = _insuranceProcessRepository
                .Query();//.Include(e => e.Employee);
            return piInsuranceProcess;
        }

        public IEnumerable<InsuranceProcessDTO> GetAll(SearchCriteria<InsuranceProcessDTO> criteria = null)
        {
            IEnumerable<InsuranceProcessDTO> catInsuranceProcess;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<InsuranceProcessDTO> pdtoInsuranceProcess;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoInsuranceProcess = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoInsuranceProcess = pdto.GetList().ToList();

                    catInsuranceProcess = pdtoInsuranceProcess.ToList();
                }
                else
                {
                    catInsuranceProcess = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catInsuranceProcess;
        }

        public InsuranceProcessDTO Find(string insuranceProcessId)
        {
            return _insuranceProcessRepository.FindById(Convert.ToInt32(insuranceProcessId));
        }

        public InsuranceProcessDTO GetByName(string displayName)
        {
            var cat = _insuranceProcessRepository
                .Query()
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(InsuranceProcessDTO insuranceProcess)
        {
            try
            {
                var validate = Validate(insuranceProcess);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(insuranceProcess))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                insuranceProcess.Synced = false;
                _insuranceProcessRepository.InsertUpdate(insuranceProcess);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(InsuranceProcessDTO insuranceProcess)
        {
            if (insuranceProcess == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _insuranceProcessRepository.Update(insuranceProcess);
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

        public int Delete(string insuranceProcessId)
        {
            try
            {
                _insuranceProcessRepository.Delete(Convert.ToInt32(insuranceProcessId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(InsuranceProcessDTO insuranceProcess)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<InsuranceProcessDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == insuranceProcess.FirstName && bp.Id != insuranceProcess.Id && bp.Type == insuranceProcess.Type)
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

        public string Validate(InsuranceProcessDTO insuranceProcess)
        {
            if (null == insuranceProcess)
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