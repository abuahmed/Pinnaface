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
    public class EmbassyProcessService : IEmbassyProcessService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<EmbassyProcessDTO> _embassyProcessRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public EmbassyProcessService()
        {
            InitializeDbContext();
        }
        public EmbassyProcessService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _embassyProcessRepository = new Repository<EmbassyProcessDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<EmbassyProcessDTO> Get()
        {
            var piEmbassyProcess = _embassyProcessRepository
                .Query();
                //.Include(e => e.Employee, e => e.Employee.Visa);
            return piEmbassyProcess;
        }

        public IEnumerable<EmbassyProcessDTO> GetAll(SearchCriteria<EmbassyProcessDTO> criteria = null)
        {
            IEnumerable<EmbassyProcessDTO> catEmbassyProcess;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<EmbassyProcessDTO> pdtoEmbassyProcess;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoEmbassyProcess = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoEmbassyProcess = pdto.GetList().ToList();

                    catEmbassyProcess = pdtoEmbassyProcess.ToList();
                }
                else
                {
                    catEmbassyProcess = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catEmbassyProcess;
        }

        public EmbassyProcessDTO Find(string embassyProcessId)
        {
            return _embassyProcessRepository.FindById(Convert.ToInt32(embassyProcessId));
        }

        public EmbassyProcessDTO GetByName(string displayName)
        {
            var cat = _embassyProcessRepository
                .Query()
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(EmbassyProcessDTO embassyProcess)
        {
            try
            {
                var validate = Validate(embassyProcess);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(embassyProcess))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;
                
                embassyProcess.Synced = false;
                _embassyProcessRepository.InsertUpdate(embassyProcess);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(EmbassyProcessDTO embassyProcess)
        {
            if (embassyProcess == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _embassyProcessRepository.Update(embassyProcess);
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

        public int Delete(string embassyProcessId)
        {
            try
            {
                _embassyProcessRepository.Delete(Convert.ToInt32(embassyProcessId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(EmbassyProcessDTO embassyProcess)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<EmbassyProcessDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == embassyProcess.FirstName && bp.Id != embassyProcess.Id && bp.Type == embassyProcess.Type)
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

        public string Validate(EmbassyProcessDTO embassyProcess)
        {
            if (null == embassyProcess)
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