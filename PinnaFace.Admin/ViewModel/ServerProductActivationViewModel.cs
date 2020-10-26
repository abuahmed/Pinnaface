using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;

namespace PinnaFace.Admin.ViewModel
{
    public class ServerProductActivationViewModel : ViewModelBase
    {
        #region Fields

        private static PinnaFace.Repository.Interfaces.IUnitOfWork _unitOfWork;
        private ObservableCollection<ProductActivationDTO> _users;
        private ProductActivationDTO _selectedProductActivation, _selectedProductActivationForSearch;
        private ICommand _saveProductActivationViewCommand, _addNewProductActivationViewCommand;
        private ICommand _closeProductActivationViewCommand;
        private bool _editCommandVisibility;

        #endregion

        #region Constructor

        public ServerProductActivationViewModel()
        {
            CleanUp();

            var iDbContext = new ServerDbContextFactory().Create();
            _unitOfWork = new UnitOfWorkServer(iDbContext);

            SelectedProductActivation = new ProductActivationDTO();
            ProductActivations = new ObservableCollection<ProductActivationDTO>();
            GetLiveProductActivations();
            EditCommandVisibility = false;
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Properties

        public bool EditCommandVisibility
        {
            get { return _editCommandVisibility; }
            set
            {
                _editCommandVisibility = value;
                RaisePropertyChanged<bool>(() => EditCommandVisibility);
            }
        }

        public ProductActivationDTO SelectedProductActivation
        {
            get { return _selectedProductActivation; }
            set
            {
                _selectedProductActivation = value;
                RaisePropertyChanged<ProductActivationDTO>(() => SelectedProductActivation);
                if (SelectedProductActivation != null)
                {
                }
            }
        }

        public ProductActivationDTO SelectedProductActivationForSearch
        {
            get { return _selectedProductActivationForSearch; }
            set
            {
                _selectedProductActivationForSearch = value;
                RaisePropertyChanged<ProductActivationDTO>(() => this.SelectedProductActivationForSearch);

                //if (SelectedProductActivationForSearch != null && !string.IsNullOrEmpty(SelectedProductActivationForSearch.ProductActivationDetail))
                //{
                //    SelectedProductActivation = SelectedProductActivationForSearch;
                //    SelectedProductActivationForSearch.ProductActivationDetail = "";
                //}
            }
        }

        public ObservableCollection<ProductActivationDTO> ProductActivations
        {
            get { return _users; }
            set
            {
                _users = value;
                RaisePropertyChanged<ObservableCollection<ProductActivationDTO>>(() => ProductActivations);

                if (ProductActivations.Count > 0)
                    SelectedProductActivation = ProductActivations.FirstOrDefault();
                else
                    ExecuteAddNewProductActivationViewCommand();
            }
        }

        #endregion

        #region Commands

        public ICommand AddNewProductActivationViewCommand
        {
            get
            {
                return _addNewProductActivationViewCommand ??
                       (_addNewProductActivationViewCommand = new RelayCommand(ExecuteAddNewProductActivationViewCommand));
            }
        }

        private void ExecuteAddNewProductActivationViewCommand()
        {
            SelectedProductActivation = new ProductActivationDTO();
        }

        public ICommand SaveProductActivationViewCommand
        {
            get
            {
                return _saveProductActivationViewCommand ??
                       (_saveProductActivationViewCommand = new RelayCommand(ExecuteSaveProductActivationViewCommand, CanSave));
            }
        }

        private void ExecuteSaveProductActivationViewCommand()
        {
            try
            {

                _unitOfWork.Repository<ProductActivationDTO>().InsertUpdate(SelectedProductActivation);
                _unitOfWork.Commit();
                GetLiveProductActivations();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //public ICommand DeleteProductActivationViewCommand
        //{
        //    get
        //    {
        //        return _deleteProductActivationViewCommand ??
        //               (_deleteProductActivationViewCommand = new RelayCommand(ExecuteDeleteProductActivationViewCommand));
        //    }
        //}

        //private void ExecuteDeleteProductActivationViewCommand()
        //{
        //    try
        //    {
        //        if (
        //            MessageBox.Show("Are you Sure You want to Delete the user?", "Delete ProductActivation",
        //                MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        //        {
        //            SelectedProductActivation.Enabled = false;
        //            _userService.Disable(SelectedProductActivation);
        //            //_unitOfWork.Repository<ProductActivationDTO>().Update(SelectedProductActivation);
        //            //_unitOfWork.Commit();

        //            GetLiveProductActivations();
        //        }
        //    }
        //    catch
        //    {
        //    }
        //}

        public ICommand CloseProductActivationViewCommand
        {
            get
            {
                return _closeProductActivationViewCommand ??
                       (_closeProductActivationViewCommand = new RelayCommand<Object>(ExecuteCloseProductActivationViewCommand));
            }
        }

        private void ExecuteCloseProductActivationViewCommand(object obj)
        {
            CloseWindow(obj);
        }

        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.DialogResult = true;
            window.Close();
        }

        #endregion

        #region Load ProductActivations

        private void GetLiveProductActivations()
        {
            var usrs = _unitOfWork.Repository<ProductActivationDTO>().Query().Include(a => a.Agency)
                .Get();

            int sNo = 1;
            foreach (var userDto in usrs)
            {
                userDto.SerialNumber = sNo;
                sNo++;
            }
            //based on the current logged in user FILTER USERS LIST
            ProductActivations = new ObservableCollection<ProductActivationDTO>(usrs.ToList());
            //_userService.GetAll().Where(u => u.ProductActivationId > 2).ToList()
        }

        #endregion

        #region Validation

        public static int Errors { get; set; }

        public bool CanSave()
        {
            return Errors == 0;
        }

        #endregion
    }
}