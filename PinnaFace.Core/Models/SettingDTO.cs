using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core.Models
{
    public class SettingDTO : CommonFieldsA
    {
        //[ForeignKey("Agency")]
        //public int? AgencyId { get; set; }
        //public AgencyDTO Agency
        //{
        //    get { return GetValue(() => Agency); }
        //    set { SetValue(() => Agency, value); }
        //}

        [StringLength(50)]
        public string AwajNumber
        {
            get { return GetValue(() => AwajNumber); }
            set { SetValue(() => AwajNumber, value); }
        }

        [StringLength(50)]        
        public string ReferencePreffix
        {
            get { return GetValue(() => ReferencePreffix); }
            set { SetValue(() => ReferencePreffix, value); }
        }

        [StringLength(50)]
        public string ReferenceSuffix
        {
            get { return GetValue(() => ReferenceSuffix); }
            set { SetValue(() => ReferenceSuffix, value); }
        }

        public bool ShowLetterReferenceNumber
        {
            get { return GetValue(() => ShowLetterReferenceNumber); }
            set { SetValue(() => ShowLetterReferenceNumber, value); }
        }
        public int CurrentLetterReferenceNumber
        {
            get { return GetValue(() => CurrentLetterReferenceNumber); }
            set { SetValue(() => CurrentLetterReferenceNumber, value); }
        }
        public TestimonialNumbers NumberOfTestimonials
        {
            get { return GetValue(() => NumberOfTestimonials); }
            set { SetValue(() => NumberOfTestimonials, value); }
        }
        public TestimonialFormats TestimonialFormat
        {
            get { return GetValue(() => TestimonialFormat); }
            set { SetValue(() => TestimonialFormat, value); }
        }
        [NotMapped]
        public string CurrentLetterReferenceNumberString
        {
            get
            {
                var currentNumString = CurrentLetterReferenceNumber.ToString();

                if (CurrentLetterReferenceNumber < 10)
                    currentNumString= "00" + CurrentLetterReferenceNumber;
                else if (CurrentLetterReferenceNumber < 100)
                    currentNumString= "0" + CurrentLetterReferenceNumber;

                return string.Format("{0}{1}{2}", ReferencePreffix, currentNumString, ReferenceSuffix);
            }
            set { SetValue(() => CurrentLetterReferenceNumberString, value); }
        }
        public bool ShowWekala  
        {
            get { return GetValue(() => ShowWekala); }
            set { SetValue(() => ShowWekala, value); }
        }

        public LabourListTypes LabourListType
        {
            get { return GetValue(() => LabourListType); }
            set { SetValue(() => LabourListType, value); }
        }
        public CoverLetterTypes CoverLetterType
        {
            get { return GetValue(() => CoverLetterType); }
            set { SetValue(() => CoverLetterType, value); }
        }
        public CvHeaderFormats CvHeaderFormat
        {
            get { return GetValue(() => CvHeaderFormat); }
            set { SetValue(() => CvHeaderFormat, value); }
        }

        public DocumentOrientationTypes WekalaDocumentOrientation
        {
            get { return GetValue(() => WekalaDocumentOrientation); }
            set { SetValue(() => WekalaDocumentOrientation, value); }
        }

        public EmbassyApplicationTypes EmbassyApplicationType
        {
            get { return GetValue(() => EmbassyApplicationType); }
            set { SetValue(() => EmbassyApplicationType, value); }
        }

        public EmbassyApplicationFormats EmbassyApplicationFormat
        {
            get { return GetValue(() => EmbassyApplicationFormat); }
            set { SetValue(() => EmbassyApplicationFormat, value); }
        }

        #region Sync Status
        public DateTime? LastToServerSyncDate
        {
            get
            {
                return GetValue(() => LastToServerSyncDate);
            }
            set { SetValue(() => LastToServerSyncDate, value); }
        }
        public DateTime? LastFromServerSyncDate
        {
            get { return GetValue(() => LastFromServerSyncDate); }
            set { SetValue(() => LastFromServerSyncDate, value); }
        }
       
        [NotMapped]
        public string LastToServerSyncDateString
        {
            get
            {
                if (LastToServerSyncDate != null)
                    return LastToServerSyncDate.Value.ToString("f") + 
                           " (" + CalendarUtil.GetEthCalendarFormated(LastToServerSyncDate.Value, "-") + ")";
                return null;
            }
            set { SetValue(() => LastToServerSyncDateString, value); }
        }
        [NotMapped]
        public string LastFromServerSyncDateString
        {
            get
            {
                if (LastFromServerSyncDate != null)
                    return LastFromServerSyncDate.Value.ToString("f") +
                           " (" + CalendarUtil.GetEthCalendarFormated(LastFromServerSyncDate.Value, "-") + ")";
                return null;
            }
            set { SetValue(() => LastFromServerSyncDateString, value); }
        }

        public bool StartSync
        {
            get { return GetValue(() => StartSync); }
            set { SetValue(() => StartSync, value); }
        }

        [Range(1,1440)]//"Sync Duration should be between 1-1440"
        public int SyncDuration
        {
            get { return GetValue(() => SyncDuration); }
            set { SetValue(() => SyncDuration, value); }
        }

        #endregion

        [NotMapped]
        public string SettingDetail
        {
            get
            {
                if(Agency!=null)
                return Agency.AgencyName + " (" + Agency.Id.ToString() + ")";
                return "";
            }
            set { SetValue(() => SettingDetail, value); }
        }
    }
}
