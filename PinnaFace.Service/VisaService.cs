using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service.Interfaces;

namespace PinnaFace.Service
{
    public class VisaService : IVisaService
    {
        #region Fields

        private IUnitOfWork _unitOfWork;
        private IRepository<VisaDTO> _visaRepository;
        private IRepository<VisaConditionDTO> _visaConditionRepository;
        private IRepository<VisaSponsorDTO> _visaSponsorRepository;
        private IRepository<EmployeeDTO> _employeeRepository;
        private readonly bool _disposeWhenDone;
        private readonly bool _includeChilds;

        #endregion

        #region Constructor

        public VisaService()
        {
            _includeChilds = true;
            InitializeDbContext();
        }

        //public VisaService(bool includeChilds)
        //{
        //    _includeChilds = includeChilds;
        //    InitializeDbContext();
        //}
        public VisaService(bool disposeWhenDone, bool includeChilds)
        {
            _includeChilds = includeChilds;
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _visaConditionRepository = new Repository<VisaConditionDTO>(iDbContext);
            _visaSponsorRepository = new Repository<VisaSponsorDTO>(iDbContext);
            _visaRepository = new Repository<VisaDTO>(iDbContext);
            _employeeRepository = new Repository<EmployeeDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods

        public IRepositoryQuery<VisaDTO> Get()
        {
            var piVisa = _visaRepository
                .Query()
                .Filter(a => !string.IsNullOrEmpty(a.VisaNumber))
                .OrderBy(q => q.OrderByDescending(c => c.Id));
            if (_includeChilds)
                piVisa = piVisa.Include(v => v.Agency, v => v.Agent, v => v.Agent.Address,
                    v => v.Employees,
                    v => v.Sponsor, v => v.Sponsor.Address, v => v.Condition);
            return piVisa;
        }

        public IEnumerable<VisaDTO> GetAll(SearchCriteria<VisaDTO> criteria = null)
        {
            int totalCount;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<VisaDTO> GetAll(SearchCriteria<VisaDTO> criteria, out int totCount)
        {
            totCount = 0;
            IEnumerable<VisaDTO> catVisa;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    #region Filter By User

                    if (Singleton.Edition == PinnaFaceEdition.WebEdition)
                    {
                        var currentUser = new UserService(true)
                            .GetAll(new UserSearchCriteria<UserDTO>())
                            .FirstOrDefault(u => u.UserId == criteria.CurrentUserId);

                        if (currentUser != null && currentUser.AgenciesWithAgents != null &&
                            currentUser.AgenciesWithAgents.Count > 0)
                        {
                            IList<Expression<Func<VisaDTO, bool>>> filtersExpressions =
                                new List<Expression<Func<VisaDTO, bool>>>();

                            var agencyAgents = currentUser.AgenciesWithAgents;
                            foreach (var agencyAgentsDto in agencyAgents)
                            {
                                var dto = agencyAgentsDto;

                                if (currentUser.AgentId!=null)
                                {
                                    filtersExpressions.Add(e =>
                                        (e.AgencyId == dto.AgencyAgent.AgencyId &&
                                         e.ForeignAgentId == dto.AgencyAgent.AgentId) 
                                         || (e.AgencyId == null &&
                                         e.ForeignAgentId == dto.AgencyAgent.AgentId));
                                }
                                else
                                {
                                    filtersExpressions.Add(e =>
                                        (e.AgencyId == dto.AgencyAgent.AgencyId &&
                                         e.ForeignAgentId == dto.AgencyAgent.AgentId));
                                }

                                //|| Always True
                                //   (e.AgencyId == dto.AgencyAgent.AgencyId &&
                                //   e.ForeignAgentId == null));
                            }

                            var filtersExp = filtersExpressions.FirstOrDefault();
                            foreach (var filtersExpression in filtersExpressions.Skip(1))
                            {
                                filtersExp = filtersExp.Or(filtersExpression);
                            }
                            pdto.FilterList(filtersExp);
                        }
                        else// To Filter out users with no AgenciesWithAgents
                        {
                            pdto.FilterList(e => e.Notes == "No Notes");
                        }
                    }

                    #endregion

                    IList<VisaDTO> pdtoVisa;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoVisa = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                        totCount = totalCount;
                    }
                    else
                        pdtoVisa = pdto.GetList().ToList();

                    catVisa = pdtoVisa.ToList();
                }
                else
                {
                    catVisa = Get().Get().ToList();
                }

                #region For Eager Loading

                foreach (var visaDTO in catVisa)
                {
                    var dto = visaDTO;
                    var empDto = _employeeRepository
                        .Query()
                        .Filter(e => e.VisaId == dto.Id)
                        .Get()
                        .FirstOrDefault();
                }

                #endregion
            }
            catch
            {
                return null;
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }


            return catVisa;
        }

        public VisaDTO Find(string visaId)
        {
            return _visaRepository.FindById(Convert.ToInt32(visaId));
        }

