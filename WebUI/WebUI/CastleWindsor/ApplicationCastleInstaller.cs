using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using BusinessLogic.Abstraction;
using BusinessLogic.Managers;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using DataAccessLayer;

namespace CastleWindsor
{
    public class ApplicationCastleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<DatabaseContext>().LifestyleSingleton());
            container.Register(Component.For<IConvertManager>().ImplementedBy<ConvertManager>().LifestyleTransient());
            container.Register(Component.For<IClassificationManager>().ImplementedBy<ClassificationManager>().LifestyleTransient());
            container.Register(Component.For<IDrawManager>().ImplementedBy<DrawManager>().LifestyleTransient());

            var controllers = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.BaseType == typeof(Controller))
                .ToList();
            foreach (var controller in controllers)
            {
                container.Register(Component.For(controller).LifestylePerWebRequest());
            }
        }
    }
}
