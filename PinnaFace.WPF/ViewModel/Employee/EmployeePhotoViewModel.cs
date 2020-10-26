using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.Service;
using PinnaFace.Service.Interfaces;

namespace PinnaFace.WPF.ViewModel
{
    public class EmployeePhotoViewModel : ViewModelBase
    {
        #region Fields

        private static IEmployeeService _employeeService;
        private static IAttachmentService _attachmentService;
        private string _headerText;
        private EmployeeDTO _selectedEmployee;
        private AttachmentDTO _shortPhotoAttachment;
        private AttachmentDTO _standPhotoAttachment;

        #endregion

        #region Constructor

        public EmployeePhotoViewModel()
        {
            CleanUp();
            _employeeService = new EmployeeService();
            _attachmentService = new AttachmentService();

            Messenger.Default.Register<EmployeeDTO>(this, message => { SelectedEmployee = message; });
        }

        public static void CleanUp()
        {
            if (_employeeService != null)
                _employeeService.Dispose();
            if (_attachmentService != null)
                _attachmentService.Dispose();
        }

        #endregion

        #region Properties

        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged<string>(() => HeaderText);
            }
        }

        public EmployeeDTO SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                _selectedEmployee = value;
                RaisePropertyChanged<EmployeeDTO>(() => SelectedEmployee);
                if (SelectedEmployee != null && SelectedEmployee.Id != 0)
                {
                    HeaderText = SelectedEmployee.FullName + " - " + SelectedEmployee.PassportNumber;
                    string photoPath = ImageUtil.GetPhotoPath();

                    _standPhotoAttachment =
                        _attachmentService.Find(SelectedEmployee.StandPhotoId.ToString());
                    if (_standPhotoAttachment != null)
                    {
                        if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
                        {
                            //var photoPath = ImageUtil.GetPhotoPath();
                            if (!string.IsNullOrWhiteSpace(_standPhotoAttachment.AttachmentUrl))
                            {
                                string fiName = _standPhotoAttachment.AttachmentUrl;
                                    // SelectedEmployee.RowGuid + "_Short.jpg";
                                string fname = Path.Combine(photoPath, fiName);
                                EmployeeLongImage = new BitmapImage(new Uri(fname,true));
                            }
                        }
                        else
                        {
                            EmployeeLongImage = ImageUtil.ToImage(_standPhotoAttachment.AttachedFile);
                        }
                    }
                    else
                        _standPhotoAttachment = SelectedEmployee.StandPhoto;
                    //if (_standPhotoAttachment != null)
                    //{
                    //    if (string.IsNullOrWhiteSpace(_standPhotoAttachment.AttachmentUrl))
                    //        EmployeeLongImage = null;
                    //    else
                    //    {
                    //        string fiName = SelectedEmployee.RowGuid + "_Long.jpg";
                    //        var fname = Path.Combine(photoPath, fiName);

                    //        EmployeeLongImage = ImageUtil.ToImageFromUrl(fname);// new BitmapImage(new Uri(fname,true));
                    //    }

                    //    //EmployeeLongImage = ImageUtil.ToImage(_standPhotoAttachment.AttachedFile);
                    //}
                    //else
                    //    _standPhotoAttachment = SelectedEmployee.StandPhoto;


                    _shortPhotoAttachment =
                        _attachmentService.Find(SelectedEmployee.PhotoId.ToString());
                    if (_shortPhotoAttachment != null)
                    {
                        if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
                        {
                            //var photoPath = ImageUtil.GetPhotoPath();
                            if (!string.IsNullOrWhiteSpace(_shortPhotoAttachment.AttachmentUrl))
                            {
                                string fiName = _shortPhotoAttachment.AttachmentUrl;
                                    // SelectedEmployee.RowGuid + "_Short.jpg";
                                string fname = Path.Combine(photoPath, fiName);
                                EmployeeShortImage = new BitmapImage(new Uri(fname,true));
                            }
                        }
                        else
                        {
                            EmployeeShortImage = ImageUtil.ToImage(_shortPhotoAttachment.AttachedFile);
                        }
                    }
                    else
                        _shortPhotoAttachment = SelectedEmployee.Photo;

                    //if (_shortPhotoAttachment != null)
                    //{
                    //    if (string.IsNullOrWhiteSpace(_shortPhotoAttachment.AttachmentUrl))
                    //    {
                    //        EmployeeShortImage = null;
                    //    }
                    //    else
                    //    {
                    //        string fiName = SelectedEmployee.RowGuid + "_Short.jpg";
                    //        var fname = Path.Combine(photoPath, fiName);
                    //        EmployeeShortImage = ImageUtil.ToImageFromUrl(fname);//new BitmapImage(new Uri(fname,true));
                    //    }
                    //    //EmployeeShortImage = ImageUtil.ToImage(_shortPhotoAttachment.AttachedFile);
                    //}
                    //else
                    //    _shortPhotoAttachment = SelectedEmployee.Photo;
                }
            }
        }

        #endregion

        #region Commands

        private ICommand _closeApplicationCommand;
        private ICommand _saveApplicationCommand;

        public ICommand SaveEmployeePhotoCommand
        {
            get
            {
                return _saveApplicationCommand ??
                       (_saveApplicationCommand = new RelayCommand<Object>(ExcuteSaveEmployeePhotoCommand));
            }
        }


        public ICommand CloseEmployeePhotoCommand
        {
            get
            {
                return _closeApplicationCommand ?? (_closeApplicationCommand = new RelayCommand<Object>(CloseWindow));
            }
        }

        private void ExcuteSaveEmployeePhotoCommand(object obj)
        {
            try
            {
                string photoPath = ImageUtil.GetPhotoPath();

                if (EmployeeLongImage != null && EmployeeLongImage.UriSource != null &&
                    !string.IsNullOrWhiteSpace(EmployeeLongImageFileName))
                {
                    //_standPhotoAttachment.AttachedFile = ImageUtil.ToBytes(EmployeeLongImage);
                    if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
                    {
                        string fiName = Guid.NewGuid() + ".jpg"; // SelectedEmployee.RowGuid + "_Long.jpg";
                        SelectedEmployee.StandPhoto.AttachmentUrl = fiName ;
                        File.Copy(EmployeeLongImageFileName, Path.Combine(photoPath, fiName), true);
                    }
                    else
                    {
                        SelectedEmployee.StandPhoto.AttachedFile = ImageUtil.ToBytes(EmployeeLongImage);
                        SelectedEmployee.StandPhoto.RowGuid = Guid.NewGuid();
                    }
                    SelectedEmployee.StandPhoto.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                    SelectedEmployee.StandPhoto.DateLastModified = DateTime.Now;
                    //SelectedEmployee.StandPhoto = _standPhotoAttachment;
                }
                if (EmployeeShortImage != null && EmployeeShortImage.UriSource != null &&
                    !string.IsNullOrWhiteSpace(EmployeeShortImageFileName))
                {
                    // _shortPhotoAttachment.AttachedFile = ImageUtil.ToBytes(EmployeeShortImage);
                    if (Singleton.PhotoStorage == PhotoStorage.FileSystem)
                    {
                        string fiName = Guid.NewGuid()+ ".jpg"; // SelectedEmployee.RowGuid + "_Short.jpg";
                        SelectedEmployee.Photo.AttachmentUrl = fiName;
                        File.Copy(EmployeeShortImageFileName, Path.Combine(photoPath, fiName), true);
                    }
                    else
                    {
                        SelectedEmployee.Photo.AttachedFile = ImageUtil.ToBytes(EmployeeShortImage);
                        SelectedEmployee.Photo.RowGuid = Guid.NewGuid();
                    }
                    SelectedEmployee.Photo.ModifiedByUserId = Singleton.User != null ? Singleton.User.UserId : 1;
                    SelectedEmployee.Photo.DateLastModified = DateTime.Now;
                    //SelectedEmployee.Photo = _shortPhotoAttachment;
                }
                CloseWindow(obj);
            }

            catch
            {
                MessageBox.Show("Can't Save Photo!");
            }
        }

        public void CloseWindow(object obj)
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

        #region Short Photo

        private BitmapImage _employeeShortImage;
        private string _employeeShortImageFileName;
        private ICommand _showEmployeeShortImageCommand;

        public BitmapImage EmployeeShortImage
        {
            get { return _employeeShortImage; }
            set
            {
                _employeeShortImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeShortImage);
            }
        }

        public string EmployeeShortImageFileName
        {
            get { return _employeeShortImageFileName; }
            set
            {
                _employeeShortImageFileName = value;
                RaisePropertyChanged<string>(() => EmployeeShortImageFileName);
            }
        }

        public ICommand ShowEmployeeShortImageCommand
        {
            get
            {
                return _showEmployeeShortImageCommand ??
                       (_showEmployeeShortImageCommand = new RelayCommand(ExecuteShowEmployeeShortImageViewCommand));
            }
        }

        private void ExecuteShowEmployeeShortImageViewCommand()
        {
            var file = new OpenFileDialog {Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg"};
            bool? result = file.ShowDialog();
            if (result != null && ((bool) result && File.Exists(file.FileName)))
            {
                EmployeeShortImageFileName = file.FileName;
                var fileName = ImageResizingUtil.ResizeImages(file.FileName);
                EmployeeShortImage = new BitmapImage(new Uri(fileName,true));//file.FileName
            }
        }

        #endregion

        #region Long Photo

        private BitmapImage _employeeLongImage;
        private string _employeeLongImageFileName;
        private ICommand _showEmployeeLongImageCommand;

        public string EmployeeLongImageFileName
        {
            get { return _employeeLongImageFileName; }
            set
            {
                _employeeLongImageFileName = value;
                RaisePropertyChanged<string>(() => EmployeeLongImageFileName);
            }
        }

        public BitmapImage EmployeeLongImage
        {
            get { return _employeeLongImage; }
            set
            {
                _employeeLongImage = value;
                RaisePropertyChanged<BitmapImage>(() => EmployeeLongImage);
            }
        }

        public ICommand ShowEmployeeLongImageCommand
        {
            get
            {
                return _showEmployeeLongImageCommand ??
                       (_showEmployeeLongImageCommand = new RelayCommand(ExecuteShowEmployeeLongImageViewCommand));
            }
        }

        private void ExecuteShowEmployeeLongImageViewCommand()
        {
            var file = new OpenFileDialog {Filter = "Image Files(*.png;*.jpg; *.jpeg)|*.png;*.jpg; *.jpeg"};
            bool? result = file.ShowDialog();
            if (result != null && ((bool) result && File.Exists(file.FileName)))
            {
                EmployeeLongImageFileName = file.FileName;
                var fileName = ImageResizingUtil.ResizeImages(file.FileName);
                EmployeeLongImage = new BitmapImage(new Uri(fileName, true));//file.FileName
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