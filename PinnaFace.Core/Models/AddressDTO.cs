using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class AddressDTO : CommonFieldsA
    {
        public AddressTypes AddressType
        {
            get { return GetValue(() => AddressType); }
            set { SetValue(() => AddressType, value); }
        }

        [Display(Name = "Detail Address")]
        public string AddressDetail
        {
            get { return GetValue(() => AddressDetail); }
            set { SetValue(() => AddressDetail, value); }
        }

        [MaxLength(250, ErrorMessage = "Exceeded 250 letters")]
        [Display(Name = "Street Address")]
        public string StreetAddress
        {
            get { return GetValue(() => StreetAddress); }
            set { SetValue(() => StreetAddress, value); }
        }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Mobile Number is invalid")]
        public string Mobile
        {
            get { return GetValue(() => Mobile); }
            set
            {
                SetValue(() => Mobile, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Alternate Mobile")]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Alternate Mobile Number is invalid")]
        public string AlternateMobile
        {
            get { return GetValue(() => AlternateMobile); }
            set { SetValue(() => AlternateMobile, value); }
        }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Fixed Line Telephone")]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Telephone is invalid")]
        public string Telephone
        {
            get { return GetValue(() => Telephone); }
            set { SetValue(() => Telephone, value); }
        }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Alternate Fixed Line Telephone")]
        [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Alternate Telephone is invalid")]
        public string AlternateTelephone
        {
            get { return GetValue(() => AlternateTelephone); }
            set { SetValue(() => AlternateTelephone, value); }
        }

        [DataType(DataType.EmailAddress)]
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
        [Display(Name = "Primary Email")]
        public string PrimaryEmail
        {
            get { return GetValue(() => PrimaryEmail); }
            set { SetValue(() => PrimaryEmail, value); }
        }

        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
        [Display(Name = "Alternate Email")]
        public string AlternateEmail
        {
            get { return GetValue(() => AlternateEmail); }
            set { SetValue(() => AlternateEmail, value); }
        }

        [DataType(DataType.Url)]
        [RegularExpression("^([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Web Address is invalid")]
        [Display(Name = "Web Address")]
        public string WebAddress
        {
            get { return GetValue(() => WebAddress); }
            set { SetValue(() => WebAddress, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Region
        {
            get { return GetValue(() => Region); }
            set { SetValue(() => Region, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string City
        {
            get { return GetValue(() => City); }
            set
            {
                SetValue(() => City, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [Display(Name = "Sub City")]
        public string SubCity
        {
            get { return GetValue(() => SubCity); }
            set
            {
                SetValue(() => SubCity, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Woreda
        {
            get { return GetValue(() => Woreda); }
            set
            {
                SetValue(() => Woreda, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Kebele
        {
            get { return GetValue(() => Kebele); }
            set
            {
                SetValue(() => Kebele, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [Display(Name = "House Number")]
        public string HouseNumber
        {
            get { return GetValue(() => HouseNumber); }
            set
            {
                SetValue(() => HouseNumber, value);
                SetValue(() => AddressDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        [Display(Name = "P.O.Box")]
        public string PoBox
        {
            get { return GetValue(() => PoBox); }
            set { SetValue(() => PoBox, value); }
        }

        [RegularExpression("^[0-9]{8,14}$", ErrorMessage = "Fax is invalid")]
        public string Fax
        {
            get { return GetValue(() => Fax); }
            set { SetValue(() => Fax, value); }
        }

        public CountryList Country
        {
            get { return GetValue(() => Country); }
            set { SetValue(() => Country, value); }
        }

        public CountryListAmharic CountryAmharic
        {
            get { return GetValue(() => CountryAmharic); }
            set { SetValue(() => CountryAmharic, value); }
        }

        [StringLength(50)]
        public string CityAmharic
        {
            get { return GetValue(() => CityAmharic); }
            set { SetValue(() => CityAmharic, value); }
        }

        [NotMapped]
        public string MobileWithCountryCode
        {
            get
            {
                if (string.IsNullOrEmpty(Mobile))
                    return "";

                if (Mobile.Length <= 10)
                {
                    if (Country.Equals(CountryList.Ethiopia))
                    {
                        string mob = Mobile.Substring(0, 1);
                        if (mob.Equals("0"))
                            return "+251" + Mobile.Substring(1);
                        return "+251" + Mobile;
                    }
                    else if(Country.Equals(CountryList.SaudiArabia))
                    {
                        string mob = Mobile.Substring(0, 1);
                        if (mob.Equals("0"))
                            return "00966" + Mobile.Substring(1);
                        return "00966" + Mobile;
                    }
                }


                return Mobile;
            }
            set { SetValue(() => MobileWithCountryCode, value); }
        }

        [NotMapped]
        public string AddressDetailShort
        {
            get
            {
                if (AddressDetail == null)
                    return "";
                if (AddressDetail.Length > 20)
                    return AddressDetail.Substring(0, 20) + "..";
                return AddressDetail;
            }
            set { SetValue(() => AddressDetailShort, value); }
        }

        [NotMapped]
        public string AddressDescription
        {
            get
            {
                string desc = "";
                if (!string.IsNullOrWhiteSpace(Mobile))
                    desc = Mobile + Environment.NewLine;
                //if (!string.IsNullOrWhiteSpace(SubCity) || !string.IsNullOrWhiteSpace(Kebele) ||
                //    !string.IsNullOrWhiteSpace(Kebele) || !string.IsNullOrWhiteSpace(HouseNumber))
                //    desc = desc + SubCity + " " + Woreda + " " + Kebele + " " + HouseNumber + Environment.NewLine;
                if (!string.IsNullOrWhiteSpace(City))
                    desc = desc + City;// + ", " + EnumUtil.GetEnumDesc(Country).ToUpper();
                return desc;
            }
            set { SetValue(() => AddressDescription, value); }
        }
    }

    //public class AddressDTO : CommonFieldsA
    //{
    //    public AddressTypes AddressType
    //    {
    //        get { return GetValue(() => AddressType); }
    //        set { SetValue(() => AddressType, value); }
    //    }

    //    [Display(Name = "Detail Address")]
    //    public string AddressDetail
    //    {
    //        get { return GetValue(() => AddressDetail); }
    //        set { SetValue(() => AddressDetail, value); }
    //    }

    //    [MaxLength(250, ErrorMessage = "Exceeded 250 letters")]
    //    [Display(Name = "Street Address")]
    //    public string StreetAddress
    //    {
    //        get { return GetValue(() => StreetAddress); }
    //        set { SetValue(() => StreetAddress, value); }
    //    }

    //    [DataType(DataType.PhoneNumber)]
    //    [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Mobile Number is invalid")]
    //    public string Mobile
    //    {
    //        get { return GetValue(() => Mobile); }
    //        set
    //        {
    //            SetValue(() => Mobile, value);
    //            SetValue(() => AddressDescription, value);
    //        }
    //    }

    //    [DataType(DataType.PhoneNumber)]
    //    [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Alternate Mobile Number is invalid")]
    //    public string AlternateMobile
    //    {
    //        get { return GetValue(() => AlternateMobile); }
    //        set { SetValue(() => AlternateMobile, value); }
    //    }

    //    [DataType(DataType.PhoneNumber)]
    //    [Display(Name = "Fixed Line Telephone")]
    //    [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Telephone is invalid")]
    //    public string Telephone
    //    {
    //        get { return GetValue(() => Telephone); }
    //        set { SetValue(() => Telephone, value); }
    //    }

    //    [DataType(DataType.PhoneNumber)]
    //    [Display(Name = "Alternate Fixed Line Telephone")]
    //    [RegularExpression("^[0-9]{10,14}$", ErrorMessage = "Alternate Telephone is invalid")]
    //    public string AlternateTelephone
    //    {
    //        get { return GetValue(() => AlternateTelephone); }
    //        set { SetValue(() => AlternateTelephone, value); }
    //    }

    //    [DataType(DataType.EmailAddress)]
    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
    //    [Display(Name = "Primary Email")]
    //    public string PrimaryEmail
    //    {
    //        get { return GetValue(() => PrimaryEmail); }
    //        set { SetValue(() => PrimaryEmail, value); }
    //    }

    //    [DataType(DataType.EmailAddress)]
    //    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is invalid")]
    //    [Display(Name = "Alternate Email")]
    //    public string AlternateEmail
    //    {
    //        get { return GetValue(() => AlternateEmail); }
    //        set { SetValue(() => AlternateEmail, value); }
    //    }

    //    [DataType(DataType.Url)]
    //    [RegularExpression("^([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Web Address is invalid")]
    //    [Display(Name = "Web Address")]
    //    public string WebAddress
    //    {
    //        get { return GetValue(() => WebAddress); }
    //        set { SetValue(() => WebAddress, value); }
    //    }

    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    public string Region
    //    {
    //        get { return GetValue(() => Region); }
    //        set { SetValue(() => Region, value); }
    //    }

    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    public string City
    //    {
    //        get { return GetValue(() => City); }
    //        set { SetValue(() => City, value); }
    //    }

    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    [Display(Name = "Sub City")]
    //    public string SubCity
    //    {
    //        get { return GetValue(() => SubCity); }
    //        set { SetValue(() => SubCity, value); }
    //    }

    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    public string Woreda
    //    {
    //        get { return GetValue(() => Woreda); }
    //        set { SetValue(() => Woreda, value); }
    //    }

    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    public string Kebele
    //    {
    //        get { return GetValue(() => Kebele); }
    //        set { SetValue(() => Kebele, value); }
    //    }

    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    [Display(Name = "House Number")]
    //    public string HouseNumber
    //    {
    //        get { return GetValue(() => HouseNumber); }
    //        set { SetValue(() => HouseNumber, value); }
    //    }

    //    [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
    //    [Display(Name = "P.O.Box")]
    //    public string PoBox
    //    {
    //        get { return GetValue(() => PoBox); }
    //        set { SetValue(() => PoBox, value); }
    //    }

    //    [RegularExpression("^[0-9]{8,14}$", ErrorMessage = "Fax is invalid")]
    //    public string Fax
    //    {
    //        get { return GetValue(() => Fax); }
    //        set { SetValue(() => Fax, value); }
    //    }

    //    public CountryList Country
    //    {
    //        get { return GetValue(() => Country); }
    //        set { SetValue(() => Country, value); }
    //    }

    //    public CountryListAmharic CountryAmharic
    //    {
    //        get { return GetValue(() => CountryAmharic); }
    //        set { SetValue(() => CountryAmharic, value); }
    //    }

    //    [StringLength(50)]
    //    public string CityAmharic
    //    {
    //        get { return GetValue(() => CityAmharic); }
    //        set { SetValue(() => CityAmharic, value); }
    //    }

    //    [NotMapped]
    //    public string AddressDetailShort
    //    {
    //        get
    //        {
    //            if (AddressDetail == null)
    //                return "";
    //            if (AddressDetail.Length > 20)
    //                return AddressDetail.Substring(0, 20) + "..";
    //            return AddressDetail;
    //        }
    //        set { SetValue(() => AddressDetailShort, value); }
    //    }

    //    [NotMapped]
    //    public string AddressDescription
    //    {
    //        get
    //        {
    //            string desc = "";
    //            if (!string.IsNullOrWhiteSpace(Mobile))
    //                desc = Mobile + Environment.NewLine;
    //            if (!string.IsNullOrWhiteSpace(SubCity) || !string.IsNullOrWhiteSpace(Kebele) ||
    //                !string.IsNullOrWhiteSpace(Kebele) || !string.IsNullOrWhiteSpace(HouseNumber))
    //                desc = desc + SubCity + " " + Woreda + " " + Kebele + " " + HouseNumber + Environment.NewLine;
    //            if (!string.IsNullOrWhiteSpace(City))
    //                desc = desc + City + ", " + EnumUtil.GetEnumDesc(Country).ToUpper();
    //            return desc;
    //        }
    //        set { SetValue(() => AddressDescription, value); }
    //    }
    //}
}