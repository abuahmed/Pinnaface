using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IEmbassyProcessService : IDisposable
    {
        IEnumerable<EmbassyProcessDTO> GetAll(SearchCriteria<EmbassyProcessDTO> criteria = null);
        EmbassyProcessDTO Find(string embassyProcessId);
        EmbassyProcessDTO GetByName(string displayName);
        string InsertOrUpdate(EmbassyProcessDTO embassyProcess);
        string Disable(EmbassyProcessDTO embassyProcess);
        int Delete(string embassyProcessId);
    }
}