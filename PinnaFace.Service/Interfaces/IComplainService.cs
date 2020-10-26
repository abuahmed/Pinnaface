using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IComplainService : IDisposable
    {
        IEnumerable<ComplainDTO> GetAll(SearchCriteria<ComplainDTO> criteria = null);
        IEnumerable<ComplainDTO> GetAll(SearchCriteria<ComplainDTO> criteria, out int totCount);
        ComplainDTO Find(string complainId);
        ComplainDTO GetByName(string displayName);
        string InsertOrUpdate(ComplainDTO complain);
        string Disable(ComplainDTO complain);
        int Delete(string complainId);

        IEnumerable<ComplainRemarkDTO> GetAllRemarks(SearchCriteria<ComplainRemarkDTO> criteria = null);
        ComplainRemarkDTO FindRemark(string complainRemarkId);
        ComplainRemarkDTO GetRemarkByName(string displayName);
        string InsertOrUpdateRemark(ComplainRemarkDTO complainRemark);
        string DisableRemark(ComplainRemarkDTO complainRemark);
        int DeleteRemark(string complainRemarkId);
    }
}