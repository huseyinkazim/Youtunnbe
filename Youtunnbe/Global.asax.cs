using Business;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Youtunnbe.Helper;

namespace Youtunnbe
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            DependencyResolver.SetResolver(new NinjectOperation());
        }
    }
    public class NinjectOperation: IDependencyResolver
    {
        private readonly IKernel _kernel;
        public NinjectOperation()
        {
            this._kernel = new StandardKernel();
            this._kernel.Bind<IYoutubeManager>().To<YoutubeManager>();
            this._kernel.Bind<IServiceManager>().To<ServiceManager>();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }
    }
    public class NinjectResolver : System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel _kernel;

        public NinjectResolver()
        {
            _kernel = new StandardKernel();
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return _kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            this._kernel.Bind<IYoutubeManager>().To<YoutubeManager>(); // Registering Types    
        }
    }
}
