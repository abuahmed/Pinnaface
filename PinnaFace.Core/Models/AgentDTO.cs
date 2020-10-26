using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core.Models
{
    public class AgentDTO : CommonFieldsA2
    {
        public AgentDTO()
        {
            Visas = new List<VisaDTO>();
            Agencies = new List<AgencyAgentDTO>();
        }
        
        public CountryList Country
        {
            get { return GetValue(() => Country); }
            set { SetValue(() => Country, value); }
        }

        [Required]
        [StringLength(150)]
        public string AgentName
        {
            get { return GetValue(() => AgentName); }
            set { SetValue(() => AgentName, value); }
        }

        [StringLength(150)]
        public string ContactPerson
        {
            get { return GetValue(() => ContactPerson); }
            set { SetValue(() => ContactPerson, value); }
        }
            
        [StringLength(50)]
        public string LicenseNumber
        {
            get { return GetValue(() => LicenseNumber); }
            set { SetValue(() => LicenseNumber, value); }
        }

        [StringLength(50)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Id No is Invalid")]
        public string PassportNumber
        {
            get { return GetValue(() => PassportNumber); }
            set { SetValue(() => PassportNumber, value); }
        }  
        
        [Required]
        [StringLength(150)]
        public string AgentNameAmharic
        {
            get { return GetValue(() => AgentNameAmharic); }
            set { SetValue(() => AgentNameAmharic, value); }
        }

        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }  

        //[MaxLength]
        //public byte[] Header
        //{
        //    get { return GetValue(() => Header); }
        //    set { SetValue(() => Header, value); }
        //}
        //[MaxLength]
        //public byte[] Footer
        //{
        //    get { return GetValue(() => Footer); }
        //    set { SetValue(() => Footer, value); }
        //}
        
        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }

        [ForeignKey("Header")]
        public int? HeaderId { get; set; }
        public AttachmentDTO Header
        {
            get { return GetValue(() => Header); }
            set { SetValue(() => Header, value); }
        }

        [ForeignKey("Footer")]
        public int? FooterId { get; set; }
        public AttachmentDTO Footer
        {
            get { return GetValue(() => Footer); }
            set { SetValue(() => Footer, value); }
        }

        public ICollection<VisaDTO> Visas
        {
            get { return GetValue(() => Visas); }
            set { SetValue(() => Visas, value); }
        }

        public ICollection<AgencyAgentDTO> Agencies
        {
            get { return GetValue(() => Agencies); }
            set { SetValue(() => Agencies, value); }
        }

        [NotMapped]
        public string AgentDetail
        {
            get
            {
                return AgentName + " (" + Id.ToString() + ")";
            }
            set { SetValue(() => AgentDetail, value); }
        }
    }
}
