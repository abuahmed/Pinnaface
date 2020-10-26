using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core.Models
{
    public class EmployeeRelativeDTO : CommonFieldsA
    {
        public RelativeTypes Type
        {
            get { return GetValue(() => Type); }
            set { SetValue(() => Type, value); }
        }

        [Required]
        [StringLength(150)]
        public string FullName
        {
            get { return GetValue(() => FullName); }
            set
            {
                SetValue(() => FullName, value);
                SetValue(() => EmployeeContactPersonDescription, value);
            }
        }

        //[Required]
        [StringLength(150)]
        public string Kinship
        {
            get { return GetValue(() => Kinship); }
            set
            {
                SetValue(() => Kinship, value);
                SetValue(() => EmployeeContactPersonDescription, value);
            }
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

        [Required]
        public Sex Sex
        {
            get { return GetValue(() => Sex); }
            set { SetValue(() => Sex, value); }
        }

        [StringLength(50)]
        public string AgeOrBirthDate
        {
            get { return GetValue(() => AgeOrBirthDate); }
            set { SetValue(() => AgeOrBirthDate, value); }
        }

        public DateTime? DateOfBirth
        {
            get { return GetValue(() => DateOfBirth); }
            set { SetValue(() => DateOfBirth, value); }
        }

        public bool EmergencyPersonPhoto
        {
            get { return GetValue(() => EmergencyPersonPhoto); }
            set { SetValue(() => EmergencyPersonPhoto, value); }
        }

        public bool EmergencyPersonIdCard
        {
            get { return GetValue(() => EmergencyPersonIdCard); }
            set { SetValue(() => EmergencyPersonIdCard, value); }
        }

        [ForeignKey("Address")]
        public int? AddressId { get; set; }

        public AddressDTO Address
        {
            get { return GetValue(() => Address); }
            set { SetValue(() => Address, value); }
        }


        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }

        public EmployeeDTO Employee
        {
            get { return GetValue(() => Employee); }
            set { SetValue(() => Employee, value); }
        }

        [StringLength(50)]
        public string Remark
        {
            get { return GetValue(() => Remark); }
            set { SetValue(() => Remark, value); }
        }

        [NotMapped]
        public string EmployeeContactPersonDescription
        {
            get
            {
                var fullName = FullName;
                if (fullName.Length > 15)
                    fullName = fullName.Substring(0, 15) + "...";
                string desc = "" + fullName;
                if (Address != null) //&& !string.IsNullOrWhiteSpace(Address.Mobile)
                    desc = desc + Environment.NewLine + Address.AddressDescription; // +" Mobile: "+ Address.Mobile;
                return desc;
            }
            set { SetValue(() => EmployeeContactPersonDescription, value); }
        }
    }
}