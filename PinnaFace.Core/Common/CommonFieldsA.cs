using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Models;

namespace PinnaFace.Core.Common
{
    public class CommonFieldsA : EntityBase
    {
        [ForeignKey("Agency")]
        public int? AgencyId { get; set; }
        public AgencyDTO Agency
        {
            get { return GetValue(() => Agency); }
            set { SetValue(() => Agency, value); }
        }

        public bool Synced
        {
            get { return GetValue(() => Synced); }
            set { SetValue(() => Synced, value); }
        }

        [NotMapped]
        [DisplayName("S.No.")]
        public int SerialNumber { get; set; }

        [NotMapped]
        [DisplayName("Selected")]
        public bool IsSelected
        {
            get { return GetValue(() => IsSelected); }
            set { SetValue(() => IsSelected, value); }
        }

        [NotMapped]
        [DisplayName("Id Encrypted")]
        public string IdEncrypted
        {
            get
            {
                return EncryptionUtility.Hash64Encode(Id);
            }
            set { SetValue(() => IdEncrypted, value); }
        }
    }

    //public class CommonFieldsA : EntityBase
    //{
    //    [ForeignKey("Agency")]
    //    public int? AgencyId { get; set; }
    //    public AgencyDTO Agency
    //    {
    //        get { return GetValue(() => Agency); }
    //        set { SetValue(() => Agency, value); }
    //    }

    //    public bool Synced
    //    {
    //        get { return GetValue(() => Synced); }
    //        set { SetValue(() => Synced, value); }
    //    }

    //    [NotMapped]
    //    [DisplayName("S.No.")]
    //    public int SerialNumber { get; set; }

    //    [NotMapped]
    //    [DisplayName("Id Encrypted")]
    //    public string IdEncrypted
    //    {
    //        get
    //        {
    //            return EncryptionUtility.Hash64Encode(Id);
    //        }
    //        set { SetValue(() => IdEncrypted, value); }
    //    }
    //}

    public class CommonFieldsA2 : EntityBase
    {
      
        public bool Synced
        {
            get { return GetValue(() => Synced); }
            set { SetValue(() => Synced, value); }
        }

        [NotMapped]
        [DisplayName("S.No.")]
        public int SerialNumber { get; set; }

        [NotMapped]
        [DisplayName("Id Encrypted")]
        public string IdEncrypted
        {
            get
            {
                return EncryptionUtility.Hash64Encode(Id);
            }
            set { SetValue(() => IdEncrypted, value); }
        }
    }
}
