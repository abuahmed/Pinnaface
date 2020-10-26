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
    public class ListService : IListService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<ListDTO> _listRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public ListService()
        {
            InitializeDbContext();
        }
        public ListService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _listRepository = new Repository<ListDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<ListDTO> Get()
        {
            var piList = _listRepository
                .Query()
                .Filter(a => !string.IsNullOrEmpty(a.DisplayName))
                .OrderBy(q => q.OrderBy(c => c.DisplayName));
            return piList;
        }

        public IEnumerable<ListDTO> GetAll(SearchCriteria<ListDTO> criteria = null)
        {
            IEnumerable<ListDTO> catList;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<ListDTO> pdtoList;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoList = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoList = pdto.GetList().ToList();

                    catList = pdtoList.ToList();
                }
                else
                {
                    catList = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catList;
        }

        public ListDTO Find(string listId)
        {
            return _listRepository.FindById(Convert.ToInt32(listId));
        }

        public ListDTO GetByName(string displayName)
        {
            var cat = _listRepository
                .Query()
                .Filter(c => c.DisplayName == displayName)
                .Get().FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(ListDTO list)
        {
            try
            {
                var validate = Validate(list);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(list))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                _listRepository.InsertUpdate(list);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(ListDTO list)
        {
            if (list == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _listRepository.Update(list);
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

        public int Delete(string listId)
        {
            try
            {
                _listRepository.Delete(listId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(ListDTO list)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<ListDTO>(iDbContext);
                var catExists = catRepository.Query()
                    .Filter(bp => bp.DisplayName == list.DisplayName && bp.Id != list.Id && bp.Type == list.Type)
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

        public string Validate(ListDTO list)
        {
            if (null == list)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(list.DisplayName))
                return list.DisplayName + " " + GenericMessages.StringIsNullOrEmpty;

            if (list.DisplayName.Length > 255)
                return list.DisplayName + " can not be more than 255 characters ";

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