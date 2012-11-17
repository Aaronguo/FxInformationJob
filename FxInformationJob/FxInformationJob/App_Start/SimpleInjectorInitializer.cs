//[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.SimpleInjectorInitializer), "Initialize")]
[assembly: WebActivator.PreApplicationStartMethod(typeof(FxInformationJob.App_Start.SimpleInjectorInitializer), "Initialize")]


namespace FxInformationJob.App_Start
{
    using System.Reflection;
    using System.Web.Mvc;

    using SimpleInjector;
    using SimpleInjector.Integration.Web.Mvc;

    public static class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as MVC3 Dependency Resolver.</summary>
        public static void Initialize()
        {
            var container = new SimpleInjector.Container();

            InitializeContainer(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.RegisterMvcAttributeFilterProvider();

            // Using Entity Framework? Please read this: http://simpleinjector.codeplex.com/discussions/363935
            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));

           
        }

        private static void InitializeContainer(Container container)
        {
            // Please note that if you updated the SimpleInjector.MVC3 package from a previous version, this
            // SimpleInjectorInitializer class replaces the previous SimpleInjectorMVC3 class. You should
            // move the registrations from the old SimpleInjectorMVC3.InitializeContainer to this method,
            // and remove the SimpleInjectorMVC3 and SimpleInjectorMVC3Extensions class from the App_Start
            // folder.
            container.RegisterSingle<Fx.Infrastructure.Caching.ICacheManager, Fx.Infrastructure.Caching.CacheManager>();
            container.RegisterSingle<FxTask.AppSettings>();
            container.RegisterSingle<FxTask.Filter>();

            //FxTaskFxCar
            container.Register<Fx.Domain.FxCar.IService.ICarBuyJob, Fx.Domain.FxCar.CarBuyJobService>();
            container.Register<Fx.Domain.FxCar.IService.ICarTransferJob, Fx.Domain.FxCar.CarTransferJobService>();

            //FxTaskFxGoods
            container.Register<Fx.Domain.FxGoods.IService.IGoodsBuyJob, Fx.Domain.FxGoods.GoodsBuyJobService>();
            container.Register<Fx.Domain.FxGoods.IService.IGoodsTransferJob, Fx.Domain.FxGoods.GoodsTransferJobService>();

            //FxTaskFxHouse
            container.Register<Fx.Domain.FxHouse.IService.IHouseBuyJob, Fx.Domain.FxHouse.HouseBuyJobService>();
            container.Register<Fx.Domain.FxHouse.IService.IHouseTransferJob, Fx.Domain.FxHouse.HouseTransferJobService>();
            
           
        }
    }
}