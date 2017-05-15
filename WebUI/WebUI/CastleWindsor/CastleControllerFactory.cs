using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace CastleWindsor
{
    public class CastleControllerFactory : DefaultControllerFactory
    {
        public IWindsorContainer Container { get; protected set; }

        public CastleControllerFactory(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            Container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }
            return Container.Resolve(controllerType) as IController;
        }

        public override void ReleaseController(IController controller)
        {
            var disposableController = controller as IDisposable;
            disposableController?.Dispose();

            Container.Release(controller);
        }
    }
}
