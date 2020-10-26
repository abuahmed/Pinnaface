using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IListService : IDisposable
    {
        IEnumerable<ListDTO> GetAll(SearchCriteria<ListDTO> criteria = null);
        ListDTO Find(string listId);
        ListDTO GetByName(string displayName);
        string InsertOrUpdate(ListDTO list);
        string Disable(ListDTO list);
        int Delete(string listId);
    }
}