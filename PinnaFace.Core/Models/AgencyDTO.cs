using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class AgencyDTO : CommonFieldsA2
    {
        public AgencyDTO()
        {
            Agents = new List<AgencyAgentDTO>();
        }

        [Required]
        [StringLength(250)]
        public string AgencyName
        {
            get { return GetValue(() => AgencyName); }
            set { SetValue(() => AgencyName, value); }
        }

        [Required]
        [StringLength(50)]
        public string Managertype
        {
            get { return GetValue(() => Managertype); }
            set { SetValue(() => Managertype, value); }
        }

        [Required]
        [StringLength(250)]
        public string ManagerName
        {
            get { return GetValue(() => ManagerName); }
            set { SetValue(() => ManagerName, value); }
        }

        [Required]
        public string DepositAmount
        {
            get { return GetValue(() => DepositAmount); }
            set { SetValue(() => DepositAmount, value); }
        }

        [Required]
        [StringLength(150)]
        public string LicenceNumber
        {
            get { return GetValue(() => LicenceNumber); }
            set { SetValue(() => LicenceNumber, value); }
        }

        [Required]
        [StringLength(150)]
        public string AgencyNameAmharic
        {
            get { return GetValue(() => AgencyNameAmharic); }
            set { SetValue(() => AgencyNameAmharic, value); }
        }

        [Required]
        [StringLength(150)]
        public string ManagerNameAmharic
        {
            get { return GetValue(() => ManagerNameAmharic); }
            set { SetValue(() => ManagerNameAmharic, value); }
        }

        #region Countries of Opertaion
        public bool SaudiOperation
        {
            get { return GetValue(() => SaudiOperation); }
            set { SetValue(() => SaudiOperation, value); }
        }
        public bool DubaiOperation
        {
            get { return GetValue(() => DubaiOperation); }
            set { SetValue(() => DubaiOperation, value); }
        }
        public bool KuwaitOperation
        {
            get { return GetValue(() => KuwaitOperation); }
            set { SetValue(() => KuwaitOperation, value); }
        }
        public bool QatarOperation
        {
            get { return GetValue(() => QatarOperation); }
            set { SetValue(() => QatarOperation, value); }
        }
        public bool JordanOperation
        {
            get { return GetValue(() => JordanOperation); }
            set { SetValue(() => JordanOperation, value); }
        }
        public bool LebanonOperation
        {
            get { return GetValue(() => LebanonOperation); }
            set { SetValue(() => LebanonOperation, value); }
        }
        public bool BahrainOperation
        {
            get { return GetValue(() => BahrainOperation); }
            set { SetValue(() => BahrainOperation, value); }
        }
        [NotMapped] 
        public IList CountriesOfOpertaion
        {
            get
            {
                var lists=new List<string>();
                if(SaudiOperation)
                    lists.Add(EnumUtil.GetEnumDesc(CountryList.SaudiArabia));
                if (DubaiOperation)
                    lists.Add(EnumUtil.GetEnumDesc(CountryList.UAE));
                if (KuwaitOperation)
                    lists.Add(EnumUtil.GetEnumDesc(CountryList.Kuwait));
                if (QatarOperation)
                    lists.Add(EnumUtil.GetEnumDesc(CountryList.Qatar));
                if (JordanOperation)
                    lists.Add(EnumUtil.GetEnumDesc(CountryList.Jordan));
                if (LebanonOperation)
                    lists.Add(EnumUtil.GetEnumDesc(CountryList.Lebanon));
                if (BahrainOperation)
                    lists.Add(EnumUtil.GetEnumDesc(CountryList.Bahrain));

                return lists;
            }   
            set { SetValue(() => CountriesOfOpertaion, value); }
        }
        #endregion
        

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

        public ICollection<AgencyAgentDTO> Agents
        {
            get { return GetValue(() => Agents); }
            set { SetValue(() => Agents, value); }
        }

        [NotMapped]
        public string AgencyNameShort
        {
            get
            {
                if (AgencyName.Length > 30)
                    return AgencyName.Substring(0, 30) + "...";
                return AgencyName;
            }
            set { SetValue(() => AgencyNameShort, value); }
        }
        [NotMapped]
        public string AgencyDetail
        {
            get
            {
                return AgencyName + " (" +Id.ToString()+ ")";
            }
            set { SetValue(() => AgencyDetail, value); }
        }
    }
}
