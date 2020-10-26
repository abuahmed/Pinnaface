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
    public class LabourProcessService : ILabourProcessService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<LabourProcessDTO> _labourProcessRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public LabourProcessService()
        {
            InitializeDbContext();
        }
        public LabourProcessService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _labourProcessRepository = new Repository<LabourProcessDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<LabourProcessDTO> Get()
        {
            var piLabourProcess = _labourProcessRepository
                .Query();
                //.Include(e => e.Employee, e => e.Employee.Visa, e => e.Employee.Visa.Sponsor,e => e.Employee.Visa.Sponsor.Address);
            return piLabourProcess;
        }

        public IEnumerable<LabourProcessDTO> GetAll(SearchCriteria<LabourProcessDTO> criteria = null)
        {
            IEnumerable<LabourProcessDTO> catLabourProcess;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<LabourProcessDTO> pdtoLabourProcess;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoLabourProcess = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoLabourProcess = pdto.GetList().ToList();

                    catLabourProcess = pdtoLabourProcess.ToList();
                }
                else
                {
                    catLabourProcess = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catLabourProcess;
        }

        public LabourProcessDTO Find(string labourProcessId)
        {
            return _labourProcessRepository.FindById(Convert.ToInt32(labourProcessId));
        }

        public LabourProcessDTO GetByName(string displayName)
        {
            var cat = _labourProcessRepository
                .Query()
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(LabourProcessDTO labourProcess)
        {
            try
            {
                var validate = Validate(labourProcess);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(labourProcess))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                labourProcess.Synced = false;
                _labourProcessRepository.InsertUpdate(labourProcess);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(LabourProcessDTO labourProcess)
        {
            if (labourProcess == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _labourProcessRepository.Update(labourProcess);
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

        public int Delete(string labourProcessId)
        {
            try
            {
                _labourProcessRepository.Delete(Convert.ToInt32(labourProcessId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(LabourProcessDTO labourProcess)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<LabourProcessDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == labourProcess.FirstName && bp.Id != labourProcess.Id && bp.Type == labourProcess.Type)
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

        public string Validate(LabourProcessDTO labourProcess)
        {
            if (null == labourProcess)
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