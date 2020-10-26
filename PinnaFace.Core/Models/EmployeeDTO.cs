using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    //Passport Info
    public partial class EmployeeDTO : CommonFieldsA
    {
        public EmployeeDTO()
        {
            EmployeeRelatives = new List<EmployeeRelativeDTO>();
            Complains = new List<ComplainDTO>();
        }

        //[ForeignKey("Agency")]
        //public int AgencyId { get; set; }

        //public AgencyDTO Agency
        //{
        //    get { return GetValue(() => Agency); }
        //    set { SetValue(() => Agency, value); }
        //}

        [ForeignKey("Agent")]
        public int? AgentId { get; set; }

        public AgentDTO Agent
        {
            get { return GetValue(() => Agent); }
            set { SetValue(() => Agent, value); }
        }

        [Required]
        [StringLength(9)]
        //[RegularExpression("^[A-Z0-9]{9,10}$", ErrorMessage = "Passport Number is invalid")]
        [RegularExpression("^([E]{1,1})+([A-Z]{1,1})+([0-9]{7,7})$", ErrorMessage = "Passport Number is invalid")]
        public string PassportNumber
        {
            get { return GetValue(() => PassportNumber); }
            set { SetValue(() => PassportNumber, value); }
        }

        [StringLength(150)]
        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                    return FirstName + " " + LastName;
                return FirstName + " " + MiddleName + " " + LastName;
            }
            set { SetValue(() => FullName, value); }
        }

        [Required]
        [StringLength(50)]
        [RegularExpression("^([A-Z])+ [A-Z]{2,15}$", ErrorMessage = "Given Name is invalid")]
        public string FirstName
        {
            get { return GetValue(() => FirstName); }
            set
            {
                SetValue(() => FirstName, value);
                SetValue(() => FullName, value);
            }
        }

        //[Required]
        [StringLength(50)]
        public string MiddleName
        {
            get { return GetValue(() => MiddleName); }
            set
            {
                SetValue(() => MiddleName, value);
                SetValue(() => FullName, value);
            }
        }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[A-Z]{2,15}$", ErrorMessage = "Last Name is invalid")]
        public string LastName
        {
            get { return GetValue(() => LastName); }
            set
            {
                SetValue(() => LastName, value);
                SetValue(() => FullName, value);
            }
        }

        [Required]
        public Sex Sex
        {
            get { return GetValue(() => Sex); }
            set { SetValue(() => Sex, value); }
        }

        [Required]
        [StringLength(50)]
        public string PlaceOfBirth
        {
            get { return GetValue(() => PlaceOfBirth); }
            set { SetValue(() => PlaceOfBirth, value.ToUpper()); }
        }

        [Required]
        public DateTime DateOfBirth
        {
            get { return GetValue(() => DateOfBirth); }
            set
            {
                SetValue(() => DateOfBirth, value);
                SetValue<string>(() => DateOfBirthString, value.ToString());
            }
        }

        [Required]
        public DateTime PassportIssueDate
        {
            get { return GetValue(() => PassportIssueDate); }
            set
            {
                SetValue(() => PassportIssueDate, value);
                SetValue<string>(() => PassportIssueDateString, value.ToString());
            }
        }

        [Required]
        public DateTime PassportExpiryDate
        {
            get { return GetValue(() => PassportExpiryDate); }
            set
            {
                SetValue(() => PassportExpiryDate, value);
                SetValue<string>(() => PassportExpiryDateString, value.ToString());
            }
        }

        [StringLength(50)]
        public string PlaceOfIssue
        {
            get { return GetValue(() => PlaceOfIssue); }
            set { SetValue(() => PlaceOfIssue, value); }
        }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }

        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }

        [ForeignKey("ContactPerson")]
        public int? ContactPersonId { get; set; }

        public EmployeeRelativeDTO ContactPerson
        {
            get { return GetValue(() => ContactPerson); }
            set { SetValue(() => ContactPerson, value); }
        }

        [ForeignKey("RequiredDocuments")]
        public int? RequiredDocumentsId { get; set; }

        public RequiredDocumentsDTO RequiredDocuments
        {
            get { return GetValue(() => RequiredDocuments); }
            set { SetValue(() => RequiredDocuments, value); }
        }
        
        [ForeignKey("Visa")]
        public int? VisaId { get; set; }

        public VisaDTO Visa
        {
            get { return GetValue(() => Visa); }
            set { SetValue(() => Visa, value); }
        }

        [StringLength(50)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Contract Number is invalid")]
        public string ContractNumber
        {
            get { return GetValue(() => ContractNumber); }
            set { SetValue(() => ContractNumber, value); }
        }

        [ForeignKey("Experience")]
        public int? ExperienceId { get; set; }

        public EmployeeExperienceDTO Experience
        {
            get { return GetValue(() => Experience); }
            set { SetValue(() => Experience, value); }
        }

        [ForeignKey("Education")]
        public int? EducationId { get; set; }

        public EmployeeEducationDTO Education
        {
            get { return GetValue(() => Education); }
            set { SetValue(() => Education, value); }
        }

        [ForeignKey("Hawala")]
        public int? HawalaId { get; set; }

        public EmployeeHawalaDTO Hawala
        {
            get { return GetValue(() => Hawala); }
            set { SetValue(() => Hawala, value); }
        }

        [ForeignKey("FlightProcess")]
        public int? FlightProcessId { get; set; }

        public FlightProcessDTO FlightProcess
        {
            get { return GetValue(() => FlightProcess); }
            set { SetValue(() => FlightProcess, value); }
        }

        [ForeignKey("EmbassyProcess")]
        public int? EmbassyProcessId { get; set; }

        public EmbassyProcessDTO EmbassyProcess
        {
            get { return GetValue(() => EmbassyProcess); }
            set { SetValue(() => EmbassyProcess, value); }
        }

        [ForeignKey("LabourProcess")]
        public int? LabourProcessId { get; set; }

        public LabourProcessDTO LabourProcess
        {
            get { return GetValue(() => LabourProcess); }
            set { SetValue(() => LabourProcess, value); }
        }

        [ForeignKey("InsuranceProcess")]
        public int? InsuranceProcessId { get; set; }

        public InsuranceProcessDTO InsuranceProcess
        {
            get { return GetValue(() => InsuranceProcess); }
            set { SetValue(() => InsuranceProcess, value); }
        }

        [ForeignKey("CurrentComplain")]
        public int? CurrentComplainId { get; set; }

        public ComplainDTO CurrentComplain
        {
            get { return GetValue(() => CurrentComplain); }
            set { SetValue(() => CurrentComplain, value); }
        }

        public ICollection<EmployeeRelativeDTO> EmployeeRelatives
        {
            get { return GetValue(() => EmployeeRelatives); }
            set { SetValue(() => EmployeeRelatives, value); }
        }

        public ICollection<ComplainDTO> Complains
        {
            get { return GetValue(() => Complains); }
            set { SetValue(() => Complains, value); }
        }

        //[NotMapped]
        //public EmployeeRelativeDTO ContactPerson
        //{
        //    get
        //    {
        //        return EmployeeRelatives.FirstOrDefault();
        //    }
        //    set { SetValue(() => ContactPerson, value); }
        //}

        [NotMapped]
        public string EmployeeDetail
        {
            get
            {
                string emp = EmployeeBasicDetail;
                if (Visa != null && Visa.Sponsor != null)
                    emp = emp + " | " + Visa.VisaNumber + " | " + Visa.Sponsor.FullName;
                emp = emp + "  " + MoreNotes;

                return emp;
            }
            set { SetValue(() => EmployeeDetail, value); }
        }

        [NotMapped]
        public string EmployeeDetail2
        {
            get
            {
                string emp = EmployeeBasicDetail;
                if (Visa != null && Visa.Sponsor != null)
                    emp = emp + Environment.NewLine + Visa.VisaNumber + " | " + Visa.Sponsor.FullName;
                emp = emp + "  " + MoreNotes;

                return emp;
            }
            set { SetValue(() => EmployeeDetail2, value); }
        }

        [NotMapped]
        public string EmployeeBasicDetail
        {
            get
            {
                string emp = "";
                if (Id != 0)
                    emp = FullName + " | " + PassportNumber + " | " + CodeNumber;
                return emp;
            }
            set { SetValue(() => EmployeeBasicDetail, value); }
        }
    }

    //More Info
    public partial class EmployeeDTO
    {
        //[Required]
        //
        //[NotMapped]
        //public string CodeNumber
        //{
        //    get { return GetValue(() => CodeNumber); }
        //    set { SetValue(() => CodeNumber, value.ToUpper()); }
        //}

        
        public int OrderNumber
        {
            get { return GetValue(() => OrderNumber); }
            set { SetValue(() => OrderNumber, value); }
        }

        [NotMapped]
        [DisplayName("Code No.")]
        [MaxLength(10, ErrorMessage = "Exceeded 10 letters")]
        public string CodeNumber
        {
            get
            {
                //string pref = Id.ToString(CultureInfo.InvariantCulture);
                //if (Id < 1000)
                //{
                //    int id = Id + 10000;
                //    pref = id.ToString(CultureInfo.InvariantCulture);
                //    pref = pref.Substring(1);
                //}
                string pref = OrderNumber.ToString(CultureInfo.InvariantCulture);
                if (OrderNumber < 1000)
                {
                    int id = OrderNumber + 100000;
                    pref = id.ToString(CultureInfo.InvariantCulture);
                    pref = pref.Substring(1);
                }
                return "E" + pref;
            }
            set { SetValue(() => CodeNumber, value); }
        }

        [NotMapped]
        [DisplayName("Track No.")]
        //[MaxLength(10, ErrorMessage = "Exceeded 10 letters")]
        public string TrackNumber
        {
            get
            {
                //var ep = Convert.ToInt32(PassportNumber.Substring(2));
                try
                {
                    int newId = Id*741705;

                    return FirstName.Substring(1, 1) + newId;
                }
                catch
                {
                    return "-";
                }
            }
            set { SetValue(() => TrackNumber, value); }
        }

        public ProcessStatusTypes CurrentStatus
        {
            get { return GetValue(() => CurrentStatus); }
            set { SetValue(() => CurrentStatus, value); }
        }

        [NotMapped]
        public string CurrentStatusString
        {
            get
            {
                string ss = EnumUtil.GetEnumDesc(CurrentStatus);
                return ss; //.Substring(0, ss.IndexOf('(')) + "";
            }
            set { SetValue(() => CurrentStatusString, value); }
        }

        [NotMapped]
        public string StatusString
        {
            get
            {
                string ss = EnumUtil.GetEnumDesc(CurrentStatus);
                //try
                //{
                //    return ss.Substring(0, ss.IndexOf('(')) + "";
                //}
                //catch
                //{
                    return ss;
                //}
            }
            set { SetValue(() => StatusString, value); }
        }

        [NotMapped]
        public int AgeInt
        {
            get
            {
                double age = DateTime.Now.Subtract(DateOfBirth).Days;
                age = age/365.25;
                return (int) age;
            }
            set { SetValue(() => AgeInt, value); }
        }

        [NotMapped]
        public string Age
        {
            get
            {
                double age = DateTime.Now.Subtract(DateOfBirth).Days;
                age = age/365.25;
                //if (age < 10)
                //return "-";
                string ss = age.ToString("N0");
                return ss;
            }
            set { SetValue(() => Age, value); }
        }

        public DateTime? CurrentStatusDate
        {
            get { return GetValue(() => CurrentStatusDate); }
            set { SetValue(() => CurrentStatusDate, value); }
        }

        public MaritalStatusTypes MaritalStatus
        {
            get { return GetValue(() => MaritalStatus); }
            set { SetValue(() => MaritalStatus, value); }
        }

        public ReligionTypes Religion
        {
            get { return GetValue(() => Religion); }
            set { SetValue(() => Religion, value); }
        }

        public CountryList AppliedCountry
        {
            get { return GetValue(() => AppliedCountry); }
            set { SetValue(() => AppliedCountry, value); }
        }

        public ProffesionTypes AppliedProfession
        {
            get { return GetValue(() => AppliedProfession); }
            set { SetValue(() => AppliedProfession, value); }
        }
        [NotMapped]
        public string AppliedProfessionDisplay
        {
            get { return EnumUtil.GetEnumDesc(AppliedProfession); }
            set { SetValue(() => AppliedProfessionDisplay, value); }
        }
        public DateTime? DocumentReceivedDate
        {
            get { return GetValue(() => DocumentReceivedDate); }
            set
            {
                SetValue(() => DocumentReceivedDate, value);
                SetValue(() => ProcessTookDays, value.ToString());
            }
        }
        [NotMapped]
        public string ProcessTookDays
        {
            get
            {
                if (DocumentReceivedDate == null)
                    return "Unknown";
                var caption = "";// on process
                var beginingDate = DocumentReceivedDate.Value;
                if (FlightProcess != null && FlightProcess.Departured)
                {
                    beginingDate = FlightProcess.SubmitDate;
                    caption = "on work";
                }

                var days = DateTime.Now.Subtract(beginingDate).Days;
                //var caption = days == 1 ? " day": " days";// on process
                //if (FlightProcess != null && FlightProcess.Departured)
                //{
                //    days = DateTime.Now.Subtract(FlightProcess.SubmitDate).Days;
                //    caption = days == 1 ? " day on work" : " days on work";
                //}

                var yy = (days/30)/12; //DateTime.Now.Year - BeginingDate.Value.Year;
                var mm = DateTime.Now.Month - beginingDate.Month;
                if (mm < 0)
                    mm = (12 - beginingDate.Month) + DateTime.Now.Month;
                var dd = DateTime.Now.Day - beginingDate.Day;
                if (dd < 0)
                {
                    var daysInMonth=DateTime.DaysInMonth(beginingDate.Year, beginingDate.Month);
                    dd = (daysInMonth - beginingDate.Day) + DateTime.Now.Day;
                }
                string yys = "", mms = "", dds = "";
                if (yy > 0)
                {
                    if (yy == 1)
                        yys = yy + " year ";
                    else yys = yy + " years ";
                }
                if (mm > 0)
                {
                    if (mm == 1)
                        mms = mm + " month ";
                    else mms = mm + " months ";
                }
                if (dd > 0)
                {
                    if (dd == 1)
                        dds = dd + " day ";
                    else dds = dd + " days ";
                }
                return yys + mms  + dds+caption;
                
                //string duration;
                //if (days > 30)
                //{
                //    var months = days/30;
                //    var monthcaption = months == 1 ? " month " : " months ";
                //    var additionalDays = days%30;

                //    duration = months.ToString()+ monthcaption + additionalDays.ToString();

                //    if (months > 11)
                //    {
                //        var year = months/12;
                //        var month = months%12;
                //        monthcaption = month == 1 ? " month " : " months ";
                //        var yearcaption = year == 1 ? " year " : " years ";
                //        duration =year.ToString() +yearcaption+ month.ToString() + monthcaption + additionalDays.ToString();
                //    }

                //}
                //else
                //{
                //    duration = days.ToString();
                //}

                //return duration + caption;
            }
            set { SetValue(() => ProcessTookDays, value); }
        }

        [StringLength(250)]
        public string MoreNotes
        {
            get { return GetValue(() => MoreNotes); }
            set { SetValue(() => MoreNotes, value); }
        }
    }


    public partial class EmployeeDTO
    {
        public ContratPeriods ContratPeriod
        {
            get { return GetValue(() => ContratPeriod); }
            set { SetValue(() => ContratPeriod, value); }
        }
        [NotMapped]
        public string ContractPeriodDisplay
        {
            get { return EnumUtil.GetEnumDesc(ContratPeriod); }
            set { SetValue(() => ContractPeriodDisplay, value); }
        }
        public float? Salary
        {
            get { return GetValue(() => Salary); }
            set { SetValue(() => Salary, value); }
        }

        public CurrencyTypes CurrencyType
        {
            get { return GetValue(() => CurrencyType); }
            set { SetValue(() => CurrencyType, value); }
        }

        public Numbers NumberOfChildren
        {
            get { return GetValue(() => NumberOfChildren); }
            set { SetValue(() => NumberOfChildren, value); }
        }


        public float? Weight
        {
            get { return GetValue(() => Weight); }
            set { SetValue(() => Weight, value); }
        }


        public float? Height
        {
            get { return GetValue(() => Height); }
            set { SetValue(() => Height, value); }
        }

        public Complexion Complexion
        {
            get { return GetValue(() => Complexion); }
            set { SetValue(() => Complexion, value); }
        }

        //[MaxLength]
        //public byte[] Photo
        //{
        //    get { return GetValue(() => Photo); }
        //    set { SetValue(() => Photo, value); }
        //}
        //[MaxLength]
        //public byte[] StandPhoto
        //{
        //    get { return GetValue(() => StandPhoto); }
        //    set { SetValue(() => StandPhoto, value); }
        //}

        [ForeignKey("Photo")]
        public int? PhotoId { get; set; }

        public AttachmentDTO Photo
        {
            get { return GetValue(() => Photo); }
            set { SetValue(() => Photo, value); }
        }

        [ForeignKey("StandPhoto")]
        public int? StandPhotoId { get; set; }

        public AttachmentDTO StandPhoto
        {
            get { return GetValue(() => StandPhoto); }
            set { SetValue(() => StandPhoto, value); }
        }
    }

    //Other Languages
    public partial class EmployeeDTO
    {
        [StringLength(150)]//+ " " + MiddleNameAmharic 
        public string FullNameAmharic
        {
            get { return FirstNameAmharic + " " + LastNameAmharic; }
            set { SetValue(() => FullNameAmharic, value); }
        }

        [StringLength(50)]
        //[RegularExpression("^([A-Z])+ [A-Z]{2,15}$", ErrorMessage = "First Name Amharic is invalid")]
        public string FirstNameAmharic
        {
            get { return GetValue(() => FirstNameAmharic); }
            set { SetValue(() => FirstNameAmharic, value); }
        }

        [StringLength(50)]
        public string MiddleNameAmharic
        {
            get { return GetValue(() => MiddleNameAmharic); }
            set { SetValue(() => MiddleNameAmharic, value); }
        }

        [StringLength(50)]
        public string LastNameAmharic
        {
            get { return GetValue(() => LastNameAmharic); }
            set { SetValue(() => LastNameAmharic, value); }
        }

        [StringLength(150)]
        public string FullNameArabic
        {
            get { return GetValue(() => FullNameArabic); }
            set { SetValue(() => FullNameArabic, value); }
        }

        [StringLength(50)]
        public string FirstNameArabic
        {
            get { return GetValue(() => FirstNameArabic); }
            set { SetValue(() => FirstNameArabic, value); }
        }

        [StringLength(50)]
        public string MiddleNameArabic
        {
            get { return GetValue(() => MiddleNameArabic); }
            set { SetValue(() => MiddleNameArabic, value); }
        }

        [StringLength(50)]
        public string LastNameArabic
        {
            get { return GetValue(() => LastNameArabic); }
            set { SetValue(() => LastNameArabic, value); }
        }

        [NotMapped]
        public string DateOfBirthStringAndAmharic
        {
            get
            {
                return DateOfBirth.ToString("dd/MM/yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(DateOfBirth, "/") +
                       ")";
            }
            set { SetValue(() => DateOfBirthStringAndAmharic, value); }
        }

        [NotMapped]
        public string DateOfBirthString
        {
            get { return DateOfBirth.ToString("dd MMM yy").ToUpper(); }
            set { SetValue(() => DateOfBirthString, value); }
        }

        [NotMapped]
        public string DateOfBirthStringAmharic
        {
            get { return CalendarUtil.GetEthCalendar(DateOfBirth, true); //"-"
            }
            set { SetValue(() => DateOfBirthStringAmharic, value); }
        }

        [NotMapped]
        public string DateOfBirthStringAmharicFormatted
        {
            get { return CalendarUtil.GetEthCalendarFormated(DateOfBirth, "/"); }
            set { SetValue(() => DateOfBirthStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string PassportIssueDateStringAndAmharic
        {
            get
            {
                return PassportIssueDate.ToString("dd/MM/yyyy") + " (" +
                       CalendarUtil.GetEthCalendarFormated(PassportIssueDate, "/") + ")";
            }
            set { SetValue(() => PassportIssueDateStringAndAmharic, value); }
        }

        [NotMapped]
        public string PassportIssueDateString
        {
            get { return PassportIssueDate.ToString("dd MMM yy").ToUpper(); }
            set { SetValue(() => PassportIssueDateString, value); }
        }
        [NotMapped]
        public string DocumentReceivedDateString
        {
            get { 
                if (DocumentReceivedDate != null) 
                return DocumentReceivedDate.Value.ToString("dd MMM yy").ToUpper();
                return DateTime.Now.ToString("dd MMM yy").ToUpper();
            }
            set { SetValue(() => PassportIssueDateString, value); }
        }
        [NotMapped]
        public string PassportIssueDateStringAmharic
        {
            get { return CalendarUtil.GetEthCalendar(PassportIssueDate, true); //"-"
            }
            set { SetValue(() => PassportIssueDateStringAmharic, value); }
        }

        [NotMapped]
        public string PassportIssueDateStringAmharicFormatted
        {
            get { return CalendarUtil.GetEthCalendarFormated(PassportIssueDate, "/"); }
            set { SetValue(() => PassportIssueDateStringAmharicFormatted, value); }
        }

        [NotMapped]
        public string PassportExpiryDateStringAndAmharic
        {
            get
            {
                return PassportExpiryDate.ToString("dd/MM/yyyy") + " (" +
                       CalendarUtil.GetEthCalendarFormated(PassportExpiryDate, "/") + ")";
            }
            set { SetValue(() => PassportExpiryDateStringAndAmharic, value); }
        }

        [NotMapped]
        public string PassportExpiryDateString
        {
            get { return PassportExpiryDate.ToString("dd MMM yy").ToUpper(); }
            set { SetValue(() => PassportExpiryDateString, value); }
        }

        [NotMapped]
        public string PassportExpiryDateStringAmharic
        {
            get { return CalendarUtil.GetEthCalendar(PassportExpiryDate, true); //"-"
            }
            set { SetValue(() => PassportExpiryDateStringAmharic, value); }
        }

        [NotMapped]
        public string PassportExpiryDateStringAmharicFormatted
        {
            get { return CalendarUtil.GetEthCalendarFormated(PassportExpiryDate, "/"); }
            set { SetValue(() => PassportExpiryDateStringAmharicFormatted, value); }
        }
    }

    //Process Date
    public partial class EmployeeDTO
    {
        [NotMapped]
        public string FirstNameShort
        {
            get
            {
                if (FirstName != null && FirstName.Length > 15)
                    return FirstName.Substring(0, 14) + ".";
                return FirstName;
            }
            set { SetValue(() => FirstNameShort, value); }
        }

        [NotMapped]
        public string VisaDetail
        {
            get
            {
                string emp = "";
                if (Visa != null)
                    emp = Visa.VisaNumber; // + " | " + Visa.Sponsor.FullName;
                return emp;
            }
            set { SetValue(() => EmployeeDetail, value); }
        }

        [NotMapped]
        public string SponsorLocation
        {
            get
            {
                string emp = "";
                if (Visa != null && Visa.Sponsor != null && Visa.Sponsor.Address != null)
                    emp = Visa.Sponsor.Address.City;
                // +" - "+ EnumUtil.GetEnumDesc(Visa.Sponsor.Address.Country);// + " | " + Visa.Sponsor.FullName;
                return emp;
            }
            set { SetValue(() => SponsorLocation, value); }
        }

        [NotMapped]
        public string AgentName
        {
            get
            {
                if (Agent == null)
                    return "-";
                if (Agent != null && Agent.AgentName != null)
                {
                    if (Agent.AgentName.Length <= 8)
                        return Agent.AgentName;
                    return Agent.AgentName.Substring(0, 8) + "...";
                }
                return "Not Assigned";
            }
            set { SetValue(() => AgentName, value); }
        }
        [NotMapped]
        [DisplayName("Agent Name")]
        public string AgentNameFull
        {
            get
            {
                if (Agent == null)
                    return "Not Assigned";
                return Agent.AgentName;
            }
            set { SetValue(() => AgentNameFull, value); }
        }
        [NotMapped]
        public string AgencyName
        {
            get
            {
                if (Agency == null)
                    return "";
                if (Agency != null && Agency.AgencyName != null)
                {
                    if (Agency.AgencyName.Length <= 8)
                        return Agency.AgencyName;
                    return Agency.AgencyName.Substring(0, 8) + "..."; 
                }
                return ""; //.Substring(0, 8) + "...";
            }
            set { SetValue(() => AgencyName, value); }
        }
        [NotMapped]
        public string AgencyNameFull
        {
            get
            {
                if (Agency == null)
                    return "";
                if (Agency.AgencyName != null)
                    return Agency.AgencyName;
                return "";
            }
            set { SetValue(() => AgencyNameFull, value); }
        }
        [NotMapped]
        public string SponsorFullNameShort
        {
            get
            {
                if (Visa == null)
                    return "";
                if (Visa.Sponsor == null)
                    return "-";
                //if (Visa.Sponsor.FullName.Length <= 15)
                //    return Visa.Sponsor.FullName;
                return Visa.Sponsor.FullName; //.Substring(0, 15) + "...";
            }
            set { SetValue(() => SponsorFullNameShort, value); }
        }

        [NotMapped]
        public string ReligionString
        {
            get { return Convert.ToInt32(Religion).ToString(); }
        }

        [NotMapped]
        public string ProfessionString
        {
            get { return Convert.ToInt32(AppliedProfession).ToString(); }
        }

        [NotMapped]
        public string ContratPeriodString
        {
            get { return Convert.ToInt32(ContratPeriod).ToString(); }
        }

        [NotMapped]
        public string ComplexionString
        {
            get { return Convert.ToInt32(Complexion).ToString(); }
        }

        [NotMapped]
        public string MaritalStatusString
        {
            get { return Convert.ToInt32(MaritalStatus).ToString(); }
        }

        [NotMapped]
        public string SexString
        {
            get { return Convert.ToInt32(Sex).ToString(); }
        }

        [NotMapped]
        public string SalaryString
        {
            get
            {
                if (Salary != null) 
                    return Salary.Value.ToString("N0") + " " + EnumUtil.GetEnumDesc(CurrencyType);
                else return "1,000 S/R";
            }
        }

        [NotMapped]
        public string NumberOfChildrenString
        {
            get { return EnumUtil.GetEnumDesc(NumberOfChildren); }
        }

        [NotMapped]
        public string ArabicLanguage
        {
            get
            {
                return Education != null && Education.ArabicLanguage != LanguageExperience.Poor
                    ? EnumUtil.GetEnumDesc(Education.ArabicLanguage)
                    : "No";
            }
        }

        [NotMapped]
        public string EnglishLanguage
        {
            get
            {
                return Education != null && Education.EnglishLanguage != LanguageExperience.Poor
                    ? EnumUtil.GetEnumDesc(Education.EnglishLanguage)
                    : "No";
            }
        }

        [NotMapped]
        public string WorkExperience
        {
            get
            {
                string emp = "First Timer";
                if (Experience != null && Experience.HaveWorkExperience)
                    emp = EnumUtil.GetEnumDesc(Experience.ExperiencePeriod) + " in "
                          + EnumUtil.GetEnumDesc(Experience.ExperienceCountry); // + " | " + Visa.Sponsor.FullName;
                return emp;
            }
            set { SetValue(() => WorkExperience, value); }
        }

        //    public DateTime? LabourSubmitDate
        //    {
        //        get { return GetValue(() => LabourSubmitDate); }
        //        set { SetValue(() => LabourSubmitDate, value); }
        //    }

        //    public DateTime? EmbassySubmitDate
        //    {
        //        get { return GetValue(() => EmbassySubmitDate); }
        //        set { SetValue(() => EmbassySubmitDate, value); }
        //    }

        //    public DateTime? FlightDate
        //    {
        //        get { return GetValue(() => FlightDate); }
        //        set { SetValue(() => FlightDate, value); }
        //    }

        //    public DateTime? ReturnedDate
        //    {
        //        get { return GetValue(() => ReturnedDate); }
        //        set { SetValue(() => ReturnedDate, value); }
        //    }

        //    public DateTime? LostDate
        //    {
        //        get { return GetValue(() => LostDate); }
        //        set { SetValue(() => LostDate, value); }
        //    }
    }


    public partial class EmployeeDTO
    {
        public bool Discontinued
        {
            get { return GetValue(() => Discontinued); }
            set { SetValue(() => Discontinued, value); }
        }

        public DateTime? DiscontinuedDate
        {
            get { return GetValue(() => DiscontinuedDate); }
            set
            {
                SetValue(() => DiscontinuedDate, value);
                if (value != null) SetValue(() => DiscontinuedDateString, value.Value.ToLongDateString());
            }
        }

        public string DiscontinuedRemark
        {
            get { return GetValue(() => DiscontinuedRemark); }
            set { SetValue(() => DiscontinuedRemark, value); }
        }

        [NotMapped]
        public string DiscontinuedDateString
        {
            get
            {
                if (DiscontinuedDate != null)
                    return DiscontinuedDate.Value.ToString("dd-MMM-yyyy") +
                           " (" + CalendarUtil.GetEthCalendarFormated(DiscontinuedDate.Value, "-") + ")";
                return "";
            }
            set { SetValue(() => DiscontinuedDateString, value); }
        }


        public AfterFlightStatusTypes AfterFlightStatus
        {
            get { return GetValue(() => AfterFlightStatus); }
            set { SetValue(() => AfterFlightStatus, value); }
        }

        public DateTime? AfterFlightStatusDate
        {
            get { return GetValue(() => AfterFlightStatusDate); }
            set
            {
                SetValue(() => AfterFlightStatusDate, value);
                if (value != null) SetValue(() => AfterFlightStatusDateString, value.Value.ToLongDateString());
            }
        }

        public string AfterFlightRemark
        {
            get { return GetValue(() => AfterFlightRemark); }
            set { SetValue(() => AfterFlightRemark, value); }
        }

        [NotMapped]
        public string AfterFlightStatusDateString
        {
            get
            {
                if (AfterFlightStatusDate != null)
                    return AfterFlightStatusDate.Value.ToString("dd-MMM-yyyy") +
                           " (" + CalendarUtil.GetEthCalendarFormated(AfterFlightStatusDate.Value, "-") + ")";
                return "";
            }
            set { SetValue(() => AfterFlightStatusDateString, value); }
        }
    }
}