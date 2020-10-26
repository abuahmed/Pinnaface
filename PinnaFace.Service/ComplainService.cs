using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
    public class ComplainService : IComplainService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<ComplainDTO> _complainRepository;
        private IRepository<ComplainRemarkDTO> _complainRemarkRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public ComplainService()
        {
            InitializeDbContext();
        }
        public ComplainService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _complainRemarkRepository = new Repository<ComplainRemarkDTO>(iDbContext);
            _complainRepository = new Repository<ComplainDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<ComplainDTO> Get()
        {
            var piComplain = _complainRepository
                .Query()
                .Include(c => c.Remarks, e => e.Employee, e => e.Employee.Visa, e => e.Employee.Visa.Sponsor)
                .Filter(a => !string.IsNullOrEmpty(a.Complain))
                .OrderBy(q => q.OrderBy(c => c.Id));
            return piComplain;
        }

        public IEnumerable<ComplainDTO> GetAll(SearchCriteria<ComplainDTO> criteria = null)
        {
            int totalCount;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<ComplainDTO> GetAll(SearchCriteria<ComplainDTO> criteria, out int totCount)
        {
            totCount = 0;
            IEnumerable<ComplainDTO> catComplain;
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

                        if (currentUser != null && currentUser.AgenciesWithAgents != null && currentUser.AgenciesWithAgents.Count > 0)
                        {
                            IList<Expression<Func<ComplainDTO, bool>>> filtersExpressions =
                                new List<Expression<Func<ComplainDTO, bool>>>();

                            var agencyAgents = currentUser.AgenciesWithAgents;
                            foreach (var agencyAgentsDto in agencyAgents)
                            {
                                var dto = agencyAgentsDto;
                                filtersExpressions.Add(e => e.Employee.AgencyId == dto.AgencyAgent.AgencyId &&
                                                            e.Employee.AgentId == dto.AgencyAgent.AgentId);
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
                            pdto.FilterList(e => e.Complain == "No Complain");
                        }
                    }

                    #endregion

                    IList<ComplainDTO> pdtoComplain;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoComplain = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                        totCount = totalCount;
                    }
                    else
                        pdtoComplain = pdto.GetList().ToList();

                    catComplain = pdtoComplain.ToList();
                }
                else
                {
                    catComplain = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catComplain;
        }

        public ComplainDTO Find(string complainId)
        {
            return _complainRepository.FindById(Convert.ToInt32(complainId));
        }

        public ComplainDTO GetByName(string displayName)
        {
            var cat = _complainRepository
                .Query()
                .Filter(c => c.Complain.Contains(displayName))
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(ComplainDTO complain)
        {
            try
            {
                var validate = Validate(complain);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(complain))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;
                
                complain.Synced = false;
                _complainRepository.InsertUpdate(complain);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (DbUpdateConcurrencyException)
            {
                return GenericMessages.DatabaseConcurrencyIssue;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }

        }

        public string Disable(ComplainDTO complain)
        {
            if (complain == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _complainRepository.Update(complain);
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

        public int Delete(string complainId)
        {
            try
            {
                _complainRepository.Delete(complainId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(ComplainDTO complain)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<ComplainDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == complain.FirstName && bp.Id != complain.Id && bp.Type == complain.Type)
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

        public string Validate(ComplainDTO complain)
        {
            if (null == complain)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(complain.Complain))
                return complain.Complain + " " + GenericMessages.StringIsNullOrEmpty;

            
            return string.Empty;
        }

        #endregion

        #region Remarks
        public IRepositoryQuery<ComplainRemarkDTO> GetRemark()
        {
            var piComplainRemark = _complainRemarkRepository
                .Query()
                .Include(c=>c.Complain)
                .Filter(a => !string.IsNullOrEmpty(a.Remark))
                .OrderBy(q => q.OrderBy(c => c.Remark));
            return piComplainRemark;
        }

        public IEnumerable<ComplainRemarkDTO> GetAllRemarks(SearchCriteria<ComplainRemarkDTO> criteria = null)
        {
            IEnumerable<ComplainRemarkDTO> catComplainRemark;
            try
            {
                if (criteria != null)
                {
                    var pdto = GetRemark();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<ComplainRemarkDTO> pdtoComplainRemark;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoComplainRemark = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoComplainRemark = pdto.GetList().ToList();

                    catComplainRemark = pdtoComplainRemark.ToList();
                }
                else
                {
                    catComplainRemark = GetRemark().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catComplainRemark;
        }

        public ComplainRemarkDTO FindRemark(string complainRemarkId)
        {
            return _complainRemarkRepository.FindById(Convert.ToInt32(complainRemarkId));
        }

        public ComplainRemarkDTO GetRemarkByName(string displayName)
        {
            var cat = _complainRemarkRepository
                .Query()
                .Filter(c => c.Remark.Contains(displayName))
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdateRemark(ComplainRemarkDTO complainRemark)
        {
            try
            {
                var validate = ValidateRemark(complainRemark);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (RemarkObjectExists(complainRemark))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;
                
                complainRemark.Synced = false;
                _complainRemarkRepository.InsertUpdate(complainRemark);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string DisableRemark(ComplainRemarkDTO complainRemark)
        {
            if (complainRemark == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _complainRemarkRepository.Update(complainRemark);
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

        public int DeleteRemark(string complainRemarkId)
        {
            try
            {
                _complainRemarkRepository.Delete(complainRemarkId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool RemarkObjectExists(ComplainRemarkDTO complainRemark)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<ComplainRemarkDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == complainRemark.FirstName && bp.Id != complainRemark.Id && bp.Type == complainRemark.Type)
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

        public string ValidateRemark(ComplainRemarkDTO complainRemark)
        {
            if (null == complainRemark)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(complainRemark.Remark))
                return complainRemark.Remark + " " + GenericMessages.StringIsNullOrEmpty;

            if (complainRemark.Remark.Length > 255)
                return complainRemark.Remark + " can not be more than 255 characters ";

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