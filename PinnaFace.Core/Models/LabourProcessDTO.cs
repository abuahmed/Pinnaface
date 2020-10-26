using System;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class LabourProcessDTO : CommonFieldsB
    {
        public DateTime? ContratBeginDate
        {
            get { return GetValue(() => ContratBeginDate); }
            set { SetValue(() => ContratBeginDate, value); }
        }
        public DateTime? ContratEndDate
        {
            get { return GetValue(() => ContratEndDate); }
            set { SetValue(() => ContratEndDate, value); }
        }

        public bool AgreementReturned
        {
            get { return GetValue(() => AgreementReturned); }
            set
            {
                SetValue(() => AgreementReturned, value);
                SetValue(() => AgreementReturnedString, value.ToString());
            }
        }

        public string AgreementFileName
        {
            get { return GetValue(() => AgreementFileName); }
            set { SetValue(() => AgreementFileName, value); }
        }
        
        [NotMapped]
        public string AgreementReturnedString
        {
            get
            {
                if (AgreementReturned)
                    return "Has Agreement";
                return SubmitDate.Year > 1980 ? "No-Agreement" : "";
            }
            set { SetValue(() => AgreementReturnedString, value); }
        }

        [NotMapped]
        public string ContratBeginDateString
        {
            get
            {
                if (ContratBeginDate != null) return ContratBeginDate.Value.ToString("dd/M/yyyy");
                return "";
            }
            set { SetValue(() => ContratBeginDateString, value); }
        }

        [NotMapped]
        public string ContratEndDateString
        {
            get
            {
                if (ContratEndDate != null) return ContratEndDate.Value.ToString("dd/M/yyyy");
                return "";
            }
            set { SetValue(() => ContratEndDateString, value); }
        }
    }
}
