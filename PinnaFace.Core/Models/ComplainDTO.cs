using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class ComplainDTO : CommonFieldsA
    {
        public ComplainDTO()
        {
            Remarks = new List<ComplainRemarkDTO>();
        }

        [NotMapped]
        public string ComplainNumber
        {
            get
            {
                var pref = Id.ToString(CultureInfo.InvariantCulture);
                if (Id < 1000)
                {
                    var id = Id + 10000;
                    pref = id.ToString(CultureInfo.InvariantCulture);
                    pref = pref.Substring(1);
                }
                return pref;
            }
            set { SetValue(() => ComplainNumber, value); }
        }
        [Required]
        public string Complain
        {
            get { return GetValue(() => Complain); }
            set { SetValue(() => Complain, value); }
        }
        [Required]
        public DateTime ComplainDate
        {
            get { return GetValue(() => ComplainDate); }
            set
            {
                SetValue(() => ComplainDate, value);
                SetValue(() => ComplainDateString, value.ToString());
            }
        }
        [NotMapped]
        public string DaysPassed
        {
            get
            {
                var pref = DateTime.Now.Subtract(ComplainDate).Days;

                return pref.ToString(CultureInfo.InvariantCulture) + " day(s)";
            }
            set { SetValue(() => DaysPassed, value); }
        }
        [Required]
        public ComplainTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }
        [NotMapped]
        public string ComplainTypeString
        {
            get
            {
                return Convert.ToInt32(Type).ToString();
            }
        }
        [NotMapped]
        public string ComplainTypeDescription
        {
            get
            {
                return EnumUtil.GetEnumDesc(Type);
            }
        }
        [Required]
        public ComplainProrityTypes Priority
        {
            get { return GetValue(() => Priority); }
            set { SetValue(() => Priority, value); }
        }
        [NotMapped]
        public string ComplainPriorityString
        {
            get
            {
                return Convert.ToInt32(Priority).ToString();
            }
        }
        [MaxLength(50, ErrorMessage = "Exceeded 50 letters")]
        public string RaisedByName
        {
            get { return GetValue(() => RaisedByName); }
            set { SetValue(() => RaisedByName, value); }
        }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Phone Number is invalid")]
        public string RaisedByTelephone
        {
            get { return GetValue(() => RaisedByTelephone); }
            set { SetValue(() => RaisedByTelephone, value); }
        }
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string RaisedByRelationship
        {
            get { return GetValue(() => RaisedByRelationship); }
            set { SetValue(() => RaisedByRelationship, value); }
        }

        public string FinalSolution
        {
            get { return GetValue(() => FinalSolution); }
            set { SetValue(() => FinalSolution, value); }
        }
        public DateTime? FinalSolutionDate
        {
            get { return GetValue(() => FinalSolutionDate); }
            set
            {
                SetValue(() => FinalSolutionDate, value);
                if (value != null) SetValue(() => FinalSolutionDateString, value.Value.ToLongDateString());
            }
        }
        public string Confirmation
        {
            get { return GetValue(() => Confirmation); }
            set { SetValue(() => Confirmation, value); }
        }
        public DateTime? ConfirmationDate
        {
            get { return GetValue(() => ConfirmationDate); }
            set
            {
                SetValue(() => ConfirmationDate, value);
                if (value != null) SetValue(() => ConfirmationDateString, value.Value.ToLongDateString());
            }
        }

        public string ReOpening
        {
            get { return GetValue(() => ReOpening); }
            set { SetValue(() => ReOpening, value); }
        }
        public DateTime? ReOpeningDate
        {
            get { return GetValue(() => ReOpeningDate); }
            set
            {
                SetValue(() => ReOpeningDate, value);
                if (value != null) SetValue(() => ReOpeningDateString, value.Value.ToLongDateString());
            }
        }
        [Required]
        public ComplainStatusTypes Status
        {
            get { return GetValue(() => Status); }
            set { SetValue(() => Status, value); }
        }
        [NotMapped]
        public string ComplainStatusString
        {
            get
            {
                return Convert.ToInt32(Status).ToString();
            }
        }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public EmployeeDTO Employee
        {
            get { return GetValue(() => Employee); }
            set { SetValue(() => Employee, value); }
        }

        [NotMapped]
        public string TypeString
        {
            get
            {
                var ts = EnumUtil.GetEnumDesc(Type);
                return ts.Length > 17 ? ts.Substring(0, 18) + "..." : ts;
            }
            set { SetValue(() => TypeString, value); }
        }

        [NotMapped]
        public string ComplainDateString
        {
            get
            {
                return ComplainDate.ToString("dd-MMM-yyyy");
            }
            set { SetValue(() => ComplainDateString, value); }
        }
        [NotMapped]
        public string ComplainDateAmharicString
        {
            get
            {
                return CalendarUtil.GetEthCalendarFormated(ComplainDate,"/");//.ToString("dd-MMM-yyyy");
            }
            set { SetValue(() => ComplainDateAmharicString, value); }
        }
        [NotMapped]
        public string FinalSolutionDateString
        {
            get { 
                if (FinalSolutionDate != null) return FinalSolutionDate.Value.ToString("dd-MMM-yyyy");
                return "";
            }
            set { SetValue(() => FinalSolutionDateString, value); }
        }
        [NotMapped]
        public string ConfirmationDateString
        {
            get
            {
                if (ConfirmationDate != null) return ConfirmationDate.Value.ToString("dd-MMM-yyyy");
                return "";
            }
            set { SetValue(() => ConfirmationDateString, value); }
        }
        [NotMapped]
        public string ReOpeningDateString
        {
            get
            {
                if (ReOpeningDate != null) return ReOpeningDate.Value.ToString("dd-MMM-yyyy");
                return "";
            }
            set { SetValue(() => ReOpeningDateString, value); }
        }
        [NotMapped]
        public string ComplainDesription
        {
            get
            {
                return ComplainDateString +" - "+TypeString +Environment.NewLine +Complain;
            }
            set { SetValue(() => ComplainDesription, value); }
        }
        public ICollection<ComplainRemarkDTO> Remarks
        {
            get { return GetValue(() => Remarks); }
            set { SetValue(() => Remarks, value); }
        }

        //Concurreny Control
        [Timestamp]
        public byte[] RowVersion
        {
            get { return GetValue(() => RowVersion); }
            set { SetValue(() => RowVersion, value); }
        }
    }
}
