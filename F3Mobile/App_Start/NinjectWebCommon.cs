using F3.Business;
using F3.Business.Calendar;
using F3.Business.Leaderboard;
using F3.Business.News;
using F3.Infrastructure;
using F3Mobile.Code;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(F3Mobile.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(F3Mobile.App_Start.NinjectWebCommon), "Stop")]

namespace F3Mobile.App_Start
{
    using System;
    using System.Web;
    using F3.Business.Workout;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
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
            F3.Business.Maps.ModelMaps.InitMaps();
            kernel.Bind<IFeed>().To<Feed>();
            kernel.Bind<IContactBusiness>().To<GoogleContactBusiness>();
            kernel.Bind<ICalendarBusiness>().To<CalendarBusiness>();
            kernel.Bind<ISubscribe>().To<MailChimpBusiness>();
            kernel.Bind<IAccessToken>().To<AuthAccessToken>();
            kernel.Bind<ICacheService>().To<CacheService>();
            kernel.Bind<INewsBusiness>().To<NewsBusiness>();
            kernel.Bind<IStravaBusiness>().To<StravaBusiness>();
            kernel.Bind<IWorkoutBusiness>().To<WorkoutBusiness>();
        }        
    }
}
