using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;

using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Admin.ViewModel;
using Microsoft.Practices.Unity;

namespace PinnaFace.Admin
{
    /// <summary>
    /// Here the DI magic come on.
    /// </summary>
    public class Bootstrapper
    {
        public IUnityContainer Container { get; set; }

        public Bootstrapper()
        {
            Container = new UnityContainer();

            ConfigureContainer();
        }

        /// <summary>
        /// We register here every service / interface / viewmodel.
        /// </summary>
        private void ConfigureContainer()
        {
            Singleton.Edition = PinnaFaceEdition.WebEdition;
            //Singleton.SqlceFileName = "PinnaFaceDb1";
            Singleton.PhotoStorage = PhotoStorage.Database;

            Singleton.UseServerDateTime = false;//TO Handle Datetime.Now from serverornot
            Singleton.BuildType = BuildType.Production;
            Singleton.SeedDefaults = true;

            Container.RegisterType<IDbContext, PinnaFaceServerDBContext>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUnitOfWork, UnitOfWork>();

            Container.RegisterType<MainViewModel>();

            //Container.RegisterType<IDbContext, PinnaFaceServerDBContext>(new ContainerControlledLifetimeManager());
            //Container.RegisterInstance<IDbContext>(new ServerDbContextFactory().Create());

            //Container.RegisterType<IUnitOfWork, UnitOfWork>();
            //Container.RegisterInstance<IUnitOfWork>(new UnitOfWork(Container.Resolve<IDbContext>()));

            //Container.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());
            //////Container.RegisterType<ProductKeyViewModel>();
        }
    }
}
