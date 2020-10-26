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
    public class LocalAgencyService : ILocalAgencyService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<AgencyDTO> _localAgencyRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public LocalAgencyService()
        {
            InitializeDbContext();
        }

        public LocalAgencyService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            //_localAgencyRepository = new Repository<AgencyDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
            _localAgencyRepository = _unitOfWork.Repository<AgencyDTO>();
        }
        #endregion

        #region Common Methods

        public IRepositoryQuery<AgencyDTO> Get()
        {
            var piList = _localAgencyRepository
                .Query()
                .Include(a => a.Address, a => a.Header, a => a.Footer)
                .Filter(a => !string.IsNullOrEmpty(a.AgencyName))
                .OrderBy(q => q.OrderBy(c => c.AgencyName));
            return piList;
        }

        public IEnumerable<AgencyDTO> GetAll(SearchCriteria<AgencyDTO> criteria = null)
        {
            IEnumerable<AgencyDTO> catLocalAgency;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<AgencyDTO> pdtoLocalAgency;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoLocalAgency = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoLocalAgency = pdto.GetList().ToList();

                    catLocalAgency = pdtoLocalAgency.ToList();
                }
                else
                {
                    catLocalAgency = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catLocalAgency;
        }


        public AgencyDTO GetLocalAgency()
        {
            AgencyDTO ag;
            try
            {
                ag = Get().Get().FirstOrDefault();
            }
            finally
            {
                if(_disposeWhenDone)
                    _unitOfWork.Dispose();
            }
            return ag;
        }

        public string InsertOrUpdate(AgencyDTO agency)
        {
            string stat;
            try
            {
                var validate = Validate(agency);
                if (!string.IsNullOrEmpty(validate))
                    stat = validate;
                else
                {
                    if (ObjectExists(agency))
                        stat = GenericMessages.DatabaseErrorRecordAlreadyExists;
                    else
                    {
                        agency.Synced = false;
                        _localAgencyRepository.InsertUpdate(agency);
                        _unitOfWork.Commit();
                        stat = string.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return stat;
        }

        public bool ObjectExists(AgencyDTO agency)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<AgencyDTO>(iDbContext);
                var catExists = catRepository
                    .Query()
                    .Filter(bp => bp.AgencyName == agency.AgencyName && bp.Id != agency.Id)
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

        public string Validate(AgencyDTO agency)
        {
            if (agency == null)
                return GenericMessages.ObjectIsNull;

            if (agency.Address == null)
                return "Address " + GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(agency.AgencyName))
                return agency.AgencyName + " " + GenericMessages.StringIsNullOrEmpty;

            if (agency.AgencyName.Length > 255)
                return agency.AgencyName + " can not be more than 255 characters ";

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

    public class SettingService : ISettingService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<SettingDTO> _localSettingRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public SettingService()
        {
            InitializeDbContext();
        }

        public SettingService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            //_localSettingRepository = new Repository<SettingDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
            _localSettingRepository = _unitOfWork.Repository<SettingDTO>();
        }
        #endregion

        #region Common Methods

        public IRepositoryQuery<SettingDTO> Get()
        {
            var piList = _localSettingRepository
                .Query();
            return piList;
        }

        public IEnumerable<SettingDTO> GetAll(SearchCriteria<SettingDTO> criteria = null)
        {
            IEnumerable<SettingDTO> catSetting;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<SettingDTO> pdtoSetting;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoSetting = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoSetting = pdto.GetList().ToList();

                    catSetting = pdtoSetting.ToList();
                }
                else
                {
                    catSetting = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catSetting;
        }


        public SettingDTO GetSetting()
        {
            SettingDTO ag;
            try
            {
                ag = Get().Get().FirstOrDefault();
            }
            finally
            {
                if (_disposeWhenDone)
                    _unitOfWork.Dispose();
            }
            return ag;
        }

        public string InsertOrUpdate(SettingDTO setting)
        {
            string stat;
            try
            {
                var validate = Validate(setting);
                if (!string.IsNullOrEmpty(validate))
                    stat = validate;
                else
                {
                    if (ObjectExists(setting))
                        stat = GenericMessages.DatabaseErrorRecordAlreadyExists;
                    else
                    {
                        setting.Synced = false;
                        _localSettingRepository.InsertUpdate(setting);
                        _unitOfWork.Commit();
                        stat = string.Empty;
                    }
                }
            }
            catch (Exception exception)
            {
                stat = exception.Message;
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }
            return stat;
        }

        public bool ObjectExists(SettingDTO setting)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var catRepository = new Repository<SettingDTO>(iDbContext);
                var catExists = catRepository
                    .Query()
                    //.Filter(bp => bp.SettingName == setting.SettingName && bp.Id != setting.Id)
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

        public string Validate(SettingDTO setting)
        {
            if (setting == null)
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