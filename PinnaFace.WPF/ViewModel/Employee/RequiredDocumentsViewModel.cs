using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Reports;
using PinnaFace.Reports.BioData;
using PinnaFace.Reports.CV;
using PinnaFace.Repository;
using PinnaFace.Service;
using PinnaFace.WPF.Views;
using Telerik.Windows.Controls;
using ViewModelBase = GalaSoft.MvvmLight.ViewModelBase;
using Window = System.Windows.Window;

namespace PinnaFace.WPF.ViewModel
{
    public class RequiredDocumentsViewModel : ViewModelBase
    {
        #region Fields
        private EmployeeDTO _selectedEmployee;
        private RequiredDocumentsDTO _selectedRequiredDocument;
        private int _requiredDocumentId;
        private string _imageVisibility, _attachVisibility, _printVisibility;
        private UnitOfWork _unitOfWork;
        #endregion
            
        #region Constructor
        public RequiredDocumentsViewModel()
        {
            CleanUp();
            UnitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            Messenger.Default.Register<EmployeeDTO>(this, message =>
            {
                SelectedEmployee = message;
            });
        }

        public UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
            set { _unitOfWork = value; }
        }

        public static void CleanUp()
        {

        }
        #endregion

        #region Properties

        public string ImageVisibility
        {
            get { return _imageVisibility; }
            set
            {
                _imageVisibility = value;
                RaisePropertyChanged<string>(() => ImageVisibility);

                if (ImageVisibility != null && ImageVisibility.Equals("Hidden"))
                {
                   
                }
            }
        }

        public string AttachVisibility
        {
            get { return _attachVisibility; }
            set
            {
                _attachVisibility = value;
                RaisePropertyChanged<string>(() => AttachVisibility);
            }
        }

