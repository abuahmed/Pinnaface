using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface ILabourProcessService : IDisposable
    {
        IEnumerable<LabourProcessDTO> GetAll(SearchCriteria<LabourProcessDTO> criteria = null);
        LabourProcessDTO Find(string labourProcessId);
        LabourProcessDTO GetByName(string displayName);
        string InsertOrUpdate(LabourProcessDTO labourProcess);
        string Disable(LabourProcessDTO labourProcess);
        int Delete(string labourProcessId);
    }
}