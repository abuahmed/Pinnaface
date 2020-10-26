using System;
using System.Collections.Generic;
using PinnaFace.Core;
using PinnaFace.Core.Models;

namespace PinnaFace.Service.Interfaces
{
    public interface IFlightProcessService : IDisposable
    {
        IEnumerable<FlightProcessDTO> GetAll(SearchCriteria<FlightProcessDTO> criteria = null);
        FlightProcessDTO Find(string flightProcessId);
        FlightProcessDTO GetByName(string displayName);
        string InsertOrUpdate(FlightProcessDTO flightProcess);
        string Disable(FlightProcessDTO flightProcess);
        int Delete(string flightProcessId);
    }
}