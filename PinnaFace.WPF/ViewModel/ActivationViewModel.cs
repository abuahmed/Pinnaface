using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using PinnaKeys.OA;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Extensions;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.WPF.Views;


namespace PinnaFace.WPF.ViewModel
{
    public class ActivationViewModel : ViewModelBase
    {
        #region Fields

        private static EntitiesModel _dbContext;
        private static IUnitOfWork _unitOfWork;
        private string _productKey;
        private ICommand _activateCommand;
        private ProductActivationDTO _productActivation;
        private string _progressBarVisibility, _biosNo;
        private bool _commandsEnability;

        #endregion

        #region Constructor

        public ActivationViewModel()
        {
            CleanUp();
            string connectionStringName = DbCommandUtil.GetActivationConnectionString();
            _dbContext = new EntitiesModel(connectionStringName);
            _unitOfWork = new UnitOfWork(DbContextUtil.GetDbContextInstance());

            ProductActivation = _unitOfWork.Repository<ProductActivationDTO>()
                .Query().Get()
                .FirstOrDefault() ?? new ProductActivationDTO();

            ProgressBarVisibility = "Collapsed";
            CommandsEnability = true;
            BiosNo = "Bios No:" + new ProductActivationDTO().BiosSn;
        }

        public static void CleanUp()
        {
            if (_dbContext != null)
                _dbContext.Dispose();
            if (_unitOfWork != null)
                _unitOfWork.Dispose();
        }

        #endregion

        #region Properties

        public string ProductKey
        {
            get { return _productKey; }
            set
            {
                _productKey = value;
                RaisePropertyChanged<string>(() => ProductKey);
            }
        }

        public ProductActivationDTO ProductActivation
        {
            get { return _productActivation; }
            set
            {
                _productActivation = value;
                RaisePropertyChanged<ProductActivationDTO>(() => ProductActivation);
            }
        }

        public string ProgressBarVisibility
        {
            get { return _progressBarVisibility; }
            set
            {
                _progressBarVisibility = value;
                RaisePropertyChanged<string>(() => ProgressBarVisibility);
            }
        }

        public bool CommandsEnability
        {
            get { return _commandsEnability; }
            set
            {
                _commandsEnability = value;
                RaisePropertyChanged<bool>(() => this.CommandsEnability);
            }
        }

        public string BiosNo
        {
            get { return _biosNo; }
            set
            {
                _biosNo = value;
                RaisePropertyChanged<string>(() => BiosNo);
            }
        }

        #endregion

        #region Commands

        private object _obj;
        private bool _login;

        public ICommand ActivateCommand
        {
            get { return _activateCommand ?? (_activateCommand = new RelayCommand<Object>(ExcuteActivateCommand)); }
        }

        private void ExcuteActivateCommand(object windowObject)
        {
            _obj = windowObject;
            ProgressBarVisibility = "Visible";
            CommandsEnability = false;
            var worker = new BackgroundWorker();
            worker.DoWork += DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ProgressBarVisibility = "Collapsed";
            CommandsEnability = true;
            if (!_login) return;
            new Login().Show();
            CloseWindow(_obj);
        }

