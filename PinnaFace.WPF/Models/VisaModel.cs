using System;
using PinnaFace.Core.Models;

namespace PinnaFace.WPF.Models
{
    public class VisaModel
    {
        public VisaModel()
        {
            VisaId = null;
            Employee = null;
            VisaNumber = String.Empty;
        }

        public string VisaNumber { get; set; }
        public int? VisaId { get; set; }
        public EmployeeDTO Employee { get; set; }
    }
}