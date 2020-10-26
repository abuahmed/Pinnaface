using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IEmployeeEducationService : IDisposable
    {
        IEnumerable<EmployeeEducationDTO> GetAll(SearchCriteria<EmployeeEducationDTO> criteria = null);
        EmployeeEducationDTO Find(string employeeEducationId);
        EmployeeEducationDTO GetByName(string displayName);
        string InsertOrUpdate(EmployeeEducationDTO employeeEducation);
        string Disable(EmployeeEducationDTO employeeEducation);
        int Delete(string employeeEducationId);
    }
}