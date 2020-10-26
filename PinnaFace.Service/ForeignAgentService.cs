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
    public class ForeignAgentService : IForeignAgentService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<AgentDTO> _foreignAgentRepository;
        private readonly bool _disposeWhenDone;
        private readonly bool _includeChilds;
        #endregion

        #region Constructor
        public ForeignAgentService()
        {
            InitializeDbContext();
        }
        public ForeignAgentService(bool disposeWhenDone, bool includeChilds)
        {
            _includeChilds = includeChilds;
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _foreignAgentRepository = new Repository<AgentDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<AgentDTO> Get()
        {
            var piForeignAgent = _foreignAgentRepository
                .Query()
                .Include(a => a.Address)
                .Filter(a => !string.IsNullOrEmpty(a.AgentName))
                .OrderBy(q => q.OrderBy(c => c.AgentName));
            if (_includeChilds)
                piForeignAgent = piForeignAgent.Include(f => f.Header, f => f.Footer);
            return piForeignAgent;
        }

        public IEnumerable<AgentDTO> GetAll(SearchCriteria<AgentDTO> criteria = null)
        {
            IEnumerable<AgentDTO> catForeignAgent;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<AgentDTO> pdtoForeignAgent;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoForeignAgent = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoForeignAgent = pdto.GetList().ToList();

                    catForeignAgent = pdtoForeignAgent.ToList();
                }
                else
                {
                    catForeignAgent = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catForeignAgent;
        }

        public AgentDTO Find(string foreignAgentId)
        {
            return _foreignAgentRepository.FindById(Convert.ToInt32(foreignAgentId));
        }

        public AgentDTO GetByName(string displayName)
        {
            var cat = _foreignAgentRepository
                .Query()
                .Filter(c => c.AgentName.Contains(displayName))
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(AgentDTO agent)
        {
            try
            {
                var validate = Validate(agent);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(agent))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                agent.Synced = false;
                _foreignAgentRepository.InsertUpdate(agent);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(AgentDTO agent)
        {
            if (agent == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _foreignAgentRepository.Update(agent);
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

        public int Delete(string foreignAgentId)
        {
            try
            {
                _foreignAgentRepository.Delete(foreignAgentId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(AgentDTO agent)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<AgentDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => bp.AgentName == agent.AgentName && bp.Id != agent.Id)
                    .Get()
                    .FirstOrDefault();
                if (catExists != null)
                    objectExists = true;
            }
            finally
            {
                iDbContext.Dispose();
            }

            return objectExists;
        }

        public string Validate(AgentDTO agent)
        {
            if (null == agent)
                return GenericMessages.ObjectIsNull;

            if (agent.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(agent.AgentName))
                return agent.AgentName + " " + GenericMessages.StringIsNullOrEmpty;

            if (agent.AgentName.Length > 255)
                return agent.AgentName + " can not be more than 255 characters ";

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