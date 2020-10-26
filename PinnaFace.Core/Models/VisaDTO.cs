using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class VisaDTO : CommonFieldsA
    {
        public VisaDTO()
        {
            Employees = new List<EmployeeDTO>();
        }

        [Required]
        [ForeignKey("Agent")]
        public int ForeignAgentId { get; set; }
        public AgentDTO Agent
        {
            get { return GetValue(() => Agent); }
            set { SetValue(() => Agent, value); }
        }

        //[ForeignKey("Agency")]
        //public int? AgencyId { get; set; }
        //public AgencyDTO Agency
        //{
        //    get { return GetValue(() => Agency); }
        //    set { SetValue(() => Agency, value); }
        //}

        [NotMapped]
        public string AgencyString
        {
            get
            {
                return AgencyId.ToString();
            }
            //set { SetValue(() => AgencyString, value); }
        }
        //public CountryList VisaCountry
        //{
        //    get { return GetValue(() => VisaCountry); }
        //    set { SetValue(() => VisaCountry, value); }
        //}

        [StringLength(50)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Contract Number is invalid")]
        public string ContratNumber
        {
            get { return GetValue(() => ContratNumber); }
            set { SetValue(() => ContratNumber, value); }
        }

        [Required]
        [StringLength(50)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Visa Number is invalid")]
        public string VisaNumber
        {
            get { return GetValue(() => VisaNumber); }
            set { SetValue(() => VisaNumber, value); }
        }

        [Required]
        [Range(1, 1000, ErrorMessage = "Quantity should be between 1-1000")]
        public int VisaQuantity
        {
            get { return GetValue(() => VisaQuantity); }
            set { SetValue(() => VisaQuantity, value); }
        }

        [StringLength(50)]
        public string FileNumber
        {
            get { return GetValue(() => FileNumber); }
            set { SetValue(() => FileNumber, value); }
        }

        [StringLength(50)]
        public string BankNumber
        {
            get { return GetValue(() => BankNumber); }
            set { SetValue(() => BankNumber, value); }
        }

        public DateTime? VisaDate
        {
            get { return GetValue(() => VisaDate); }
            set { SetValue(() => VisaDate, value); }
        }

        //[RegularExpression("^[0-9/]{1-15}$", ErrorMessage = "Visa Date is invalid, eg 1435/01/01")]
        //[Required]
        [StringLength(50)]
        public string VisaDateArabic
        {
            get { return GetValue(() => VisaDateArabic); }
            set { SetValue(() => VisaDateArabic, value); }
        }
   
        
        [StringLength(50)]
        public string WekalaNumber
        {
            get { return GetValue(() => WekalaNumber); }
            set { SetValue(() => WekalaNumber, value); }
        }
        [StringLength(50)]
        public string WekalaDate
        {
            get { return GetValue(() => WekalaDate); }
            set { SetValue(() => WekalaDate, value); }
        }

        public string Notes
        {
            get { return GetValue(() => Notes); }
            set { SetValue(() => Notes, value); }
        }

        [Required]
        [ForeignKey("Sponsor")]
        public int SponsorId { get; set; }
        public VisaSponsorDTO Sponsor
        {
            get { return GetValue(() => Sponsor); }
            set { SetValue(() => Sponsor, value); }
        }

        [Required]
        [ForeignKey("Condition")]
        public int ConditionId { get; set; }
        public VisaConditionDTO Condition
        {
            get { return GetValue(() => Condition); }
            set { SetValue(() => Condition, value); }
        }

        [NotMapped]
        [DisplayName("Agent Name")]
        public string AgentName
        {
            get
            {
                if (Agent == null)
                    return "Not Assigned";
                if (Agent.AgentName.Length <= 8)
                    return Agent.AgentName;
                return Agent.AgentName.Substring(0, 8) + "...";
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
                return Agent.AgentName;//.Substring(0, 8) + "...";
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
                if (Sponsor == null)
                    return "-";
                if (Sponsor.FullName.Length <= 15)
                    return Sponsor.FullName;
                return Sponsor.FullName.Substring(0, 15) + "...";
            }
            set { SetValue(() => SponsorFullNameShort, value); }
        }
        [NotMapped]
        public string VisaDetail
        {
            get
            {
                var vis = VisaNumber;

                if (Sponsor != null && Sponsor.FullName != null) vis = string.Format("{0} | {1}", vis, Sponsor.FullName);

                if (Sponsor != null && Sponsor.PassportNumber != null) vis = string.Format("{0} | {1}", vis, Sponsor.PassportNumber);

                if (Condition != null) vis = vis + " | " + Condition.Notes;

                return vis;
            }
            set { SetValue(() => VisaDetail, value); }
        }
        [NotMapped]
        public string VisaDetail2
        {
            get
            {
                var vis = VisaNumber;

                if (Sponsor != null && Sponsor.PassportNumber != null) vis = vis + " - " + Sponsor.PassportNumber ;

                if (Sponsor != null && Sponsor.FullName != null)
                {
                    var fullName = Sponsor.FullName;
                    if (fullName.Length > 35)
                        fullName = fullName.Substring(0, 35) + "...";
                    vis = vis + Environment.NewLine +fullName ;
                }
                
                if (Sponsor != null && Sponsor.Address != null && Sponsor.Address.Mobile != null) vis = vis + Environment.NewLine + Sponsor.Address.Mobile + 
                    " - " + Sponsor.Address.City;

                //if (Condition != null) vis = vis + " | " + Condition.Notes;

                return vis;
            }
            set { SetValue(() => VisaDetail2, value); }
        }
        [NotMapped]
        public string VisaStatus
        {
            get
            {
                if (Employees == null || Employees.Count <= 0) return "Not Assigned";

                return Employees.Count.ToString() + " Employee(s)";
                //var employeeDTO = Employees.FirstOrDefault();
                //return employeeDTO != null ? employeeDTO.CurrentStatusString : "Status Not Known";

            }
            set { SetValue(() => VisaStatus, value); }
        }
        [NotMapped]
        public string EmployeesCount
        {
            get
            {
                try
                {
                    if (Employees == null || Employees.Count <= 0) return "Not Assigned";
                    //var emps =
                    //    Employees.Aggregate("",
                    //        (current, emp) => current + ", " + "" + emp.PassportNumber) +
                    //    "";
                    var emps = Employees.Aggregate("", (current, emp) => current + "), " + emp.PassportNumber + "(" + emp.CurrentStatusString) + ")";
                    //+ emp.FirstName.ToUpper() 

                    //foreach (var emp in Employees)
                    //{
                    //    emps = emps + ", " + emp.FullName.ToUpperInvariant();
                    //}
                    emps = Employees.Count + "(" + emps.Substring(3) + ")";
                    return emps;
                }
                catch
                {
                    return "Not Assigned";
                }
            }
            set { SetValue(() => EmployeesCount, value); }
        }

        public IList<EmployeeDTO> Employees
        {
            get { return GetValue(() => Employees); }
            set { SetValue(() => Employees, value); }
        }
    }
}
