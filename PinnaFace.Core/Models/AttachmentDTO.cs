using System.ComponentModel.DataAnnotations;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class AttachmentDTO : CommonFieldsA
    {
        [MaxLength]
        public byte[] AttachedFile
        {
            get { return GetValue(() => AttachedFile); }
            set { SetValue(() => AttachedFile, value); }
        }

        public string AttachmentUrl
        {
            get { return GetValue(() => AttachmentUrl); }
            set { SetValue(() => AttachmentUrl, value); }
        }

        public string Comments
        {
            get { return GetValue(() => Comments); }
            set { SetValue(() => Comments, value); }
        }
    }
}