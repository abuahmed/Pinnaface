using System;
using System.Collections.Generic;
using System.Linq;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service.Interfaces;

namespace PinnaFace.Service
{
    public class FlightProcessService : IFlightProcessService
    {
        #region Fields
        private IUnitOfWork _unitOfWork;
        private IRepository<FlightProcessDTO> _flightProcessRepository;
        private readonly bool _disposeWhenDone;
        #endregion

        #region Constructor
        public FlightProcessService()
        {
            InitializeDbContext();
        }
        public FlightProcessService(IDbContext iDbContext)
        {
            _flightProcessRepository = new Repository<FlightProcessDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        public FlightProcessService(bool disposeWhenDone)
        {
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            var iDbContext = DbContextUtil.GetDbContextInstance();
            _flightProcessRepository = new Repository<FlightProcessDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }
        #endregion

        #region Common Methods
        public IRepositoryQuery<FlightProcessDTO> Get()
        {
            var piFlightProcess = _flightProcessRepository
                .Query();
            //.Include(e => e.Employee, e => e.Employee.Visa, e => e.Employee.Visa.Sponsor, e => e.Employee.Visa.Sponsor.Address);
            return piFlightProcess;
        }

        public IEnumerable<FlightProcessDTO> GetAll(SearchCriteria<FlightProcessDTO> criteria = null)
        {
            IEnumerable<FlightProcessDTO> catFlightProcess;
            try
            {
                if (criteria != null)
                {
                    var pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    IList<FlightProcessDTO> pdtoFlightProcess;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoFlightProcess = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                    }
                    else
                        pdtoFlightProcess = pdto.GetList().ToList();

                    catFlightProcess = pdtoFlightProcess.ToList();
                }
                else
                {
                    catFlightProcess = Get().Get().ToList();
                }

            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catFlightProcess;
        }

        public FlightProcessDTO Find(string flightProcessId)
        {
            return _flightProcessRepository.FindById(Convert.ToInt32(flightProcessId));
        }

        public FlightProcessDTO GetByName(string displayName)
        {
            var cat = _flightProcessRepository
                .Query()
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(FlightProcessDTO flightProcess)
        {
            try
            {
                var validate = Validate(flightProcess);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(flightProcess))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                flightProcess.Synced = false;
                _flightProcessRepository.InsertUpdate(flightProcess);
                _unitOfWork.Commit();
                return string.Empty;
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(FlightProcessDTO flightProcess)
        {
            if (flightProcess == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _flightProcessRepository.Update(flightProcess);
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

        public int Delete(string flightProcessId)
        {
            try
            {
                _flightProcessRepository.Delete(Convert.ToInt32(flightProcessId));
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public bool ObjectExists(FlightProcessDTO flightProcess)
        {
            //var objectExists = false;
            //var iDbContext = DbContextUtil.GetDbContextInstance();
            //try
            //{
            //    var catRepository = new Repository<FlightProcessDTO>(iDbContext);
            //    var catExists = catRepository.Query()
            //        .Filter(bp => bp.FirstName == flightProcess.FirstName && bp.Id != flightProcess.Id && bp.Type == flightProcess.Type)
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

        public string Validate(FlightProcessDTO flightProcess)
        {
            if (null == flightProcess)
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