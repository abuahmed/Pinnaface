using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.WPF.Views;

namespace PinnaFace.WPF.ViewModel
{
    public class VisaConditionViewModel : ViewModelBase
    {
        #region Fields
        private VisaConditionDTO _selectedVisaCondition;
        private ICommand _saveVisaConditionViewCommand;
        #endregion

        #region Constructor

        public VisaConditionViewModel()
        {
            LoadLists();
            Messenger.Default.Register<VisaConditionDTO>(this, message =>
            {
                SelectedVisaCondition = message;
            });
        }

        #endregion

        #region Properties
        public VisaConditionDTO SelectedVisaCondition
        {
            get { return _selectedVisaCondition; }
            set
            {
                _selectedVisaCondition = value;
                RaisePropertyChanged<VisaConditionDTO>(() => SelectedVisaCondition);
                if (SelectedVisaCondition != null)
                {
                    SelectedProfession = Professions.FirstOrDefault(c => c.Display.Equals(SelectedVisaCondition.Profession));
                    SelectedProfessionAmharic = ProfessionsAmharic.FirstOrDefault(c => c.Display.Equals(SelectedVisaCondition.ProfessionAmharic));
                }
            }
        }

        #endregion

        #region Commands
        public ICommand SaveVisaConditionViewCommand
        {
            get { return _saveVisaConditionViewCommand ?? (_saveVisaConditionViewCommand = new RelayCommand<Object>(ExecuteSaveVisaConditionViewCommand, CanSave)); }
        }
        private void ExecuteSaveVisaConditionViewCommand(object obj)
        {
            try
            {
                SelectedVisaCondition.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                SelectedVisaCondition.DateLastModified = DateTime.Now;
                CloseWindow(obj);
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


        #region Open List Commands
        private ICommand _professionListAmharicViewCommand, _professionListViewCommand;

        public ICommand ProfessionListAmharicViewCommand
        {
            get
            {
                return _professionListAmharicViewCommand ??
                       (_professionListAmharicViewCommand = new RelayCommand(ExcuteProfessionListAmharicViewCommand));
            }
        }
        public void ExcuteProfessionListAmharicViewCommand()
        {
            var listWindow = new Lists(ListTypes.ProfessionAmharic);
            listWindow.ShowDialog();
            if (listWindow.DialogResult != null && (bool)listWindow.DialogResult)
            {
                LoadProfessionsAmharic();
                SelectedProfessionAmharic = ProfessionsAmharic.FirstOrDefault(c => c.Display.Equals(listWindow.TxtDisplayName.Text));
            }
        }

        public ICommand ProfessionListViewCommand
        {
            get
            {
                return _professionListViewCommand ??
                       (_professionListViewCommand = new RelayCommand(ExcuteProfessionListViewCommand));
            }
        }
        public void ExcuteProfessionListViewCommand()
        {
            var listWindow = new Lists(ListTypes.Profession);
            listWindow.ShowDialog();
            if (listWindow.DialogResult != null && (bool)listWindow.DialogResult)
            {
                LoadProfessions();
                SelectedProfession = Professions.FirstOrDefault(c => c.Display.Equals(listWindow.TxtDisplayName.Text));
            }
        }

        public void LoadLists()
        {
            LoadProfessions();
            LoadProfessionsAmharic();
        }

        private ListDataItem _selectedProfession, _selectedProfessionAmharic;
        private ObservableCollection<ListDataItem> _professions, _professionsAmharic;

        public ListDataItem SelectedProfession
        {
            get { return _selectedProfession; }
            set
            {
                _selectedProfession = value;
                RaisePropertyChanged<ListDataItem>(() => this.SelectedProfession);
            }
        }
        public ObservableCollection<ListDataItem> Professions
        {
            get { return _professions; }
            set
            {
                _professions = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => this.Professions);
            }
        }
        public void LoadProfessions()
        {
            Professions = new ObservableCollection<ListDataItem>();
            SelectedProfession = new ListDataItem();

            IEnumerable<string> professionsList = new ListService(true)
                .GetAll()
                .Where(l => l.Type == ListTypes.Profession)
                .Select(l => l.DisplayName).Distinct().ToList();
            //IEnumerable<string> professionsList2 = new VisaService(true)
            //    .GetAllConditions()
            //    .Select(l => l.Profession).Distinct().ToList();
            //professionsList = professionsList.Union(professionsList2).Distinct();

            int i = 0;
            foreach (var item in professionsList)
            {
                var dataItem = new ListDataItem
                {
                    Display = item,
                    Value = i
                };
                Professions.Add(dataItem);
                i++;
            }
        }

        public ListDataItem SelectedProfessionAmharic
        {
            get { return _selectedProfessionAmharic; }
            set
            {
                _selectedProfessionAmharic = value;
                RaisePropertyChanged<ListDataItem>(() => this.SelectedProfessionAmharic);
            }
        }
        public ObservableCollection<ListDataItem> ProfessionsAmharic
        {
            get { return _professionsAmharic; }
            set
            {
                _professionsAmharic = value;
                RaisePropertyChanged<ObservableCollection<ListDataItem>>(() => this.ProfessionsAmharic);
            }
        }
        public void LoadProfessionsAmharic()
        {
            ProfessionsAmharic = new ObservableCollection<ListDataItem>();
            SelectedProfessionAmharic = new ListDataItem();

            IEnumerable<string> professionsAmharicList = new ListService(true)
                .GetAll()
                .Where(l => l.Type == ListTypes.ProfessionAmharic)
                .Select(l => l.DisplayName).Distinct().ToList();
            //IEnumerable<string> professionsAmharicList2 = new VisaService(true)
            //    .GetAllConditions()
            //    .Select(l => l.ProfessionAmharic).Distinct().ToList();

            //professionsAmharicList = professionsAmharicList.Union(professionsAmharicList2).Distinct();

            var i = 0;
            foreach (var item in professionsAmharicList)
            {
                var dataItem = new ListDataItem
                {
                    Display = item,
                    Value = i
                };
                ProfessionsAmharic.Add(dataItem);
                i++;
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
