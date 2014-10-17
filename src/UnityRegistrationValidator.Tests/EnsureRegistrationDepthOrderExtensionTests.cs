using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UnityRegistrationValidator.Tests
{
    [TestFixture]
    public class EnsureRegistrationDepthOrderExtensionTests
    {
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

        [Test]
        public void RootResolutionViaChildContainerForChildRegisteredAsSingletonOnAfterPreviousFailureShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();

            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            Assert.Throws<ResolutionFailedException>(() => childContainer.Resolve<IRoot>());

            var result = childContainer.Resolve<IChild2>();
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ResolutionFailedException))]
        public void ServiceResolutionViaTwoDifferentChildContainersShouldFail()
        {
            var rootContainer = CreateUnityContainer(); // when used directly the whole test will pass

            rootContainer.RegisterType<IService, Service>(new ContainerControlledLifetimeManager());
            var childContainer = rootContainer.CreateChildContainer();
            var childContainer2 = rootContainer.CreateChildContainer();

            childContainer.RegisterInstance<IServiceDependency>(new ServiceDependency("Dep1"));
            childContainer2.RegisterInstance<IServiceDependency>(new ServiceDependency("Dep2"));

            var childContainerResult1 = childContainer.Resolve<IService>();
            var childContainerResult2 = childContainer.Resolve<IService>();
            
            var childContainer2Result1 = childContainer.Resolve<IService>();
            var childContainer2Result2 = childContainer.Resolve<IService>();

            Assert.AreEqual(childContainerResult1, childContainerResult2);
            Assert.AreEqual(childContainer2Result1, childContainer2Result2);

            Assert.AreEqual(childContainer2Result1, childContainerResult1);
        }

        private static UnityContainer CreateUnityContainer()
        {
            var rootContainer = new UnityContainer();
            rootContainer.AddNewExtension<EnsureRegistrationDepthOrderExtension>();
            return rootContainer;
        }
    }
}