using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface ILocalAgencyService : IDisposable
    {
        IEnumerable<AgencyDTO> GetAll(SearchCriteria<AgencyDTO> criteria = null);
        AgencyDTO GetLocalAgency();
        string InsertOrUpdate(AgencyDTO agency);
    }

    public interface ISettingService : IDisposable
    {
        IEnumerable<SettingDTO> GetAll(SearchCriteria<SettingDTO> criteria = null);
        SettingDTO GetSetting();
        string InsertOrUpdate(SettingDTO agency);
    }
}