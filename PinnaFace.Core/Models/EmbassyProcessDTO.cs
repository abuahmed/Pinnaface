using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class EmbassyProcessDTO : CommonFieldsB
    {
        [Required]
        //[RegularExpression("^[a-zA-Z0-9]{8,10}$", ErrorMessage = "Enjaz Number is invalid")]
        [RegularExpression("^([E]{1,1})+([0-9]{9,9})$", ErrorMessage = "Enjaz Number is invalid")]
        public string EnjazNumber
        {
            get { return GetValue(() => EnjazNumber); }
            set { SetValue(() => EnjazNumber, value); }
        }
        public DateTime? StampedDate
        {
            get { return GetValue(() => StampedDate); }
            set { SetValue(() => StampedDate, value); }
        }
        
        public bool Stammped
        {
            get { return GetValue(() => Stammped); }
            set { SetValue(() => Stammped, value); }
        }
        public DateTime? CanceledDate
        {
            get { return GetValue(() => CanceledDate); }
            set { SetValue(() => CanceledDate, value); }
        }

        public bool Canceled
        {
            get { return GetValue(() => Canceled); }
            set { SetValue(() => Canceled, value); }
        }
        [NotMapped]
        public string StammpedString
        {
            get {
                return Stammped ? "Stammped" : "";
            }
            set { SetValue(() => StammpedString, value); }
        }
    }
}