        public string PrintVisibility
        {
            get { return _printVisibility; }
            set
            {
                _printVisibility = value;
                RaisePropertyChanged<string>(() => PrintVisibility);
            }
        }

        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);
                if (SelectedEmployee != null)
                {
                    if (SelectedEmployee.RequiredDocuments == null)
                        SelectedEmployee.RequiredDocuments = new RequiredDocumentsDTO();

                    RequiredDocumentId = SelectedEmployee.RequiredDocuments.Id;

                }
            }
        }

        public int RequiredDocumentId
        {
            get { return _requiredDocumentId; }
            set
            {
                _requiredDocumentId = value;
                RaisePropertyChanged<int>(() => RequiredDocumentId);
                if (RequiredDocumentId != 0)
                {

                    SelectedRequiredDocument = UnitOfWork.Repository<RequiredDocumentsDTO>()
                        .Query()
                        .Include(d => d.AgreementAttachment, d => d.PassportAttachment, d => d.IdCardAttachment, d => d.ContactIdCardAttachment)
                        .Include(d => d.FingerPrintAttachment, d => d.MedicalAttachment, d => d.PreDepartureAttachment, d => d.GradeEightAttachment)
                        .Include(d => d.CocAttachment, d => d.InsuranceAttachment)
                        .Get()
                        .FirstOrDefault(d=>d.Id==RequiredDocumentId);
                }
            }
        }

        public RequiredDocumentsDTO SelectedRequiredDocument
        {
            get { return _selectedRequiredDocument; }
            set
            {
                _selectedRequiredDocument = value;
                RaisePropertyChanged<RequiredDocumentsDTO>(() => SelectedRequiredDocument);
                if (SelectedRequiredDocument != null)
                {
                   

                }
            }
        }

        public RadTabItem SelectedTabItem
        {
            get { return _selectedTabItem; }
            set
            {
                _selectedTabItem = value;
                RaisePropertyChanged<RadTabItem>(() => SelectedTabItem);
                if (SelectedTabItem != null)
                {
                   if (SelectedRequiredDocument == null)
                       return;
                        
                    ImageVisibility = "Hidden";
                    AttachVisibility = "Hidden";
                    PrintVisibility = "Hidden";
          
                    switch (SelectedTabItem.Name)
                    {
                        case "TabAgreement":
                            if (SelectedRequiredDocument.AbroadJobAgreement)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.AgreementAttachment != null)
                                if (SelectedRequiredDocument.AgreementAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.Passport)
                                        ImageVisibility = "Visible";
                                    var path = Environment.CurrentDirectory + "\\Resources\\Images\\ContractSample.png";
                                    DocumentThumbnail = new BitmapImage(new Uri(path, true));
                                    
                                }
                            break;
                        case "TabPassport":
                            if (SelectedRequiredDocument.Passport)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.PassportAttachment != null)
                                if (SelectedRequiredDocument.PassportAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.Passport)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.PassportAttachment.AttachedFile);
                                }
                            break;
                        case "TabIdCard":
                            if (SelectedRequiredDocument.IdCard)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.IdCardAttachment != null)
                                if (SelectedRequiredDocument.IdCardAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.IdCard)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.IdCardAttachment.AttachedFile);
                                }
                            break;
                        case "TabContactIdCard":
                            if (SelectedRequiredDocument.EmergencyPersonIdCard)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.ContactIdCardAttachment != null)
                                if (SelectedRequiredDocument.ContactIdCardAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.EmergencyPersonIdCard)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.ContactIdCardAttachment.AttachedFile);
                                }
                            break;
                        case "TabFingerPrint":
                            if (SelectedRequiredDocument.Fingerprint)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.FingerPrintAttachment != null)
                                if (SelectedRequiredDocument.FingerPrintAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.Fingerprint)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.FingerPrintAttachment.AttachedFile);
                                }
                            break;
                        case "TabMedical":
                            if (SelectedRequiredDocument.Medical)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.MedicalAttachment != null)
                                if (SelectedRequiredDocument.MedicalAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.Medical)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.MedicalAttachment.AttachedFile);
                                }
                            break;
                        case "TabPreDeparture":
                            if (SelectedRequiredDocument.TripOrientation)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.PreDepartureAttachment != null)
                                if (SelectedRequiredDocument.PreDepartureAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.TripOrientation)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.PreDepartureAttachment.AttachedFile);
                                }
                            break;
                        case "TabGradeEight":
                            if (SelectedRequiredDocument.Grade8Certificate)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.GradeEightAttachment != null)
                                if (SelectedRequiredDocument.GradeEightAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.Grade8Certificate)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.GradeEightAttachment.AttachedFile);
                                }
                            break;
                        case "TabCoc":
                            if (SelectedRequiredDocument.CocCertificate)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.CocAttachment != null)
                                if (SelectedRequiredDocument.CocAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.CocCertificate)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.CocAttachment.AttachedFile);
                                }
                            break;
                        case "TabInsurance":
                            if (SelectedRequiredDocument.Insurance)
                            {
                                AttachVisibility = "Visible";
                                PrintVisibility = "Visible";
                            }
                            if (SelectedRequiredDocument.InsuranceAttachment != null)
                                if (SelectedRequiredDocument.InsuranceAttachment.AttachedFile != null)
                                {
                                    if (SelectedRequiredDocument.Insurance)
                                    ImageVisibility = "Visible";
                                    DocumentThumbnail = ImageUtil.ToImage(SelectedRequiredDocument.InsuranceAttachment.AttachedFile);
                                }
                            break;
                    }
                }
             
            }
        }


        #endregion
        
        #region Commands

        private ICommand _saveApplicationCommand, _closeApplicationCommand;

        public ICommand SaveRequiredDocumentsCommand
        {
            get
            {
                return _saveApplicationCommand ?? (_saveApplicationCommand = new RelayCommand<object>(ExcuteSaveRequiredDocumentsCommand, CanSave));
            }
        }
        private void ExcuteSaveRequiredDocumentsCommand(object obj)
        {
            try
            {
                UnitOfWork.Dispose();
                SelectedEmployee.RequiredDocuments.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedEmployee.RequiredDocuments.DateLastModified = DateTime.Now;
                CloseWindow(obj);
            }

            catch
            {
                MessageBox.Show("Can't Save RequiredDocuments!");
            }
        }

        public ICommand CloseEmployeeApplicationCommand
        {
            get
            {
                return _closeApplicationCommand ?? (_closeApplicationCommand = new RelayCommand<object>(CloseWindow));
            }
        }
        public void CloseWindow(object obj)
        {
            if (obj != null)
            {
                UnitOfWork.Dispose();
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }
        
        #endregion

        #region Attachments
        private BitmapImage _documentThumbnail;
        public BitmapImage DocumentThumbnail
        {
            get { return _documentThumbnail; }
            set
            {
                _documentThumbnail = value;
                RaisePropertyChanged<BitmapImage>(() => DocumentThumbnail);
            }
        }

        private ICommand _attachDocumentCommand, _viewDocumentCommand, _deleteDocumentCommand;
        private RadTabItem _selectedTabItem;

        public ICommand AttachDocumentCommand
        {
            get
            {
                return _attachDocumentCommand ?? (_attachDocumentCommand = new RelayCommand<object>(AttachDocumentsCommand, CanSave));
            }
        }
        private void AttachDocumentsCommand(object obj)
        {
            try
            {
                //var chkBox = obj as System.Windows.Controls.CheckBox;
                //if (chkBox == null)
                //    return;

                var file = new OpenFileDialog {Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg"};
                if (SelectedTabItem.Name == "TabAgreement")
                file = new OpenFileDialog { Filter = "PDF Files(*.pdf;)|*.pdf;" };
                bool? result = file.ShowDialog();
                if (result != null && ((bool) result && File.Exists(file.FileName)))
                {
                    byte[] attachedFile = null;
                    if (SelectedTabItem.Name == "TabAgreement")
                    {
                        attachedFile = File.ReadAllBytes(file.FileName);
                    }
                    else
                    {
                         var fileName = ImageResizingUtil.ResizeImages(file.FileName);
                         attachedFile =  ImageUtil.ToBytes(new BitmapImage(new Uri(fileName, true)));//file.FileName
                    }
                    
                     

                   var attachment = new AttachmentDTO
                   {
                       AgencyId = Singleton.Agency.Id,
                       AttachedFile = attachedFile,
                       ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1,
                       DateLastModified = DateTime.Now
                   };


                   var name = SelectedTabItem.Name;// chkBox.Name;
                   switch (name)
                   {
                       case "TabAgreement":
                               SelectedRequiredDocument.AgreementAttachment = attachment;
                           break;
                       case "TabPassport":
                               SelectedRequiredDocument.PassportAttachment = attachment;
                           break;
                       case "TabIdCard":
                           SelectedRequiredDocument.IdCardAttachment = attachment;
                           break;
                       case "TabContactIdCard":
                           SelectedRequiredDocument.ContactIdCardAttachment = attachment;
                           break;
                       case "TabFingerPrint":
                           SelectedRequiredDocument.FingerPrintAttachment = attachment;
                           break;
                       case "TabMedical":
                           SelectedRequiredDocument.MedicalAttachment = attachment;
                           break;
                       case "TabPreDeparture":
                           SelectedRequiredDocument.PreDepartureAttachment = attachment;
                           break;
                       case "TabGradeEight":
                           SelectedRequiredDocument.GradeEightAttachment = attachment;
                           break;
                       case "TabCoc":
                           SelectedRequiredDocument.CocAttachment = attachment;
                           break;
                       case "TabInsurance":
                           SelectedRequiredDocument.InsuranceAttachment = attachment;
                           break;
                   }

                   UnitOfWork.Repository<RequiredDocumentsDTO>().InsertUpdate(SelectedRequiredDocument);
                   UnitOfWork.Commit();

 
                    ImageVisibility = "Visible";
                    if (SelectedTabItem.Name == "TabAgreement")
                    {
                        var path = Environment.CurrentDirectory + "\\Resources\\Images\\ContractSample.png";
                        DocumentThumbnail = new BitmapImage(new Uri(path, true));
                        
                    }
                    else
                    {
                        DocumentThumbnail = ImageUtil.ToImage(attachment.AttachedFile);
                    }
                }

            }

            catch
            {
                MessageBox.Show("Can't Save RequiredDocuments!");
            }
        }

        public ICommand ViewDocumentCommand
        {
            get
            {
                return _viewDocumentCommand ?? (_viewDocumentCommand = new RelayCommand<object>(ExcuteViewDocumentCommand));
            }
        }
        public void ExcuteViewDocumentCommand(object obj)
        {
            try
            {
                AttachmentDTO attachment = null;
                switch (SelectedTabItem.Name)
                {
                    case "TabAgreement":
                        attachment = SelectedRequiredDocument.AgreementAttachment;
                        break;
                    case "TabPassport":
                        attachment = SelectedRequiredDocument.PassportAttachment;
                        break;
                    case "TabIdCard":
                        attachment = SelectedRequiredDocument.IdCardAttachment;
                        break;
                    case "TabContactIdCard":
                        attachment = SelectedRequiredDocument.ContactIdCardAttachment;
                        break;
                    case "TabFingerPrint":
                        attachment = SelectedRequiredDocument.FingerPrintAttachment;
                        break;
                    case "TabMedical":
                        attachment = SelectedRequiredDocument.MedicalAttachment;
                        break;
                    case "TabPreDeparture":
                        attachment = SelectedRequiredDocument.PreDepartureAttachment;
                        break;
                    case "TabGradeEight":
                        attachment = SelectedRequiredDocument.GradeEightAttachment;
                        break;
                    case "TabCoc":
                        attachment = SelectedRequiredDocument.CocAttachment;
                        break;
                    case "TabInsurance":
                        attachment = SelectedRequiredDocument.InsuranceAttachment;
                        break;
                }

                if (attachment != null && attachment.AttachedFile != null)
                {
                    if (SelectedTabItem.Name == "TabAgreement")
                    {
                        var pdfFilePath = Environment.SpecialFolder.MyDocuments + attachment.Id + ".pdf";
                        System.IO.File.WriteAllBytes(pdfFilePath, attachment.AttachedFile);
                        System.Diagnostics.Process.Start(pdfFilePath);
                    }
                    else
                    {
                    var myDataSet = new ReportsDataSet();

                    myDataSet.LetterHeads.Rows.Add("1", attachment.AttachedFile, null, null, "", "");
                
                    var myReport = new AttachedDocument();
                    myReport.SetDataSource(myDataSet);

                    var report = new ReportViewerCommon(myReport);
                    report.ShowDialog();
                    }


                }
              
            }
            catch
            {
            }
        }

        public ICommand DeleteDocumentCommand
        {
            get
            {
                return _deleteDocumentCommand ?? (_deleteDocumentCommand = new RelayCommand<object>(ExcuteDeleteDocumentCommand));
            }
        }
        public void ExcuteDeleteDocumentCommand(object obj)
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want to delete the document?", "Delete Document",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.No);
                if(result==MessageBoxResult.Cancel)
                    return;
                switch (SelectedTabItem.Name)
                {
                    case "TabAgreement":
                        SelectedRequiredDocument.AgreementAttachment = null;
                        break;
                    case "TabPassport":
                        SelectedRequiredDocument.PassportAttachment = null;
                        break;
                    case "TabIdCard":
                        SelectedRequiredDocument.IdCardAttachment = null;
                        break;
                    case "TabContactIdCard":
                        SelectedRequiredDocument.ContactIdCardAttachment = null;
                        break;
                    case "TabFingerPrint":
                        SelectedRequiredDocument.FingerPrintAttachment = null;
                        break;
                    case "TabMedical":
                        SelectedRequiredDocument.MedicalAttachment = null;
                        break;
                    case "TabPreDeparture":
                        SelectedRequiredDocument.PreDepartureAttachment = null;
                        break;
                    case "TabGradeEight":
                        SelectedRequiredDocument.GradeEightAttachment = null;
                        break;
                    case "TabCoc":
                        SelectedRequiredDocument.CocAttachment = null;
                        break;
                    case "TabInsurance":
                        SelectedRequiredDocument.InsuranceAttachment = null;
                        break;
                }

                UnitOfWork.Repository<RequiredDocumentsDTO>().InsertUpdate(SelectedRequiredDocument);
                UnitOfWork.Commit();

                ImageVisibility = "Hidden";
            }
            catch
            {
            }
        }

        #endregion
        
        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;
        }
        #endregion
    }
}