using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;

namespace PinnaFace.Core.Models
{
    public class EmployeeEducationDTO : CommonFieldsA
    {
        public LanguageExperience EnglishLanguage
        {
            get { return GetValue(() => EnglishLanguage); }
            set
            {
                SetValue(() => EnglishLanguage, value);
                SetValue(() => EmployeeEducationDescription, EnumUtil.GetEnumDesc(value)); 
            }
        }
        public LanguageExperience ArabicLanguage
        {
            get { return GetValue(() => ArabicLanguage); }
            set
            {
                SetValue(() => ArabicLanguage, value);
                SetValue(() => EmployeeEducationDescription, EnumUtil.GetEnumDesc(value)); 
            }
        }
        public LevelOfQualificationTypes LevelOfQualification
        {
            get { return GetValue(() => LevelOfQualification); }
            set
            {
                SetValue(() => LevelOfQualification, value);
                SetValue(() => EmployeeEducationDescription, EnumUtil.GetEnumDesc(value));
            }
        }
        public QualificationTypes QualificationType
        {
            get { return GetValue(() => QualificationType); }
            set
            {
                SetValue(() => QualificationType, value);
                SetValue(() => EmployeeEducationDescription, EnumUtil.GetEnumDesc(value));
            }
        }
        public AwardTypes Award
        {
            get { return GetValue(() => Award); }
            set { SetValue(() => Award, value); }
        }

        [StringLength(50)]
        public string FieldOfStudy
        {
            get { return GetValue(() => FieldOfStudy); }
            set { SetValue(() => FieldOfStudy, value); }
        }
        [StringLength(50)]
        public string YearCompleted
        {
            get { return GetValue(() => YearCompleted); }
            set { SetValue(() => YearCompleted, value); }
        }
        [StringLength(50)]
        public string ProffesionalSkill
        {
            get { return GetValue(() => ProffesionalSkill); }
            set { SetValue(() => ProffesionalSkill, value); }
        }
        [StringLength(50)]
        public string EducateQG
        {
            get { return GetValue(() => EducateQG); }
            set { SetValue(() => EducateQG, value); }
        }
        [StringLength(250)]
        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }

        [NotMapped]
        public string LevelString
        {
            get
            {
                return EnumUtil.GetEnumDesc(LevelOfQualification);
            }
            set { 
                SetValue(() => LevelString, value); 
                SetValue(() => EmployeeEducationDescription, value); }
        }
        [NotMapped]
        public string EmployeeEducationDescription
        {
            get
            {
                string desc = EnumUtil.GetEnumDesc(QualificationType) + Environment.NewLine + 
                    "Arabic: " + EnumUtil.GetEnumDesc(ArabicLanguage) + Environment.NewLine + 
                    "English: " + EnumUtil.GetEnumDesc(EnglishLanguage);
                return desc;
            }
            set { SetValue(() => EmployeeEducationDescription, value); }
        }
        [NotMapped]
        public string ArabicLanguageString
        {
            get
            {
                return Convert.ToInt32(ArabicLanguage).ToString();
            }
        }
        [NotMapped]
        public string EnglishLanguageString
        {
            get
            {
                return Convert.ToInt32(EnglishLanguage).ToString();
            }
        }
        
        //[Required]
        //[ForeignKey("Employee")]
        //public int EmployeeId { get; set; }
        //public EmployeeDTO Employee
        //{
        //    get { return GetValue(() => Employee); }
        //    set { SetValue(() => Employee, value); }
        //}
    }
}
