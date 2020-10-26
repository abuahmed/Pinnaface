using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service.Interfaces;

namespace PinnaFace.Service
{
    public class EmployeeService : IEmployeeService
    {
        #region Fields

        private readonly bool _disposeWhenDone;
        private readonly bool _includeChilds;
        private IRepository<EmployeeDTO> _employeeRepository;
        private IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        public EmployeeService()
        {
            _includeChilds = true;
            InitializeDbContext();
        }

        public EmployeeService(IDbContext iDbContext)
        {
            _employeeRepository = new Repository<EmployeeDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        public EmployeeService(bool disposeWhenDone, bool includeChilds)
        {
            _includeChilds = includeChilds;
            _disposeWhenDone = disposeWhenDone;
            InitializeDbContext();
        }

        public void InitializeDbContext()
        {
            IDbContext iDbContext = DbContextUtil.GetDbContextInstance();
            _employeeRepository = new Repository<EmployeeDTO>(iDbContext);
            _unitOfWork = new UnitOfWork(iDbContext);
        }

        #endregion

        #region Common Methods

        public IEnumerable<EmployeeDTO> GetAll(SearchCriteria<EmployeeDTO> criteria = null)
        {
            int totalCount;
            return GetAll(criteria, out totalCount);
        }

        public IEnumerable<EmployeeDTO> GetAll(SearchCriteria<EmployeeDTO> criteria, out int totCount)
        {
            totCount = 0;
            IEnumerable<EmployeeDTO> catEmployee;
            try
            {
                if (criteria != null)
                {
                    IRepositoryQuery<EmployeeDTO> pdto = Get();

                    foreach (var cri in criteria.FiList)
                    {
                        pdto.FilterList(cri);
                    }

                    #region By Duration

                    if (criteria.BeginingDate != null)
                    {
                        var beginDate = new DateTime(criteria.BeginingDate.Value.Year, criteria.BeginingDate.Value.Month,
                            criteria.BeginingDate.Value.Day, 0, 0, 0);
                        switch (criteria.ReportType)
                        {
                            case ReportTypes.TicketList:
                            case ReportTypes.TicketAmountList:
                            case ReportTypes.LabourMonthly:
                                pdto.FilterList(p => p.FlightProcess.SubmitDate >= beginDate);
                                break;
                            case ReportTypes.LabourReturned:
                            case ReportTypes.LabourLost:
                                pdto.FilterList(p => p.AfterFlightStatusDate >= beginDate);
                                break;
                            case ReportTypes.LabourContractEnd:
                                pdto.FilterList(p => p.LabourProcess.ContratEndDate >= beginDate);
                                break;
                            case ReportTypes.LabourDiscontinued:
                                pdto.FilterList(p => p.DiscontinuedDate >= beginDate);
                                break;
                            case ReportTypes.EmbassyMonthly:
                                pdto.FilterList(p => p.EmbassyProcess.SubmitDate >= beginDate);
                                break;
                        }
                    }

                    if (criteria.EndingDate != null)
                    {
                        var endDate = new DateTime(criteria.EndingDate.Value.Year, criteria.EndingDate.Value.Month,
                            criteria.EndingDate.Value.Day, 23, 59, 59);
                        switch (criteria.ReportType)
                        {
                            case ReportTypes.TicketList:
                            case ReportTypes.TicketAmountList:
                            case ReportTypes.LabourMonthly:
                                pdto.FilterList(p => p.FlightProcess.SubmitDate <= endDate);
                                break;
                            case ReportTypes.LabourReturned:
                            case ReportTypes.LabourLost:
                                pdto.FilterList(p => p.AfterFlightStatusDate <= endDate);
                                break;
                            case ReportTypes.LabourContractEnd:
                                pdto.FilterList(p => p.LabourProcess.ContratEndDate <= endDate);
                                break;
                            case ReportTypes.LabourDiscontinued:
                                pdto.FilterList(p => p.DiscontinuedDate <= endDate);
                                break;
                            case ReportTypes.EmbassyMonthly:
                                pdto.FilterList(p => p.EmbassyProcess.SubmitDate <= endDate);
                                break;
                        }
                    }

                    #endregion

                    #region Filter By User

                    if (Singleton.Edition == PinnaFaceEdition.WebEdition && criteria.CurrentUserId!=1)
                    {
                        var currentUser = new UserService(true)
                            .GetAll(new UserSearchCriteria<UserDTO>())
                            .FirstOrDefault(u => u.UserId == criteria.CurrentUserId);

                        if (currentUser != null && currentUser.AgenciesWithAgents != null &&
                            currentUser.AgenciesWithAgents.Count > 0)
                        {
                            IList<Expression<Func<EmployeeDTO, bool>>> filtersExpressions =
                                new List<Expression<Func<EmployeeDTO, bool>>>();

                            var agencyAgents = currentUser.AgenciesWithAgents;
                            foreach (var agencyAgentsDto in agencyAgents)
                            {
                                var dto = agencyAgentsDto;

                                filtersExpressions.Add(e =>
                                    (e.AgencyId == dto.AgencyAgent.AgencyId &&
                                     e.AgentId == dto.AgencyAgent.AgentId) ||
                                    (e.AgencyId == dto.AgencyAgent.AgencyId &&
                                     e.AgentId == null));
                                //(e.AgencyId == null && ALWAYS TRUE
                                //e.AgentId == dto.AgencyAgent.AgentId) ||
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
                            pdto.FilterList(e => e.MoreNotes == "No Notes");
                        }
                    }

                    #endregion

                    IList<EmployeeDTO> pdtoEmployee;
                    if (criteria.Page != 0 && criteria.PageSize != 0)
                    {
                        int totalCount;
                        pdtoEmployee = pdto.GetPage(criteria.Page, criteria.PageSize, out totalCount).ToList();
                        totCount = totalCount;
                    }
                    else
                        pdtoEmployee = pdto.GetList().ToList();

                    catEmployee = pdtoEmployee.ToList();
                }
                else
                {
                    catEmployee = Get().Get().ToList();
                }
            }
            finally
            {
                Dispose(_disposeWhenDone);
            }

            return catEmployee;
        }

        public EmployeeDTO Find(string employeeId)
        {
            return _employeeRepository.FindById(Convert.ToInt32(employeeId));
        }

        public EmployeeDTO GetByName(string displayName)
        {
            EmployeeDTO cat = _employeeRepository
                .Query()
                .Filter(c => c.FirstName.Contains(displayName))
                .Get()
                .FirstOrDefault();
            return cat;
        }

        public string InsertOrUpdate(EmployeeDTO employee)
        {
            try
            {
                string validate = Validate(employee);
                if (!string.IsNullOrEmpty(validate))
                    return validate;

                if (ObjectExists(employee))
                    return GenericMessages.DatabaseErrorRecordAlreadyExists;

                employee.CurrentStatus = GetEmployeeCurrentStatus(employee);
                employee.Synced = false;
                _employeeRepository.InsertUpdate(employee);
                int err=_unitOfWork.Commit();
                if (err != -1)
                    return string.Empty;
                else return "Error Saving Employee";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }

        public string Disable(EmployeeDTO employee)
        {
            if (employee == null)
                return GenericMessages.ObjectIsNull;

            string stat;
            IDbContext iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                _employeeRepository.Update(employee);
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

        public int Delete(string employeeId)
        {
            try
            {
                _employeeRepository.Delete(employeeId);
                _unitOfWork.Commit();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public IRepositoryQuery<EmployeeDTO> Get()
        {
            RepositoryQuery<EmployeeDTO> piEmployee = _employeeRepository
                .Query()
                .Include(c => c.Photo, c => c.StandPhoto, c => c.Address, c => c.ContactPerson,
                    c => c.ContactPerson.Address)
                .Include(c => c.Education, c => c.Experience, c => c.Hawala, c => c.InsuranceProcess,
                    c => c.RequiredDocuments)
                .Include(c => c.Agency, c => c.Agent)
                .Filter(a => !string.IsNullOrEmpty(a.FirstName))
                .OrderBy(q => q.OrderByDescending(c => c.Id));

            if (_includeChilds)
                piEmployee = piEmployee.Include(
                    c => c.CurrentComplain, c => c.CurrentComplain.Remarks, c => c.Complains,
                    c => c.Visa, c => c.Visa.Condition,
                    c => c.Visa.Sponsor, c => c.Visa.Sponsor.Address,
                    c => c.LabourProcess, c => c.EmbassyProcess, c => c.FlightProcess,
                    c => c.Visa.Agent, c => c.Visa.Agent.Address);
            
            return piEmployee;
        }

        public bool ObjectExists(EmployeeDTO employee)
        {
            var objectExists = false;
            var iDbContext = DbContextUtil.GetDbContextInstance();
            try
            {
                var empRepository = new Repository<EmployeeDTO>(iDbContext);
                var empExists = empRepository.Query()
                    .Filter(employeeDTO => employeeDTO.PassportNumber == employee.PassportNumber && employeeDTO.Id != employee.Id)
                    .Get()
                    .FirstOrDefault();
                if (empExists != null)
                    objectExists = true;
            }
            finally
            {
                iDbContext.Dispose();
            }

            return objectExists;
        }

        public string Validate(EmployeeDTO employee)
        {
            if (null == employee)
                return GenericMessages.ObjectIsNull;

            if (String.IsNullOrEmpty(employee.FirstName))
                return employee.FirstName + " " + GenericMessages.StringIsNullOrEmpty;

            if (employee.FirstName.Length > 255)
                return employee.FirstName + " can not be more than 255 characters ";

            return string.Empty;
        }

        #endregion

        #region Disposing

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
        }

        #endregion

        private ProcessStatusTypes GetEmployeeCurrentStatus(EmployeeDTO selectedEmployee)
        {
            var currentStatus = ProcessStatusTypes.New;
            if (selectedEmployee.Visa != null || (selectedEmployee.VisaId != null && selectedEmployee.VisaId != 0))
                currentStatus = ProcessStatusTypes.VisaAssigned;
            if (selectedEmployee.LabourProcess != null || selectedEmployee.EmbassyProcess != null)
                currentStatus = ProcessStatusTypes.OnProcess;
            if (selectedEmployee.LabourProcess != null && selectedEmployee.Discontinued)
                currentStatus = ProcessStatusTypes.Discontinued;
            if (selectedEmployee.EmbassyProcess != null && selectedEmployee.EmbassyProcess.Stammped)
                currentStatus = ProcessStatusTypes.FlightProcess;
            if (selectedEmployee.EmbassyProcess != null && selectedEmployee.EmbassyProcess.Canceled)
                currentStatus = ProcessStatusTypes.Canceled;
            if (selectedEmployee.FlightProcess != null)
                currentStatus = ProcessStatusTypes.BookedDepartured;
            if (selectedEmployee.FlightProcess != null && selectedEmployee.FlightProcess.Departured)
                currentStatus = ProcessStatusTypes.OnGoodCondition;
            if (selectedEmployee.FlightProcess != null &&
                selectedEmployee.AfterFlightStatus == AfterFlightStatusTypes.Returned)
                currentStatus = ProcessStatusTypes.Returned;
            if (selectedEmployee.FlightProcess != null &&
                selectedEmployee.AfterFlightStatus == AfterFlightStatusTypes.Lost)
                currentStatus = ProcessStatusTypes.Lost;
            //if (selectedEmployee.Complains.ToList()
            //    .Where(c =>
            //            c.Status == ComplainStatusTypes.Opened ||
            //            c.Status == ComplainStatusTypes.ReOpened ||
            //            c.Status == ComplainStatusTypes.OnProcess)
            //    .ToList().Count > 0)
            if (selectedEmployee.CurrentComplain != null)
                currentStatus = ProcessStatusTypes.WithComplain;

            return currentStatus;
        }
    }
}