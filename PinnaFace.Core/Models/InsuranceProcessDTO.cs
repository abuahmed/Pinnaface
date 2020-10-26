using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class InsuranceProcessDTO : CommonFieldsB
    {
        //[Required]
        [StringLength(50)]
        public string PolicyNumber
        {
            get { return GetValue(() => PolicyNumber); }
            set
            {
                SetValue(() => PolicyNumber, value);
                SetValue(() => InsuranceProcessDescription, value);
            }
        }

        public InsuranceList InsuranceCompany
        {
            get { return GetValue(() => InsuranceCompany); }
            set
            {
                SetValue(() => InsuranceCompany, value);
                SetValue(() => InsuranceProcessDescription, value.ToString());
            }
        }

        [StringLength(50)]
        public string InsuranceBranch
        {
            get { return GetValue(() => InsuranceBranch); }
            set { SetValue(() => InsuranceBranch, value); }
        }

        [DataType(DataType.Currency)]
        public float InsuredAmount
        {
            get { return GetValue(() => InsuredAmount); }
            set
            {
                SetValue(() => InsuredAmount, value);
                SetValue(() => InsuranceProcessDescription, value.ToString());
            }
        }
        
        public DateTime? BeginingDate
        {
            get { return GetValue(() => BeginingDate); }
            set { SetValue(() => BeginingDate, value); }
        }
        public DateTime? EndDate
        {
            get { return GetValue(() => EndDate); }
            set { SetValue(() => EndDate, value); }
        }

        public bool MedicalFirst
        {
            get { return GetValue(() => MedicalFirst); }
            set { SetValue(() => MedicalFirst, value); }
        }

        public bool MedicalSecond
        {
            get { return GetValue(() => MedicalSecond); }
            set { SetValue(() => MedicalSecond, value); }
        }

        [NotMapped]
        public string InsuranceCompanyDescription
        {
            get
            {
                return EnumUtil.GetEnumDesc(InsuranceCompany);
            }
            set { SetValue(() => InsuranceCompanyDescription, value); }
        }
        [NotMapped]
        public string InsuranceProcessDescription
        {
            get
            {
                if (InsuredAmount < 1)
                    return "";
                return EnumUtil.GetEnumDesc(InsuranceCompany) + Environment.NewLine + 
                    InsuredAmount.ToString("N2") + Environment.NewLine + PolicyNumber;
            }
            set { SetValue(() => InsuranceProcessDescription, value); }
        }
    }
}
