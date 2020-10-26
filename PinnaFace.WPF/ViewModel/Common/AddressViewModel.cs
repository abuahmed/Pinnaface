using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class AddressViewModel : ViewModelBase
    {
        #region Fields

        private AddressTypes _addressType;
        private ICommand _closeAddressViewCommand;
        private string _headerText;
        private ICommand _resetAddressViewCommand;
        private ICommand _saveAddressViewCommand;
        private AddressDTO _selectedAddress;
        private int _windowHeight;

        #endregion

        #region Constructor

        public AddressViewModel()
        {
            //WindowHeight = 470;
            Messenger.Default.Register<AddressDTO>(this, message => { SelectedAddress = message; });
        }

        #endregion

        #region Public Properties

        public AddressDTO SelectedAddress
        {
            get { return _selectedAddress; }
            set
            {
                _selectedAddress = value;
                RaisePropertyChanged(() => SelectedAddress);
                if (SelectedAddress != null)
                {
                    _addressType = SelectedAddress.AddressType;

                    if (_addressType == AddressTypes.Local)
                    {
                        LocalOnlyFieldIsEnabled = true;
                        ForeignOnlyFieldIsEnabled = false;
                        //WindowHeight = 300;
                    }
                    else
                    {
                        LocalOnlyFieldIsEnabled = false;
                        ForeignOnlyFieldIsEnabled = true;
                        //WindowHeight = 470;
                    }
                }
            }
        }

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged(() => HeaderText);
            }
        }

        public int WindowHeight
        {
            get { return _windowHeight; }
            set
            {
                _windowHeight = value;
                RaisePropertyChanged(() => WindowHeight);
            }
        }

        #endregion

        #region Commands

        public ICommand SaveAddressViewCommand
        {
            get
            {
                return _saveAddressViewCommand ??
                       (_saveAddressViewCommand = new RelayCommand<Object>(SaveAddress, CanSave));
            }
        }

        public ICommand ResetAddressViewCommand
        {
            get { return _resetAddressViewCommand ?? (_resetAddressViewCommand = new RelayCommand(ResetAddress)); }
        }

        public ICommand CloseAddressViewCommand
        {
            get
            {
                return _closeAddressViewCommand ?? (_closeAddressViewCommand = new RelayCommand<Object>(CloseWindow));
            }
        }

        private void SaveAddress(object obj)
        {
            try
            {
                SelectedAddress.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedAddress.DateLastModified = DateTime.Now;
                CloseWindow(obj);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't save"
                                + Environment.NewLine + exception.Message, "Can't save", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ResetAddress()
        {
            SelectedAddress = new AddressDTO
            {
                AddressType = _addressType
            };

            if (_addressType == AddressTypes.Local)
            {
                SelectedAddress.Country = CountryList.Ethiopia;
                SelectedAddress.City = EnumUtil.GetEnumDesc(CityList.AddisAbeba);
            }
            else
            {
                SelectedAddress.Country = CountryList.SaudiArabia;
                SelectedAddress.City = EnumUtil.GetEnumDesc(CityList.Riyadh);
            }
        }

        private void CloseWindow(object obj)
        {
            if (obj != null)
            {
                var window = obj as Window;
                if (window != null)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
        }

        #endregion
        
        #region Open List Commands

        private ICommand _cityListAmharicViewCommand;
        private ICommand _cityListEnglishViewCommand;
        private bool _foreignOnlyFieldIsEnabled;
        private bool _localOnlyFieldIsEnabled;
        private ICommand _subCityListViewCommand;


        public ICommand CityListEnglishViewCommand
        {
            get
            {
                return _cityListEnglishViewCommand ??
                       (_cityListEnglishViewCommand = new RelayCommand(ExcuteCityListEnglishViewCommand));
            }
        }

        public ICommand CityListAmharicViewCommand
        {
            get
            {
                return _cityListAmharicViewCommand ??
                       (_cityListAmharicViewCommand = new RelayCommand(ExcuteCityListAmharicViewCommand));
            }
        }

        public ICommand SubCityListViewCommand
        {
            get
            {
                return _subCityListViewCommand ??
                       (_subCityListViewCommand = new RelayCommand(ExcuteSubCityListViewCommand));
            }
        }

        public bool LocalOnlyFieldIsEnabled
        {
            get { return _localOnlyFieldIsEnabled; }
            set
            {
                _localOnlyFieldIsEnabled = value;
                RaisePropertyChanged(() => LocalOnlyFieldIsEnabled);
            }
        }

        public bool ForeignOnlyFieldIsEnabled
        {
            get { return _foreignOnlyFieldIsEnabled; }
            set
            {
                _foreignOnlyFieldIsEnabled = value;
                RaisePropertyChanged(() => ForeignOnlyFieldIsEnabled);
            }
        }

        public void ExcuteCityListEnglishViewCommand()
        {
            var listWindow = new Lists(ListTypes.City);
            if (_addressType == AddressTypes.Local)
                listWindow = new Lists(ListTypes.LocalCity);

            listWindow.ShowDialog();
            if (listWindow.DialogResult != null && (bool) listWindow.DialogResult)
            {
                SelectedAddress.City = listWindow.TxtDisplayName.Text;
            }
        }

        public void ExcuteCityListAmharicViewCommand()
        {
            var listWindow = new Lists(ListTypes.CityAmharic);
            listWindow.ShowDialog();
            if (listWindow.DialogResult != null && (bool) listWindow.DialogResult)
            {
                SelectedAddress.CityAmharic = listWindow.TxtDisplayName.Text;
            }
        }

        public void ExcuteSubCityListViewCommand()
        {
            var listWindow = new Lists(ListTypes.SubCity);
            listWindow.ShowDialog();
            if (listWindow.DialogResult != null && (bool) listWindow.DialogResult)
            {
                SelectedAddress.SubCity = listWindow.TxtDisplayName.Text;
            }
        }

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave(object parameter)
        {
            return Errors == 0;
        }

        #endregion
    }
}