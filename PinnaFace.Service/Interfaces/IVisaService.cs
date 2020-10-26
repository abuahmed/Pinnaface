using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IVisaService : IDisposable
    {
        IEnumerable<VisaDTO> GetAll(SearchCriteria<VisaDTO> criteria = null);
        IEnumerable<VisaDTO> GetAll(SearchCriteria<VisaDTO> criteria, out int totCount);
        VisaDTO Find(string visaId);
        VisaDTO GetByName(string displayName);
        string InsertOrUpdate(VisaDTO visa);
        string Disable(VisaDTO visa);
        int Delete(string visaId);

        IEnumerable<VisaConditionDTO> GetAllConditions(SearchCriteria<VisaConditionDTO> criteria = null);
        VisaConditionDTO FindCondition(string visaConditionId);
        VisaConditionDTO GetConditionByName(string displayName);
        string InsertOrUpdateCondition(VisaConditionDTO visaCondition);
        string DisableCondition(VisaConditionDTO visaCondition);
        int DeleteCondition(string visaConditionId);

        IEnumerable<VisaSponsorDTO> GetAllSponsors(SearchCriteria<VisaSponsorDTO> criteria = null);
        VisaSponsorDTO FindSponsor(string visaSponsorId);
        VisaSponsorDTO GetSponsorByName(string displayName);
        string InsertOrUpdateSponsor(VisaSponsorDTO visaSponsor);
        string DisableSponsor(VisaSponsorDTO visaSponsor);
        int DeleteSponsor(string visaSponsorId);
    }
}