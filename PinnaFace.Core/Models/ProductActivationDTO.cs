using System;
using System.ComponentModel.DataAnnotations;
using PinnaFace.Core.Common;
using System.Management;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Enumerations;

namespace PinnaFace.Core.Models
{
    public class ProductActivationDTO : CommonFieldsA
    {
        public ProductActivationDTO()
        {
            BiosSn = Get_BIOS_SN();
        }

        //[ForeignKey("Agency")]
        //public int? AgencyId { get; set; }
        //public AgencyDTO Agency
        //{
        //    get { return GetValue(() => Agency); }
        //    set { SetValue(() => Agency, value); }
        //}

        [Required]
        [StringLength(150)]
        public string ProductKey
        {
            get { return GetValue(() => ProductKey); }
            set { SetValue(() => ProductKey, value); }
        }

        [Required]
        public KeyStatus KeyStatus
        {
            get { return GetValue(() => KeyStatus); }
            set { SetValue(() => KeyStatus, value); }
        }

        [Required]
        [StringLength(150)]
        public string LicensedTo
        {
            get { return GetValue(() => LicensedTo); }
            set { SetValue(() => LicensedTo, value); }
        }

        public int DatabaseVersionDate
        {
            get { return GetValue(() => DatabaseVersionDate); }
            set { SetValue(() => DatabaseVersionDate, value); }
        }

        public int MaximumSystemVersion
        {
            get { return GetValue(() => MaximumSystemVersion); }
            set { SetValue(() => MaximumSystemVersion, value); }
        }

        //[Required]
        //public int WarehouseId
        //{
        //    get { return GetValue(() => WarehouseId); }
        //    set { SetValue(() => WarehouseId, value); }
        //}

        public PinnaFaceEdition Edition
        {
            get { return GetValue(() => Edition); }
            set { SetValue(() => Edition, value); }
        }

        [Required]
        public DateTime FirstActivatedDate
        {
            get { return GetValue(() => FirstActivatedDate); }
            set { SetValue(() => FirstActivatedDate, value); }
        }

        [Required]
        public DateTime ExpiryDate
        {
            get { return GetValue(() => ExpiryDate); }
            set { SetValue(() => ExpiryDate, value); }
        }

        public DateTime? LastRenewedDate
        {
            get { return GetValue(() => LastRenewedDate); }
            set { SetValue(() => LastRenewedDate, value); }
        }

        [Required]
        [StringLength(150)]
        public string RegisteredBiosSn
        {
            get { return GetValue(() => RegisteredBiosSn); }
            set { SetValue(() => RegisteredBiosSn, value); }
        }

        [Required]
        [NotMapped]
        public string BiosSn
        {
            get { return GetValue(() => BiosSn); }
            set { SetValue(() => BiosSn, value); }
        }

        /// <summary>
        /// It discovers the BIOS serial number
        /// </summary>
        public static string Get_BIOS_SN()
        {
            var biossn = string.Empty;

            //return Environment.MachineName;
            var searcher = new ManagementObjectSearcher("select SerialNumber from WIN32_BIOS");
            var result = searcher.Get();

            foreach (var o in result)
            {
                var obj = (ManagementObject)o;
                if (obj["SerialNumber"] != null)
                    biossn = obj["SerialNumber"].ToString();
            }

            result.Dispose();
            searcher.Dispose();

            return biossn;

        }

        #region User Accounts
        //[NotMapped]
        public string SuperName
        {
            get { return GetValue(() => SuperName); }
            set { SetValue(() => SuperName, value); }
        }
        //[NotMapped]
        public string SuperPass
        {
            get { return GetValue(() => SuperPass); }
            set { SetValue(() => SuperPass, value); }
        }
        //[NotMapped]
        public string AdminName
        {
            get { return GetValue(() => AdminName); }
            set { SetValue(() => AdminName, value); }
        }
        //[NotMapped]
        public string AdminPass
        {
            get { return GetValue(() => AdminPass); }
            set { SetValue(() => AdminPass, value); }
        }
        //[NotMapped]
        public string User1Name
        {
            get { return GetValue(() => User1Name); }
            set { SetValue(() => User1Name, value); }
        }
        //[NotMapped]
        public string User1Pass
        {
            get { return GetValue(() => User1Pass); }
            set { SetValue(() => User1Pass, value); }
        }
        #endregion

        [NotMapped]
        public string ActivatedDateString
        {
            get
            {
                return FirstActivatedDate.ToString("dd-MMM-yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(FirstActivatedDate, "-") + ")";
            }
            set { SetValue(() => ActivatedDateString, value); }
        }

        [NotMapped]
        public string ExpiryDateString
        {
            get
            {
                return ExpiryDate.ToString("dd-MMM-yyyy") + " (" + CalendarUtil.GetEthCalendarFormated(ExpiryDate, "-") + ")";
            }
            set { SetValue(() => ExpiryDateString, value); }
        }

        [NotMapped]
        public string DaysLeft
        {
            get
            {
                return ExpiryDate.Subtract(DateTime.Now).Days.ToString("N0");
                
            }
            set { SetValue(() => DaysLeft, value); }
        }

        [NotMapped]
        public string ProductActivationDetail
        {
            get
            {
                if (Agency != null)
                    return Agency.AgencyName + " (" + Agency.Id.ToString() + ")";
                return "";
            }
            set { SetValue(() => ProductActivationDetail, value); }
        }
    }   
}
