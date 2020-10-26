using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IEmployeeService : IDisposable
    {
        IEnumerable<EmployeeDTO> GetAll(SearchCriteria<EmployeeDTO> criteria = null);
        IEnumerable<EmployeeDTO> GetAll(SearchCriteria<EmployeeDTO> criteria,out int totalCount);
        EmployeeDTO Find(string employeeId);
        EmployeeDTO GetByName(string displayName);
        string InsertOrUpdate(EmployeeDTO employee);
        string Disable(EmployeeDTO employee);
        int Delete(string employeeId);
    }
}