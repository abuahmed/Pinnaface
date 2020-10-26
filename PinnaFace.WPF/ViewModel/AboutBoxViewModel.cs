using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;

namespace PinnaFace.WPF.ViewModel
{
    public class AboutBoxViewModel : ViewModelBase
    {
        #region Fields

        private static IUnitOfWork _unitOfWork;
        private ProductActivationDTO _productActivation;

        #endregion

        #region Constructor

        public AboutBoxViewModel()
        {
            CleanUp();
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());
            ProductActivation = _unitOfWork.Repository<ProductActivationDTO>()
                .Query()
                .Get()
                .FirstOrDefault();
        }

        public static void CleanUp()
        {
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Properties
        
        public ProductActivationDTO ProductActivation
        {
            get { return _productActivation; }
            set
            {
                _productActivation = value;
                RaisePropertyChanged<ProductActivationDTO>(() => ProductActivation);
            }
        }

        #endregion

        #region Actions

        private ICommand _closeSplashView;

        public ICommand CloseSplashView
        {
            get { return _closeSplashView ?? (_closeSplashView = new RelayCommand<Object>(CloseWindow)); }
        }


        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window == null) return;
            window.Close();
        }

        #endregion
        
    }
}