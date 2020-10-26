using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class ForeignAgentViewModel : ViewModelBase
    {
        #region Fields
        private static IForeignAgentService _foreignAgentService;
        private static IAttachmentService _attachmentService;
        private IEnumerable<AgentDTO> _foreignAgentsList;
        private ObservableCollection<AgentDTO> _foreignAgents;
        private AgentDTO _selectedAgent;
        private ICommand _saveForeignAgentViewCommand, _addNewForeignAgentViewCommand, _deleteForeignAgentViewCommand;
        private ObservableCollection<AddressDTO> _foreignAgentAddressDetail;
        private ICommand _foreignAgentAddressViewCommand;
        private bool _editCommandVisibility;
        private AttachmentDTO _headerAttachment, _footerAttachment;
           private ObservableCollection<ListDataItem> _applyCountries;
        private ListDataItem _selectedAppliedCountry;
        #endregion

        #region Constructor
        public ForeignAgentViewModel()
        {
            CleanUp();
            LoadApplyCountries();
            _foreignAgentService = new ForeignAgentService();
            _attachmentService = new AttachmentService();

            GetLiveForeignAgents();
            EditCommandVisibility = false;
        }

        public static void CleanUp()
        {
            if (_foreignAgentService != null)
                _foreignAgentService.Dispose();
            if (_attachmentService != null)
                _attachmentService.Dispose();
        }
        #endregion

        #region Public Properties
        public bool EditCommandVisibility
        {
            get { return _editCommandVisibility; }
            set
            {
                _editCommandVisibility = value;
                RaisePropertyChanged<bool>(() => EditCommandVisibility);
            }
        }
        public AgentDTO SelectedAgent
        {
            get { return _selectedAgent; }
            set
            {
                _selectedAgent = value;
                RaisePropertyChanged<AgentDTO>(() => SelectedAgent);
                if (SelectedAgent != null)
                {
                    //LetterHeadImage = ImageUtil.ToImage(SelectedAgent.Header.AttachedFile);
                    //LetterFootImage = ImageUtil.ToImage(SelectedAgent.Footer.AttachedFile);

                    SelectedAppliedCountry =
                       ApplyCountries.FirstOrDefault(e => e.Value == (int)SelectedAgent.Country);

                    _headerAttachment = _attachmentService.Find(SelectedAgent.HeaderId.ToString());
                    if (_headerAttachment != null)
                        LetterHeadImage = ImageUtil.ToImage(_headerAttachment.AttachedFile);

                    _footerAttachment = _attachmentService.Find(SelectedAgent.FooterId.ToString());
                    if (_footerAttachment != null)
                        LetterFootImage = ImageUtil.ToImage(_footerAttachment.AttachedFile);

                    EditCommandVisibility = true;

                    ForeignAgentAdressDetail = new ObservableCollection<AddressDTO>
                    {
                        SelectedAgent.Address
                    };
                }
                else
                    EditCommandVisibility = false;
                
            }
        }

        public IEnumerable<AgentDTO> ForeignAgentsList
        {
            get { return _foreignAgentsList; }
            set
            {
                _foreignAgentsList = value;
                RaisePropertyChanged<IEnumerable<AgentDTO>>(() => ForeignAgentsList);
            }

        }
        public ObservableCollection<AgentDTO> ForeignAgents
        {
            get { return _foreignAgents; }
            set
            {
                _foreignAgents = value;
                RaisePropertyChanged<ObservableCollection<AgentDTO>>(() => ForeignAgents);

                if (ForeignAgents.Any())
                    SelectedAgent = ForeignAgents.FirstOrDefault();
                else
                    ExecuteAddNewForeignAgentViewCommand();
            }
        }

        public ObservableCollection<AddressDTO> ForeignAgentAdressDetail
        {
            get { return _foreignAgentAddressDetail; }
            set
            {
                _foreignAgentAddressDetail = value;
                RaisePropertyChanged<ObservableCollection<AddressDTO>>(() => ForeignAgentAdressDetail);
            }
        }

                public ObservableCollection<ListDataItem> ApplyCountries
        {
            get { return _applyCountries; }
            set
            {
                _applyCountries = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => ApplyCountries);
            }
        }

        public ListDataItem SelectedAppliedCountry
        {
            get { return _selectedAppliedCountry; }
            set
            {
                _selectedAppliedCountry = value;
                RaisePropertyChanged<ListDataItem>(() => this.SelectedAppliedCountry);

                if (SelectedAppliedCountry != null && SelectedAgent != null)
                {
                    SelectedAgent.Country = (CountryList)SelectedAppliedCountry.Value;
                }
                    
            }
        }

        public void LoadApplyCountries()
        {
            var countries = Singleton.Agency.CountriesOfOpertaion;
            var applyCountry = new List<ListDataItem>();
            var idd = 0;
            foreach (var country in countries)
            {
                applyCountry.Add(new ListDataItem
                {
                    Display = country.ToString(),
                    Value = idd
                });
                idd++;
            }
            ApplyCountries = new ObservableCollection<ListDataItem>(applyCountry);

        }
        #endregion
      
        #region Commands
        public ICommand ForeignAgentAddressViewCommand
        {
            get { return _foreignAgentAddressViewCommand ?? (_foreignAgentAddressViewCommand = new RelayCommand(ForeignAgentAddress)); }
        }
        public void ForeignAgentAddress()
        {
            new AddressEntry(SelectedAgent.Address).ShowDialog();
        }

        public ICommand AddNewForeignAgentViewCommand
        {
            get { return _addNewForeignAgentViewCommand ?? (_addNewForeignAgentViewCommand = new RelayCommand(ExecuteAddNewForeignAgentViewCommand)); }
        }
        private void ExecuteAddNewForeignAgentViewCommand()
        {
            SelectedAgent = new AgentDTO
            {
                Address = new AddressDTO
                {
                    Country = CountryList.SaudiArabia,
                    City = EnumUtil.GetEnumDesc(CityList.Riyadh)
                },
                Header = new AttachmentDTO(),
                Footer = new AttachmentDTO()
            };
        }

        public ICommand SaveForeignAgentViewCommand
        {
            get { return _saveForeignAgentViewCommand ?? (_saveForeignAgentViewCommand = new RelayCommand<Object>(ExecuteSaveForeignAgentViewCommand, CanSave)); }
        }
        private void ExecuteSaveForeignAgentViewCommand(object obj)
        {
            try
            {
                //if (LetterHeadImage.UriSource != null)
                //    SelectedAgent.Header.AttachedFile = ImageUtil.ToBytes(LetterHeadImage);
                //if (LetterFootImage.UriSource != null)
                //    SelectedAgent.Footer.AttachedFile = ImageUtil.ToBytes(LetterFootImage);

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

                if (SelectedAgent != null && _foreignAgentService.InsertOrUpdate(SelectedAgent) == string.Empty)
                    CloseWindow(obj);
                else
                    MessageBox.Show("Got Problem while saving, try again...", "error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ICommand DeleteForeignAgentViewCommand
        {
            get { return _deleteForeignAgentViewCommand ?? (_deleteForeignAgentViewCommand = new RelayCommand(ExecuteDeleteForeignAgentViewCommand)); }
        }
        private void ExecuteDeleteForeignAgentViewCommand()
        {
            try
            {
                if (SelectedAgent.Id == 1)
                    return;//can delete the last agent
                if (MessageBox.Show("Are you Sure You want to Delete this Foreign Agent?", "Delete Foreign Agent",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes)
                    return;

                var ids = DbCommandUtil.QueryCommand("Select ForeignAgentId as Id from Visas " +
                                                     " where Id='" + SelectedAgent.Id + "' and enabled='1'").ToList();
                if (ids.Count == 0)
                {
                     SelectedAgent.Enabled = false;
                    _foreignAgentService.InsertOrUpdate(SelectedAgent);
                }else
                    MessageBox.Show("There may exist Visas Assigned to this agent, you have to update or delete " +
                                    "those Visas related with " + SelectedAgent.AgentName, 
                                    "Can't Delete Agent", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
                MessageBox.Show("There may exist Visas Assigned to this agent, you have to update or delete those Visas related with " + SelectedAgent.AgentName, "Can't Delete Agent", MessageBoxButton.OK, MessageBoxImage.Error);

            }
            GetLiveForeignAgents();
        }

        public void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.DialogResult = true;
            window.Close();
        }
        #endregion

        private void GetLiveForeignAgents()
        {
            ForeignAgentsList = _foreignAgentService.GetAll().ToList();
            var sno = 1;
            foreach (var agentDTO in ForeignAgentsList)
            {
                agentDTO.SerialNumber = sno;
                sno++;
            }
            ForeignAgents = new ObservableCollection<AgentDTO>(ForeignAgentsList);
        }

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
            var file = new OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
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
            var file = new OpenFileDialog { Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg" };
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

        #endregion
    }
}