        public VisaDTO GetByName(string displayName)
        {
            var cat = _visaRepository
                .Query()
                .Filter(c => c.VisaNumber.Contains(displayName))
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(VisaDTO visa)
        {
            try
            {
                var validate = Validate(visa);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(visa))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                visa.Synced = false;
                visa.Sponsor.Synced = false;
                visa.Sponsor.Address.Synced = false;
                visa.Condition.Synced = false;

                visa.Sponsor.DateLastModified = DateTime.Now;
                visa.Sponsor.Address.DateLastModified = DateTime.Now;
                visa.Condition.DateLastModified = DateTime.Now;

                _visaRepository.InsertUpdate(visa);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Disable(VisaDTO visa)
        {
            if (visa == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _visaRepository.Update(visa);
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

        public int Delete(string visaId)
        {
            try
            {
                _visaRepository.Delete(visaId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(VisaDTO visa)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();

            try
            {
                var visaRepository = new Repository<VisaDTO>(iDbContext);
                var visaExists = visaRepository.Query()
                    .Filter(visaDTO => visaDTO.VisaNumber == visa.VisaNumber && visaDTO.Id != visa.Id)
                    .Get()
                    .FirstOrDefault();
                if (visaExists != null)
                    objectExists = true;
            }

            finally
            {
                iDbContext.Dispose();
            }

            return objectExists;
        }

        public string Validate(VisaDTO visa)
        {
            if (null == visa)
                return GenericMessages.ObjectIsNull;
            //if (null == visa.Agent)
            //return GenericMessages.ObjectIsNull;
            if (null == visa.Sponsor)
                return GenericMessages.ObjectIsNull;
            if (null == visa.Condition)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(visa.VisaNumber))
                return visa.VisaNumber + " " + GenericMessages.StringIsNullOrEmpty;

            if (visa.VisaNumber.Length > 255)
                return visa.VisaNumber + " can not be more than 255 characters ";

            return string.Empty;
        }

        #endregion

        #region Visa Condition Methods

        public IRepositoryQuery<VisaConditionDTO> GetConditions()
        {
            var piVisaCondition = _visaConditionRepository
                .Query();
            return piVisaCondition;
        }

        public IEnumerable<VisaConditionDTO> GetAllConditions(SearchCriteria<VisaConditionDTO> criteria = null)
        {
            IEnumerable<VisaConditionDTO> catVisaCondition;
            try
            {
                if (criteria != null)
                {
                    var pdto = GetConditions();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<VisaConditionDTO> pdtoVisaCondition;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoVisaCondition = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoVisaCondition = pdto.GetList().ToList();

                    catVisaCondition = pdtoVisaCondition.ToList();
                }
                else
                {
                    catVisaCondition = GetConditions().Get().ToList();
                }
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catVisaCondition;
        }

        public VisaConditionDTO FindCondition(string visaConditionId)
        {
            return _visaConditionRepository.FindById(Convert.ToInt32(visaConditionId));
        }

        public VisaConditionDTO GetConditionByName(string displayName)
        {
            var cat = _visaConditionRepository
                .Query()
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdateCondition(VisaConditionDTO visaCondition)
        {
            try
            {
                var validate = ValidateCondition(visaCondition);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ConditionObjectExists(visaCondition))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                visaCondition.Synced = false;
                _visaConditionRepository.InsertUpdate(visaCondition);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string DisableCondition(VisaConditionDTO visaCondition)
        {
            if (visaCondition == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _visaConditionRepository.Update(visaCondition);
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

        public int DeleteCondition(string visaConditionId)
        {
            try
            {
                _visaConditionRepository.Delete(visaConditionId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ConditionObjectExists(VisaConditionDTO visaCondition)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<VisaConditionDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == visaCondition.FirstName && bp.Id != visaCondition.Id && bp.Type == visaCondition.Type)
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

        public string ValidateCondition(VisaConditionDTO visaCondition)
        {
            if (null == visaCondition)
                return GenericMessages.ObjectIsNull;

            return string.Empty;
        }

        #endregion

        #region Visa Sponsor Methods

        public IRepositoryQuery<VisaSponsorDTO> GetSponsors()
        {
            var piVisaSponsor = _visaSponsorRepository
                .Query()
                .Include(a => a.Address)
                .Filter(a => !string.IsNullOrEmpty(a.FirstName))
                .OrderBy(q => q.OrderBy(c => c.FirstName));
            return piVisaSponsor;
        }

        public IEnumerable<VisaSponsorDTO> GetAllSponsors(SearchCriteria<VisaSponsorDTO> criteria = null)
        {
            IEnumerable<VisaSponsorDTO> catVisaSponsor;
            try
            {
                if (criteria != null)
                {
                    var pdto = GetSponsors();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<VisaSponsorDTO> pdtoVisaSponsor;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoVisaSponsor = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoVisaSponsor = pdto.GetList().ToList();

                    catVisaSponsor = pdtoVisaSponsor.ToList();
                }
                else
                {
                    catVisaSponsor = GetSponsors().Get().ToList();
                }
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catVisaSponsor;
        }

        public VisaSponsorDTO FindSponsor(string visaSponsorId)
        {
            return _visaSponsorRepository.FindById(Convert.ToInt32(visaSponsorId));
        }

        public VisaSponsorDTO GetSponsorByName(string displayName)
        {
            var cat = _visaSponsorRepository
                .Query()
                .Filter(c => c.FirstName.Contains(displayName))
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdateSponsor(VisaSponsorDTO visaSponsor)
        {
            try
            {
                var validate = ValidateSponsor(visaSponsor);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (SponsorObjectExists(visaSponsor))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                visaSponsor.Synced = false;
                _visaSponsorRepository.InsertUpdate(visaSponsor);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string DisableSponsor(VisaSponsorDTO visaSponsor)
        {
            if (visaSponsor == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _visaSponsorRepository.Update(visaSponsor);
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

        public int DeleteSponsor(string visaSponsorId)
        {
            try
            {
                _visaSponsorRepository.Delete(visaSponsorId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool SponsorObjectExists(VisaSponsorDTO visaSponsor)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<VisaSponsorDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == visaSponsor.FirstName && bp.Id != visaSponsor.Id && bp.Type == visaSponsor.Type)
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

        public string ValidateSponsor(VisaSponsorDTO visaSponsor)
        {
            if (null == visaSponsor)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(visaSponsor.FirstName))
                return visaSponsor.FirstName + " " + GenericMessages.StringIsNullOrEmpty;

            if (visaSponsor.FirstName.Length > 255)
                return visaSponsor.FirstName + " can not be more than 255 characters ";

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