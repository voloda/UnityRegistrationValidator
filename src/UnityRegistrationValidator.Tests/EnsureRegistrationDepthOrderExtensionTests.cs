using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UnityRegistrationValidator.Tests
{
    [TestFixture]
    public class EnsureRegistrationDepthOrderExtensionTests
    {
        [Test]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void PerCallResolutionViaChildContainerShouldFail()
        {
            var rootContainer = CreateUnityContainer();
            rootContainer.RegisterType<IRoot, Root>();

            var childContainer = rootContainer.CreateChildContainer();

            childContainer.RegisterType<IChild, Child>();
            childContainer.RegisterType<IChild2, Child2>();

            childContainer.Resolve<IRoot>();
        }

        [Test]
        [ExpectedException(typeof (ResolutionFailedException))]
        public void PerCallResolutionViaRootContainerShouldFailDueToMissingDependency()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>();

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IChild, Child>();
            childContainer.RegisterType<IChild2, Child2>();

            rootContainer.Resolve<IRoot>();
        }

        [Test]
        [ExpectedException(typeof (ResolutionFailedException))]
        public void RootResolutionViaChildContainerForChildWithoutLifetimeManagerShouldFail()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IChild, Child>();
            childContainer.RegisterType<IChild2, Child2>();

            childContainer.Resolve<IRoot>();
        }

        [Test]
        [ExpectedException(typeof (ResolutionFailedException))]
        public void RootResolutionViaChildContainerForChildRegisteredAsInstanceShouldFail()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterInstance<IChild>(new Child());
            childContainer.RegisterInstance<IChild2>(new Child2(new Child()));

            childContainer.Resolve<IRoot>();
        }
        
        [Test]
        [ExpectedException(typeof (ResolutionFailedException))]
        public void RootResolutionViaChildContainerForChildRegisteredAsSingletonShouldFail()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            childContainer.Resolve<IRoot>();
        }

        [Test]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void RootResolutionViaChildContainerOfChildContainerForChildRegisteredAsSingletonShouldFail()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            childContainer.CreateChildContainer().Resolve<IRoot>();
        }
        
        [Test]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void RootResolutionViaChildContainerOfChildContainerForChildRegisteredAsSingletonOnDifferentLevelsShouldFail()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            
            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            childContainer.CreateChildContainer().Resolve<IRoot>();
        }

        private static UnityContainer CreateUnityContainer()
        {
            var rootContainer = new UnityContainer();
            rootContainer.AddNewExtension<EnsureRegistrationDepthOrderExtension>();
            return rootContainer;
        }
    }
}