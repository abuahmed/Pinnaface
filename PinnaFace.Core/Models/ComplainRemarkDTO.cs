using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class ComplainRemarkDTO : CommonFieldsA
    {
        public DateTime RemarkDate
        {
            get { return GetValue(() => RemarkDate); }
            set { SetValue(() => RemarkDate, value); }
        }

        [Required]
        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }

        [ForeignKey("Complain")]
        public int ComplainId { get; set; }
        public ComplainDTO Complain
        {
            get { return GetValue(() => Complain); }
            set { SetValue(() => Complain, value); }
        }

        [NotMapped]
        public string ComplainRemarkDateString
        {
            get
            {
                try
                {
                    return RemarkDate.ToString("dd-MMM-yyyy");
                }
                catch
                {
                    return DateTime.Now.ToString("dd-MMM-yyyy");
                }
                //return DateTime.Now.ToString("dd-MMM-yyyy");
            }
            set { SetValue(() => ComplainRemarkDateString, value); }
        }
    }
}
