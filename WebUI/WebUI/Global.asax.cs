using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using CastleWindsor;

namespace WebUI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var container = new WindsorContainer();
            container.Install(new ApplicationCastleInstaller());
            var castleControllerFactory = new CastleControllerFactory(container);
            ControllerBuilder.Current.SetControllerFactory(castleControllerFactory);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
