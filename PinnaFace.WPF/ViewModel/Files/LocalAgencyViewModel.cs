using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PinnaFace.Core;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class LocalAgencyViewModel : ViewModelBase
    {
        #region Fields
        private static ILocalAgencyService _localAgencyService;
        private static IAttachmentService _attachmentService;
        private AgencyDTO _selectedAgency;
        private ObservableCollection<AddressDTO> _localAgencyAddressDetail;
        private ICommand _saveLocalAgencyViewCommand, _localAgencyAddressViewCommand;
        private AttachmentDTO _headerAttachment, _footerAttachment;
        #endregion

        #region Constructor

        public LocalAgencyViewModel()
        {
            CleanUp();
            _localAgencyService = new LocalAgencyService();
            _attachmentService = new AttachmentService();
            SelectedAgency = _localAgencyService.GetLocalAgency();
            //?? new AgencyDTO
            //{
            //    SaudiOperation = true,
            //    DepositAmount = "30,000 USD",
            //    Managertype = "ዋና ስራ አስኪያጅ",
            //    Address = new AddressDTO
            //    {
            //        Country = CountryList.Ethiopia,
            //        City = EnumUtil.GetEnumDesc(CityList.AddisAbeba)
            //    }
            //};
        }

        public static void CleanUp()
        {
            if (_localAgencyService != null)
                _localAgencyService.Dispose();
            if (_attachmentService != null)
                _attachmentService.Dispose();
        }

        #endregion

        #region Properties

        public AgencyDTO SelectedAgency
        {
            get { return _selectedAgency; }
            set
            {
                _selectedAgency = value;
                RaisePropertyChanged<AgencyDTO>(() => SelectedAgency);
                if (SelectedAgency != null)
                {
                    //LetterHeadImage = ImageUtil.ToImage(SelectedAgency.Header.AttachedFile);
                    //LetterFootImage = ImageUtil.ToImage(SelectedAgency.Footer.AttachedFile);

                    _headerAttachment = _attachmentService.Find(SelectedAgency.HeaderId.ToString());
                    if (_headerAttachment != null)
                        LetterHeadImage = ImageUtil.ToImage(_headerAttachment.AttachedFile);

                    _footerAttachment = _attachmentService.Find(SelectedAgency.FooterId.ToString());
                    if (_footerAttachment != null)
                        LetterFootImage = ImageUtil.ToImage(_footerAttachment.AttachedFile);


                    LocalAgencyAdressDetail = new ObservableCollection<AddressDTO>
                    {
                        SelectedAgency.Address
                    };
                }
            }
        }
        public ObservableCollection<AddressDTO> LocalAgencyAdressDetail
        {
            get { return _localAgencyAddressDetail; }
            set
            {
                _localAgencyAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => LocalAgencyAdressDetail);
            }
        }

        #endregion

        #region Commands

        public ICommand LocalAgencyAddressViewCommand
        {
            get { return _localAgencyAddressViewCommand ?? (_localAgencyAddressViewCommand = new RelayCommand(LocalAgencyAddress)); }
        }
        public void LocalAgencyAddress()
        {
            new AddressEntry(SelectedAgency.Address).ShowDialog();
        }

        public ICommand SaveLocalAgencyViewCommand
        {
            get { return _saveLocalAgencyViewCommand ?? (_saveLocalAgencyViewCommand = new RelayCommand<Object>(ExecuteSaveLocalAgencyViewCommand, CanSave)); }
        }
        private void ExecuteSaveLocalAgencyViewCommand(object obj)
        {
            try
            {
                //if (LetterHeadImage.UriSource != null)
                //    SelectedAgency.Header.AttachedFile = ImageUtil.ToBytes(LetterHeadImage);
                //if (LetterFootImage.UriSource != null)
                //    SelectedAgency.Footer.AttachedFile = ImageUtil.ToBytes(LetterFootImage);

                if (LetterHeadImage != null && LetterHeadImage.UriSource != null)
                {
                    _headerAttachment.AttachedFile = ImageUtil.ToBytes(LetterHeadImage);
                    _headerAttachment.RowGuid = Guid.NewGuid();
                    _attachmentService.InsertOrUpdate(_headerAttachment);
                }

                if (LetterFootImage != null && LetterFootImage.UriSource != null)
                {
                    _footerAttachment.AttachedFile = ImageUtil.ToBytes(LetterFootImage);
                    _footerAttachment.RowGuid = Guid.NewGuid();
                    _attachmentService.InsertOrUpdate(_footerAttachment);
                }

                if (SelectedAgency != null && _localAgencyService.InsertOrUpdate(SelectedAgency) == string.Empty)
                    CloseWindow(obj);
                else
                    MessageBox.Show("Got Problem while saving, try again...", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.InnerException.Message + Environment.NewLine + exception.Message, "error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }
        #endregion

        #region Letter Head
        private BitmapImage _letterHeadImage, _letterFootImage;
        private ICommand _showLetterHeaderImageCommand, _showLetterFooterImageCommand;

        public BitmapImage LetterHeadImage
        {
            get { return _letterHeadImage; }
            set
            {
                _letterHeadImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterHeadImage);
            }
        }
        public ICommand ShowLetterHeaderImageCommand
        {
            get { return _showLetterHeaderImageCommand ?? (_showLetterHeaderImageCommand = new RelayCommand(ExecuteShowLetterHeaderImageViewCommand)); }
        }
        private void ExecuteShowLetterHeaderImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterHeadImage = new BitmapImage(new Uri(file.FileName, true));
            }
        }

        public BitmapImage LetterFootImage
        {
            get { return _letterFootImage; }
            set
            {
                _letterFootImage = value;
                RaisePropertyChanged<BitmapImage>(() => LetterFootImage);
            }
        }
        public ICommand ShowLetterFooterImageCommand
        {
            get { return _showLetterFooterImageCommand ?? (_showLetterFooterImageCommand = new RelayCommand(ExecuteShowLetterFooterImageViewCommand)); }
        }
        private void ExecuteShowLetterFooterImageViewCommand()
        {
            var file = new Microsoft.Win32.OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
            var result = file.ShowDialog();
            if (result != null && ((bool)result && File.Exists(file.FileName)))
            {
                LetterFootImage = new BitmapImage(new Uri(file.FileName, true));
            }
        }
        #endregion

        #region Validation
        public static int Errors { get; set; }
        public bool CanSave(object obj)
        {
            return Errors == 0;

        }

        public static int LineErrors { get; set; }
        public bool CanSaveLine()
        {
            return LineErrors == 0;

        }
        #endregion
    }

}
