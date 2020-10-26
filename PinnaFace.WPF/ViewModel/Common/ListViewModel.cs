using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PinnaFace.Core;
using PinnaFace.Service.Interfaces;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace PinnaFace.WPF.ViewModel
{
    public class ListViewModel : ViewModelBase
    {
        #region Fields
        private static IListService _listService;
        private ICommand _saveListViewCommand, _closeListViewCommand, _deleteListViewCommand, _addNewListCommand;
        private ObservableCollection<ListDTO> _lists;
        private ListDTO _selectedList;
        private ListTypes _listType;
        private string _inputLanguage, _headerText;
        #endregion

        #region Constructor
        public ListViewModel()
        {
            CleanUp();
            _listService = new ListService();
            Messenger.Default.Register<ListTypes>(this, message =>
            {
                ListType = message;
            });
        }
        public static void CleanUp()
        {
            if (_listService != null)
            {
                _listService.Dispose();
            }

        }
        #endregion

        #region Properties
        public ListTypes ListType
        {
            get { return _listType; }
            set
            {
                _listType = value;
                RaisePropertyChanged<ListTypes>(() => ListType);

                HeaderText = new EnumerationExtension(ListTypes.City.GetType()).GetDescription(ListType);
                LoadCategories();

                InputLanguage = HeaderText.ToLower().Contains("amh") ? "am-ET" : "en-US";
            }
        }

        public string InputLanguage
        {
            get { return _inputLanguage; }
            set
            {
                _inputLanguage = value;
                RaisePropertyChanged<string>(() => this.InputLanguage);
            }
        }
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => this.HeaderText);
            }
        }

        public ListDTO SelectedList
        {
            get { return _selectedList; }
            set
            {
                _selectedList = value;
                RaisePropertyChanged<ListDTO>(() => SelectedList);
            }
        }
        public ObservableCollection<ListDTO> Lists
        {
            get { return _lists; }
            set
            {
                _lists = value;
                RaisePropertyChanged<ObservableCollection<ListDTO>>(() => Lists);
                ExcuteAddNewListCommand();
            }
        }
        #endregion

        #region Commands
        public ICommand AddNewListCommand
        {
            get
            {
                return _addNewListCommand ?? (_addNewListCommand = new RelayCommand(ExcuteAddNewListCommand));
            }
        }
        private void ExcuteAddNewListCommand()
        {
            SelectedList = new ListDTO
            {
                Type = ListType
            };
        }

        public ICommand SaveListCommand
        {
            get { return _saveListViewCommand ?? (_saveListViewCommand = new RelayCommand<Object>(ExecuteSaveItemViewCommand, CanSave)); }
        }
        private void ExecuteSaveItemViewCommand(object obj)
        {
            if (SaveList(obj))
            {
                if (obj != null)
                    CloseWindow(obj);
            }
        }
        public bool SaveList(object obj)
        {
            try
            {
                var isNewObject = SelectedList.Id;
                var stat = _listService.InsertOrUpdate(SelectedList);

                if (string.IsNullOrEmpty(stat))
                {
                    if (isNewObject == 0)
                    {
                        Lists.Insert(0, SelectedList);
                    }
                    return true;
                }

                MessageBox.Show(stat);
                return false;
            }
            catch
            {
                return false;
            }
        }

        public ICommand DeleteListViewCommand
        {
            get { return _deleteListViewCommand ?? (_deleteListViewCommand = new RelayCommand<Object>(ExecuteDeleteListViewCommand, CanSave)); }
        }
        private void ExecuteDeleteListViewCommand(object obj)
        {
            if (MessageBox.Show("Are you Sure You want to Delete this List?", "Delete List",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SelectedList.Enabled = false;
                var stat = _listService.Disable(SelectedList);

                if (string.IsNullOrEmpty(stat))
                    Lists.Remove(SelectedList);
                else
                    MessageBox.Show("Can't delete " + ListType.ToString() + ", may be the " + stat, "Can't Delete",
                        MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public ICommand CloseListViewCommand
        {
            get
            {
                return _closeListViewCommand ?? (_closeListViewCommand = new RelayCommand<Object>(ExecuteCloseListViewCommand));
            }
        }
        private void ExecuteCloseListViewCommand(object obj)
        {
            CloseWindow(obj);
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

        #region Load Categories
        public void LoadCategories()
        {
            var cat = new SearchCriteria<ListDTO>();
            cat.FiList.Add(c => c.Type == ListType);
            var liList = _listService.GetAll(cat).ToList();
            Lists = new ObservableCollection<ListDTO>(liList);
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
