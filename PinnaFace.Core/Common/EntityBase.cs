using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using PinnaFace.Core.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace PinnaFace.Core
{
    public abstract class EntityBase: PropertyChangedNotification, IObjectState
    {
        protected EntityBase()
        {
            RowGuid = Guid.NewGuid();
            Enabled = true;
            CreatedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateRecordCreated = DateTime.Now;
            ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateLastModified = DateTime.Now;
        }

        [NotMapped]
        public ObjectState ObjectState { get; set; }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 0)]
        public int Id { get; set; }

        public Guid RowGuid { get; set; }

        public bool Enabled { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateTime? DateRecordCreated { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? DateLastModified { get; set; }

        [NotMapped]
        public string DateRecordCreatedString
        {
            get
            {
                if (DateRecordCreated != null)
                    return DateRecordCreated.Value.ToString("dd-MMM-yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(DateRecordCreated.Value, "-") + ")";
                return "";
            }
            set { SetValue(() => DateRecordCreatedString, value); }
        }
        
        [NotMapped]
        public string DateLastModifiedString
        {
            get
            {
                if (DateLastModified != null)
                    return DateLastModified.Value.ToString("dd-MMM-yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(DateLastModified.Value, "-") + ")";
                return null;
            }
            set { SetValue(() => DateLastModifiedString, value); }
        } 
    }

    public abstract class UserEntityBase : PropertyChangedNotification, IObjectState
    {
        protected UserEntityBase()
        {
            RowGuid = Guid.NewGuid();
            Enabled = true;
            Synced = false;
            CreatedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateRecordCreated = DateTime.Now;
            ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
            DateLastModified = DateTime.Now;
        }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        [NotMapped]
        [DisplayName("No.")]
        public int SerialNumber { get; set; }

        public bool? Synced { get; set; }   

        public Guid? RowGuid { get; set; }

        public bool? Enabled { get; set; }

        public int? CreatedByUserId { get; set; }

        public DateTime? DateRecordCreated { get; set; }

        public int? ModifiedByUserId { get; set; }

        public DateTime? DateLastModified { get; set; }

        [NotMapped]
        public string DateRecordCreatedString
        {
            get
            {
                if (DateRecordCreated != null)
                    return DateRecordCreated.Value.ToString("dd-MMM-yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(DateRecordCreated.Value, "-") + ")";
                return "";
            }
            set { SetValue(() => DateRecordCreatedString, value); }
        }
        
        [NotMapped]
        public string DateLastModifiedString
        {
            get
            {
                if (DateLastModified != null)
                    return DateLastModified.Value.ToString("dd-MMM-yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(DateLastModified.Value, "-") + ")";
                return null;
            }
            set { SetValue(() => DateLastModifiedString, value); }
        } 
    }
}
