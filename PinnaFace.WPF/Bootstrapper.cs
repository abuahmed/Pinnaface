using Microsoft.Practices.Unity;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.WPF.ViewModel;

namespace PinnaFace.WPF
{
    /// <summary>
    ///     Here the DI magic come on.
    /// </summary>
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            Container = new UnityContainer();
            ConfigureContainer();
        }

        public IUnityContainer Container { get; set; }

        private void ConfigureContainer()
        {
            switch (Singleton.Edition)
            {
                case PinnaFaceEdition.CompactEdition:
                    Singleton.SqlceFileName = PathUtil.GetDatabasePath();
                    Singleton.PhotoStorage = PhotoStorage.FileSystem;
                    break;

                case PinnaFaceEdition.ServerEdition:
                    Singleton.SqlceFileName = "PinnaFaceDbProd";//"PinnaFaceDb25";//munahan_pfdbtest1";//"PinnaFaceDb25";//PinnaFaceDbProd
                    Singleton.PhotoStorage = PhotoStorage.Database;
                    break;
            }

            Singleton.UseServerDateTime = false;    //TO Handle Datetime.Now from serverornot
            Singleton.BuildType = BuildType.Production;
            Singleton.SeedDefaults = true;
            Singleton.SystemVersionDate = DbCommandUtil.GetCurrentDatabaseVersion(); //Whenever System is updated change the number to higher value

            Container.RegisterType<IDbContext, PinnaFaceDbContext>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUnitOfWork, UnitOfWork>();

            Container.RegisterType<MainViewModel>();
        }
    }
}