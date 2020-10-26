using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class EmployeeExperienceDTO : CommonFieldsA
    {
        //Experience
        public bool HaveWorkExperience
        {
            get { return GetValue(() => HaveWorkExperience); }
            set
            {
                SetValue(() => HaveWorkExperience, value);
                SetValue<string>(() => EmployeeExperienceDescription, value.ToString());
            }
        }
 
        public CountryList ExperienceCountry
        {
            get { return GetValue(() => ExperienceCountry); }
            set
            {
                SetValue(() => ExperienceCountry, value);
                SetValue(() => EmployeeExperienceDescription,EnumUtil.GetEnumDesc(value));
            }
        } 
      
        public ContratPeriods ExperiencePeriod
        {
            get { return GetValue(() => ExperiencePeriod); }
            set
            {
                SetValue(() => ExperiencePeriod, value);
                SetValue(() => EmployeeExperienceDescription, EnumUtil.GetEnumDesc(value));
            }
        }

        public bool HaveWorkExperienceInCountry
        {
            get { return GetValue(() => HaveWorkExperienceInCountry); }
            set
            {
                SetValue(() => HaveWorkExperienceInCountry, value);
            }
        }
        public ContratPeriods ExperiencePeriodInCountry
        {
            get { return GetValue(() => ExperiencePeriodInCountry); }
            set
            {
                SetValue(() => ExperiencePeriodInCountry, value);
                SetValue(() => EmployeeExperienceDescription, EnumUtil.GetEnumDesc(value));
            }
        }
        public ProffesionTypes ExperiencePosition
        {
            get { return GetValue(() => ExperiencePosition); }
            set
            {
                SetValue(() => ExperiencePosition, value);
                SetValue(() => EmployeeExperienceDescription, EnumUtil.GetEnumDesc(value));
            }
        }

        [NotMapped]
        public string ExperienceCountryString
        {
            get
            {
                return Convert.ToInt32(ExperienceCountry).ToString();
            }
        }
        [NotMapped]
        public string ExperiencePeriodString
        {
            get
            {
                return Convert.ToInt32(ExperiencePeriod).ToString();
            }
        }
        [NotMapped]
        public string ExperiencePositionString
        {
            get
            {
                return Convert.ToInt32(ExperiencePosition).ToString();
            }
        }
        //Skills
        public bool Driving
        {
            get { return GetValue(() => Driving); }
            set { SetValue(() => Driving, value); }
        }
        public bool HardWorker
        {
            get { return GetValue(() => HardWorker); }
            set { SetValue(() => HardWorker, value); }
        }
        public bool BabySitting
        {
            get { return GetValue(() => BabySitting); }
            set { SetValue(() => BabySitting, value); }
        }
        public bool Nanny
        {
            get { return GetValue(() => Nanny); }
            set { SetValue(() => Nanny, value); }
        }
        public bool Washing
        {
            get { return GetValue(() => Washing); }
            set { SetValue(() => Washing, value); }
        }
        public bool Cleaning
        {
            get { return GetValue(() => Cleaning); }
            set { SetValue(() => Cleaning, value); }
        }
        public bool WashingDishes
        {
            get { return GetValue(() => WashingDishes); }
            set { SetValue(() => WashingDishes, value); }
        }
        public bool Cooking
        {
            get { return GetValue(() => Cooking); }
            set { SetValue(() => Cooking, value); }
        }
        
        public bool ArabicCooking
        {
            get { return GetValue(() => ArabicCooking); }
            set { SetValue(() => ArabicCooking, value); }
        }
        public bool Sewing
        {
            get { return GetValue(() => Sewing); }
            set { SetValue(() => Sewing, value); }
        }
        public bool Tutoring
        {
            get { return GetValue(() => Tutoring); }
            set { SetValue(() => Tutoring, value); }
        }
        public bool Computer
        {
            get { return GetValue(() => Computer); }
            set { SetValue(() => Computer, value); }
        }

        public bool Decorating
        {
            get { return GetValue(() => Decorating); }
            set { SetValue(() => Decorating, value); }
        }

        [StringLength(250)]
        public string OtherSkills
        {
            get { return GetValue(() => OtherSkills); }
            set { SetValue(() => OtherSkills, value); }
        }

        [StringLength(250)]
        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }
        
        //[ForeignKey("Employee")]
        //public int EmployeeId { get; set; }
        //public EmployeeDTO Employee
        //{
        //    get { return GetValue(() => Employee); }
        //    set { SetValue(() => Employee, value); }
        //}

        [NotMapped]
        public string EmployeeExperienceDescription
        {
            get
            {
                string desc = "First Timer";// "No Experience";

                if (HaveWorkExperience)
                {
                    desc = EnumUtil.GetEnumDesc(ExperienceCountry) + Environment.NewLine;
                    desc = desc + EnumUtil.GetEnumDesc(ExperiencePeriod) + Environment.NewLine;
                    desc = desc + EnumUtil.GetEnumDesc(ExperiencePosition) ;
                }

                return desc;
            }
            set { SetValue(() => EmployeeExperienceDescription, value); }
        }

        //Skills
        [NotMapped]
        public string DrivingString
        {
            get { return Driving ? "YES" : "NO"; }
            set { SetValue(() => DrivingString, value); }
        }
        [NotMapped]
        public string HardWorkerString
        {
            get { return HardWorker ? "YES" : "NO"; }
            set { SetValue(() => HardWorkerString, value); }
        }
        [NotMapped]
        public string BabySittingString
        {
            get { return BabySitting ? "YES" : "NO"; }
            set { SetValue(() => BabySittingString, value); }
        }
        [NotMapped]
        public string NannyString
        {
            get { return Nanny ? "YES" : "NO"; }
            set { SetValue(() => NannyString, value); }
        }
        [NotMapped]
        public string WashingString
        {
            get { return Washing ? "YES" : "NO"; }
            set { SetValue(() => WashingString, value); }
        }
        [NotMapped]
        public string CleaningString
        {
            get { return Cleaning ? "YES" : "NO"; }
            set { SetValue(() => CleaningString, value); }
        }
        [NotMapped]
        public string WashingDishesString
        {
            get { return WashingDishes ? "YES" : "NO"; }
            set { SetValue(() => WashingDishesString, value); }
        }
        [NotMapped]
        public string CookingString
        {
            get { return Cooking ? "YES" : "NO"; }
            set { SetValue(() => CookingString, value); }
        }
        [NotMapped]
        public string ArabicCookingString
        {
            get { return ArabicCooking ? "YES" : "NO"; }
            set { SetValue(() => ArabicCookingString, value); }
        }
        [NotMapped]
        public string SewingString
        {
            get { return Sewing ? "YES" : "NO"; }
            set { SetValue(() => SewingString, value); }
        }
        [NotMapped]
        public string TutoringString
        {
            get { return Tutoring ? "YES" : "NO"; }
            set { SetValue(() => TutoringString, value); }
        }
        [NotMapped]
        public string ComputerString
        {
            get { return Computer ? "YES" : "NO"; }
            set { SetValue(() => ComputerString, value); }
        }

    }
}