        private void DoWork(object sender, DoWorkEventArgs e)
        {
            ProductActivation.ProductKey = ProductKey;
            var key = _dbContext.ActivationKeys
                .FirstOrDefault(a => a.ProductKey == ProductActivation.ProductKey
                                     && a.KeyStatus == 0 && a.ProductType == 0);
            //O represents Active and O represents PinnaFace

            if (key != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(key.BIOS_SN))
                    {
                        key.BIOS_SN = ProductActivation.BiosSn;
                        //key.FirstActivatedDate = DateTime.Now;
                        //    //GETUTCDATETIME StoredPr. //the time will be better if it is the server timer
                        //key.ExpirationDate = key.FirstActivatedDate.Value.AddDays(key.ExpiryDuration);
                    }
                    else
                    {
                        if (!key.BIOS_SN.Contains(ProductActivation.BiosSn))
                        {
                            if (key.NoOfAllowedPcs == 1)
                            {
                                MessageBox.Show(
                                    "Can't Activate the product, " +
                                    "Check your product key and try again, " +
                                    "or contact pinnaface office!", "More than Allowed Pcs");
                                ProductKey = "";
                                CommandsEnability = true;

                                return;
                            }

                            key.BIOS_SN = key.BIOS_SN + "," + ProductActivation.BiosSn;
                            if (key.BIOS_SN.Split(',').Count() > key.NoOfAllowedPcs)
                            {
                                MessageBox.Show(
                                    "Can't Activate the product, " +
                                    "Check your product key and try again, " +
                                    "or contact pinnaface office!", "More than Allowed Pcs");
                                ProductKey = "";
                                CommandsEnability = true;
                                return;
                            }
                        }
                    }

                    key.NoOfActivations = key.NoOfActivations + 1;
                    if (key.NoOfActivations > key.NoOfAllowedActivations)
                    {
                        MessageBox.Show(
                            "Can't Activate the product, " +
                            "Check your product key and try again, " +
                            "or contact pinnaface office!", "More than Allowed Activations");
                        ProductKey = "";
                        CommandsEnability = true;
                        return;
                    }

                    _dbContext.Add(key);
                    _dbContext.SaveChanges();

                    ProductActivation.RegisteredBiosSn = key.BIOS_SN;
                    ProductActivation.DateLastModified = DbCommandUtil.GetCurrentSqlDate(true);//  DateTime.Now; //GETUTCDATETIME StoredPr
                    ProductActivation.Synced = false;

                    if (ProductActivation.Id == 0)
                    {
                        ProductActivation.LicensedTo = key.CustomerName;
                        ProductActivation.DatabaseVersionDate = Singleton.SystemVersionDate;
                        ProductActivation.MaximumSystemVersion = key.MaximumAllowedSystemVersion;


                        ProductActivation.ModifiedByUserId = 1;
                        ProductActivation.CreatedByUserId = 1;
                        ProductActivation.DateRecordCreated = DbCommandUtil.GetCurrentSqlDate(true);//DateTime.Now; //GETUTCDATETIME StoredPr

                        #region Set UserAccounts

                        ProductActivation.SuperName = key.SuperName; //.Email;
                        ProductActivation.SuperPass = key.SuperPass;
                        ProductActivation.AdminName = key.AdminName;
                        ProductActivation.AdminPass = key.AdminPass;
                        ProductActivation.User1Name = key.User1Name;
                        ProductActivation.User1Pass = key.User1Pass;

                        #endregion

                        #region Agency

                        var localAgency = new AgencyDTO
                        {
                            AgencyName = key.CustomerName ?? (key.CustomerName = ""),
                            AgencyNameAmharic = "-",
                            ManagerName = "-",
                            ManagerNameAmharic = "-",
                            Address = new AddressDTO
                            {
                                AddressType = AddressTypes.Local,
                                Country = CountryList.Ethiopia,
                                City = EnumUtil.GetEnumDesc(CityList.AddisAbeba),
                                Region = "14",
                                Telephone = key.Telephone ?? (key.Telephone = ""),
                                PrimaryEmail = key.Email ?? (key.Email = "")
                            },
                            Header = new AttachmentDTO(),
                            Footer = new AttachmentDTO(),
                            LicenceNumber = "-",
                            SaudiOperation = key.SaudiOperation,
                            DubaiOperation = key.DubaiOperation,
                            KuwaitOperation = key.KuwaitOperation,
                            QatarOperation = key.QatarOperation,
                            JordanOperation = key.JordanOperation,
                            LebanonOperation = key.LebanonOperation,
                            BahrainOperation = key.BahrainOperation,
                            DepositAmount = "100,000 USD",
                            Managertype = "ዋና ስራ አስኪያጅ"
                        };

                        #endregion

                        #region Foreign Agents

                        var foreignAgent = new AgentDTO
                        {
                            AgentName = "-",
                            AgentNameAmharic = "-",
                            Address = new AddressDTO
                            {
                                AddressType = AddressTypes.Foreign,
                                Country = CountryList.SaudiArabia,
                                City = EnumUtil.GetEnumDesc(CityList.Riyadh)
                            },
                            LicenseNumber = "-",
                            Header = new AttachmentDTO(),
                            Footer = new AttachmentDTO(),
                        };

                        #endregion

                        #region Setting

                        var setting = new SettingDTO
                        {
                            AwajNumber = "923/2008",
                            EmbassyApplicationType = EmbassyApplicationTypes.SponsorNameOnTop,
                            SyncDuration = 1,
                            StartSync = true,
                        };

                        #endregion

                        ProductActivation.FirstActivatedDate = key.FirstActivatedDate;// DateTime.Now; //GETUTCDATETIME StoredPr
                        ProductActivation.ExpiryDate = key.ExpiryDate;// DateTime.Now.AddDays(key.ExpiryDuration);

                        ProductActivation.Agency = localAgency;
                        setting.Agency = localAgency;

                        ////Since we don't update Header and Footer from Server we didnt need the following 6 lines 
                        //localAgency.Address.Agency = localAgency;
                        //localAgency.Header.Agency = localAgency;
                        //localAgency.Footer.Agency = localAgency;

                        //foreignAgent.Address.Agency = localAgency;
                        //foreignAgent.Header.Agency = localAgency;
                        //foreignAgent.Footer.Agency = localAgency;

                        _unitOfWork.Repository<AgencyDTO>().Insert(localAgency);
                        _unitOfWork.Repository<AgentDTO>().Insert(foreignAgent);

                        _unitOfWork.Repository<SettingDTO>().Insert(setting);
                        _unitOfWork.Repository<ProductActivationDTO>().Insert(ProductActivation);
                    }
                    else
                    {
                        //localAgency.Synced = false;
                        //foreignAgent.Synced = false;

                        //_unitOfWork.Repository<AgencyDTO>().Update(localAgency);
                        //_unitOfWork.Repository<AgentDTO>().Update(foreignAgent);

                        _unitOfWork.Repository<ProductActivationDTO>().Update(ProductActivation);
                    }

                    int changes=_unitOfWork.Commit();
                    if (changes > 0)
                    {
                        Singleton.ProductActivation = ProductActivation;
                        _login = true;
                    }
                    else
                    {
                        MessageBox.Show(
                            "Can't Activate the product, check your product key and try again, or contact pinnasofts!");
                        ProductKey = "";
                        CommandsEnability = true;
                    }
                }
                catch
                {
                    MessageBox.Show("Error:" + Environment.NewLine + " There may be no Internet connection." +
                                    Environment.NewLine + "Check your connection and try again.");
                    CommandsEnability = true;
                }
            }
            else
            {
                MessageBox.Show(
                    "Can't Activate the product, check your product key and try again, or contact pinnasofts!");
                ProductKey = "";
                CommandsEnability = true;
            }
        }


        private ICommand _closeWindowCommand;

        public ICommand CloseWindowCommand
        {
            get { return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand<Object>(CloseWindow)); }
        }

        private void CloseWindow(object obj)
        {
            if (obj == null) return;
            var window = obj as Window;
            if (window != null)
            {
                window.Close();
            }
        }

        #endregion
    }
}