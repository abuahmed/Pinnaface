using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IInsuranceProcessService : IDisposable
    {
        IEnumerable<InsuranceProcessDTO> GetAll(SearchCriteria<InsuranceProcessDTO> criteria = null);
        InsuranceProcessDTO Find(string insuranceProcessId);
        InsuranceProcessDTO GetByName(string displayName);
        string InsertOrUpdate(InsuranceProcessDTO insuranceProcess);
        string Disable(InsuranceProcessDTO insuranceProcess);
        int Delete(string insuranceProcessId);
    }
}