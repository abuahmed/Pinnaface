using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IForeignAgentService : IDisposable
    {
        IEnumerable<AgentDTO> GetAll(SearchCriteria<AgentDTO> criteria = null);
        AgentDTO Find(string foreignAgentId);
        AgentDTO GetByName(string displayName);
        string InsertOrUpdate(AgentDTO agent);
        string Disable(AgentDTO agent);
        int Delete(string foreignAgentId);
    }
}