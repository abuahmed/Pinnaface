using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IEmployeeExperienceService : IDisposable
    {
        IEnumerable<EmployeeExperienceDTO> GetAll(SearchCriteria<EmployeeExperienceDTO> criteria = null);
        EmployeeExperienceDTO Find(string employeeApplicationId);
        EmployeeExperienceDTO GetByName(string displayName);
        string InsertOrUpdate(EmployeeExperienceDTO employeeExperience);
        string Disable(EmployeeExperienceDTO employeeExperience);
        int Delete(string employeeApplicationId);
    }
}