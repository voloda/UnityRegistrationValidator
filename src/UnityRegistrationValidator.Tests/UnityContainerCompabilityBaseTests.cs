using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UnityRegistrationValidator.Tests
{
    public abstract class UnityContainerCompabilityBaseTests
    {
        [Test]
        public void PerCallResolutionViaChildContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();
            rootContainer.RegisterType<IRoot, Root>();

            var childContainer = rootContainer.CreateChildContainer();

            childContainer.RegisterType<IChild, Child>();
            childContainer.RegisterType<IChild2, Child2>();

            var result = childContainer.Resolve<IRoot>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void RootResolutionViaChildContainerForChildRegisteredAsSingletonInRootContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());

            var root = childContainer.Resolve<IRoot>();
            Assert.IsNotNull(root);
        }

        [Test]
        public void RootResolutionViaChildContainerForChildReRegisteredAsSingletonInRootContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());
            childContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var root = childContainer.Resolve<IRoot>();
            Assert.IsNotNull(root);
        }
        
        [Test]
        public void RootResolutionViaChildContainerOfChildContainerForChildRegisteredAsSingletonInRootContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());

            var root = childContainer.CreateChildContainer().Resolve<IRoot>();
            Assert.IsNotNull(root);
        }

        [Test]
        public void RootResolutionForAllDependenciesRegisteredAsSingletonsInSameContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var root = rootContainer.Resolve<IRoot>();
            Assert.IsNotNull(root);
        }

        [Test]
        public void RootResolutionViaChildContainerForEverythingRegisteredAsSingletonInRootContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();

            var r1 = childContainer.Resolve<IRoot>();
            var r2 = rootContainer.Resolve<IRoot>();

            Assert.AreEqual(r1, r2);
        }

        [Test]
        public void RootResolutionViaChildContainerForRootWithoutLifetimeManagerAndChildSingletonShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>();

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var result = childContainer.Resolve<IRoot>();

            Assert.IsNotNull(result);
        }

        [Test]
        public void RootResolutionViaChildContainerForRootWithoutLifetimeManagerAndChildInstancesShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>();

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterInstance<IChild>(new Child());
            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var result = childContainer.Resolve<IRoot>();

            Assert.IsNotNull(result);
        }   
        
        [Test]
        public void RootResolutionViaChildContainerForRootWithoutLifetimeManagerAndChildSingletonInRootContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>();
            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            
            var result = childContainer.Resolve<IRoot>();

            Assert.IsNotNull(result);
        }
        
        [Test]
        public void ChildResolutionViaChildContainerForChildRegisteredAsInstanceShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterInstance<IChild>(new Child());

            var child = childContainer.Resolve<IChild>();
            Assert.IsNotNull(child);
        }

        [Test]
        public void RootReferencingClassResolutionViaChildContainerForChildNotRegisteredShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, RootReferencingClass>(new ContainerControlledLifetimeManager());

            var childContainer = rootContainer.CreateChildContainer();

            var result = childContainer.Resolve<IRoot>();

            Assert.IsNotNull(result);
        }


        protected abstract UnityContainer CreateUnityContainer();

    }
}