using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IEmployeeRelativeService : IDisposable
    {
        IEnumerable<EmployeeRelativeDTO> GetAll(SearchCriteria<EmployeeRelativeDTO> criteria = null);
        EmployeeRelativeDTO Find(string employeeRelativeId);
        EmployeeRelativeDTO GetByName(string displayName);
        string InsertOrUpdate(EmployeeRelativeDTO employeeRelative);
        string Disable(EmployeeRelativeDTO employeeRelative);
        int Delete(string employeeRelativeId);
    }
}