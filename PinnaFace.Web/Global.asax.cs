using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using PinnaFace.Core;
using PinnaFace.Core.Enumerations;
using PinnaFace.Core.Models;
using PinnaFace.DAL;
using PinnaFace.DAL.Interfaces;
using PinnaFace.Repository;
using PinnaFace.Repository.Interfaces;
using PinnaFace.Service;
using PinnaFace.Web.Models;

namespace PinnaFace.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public IUnityContainer Container { get; set; }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            Singleton.Edition = PinnaFaceEdition.WebEdition;
            Singleton.BuildType = BuildType.Production;
            Singleton.SeedDefaults = true;
            Singleton.UseServerDateTime = true;//TO Handle Datetime.Now from serverornot

            /**************/
            Singleton.SystemVersionDate = DbCommandUtil.GetCurrentDatabaseVersion();
            
            if (!ValidateProduct())
            {
                LogUtil.LogError(ErrorSeverity.Critical, "ValidateProduct",
                  "Higher Database Version", "", "");
                return;
            }
            /*****************/

            IEnumerable<ListDTO> aa = new ListService().GetAll();
            new InitializeObjects().InitializeWebSecurity();
            
            Container = new UnityContainer();
            Container.RegisterType<IDbContext, PinnaFaceServerDBContext>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUnitOfWork, UnitOfWork>();
        }

        public bool ValidateProduct()
        {
            var activationModel = DbCommandUtil.ValidateProductSql();

            if (activationModel != null
                && activationModel.DatabaseVersionDate != 0
                && activationModel.MaximumSystemVersion != 0)
            {
                if (Singleton.SystemVersionDate < activationModel.DatabaseVersionDate)
                    return false;
            }
            return true;
        }
    }
}