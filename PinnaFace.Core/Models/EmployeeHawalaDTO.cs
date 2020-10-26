using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class EmployeeHawalaDTO : CommonFieldsA
    {
        public BankList BankName
        {
            get { return GetValue(() => BankName); }
            set
            {
                SetValue(() => BankName, value);
                SetValue(() => EmployeeHawalaDescription, EnumUtil.GetEnumDesc(value));
                SetValue(() => SwiftCodeDescription, EnumUtil.GetEnumDesc(value));
            }
        }

        public SwiftCodeList SwiftCode
        {
            get { return GetValue(() => SwiftCode); }
            set
            {
                SetValue(() => SwiftCode, value);
            }
        }

        [Required]
        [RegularExpression("^[0-9]{13}$", ErrorMessage = "Account Number is invalid, must be 13 Digit")]
        public string AccountNumber
        {
            get { return GetValue(() => AccountNumber); }
            set
            {
                SetValue(() => AccountNumber, value);
                SetValue(() => EmployeeHawalaDescription, value);
            }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string BankBranch
        {
            get { return GetValue(() => BankBranch); }
            set { SetValue(() => BankBranch, value); }
        }
        
        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string AccountFormat
        {
            get { return GetValue(() => AccountFormat); }
            set { SetValue(() => AccountFormat, value); }
        }

        [MaxLength(30, ErrorMessage = "Exceeded 30 letters")]
        public string Iban
        {
            get { return GetValue(() => Iban); }
            set { SetValue(() => Iban, value); }
        }

        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }

        [NotMapped]
        public string SwiftCodeDescription
        {
            get
            {
                int bn = Convert.ToInt32(BankName);
                SwiftCode = (SwiftCodeList)bn;
                return EnumUtil.GetEnumDesc((SwiftCodeList)bn);
            }
            set { SetValue(() => SwiftCodeDescription, value); }
        }
        [NotMapped]
        public string BankNameDescription
        {
            get
            {
                return EnumUtil.GetEnumDesc(BankName);
            }
            set { SetValue(() => BankNameDescription, value); }
        }
        [NotMapped]
        public string EmployeeHawalaDescription
        {
            get
            {
                if (AccountNumber == "0000000000000")
                    return "";
                string desc = EnumUtil.GetEnumDesc(BankName) + Environment.NewLine + SwiftCodeDescription;
                return desc + Environment.NewLine + AccountNumber;
            }
            set { SetValue(() => EmployeeHawalaDescription, value); }
        }
    }
}