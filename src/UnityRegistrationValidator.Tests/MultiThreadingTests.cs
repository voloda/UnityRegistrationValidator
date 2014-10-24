using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UnityRegistrationValidator.Tests
{
    [TestFixture]
    public class MultiThreadingTest
    {
        [Test]
        [MaxTime(15000)]
        public void ChildResolutionViaChildContainerForChildrenRegisteredAsSingletonsInDifferentContainersShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());
            var failure = false;

            ThreadStart threadStart = () =>
            {
                try
                {
                    var childContainer = rootContainer.CreateChildContainer();

                    childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateChild2));
                    childContainer.Resolve<IChild2>();
                }
                catch
                {
                    failure = true;
                }
            };

            const int maxThreads = 5;

            RunThreadsAndWaitUntilAllFinish(maxThreads, threadStart);

            Assert.IsFalse(failure);
        }

        [Test]
        [MaxTime(15000)]
        public void ChildResolutionViaChildContainerForChildRegisteredAsSingletonInChildContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>();
            var failure = false;

            ThreadStart threadStart = () =>
            {
                try
                {
                    var childContainer = rootContainer.CreateChildContainer();

                    childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateChild2));
                    childContainer.Resolve<IChild2>();
                }
                catch
                {
                    failure = true;
                }
            };

            const int maxThreads = 50;

            RunThreadsAndWaitUntilAllFinish(maxThreads, threadStart);

            Assert.IsFalse(failure);
        }

        [Test]
        [MaxTime(15000)]
        public void ChildResolutionViaChildContainerForChildrenRegisteredInRootContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateChild));
            rootContainer.RegisterType<IChild2, Child2>(new InjectionFactory(CreateChild2));

            var failure = false;

            ThreadStart threadStart = () =>
            {
                try
                {
                    var childContainer = rootContainer.CreateChildContainer();

                    childContainer.Resolve<IChild2>();
                }
                catch
                {
                    failure = true;
                }
            };

            const int maxThreads = 50;

            RunThreadsAndWaitUntilAllFinish(maxThreads, threadStart);

            Assert.IsFalse(failure);
        }
        [Test]
        [MaxTime(15000)]
        public void ChildResolutionViaRootContainerForChildrenRegisteredInRootContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateChild));
            rootContainer.RegisterType<IChild2, Child2>(new InjectionFactory(CreateChild2));

            var failure = false;

            ThreadStart threadStart = () =>
            {
                try
                {
                    rootContainer.Resolve<IChild2>();
                }
                catch
                {
                    failure = true;
                }
            };

            const int maxThreads = 50;

            RunThreadsAndWaitUntilAllFinish(maxThreads, threadStart);

            Assert.IsFalse(failure);
        }

        [Test]
        [MaxTime(15000)]
        public void ChildResolutionViaChildContainerForChildReRegisteredAsSingletonShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateChild));
            rootContainer.RegisterType<IChild2, Child2>(new InjectionFactory(CreateChild2));

            var failure = false;

            ThreadStart threadStart = () =>
            {
                try
                {
                    var childContainer = rootContainer.CreateChildContainer();
                    childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateChild2));

                    childContainer.Resolve<IChild2>();
                }
                catch (Exception)
                {
                    failure = true;
                }
            };

            const int maxThreads = 50;

            RunThreadsAndWaitUntilAllFinish(maxThreads, threadStart);

            Assert.IsFalse(failure);
        }

        [Test]
        [MaxTime(15000)]
        public void RootResolutionViaChildContainerForRootRegisteredAsSingletonInChildContainerShouldWork()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateChild));
            rootContainer.RegisterType<IChild2, Child2>(new InjectionFactory(CreateChild2));

            var failure = false;

            ThreadStart threadStart = () =>
            {
                try
                {
                    var childContainer = rootContainer.CreateChildContainer();
                    childContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager(), new InjectionFactory(CreateRoot));

                    childContainer.Resolve<IRoot>();
                }
                catch (Exception)
                {
                    failure = true;
                }
            };

            const int maxThreads = 50;

            RunThreadsAndWaitUntilAllFinish(maxThreads, threadStart);

            Assert.IsFalse(failure);
        }

        [Test]
        [MaxTime(15000)]
        public void RecoveryShouldWorkProperly()
        {
            var rootContainer = CreateUnityContainer();

            rootContainer.RegisterType<IRoot, Root>(new ContainerControlledLifetimeManager());
            rootContainer.RegisterType<IChild, Child>(new ContainerControlledLifetimeManager());

            var failure = false;
            var childContainer = rootContainer.CreateChildContainer();
            childContainer.RegisterType<IChild2, Child2>(new ContainerControlledLifetimeManager());

            ThreadStart threadStart = () =>
            {
                try
                {
                    var exception = false;

                    try { childContainer.Resolve<IRoot>(); }
                    catch { exception = true; }
                    finally { if (!exception) failure = true; }

                    childContainer.Resolve<IChild2>();
                }
                catch (Exception)
                {
                    failure = true;
                }
            };

            const int maxThreads = 50;

            RunThreadsAndWaitUntilAllFinish(maxThreads, threadStart);

            Assert.IsFalse(failure);
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

        private static void RunThreadsAndWaitUntilAllFinish(int maxThreads, ThreadStart threadStart)
        {
            var threads = new List<Thread>();

            for (int i = 0; i < maxThreads; i++)
            {
                var thread = new Thread(threadStart);

                threads.Add(thread);
            }

            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
        }

        private IChild2 CreateChild2(IUnityContainer arg)
        {
            Thread.Sleep(3000);
            return new Child2(arg.Resolve<IChild>());
        }

        private IChild CreateChild(IUnityContainer arg)
        {
            Thread.Sleep(3000);
            return new Child();
        }

        private IRoot CreateRoot(IUnityContainer arg)
        {
            Thread.Sleep(3000);
            return new Root(arg.Resolve<IChild>(), arg.Resolve<IChild2>());
        }

        private IUnityContainer CreateUnityContainer()
        {
            var container = new UnityContainer();
            container.AddNewExtension<EnsureRegistrationDepthOrderExtension>();
            return container;
        }
    }
}