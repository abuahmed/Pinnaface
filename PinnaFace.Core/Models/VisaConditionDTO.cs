using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class VisaConditionDTO : CommonFieldsA
    {
        public ProffesionTypes Profession
        {
            get { return GetValue(() => Profession); }
            set
            {
                SetValue(() => Profession, value);
                SetValue(() => ConditionDescription, EnumUtil.GetEnumDesc(value));
            }
        }

        public ProffesionTypesAmharic ProfessionAmharic
        {
            get { return GetValue(() => ProfessionAmharic); }
            set { SetValue(() => ProfessionAmharic, value); }
        }

        [NotMapped]
        public string ReligionString
        {
            get
            {
                return Convert.ToInt32(Religion).ToString();
            }
        }
        [NotMapped]
        public string ProfessionString
        {
            get
            {
                return Convert.ToInt32(Profession).ToString();
            }
        }
        [NotMapped]
        public string AgeString
        {
            get
            {
                return Convert.ToInt32(Age).ToString();
            }
        }
        [NotMapped]
        public string ContratPeriodString
        {
            get
            {
                return Convert.ToInt32(ContratPeriod).ToString();
            }
        }
        [NotMapped]
        public string ComplexionString
        {
            get
            {
                return Convert.ToInt32(Complexion).ToString();
            }
        }
        [NotMapped]
        public string SalaryString
        {
            get
            {
                return Salary.ToString("N0") + EnumUtil.GetEnumDesc(CurrencyType);
            }
        }

        [Required]
        public float Salary
        {
            get { return GetValue(() => Salary); }
            set { SetValue(() => Salary, value); }
        }

        public CurrencyTypes CurrencyType
        {
            get { return GetValue(() => CurrencyType); }
            set
            {
                SetValue(() => CurrencyType, value);
                SetValue(() => ConditionDescription, EnumUtil.GetEnumDesc(value));
            }
        }

        public string Notes
        {
            get { return GetValue(() => Notes); }
            set { SetValue(() => Notes, value); }
        }

        [Required]
        public ReligionTypes Religion
        {
            get { return GetValue(() => Religion); }
            set { SetValue(() => Religion, value); }
        }

        public AgeCategory Age
        {
            get { return GetValue(() => Age); }
            set
            {
                SetValue(() => Age, value);
                SetValue(() => ConditionDescription,EnumUtil.GetEnumDesc(value));
            }
        }
        public ContratPeriods ContratPeriod
        {
            get { return GetValue(() => ContratPeriod); }
            set { SetValue(() => ContratPeriod, value); }
        }
        public Complexion Complexion
        {
            get { return GetValue(() => Complexion); }
            set { SetValue(() => Complexion, value); }
        }
        
        [NotMapped]
        public int AgeFrom
        {
            get { return Convert.ToInt16(EnumUtil.GetEnumDesc(Age).Substring(0,2)); }
            set { SetValue(() => AgeFrom, value); }
        }

        [NotMapped]
        public int AgeTo
        {
            get
            {
                return Convert.ToInt16(EnumUtil.GetEnumDesc(Age).Substring(3, 2));
            }
            set { SetValue(() => AgeTo, value); }
        }

        public bool FirstTime
        {
            get { return GetValue(() => FirstTime); }
            set { SetValue(() => FirstTime, value); }
        }

        public bool GoodLooking
        {
            get { return GetValue(() => GoodLooking); }
            set { SetValue(() => GoodLooking, value); }
        }

        public bool WriteRead
        {
            get { return GetValue(() => WriteRead); }
            set { SetValue(() => WriteRead, value); }
        }

        [NotMapped]
        public string ConditionDescription
        {
            get
            {
                string desc = Salary +" " + EnumUtil.GetEnumDesc(CurrencyType) + Environment.NewLine;
                desc = desc + EnumUtil.GetEnumDesc(Age) + Environment.NewLine;
                desc = desc + EnumUtil.GetEnumDesc(Profession);
                return desc;
            }
            set { SetValue(() => ConditionDescription, value); }
        }
    }
}