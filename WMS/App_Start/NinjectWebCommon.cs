[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(WMS.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(WMS.App_Start.NinjectWebCommon), "Stop")]

namespace WMS.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Ninject.Web.Common.WebHost;
    using WMS.DAO.IService;
    using WMS.DAO.Service;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application.
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<IAuthenticateService>().To<AuthenticateService>();
            kernel.Bind<IConfigurationService>().To<ConfigurationService>();
            kernel.Bind<IProfileService>().To<ProfileService>();
            kernel.Bind<IQualityService>().To<QualityService>();
            kernel.Bind<IReadCodebars>().To<QualityService>();
            kernel.Bind<IFolioService>().To<FolioService>();
            kernel.Bind<IInventoryService>().To<InventoryService>();
            kernel.Bind<ITransferService>().To<TransferService>();
            kernel.Bind<IShippingService>().To<ShippingService>();
            kernel.Bind<IShipmentService>().To<ShipmentService>();
        }
    }
}