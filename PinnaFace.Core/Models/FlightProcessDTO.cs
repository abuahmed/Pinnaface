using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class FlightProcessDTO : CommonFieldsB
    {
        [StringLength(50)]
        //[RegularExpression("^[a-zA-Z0-9]{1,25}$", ErrorMessage = "Ticket Number is invalid")]
        public string TicketNumber
        {
            get { return GetValue(() => TicketNumber); }
            set { SetValue(() => TicketNumber, value); }
        }
        
        public decimal TicketAmount
        {
            get { return GetValue(() => TicketAmount); }
            set { SetValue(() => TicketAmount, value); }
        }

        [StringLength(50)]
        public string FlightNumber
        {
            get { return GetValue(() => FlightNumber); }
            set { SetValue(() => FlightNumber, value); }
        }

        [Required]
        [StringLength(50)]//musanedcode: riyadh 4356 addis ababa 5020
        public string TicketCity
        {
            get { return GetValue(() => TicketCity); }
            set { SetValue(() => TicketCity, value); }
        }
        
        public bool Departured
        {
            get { return GetValue(() => Departured); }
            set { SetValue(() => Departured, value); }
        }
        [NotMapped]
        public string DeparturedString
        {
            get
            {
                if (Departured)
                    return "Departured(" + SubmitDateString + ")";
                return "";
            }
            set { SetValue(() => DeparturedString, value); }
        }

    }
}
