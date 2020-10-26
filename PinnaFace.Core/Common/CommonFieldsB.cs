using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;

namespace PinnaFace.Core.Common
{
    public class CommonFieldsB : CommonFieldsA
    {
        [Required]
        public DateTime SubmitDate
        {
            get { return GetValue(() => SubmitDate); }
            set
            {
                SetValue(() => SubmitDate, value);
                SetValue(() => SubmitDateString, value.ToLongDateString());
            }
        } //Insured/Submitted/Booked/Departured        

        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }
        
        [NotMapped]
        public string SubmitDateString
        {
            get
            {
                return SubmitDate.ToString("dd-MMM-yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(SubmitDate,"-") + ")";
            }
            set { SetValue(() => SubmitDateString, value); }
        }
    }

}
