using System.ComponentModel.DataAnnotations.Schema;
using PinnaFace.Core.Common;

namespace PinnaFace.Core.Models
{
    public class RequiredDocumentsDTO : CommonFieldsA
    {
        //public bool LocalJobAgreement
        //{
        //    get { return GetValue(() => LocalJobAgreement); }
        //    set { SetValue(() => LocalJobAgreement, value); }
        //}
   
        public bool AbroadJobAgreement
        {
            get { return GetValue(() => AbroadJobAgreement); }
            set { SetValue(() => AbroadJobAgreement, value); }
        }

        //public bool Visa
        //{
        //    get { return GetValue(() => Visa); }
        //    set { SetValue(() => Visa, value); }
        //}

        public bool Passport
        {
            get { return GetValue(() => Passport); }
            set { SetValue(() => Passport, value); }
        }

        public bool IdCard
        {
            get { return GetValue(() => IdCard); }
            set { SetValue(() => IdCard, value); }
        }

        public bool Photo
        {
            get { return GetValue(() => Photo); }
            set { SetValue(() => Photo, value); }
        }

        public bool EmergencyPersonIdCard
        {
            get { return GetValue(() => EmergencyPersonIdCard); }
            set { SetValue(() => EmergencyPersonIdCard, value); }
        }

        public bool Fingerprint
        {
            get { return GetValue(() => Fingerprint); }
            set { SetValue(() => Fingerprint, value); }
        }

        public bool Medical
        {
            get { return GetValue(() => Medical); }
            set { SetValue(() => Medical, value); }
        }
        
        public bool TripOrientation
        {
            get { return GetValue(() => TripOrientation); }
            set { SetValue(() => TripOrientation, value); }
        }

        public bool Grade8Certificate
        {
            get { return GetValue(() => Grade8Certificate); }
            set { SetValue(() => Grade8Certificate, value); }
        }
        
        public bool CocCertificate
        {
            get { return GetValue(() => CocCertificate); }
            set { SetValue(() => CocCertificate, value); }
        }

        //public bool BioDateForm
        //{
        //    get { return GetValue(() => BioDateForm); }
        //    set { SetValue(() => BioDateForm, value); }
        //}

        public bool Insurance
        {
            get { return GetValue(() => Insurance); }
            set { SetValue(() => Insurance, value); }
        }

        //public bool ServicePayment
        //{
        //    get { return GetValue(() => ServicePayment); }
        //    set { SetValue(() => ServicePayment, value); }
        //}

        #region Attachments
        [ForeignKey("AgreementAttachment")]
        public int? AgreementAttachmentId { get; set; }
        public AttachmentDTO AgreementAttachment
        {
            get { return GetValue(() => AgreementAttachment); }
            set { SetValue(() => AgreementAttachment, value); }
        }

        [ForeignKey("PassportAttachment")]
        public int? PassportAttachmentId { get; set; }
        public AttachmentDTO PassportAttachment
        {
            get { return GetValue(() => PassportAttachment); }
            set { SetValue(() => PassportAttachment, value); }
        }

        [ForeignKey("IdCardAttachment")]
        public int? IdCardAttachmentId { get; set; }
        public AttachmentDTO IdCardAttachment
        {
            get { return GetValue(() => IdCardAttachment); }
            set { SetValue(() => IdCardAttachment, value); }
        }

        [ForeignKey("ContactIdCardAttachment")]
        public int? ContactIdCardAttachmentId { get; set; }
        public AttachmentDTO ContactIdCardAttachment
        {
            get { return GetValue(() => ContactIdCardAttachment); }
            set { SetValue(() => ContactIdCardAttachment, value); }
        }

        [ForeignKey("FingerPrintAttachment")]
        public int? FingerPrintAttachmentId { get; set; }
        public AttachmentDTO FingerPrintAttachment
        {
            get { return GetValue(() => FingerPrintAttachment); }
            set { SetValue(() => FingerPrintAttachment, value); }
        }

        [ForeignKey("MedicalAttachment")]
        public int? MedicalAttachmentId { get; set; }
        public AttachmentDTO MedicalAttachment
        {
            get { return GetValue(() => MedicalAttachment); }
            set { SetValue(() => MedicalAttachment, value); }
        }

        [ForeignKey("PreDepartureAttachment")]
        public int? PreDepartureAttachmentId { get; set; }
        public AttachmentDTO PreDepartureAttachment
        {
            get { return GetValue(() => PreDepartureAttachment); }
            set { SetValue(() => PreDepartureAttachment, value); }
        }

        [ForeignKey("GradeEightAttachment")]
        public int? GradeEightAttachmentId { get; set; }
        public AttachmentDTO GradeEightAttachment
        {
            get { return GetValue(() => GradeEightAttachment); }
            set { SetValue(() => GradeEightAttachment, value); }
        }

        [ForeignKey("CocAttachment")]
        public int? CocAttachmentId { get; set; }
        public AttachmentDTO CocAttachment
        {
            get { return GetValue(() => CocAttachment); }
            set { SetValue(() => CocAttachment, value); }
        }

        [ForeignKey("InsuranceAttachment")]
        public int? InsuranceAttachmentId { get; set; }
        public AttachmentDTO InsuranceAttachment
        {
            get { return GetValue(() => InsuranceAttachment); }
            set { SetValue(() => InsuranceAttachment, value); }
        }


        #endregion


        [NotMapped]
        public string DocumentsCompletion
        {
            get
            {//!LocalJobAgreement || !Visa || !BioDateForm || !ServicePayment||
                if (!AbroadJobAgreement || !Passport || !IdCard || !Photo ||
                    !EmergencyPersonIdCard || !Fingerprint || !Medical || 
                    !TripOrientation || !Grade8Certificate || !CocCertificate ||
                    !Insurance )
                    return "Not Completed";
                return "Completed";
            }

        }
    }
}