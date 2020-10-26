using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class VisaSponsorDTO : CommonFieldsA
    {
        [Required]
        [StringLength(50)]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Sponsor Id is invalid")]
        [DisplayName("Id Card No.")]
        public string PassportNumber
        {
            get { return GetValue(() => PassportNumber); }
            set { SetValue(() => PassportNumber, value); }
        }

        [Required]
        [StringLength(150)]
        //[RegularExpression("^([a-zA-Z-]+\\ )+([a-zA-Z-]+\\ )+[a-zA-Z-]{1,16}$", ErrorMessage = "Name is invalid, must be full name with spaces")]
        [DisplayName("Full Name")]
        public string FullName
        {
            get { return GetValue(() => FullName); }
            set { SetValue(() => FullName, value); }
        }
        [StringLength(50)]
        public string FirstName
        {
            get { return GetValue(() => FirstName); }
            set { SetValue(() => FirstName, value); }
        }
        [StringLength(50)]
        public string MiddleName
        {
            get { return GetValue(() => MiddleName); }
            set { SetValue(() => MiddleName, value); }
        }
        [StringLength(50)]
        public string LastName
        {
            get { return GetValue(() => LastName); }
            set { SetValue(() => LastName, value); }
        }
        
        //[Required]
        [StringLength(150)]
        public string FullNameAmharic
        {
            get { return GetValue(() => FullNameAmharic); }
            set { SetValue(() => FullNameAmharic, value); }
        }
        [StringLength(50)]
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
        public string CityArabic
        {
            get { return GetValue(() => CityArabic); }
            set { SetValue(() => CityArabic, value); }
        }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }
        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }
    
    }
}
