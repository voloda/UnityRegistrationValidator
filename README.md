# UnityRegistrationValidator

Microsoft Unity Extension which verifies registrations between parent and child containers. This is mainly issue when you
accidently inject a dependency from a child container to the parent container while using ContainerControlledLifetimeManager (singleton).

## Introduction

When using Microsoft Unity IoC container you may face some weird issues when dealing with 'singletons' (ContainerControlledLifetimeManager) and child containers. It may or may not be what you expected.

Let's consider following classes:

```csharp
public interface IServiceDependency
{
}

public interface IService
{
}

public class ServiceDependency : IServiceDependency
{
    private readonly string _dep1;

    public ServiceDependency(string dep1)
    {
        _dep1 = dep1;
    }
}

public class Service : IService
{
    private readonly IServiceDependency _dependency;

    public Service(IServiceDependency dependency)
    {
        _dependency = dependency;
    }
}
```

Now let's setup some test case:

```csharp
[Test]
public void ServiceResolutionViaTwoDifferentChildContainersShouldFail()
{
    var rootContainer = new UnityContainer();

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
```

**This actually will pass all the assertions.** 

### Let's analyze it a bit:
* Let's step thru the test case in debugger.
* Setup some watches:
 * `((ServiceDependency)((Service)childContainerResult1)._dependency)._dep1`
 * `((ServiceDependency)((Service)childContainer2Result1)._dependency)._dep1`
* You can see that both of them point to `Dep1`.
 * Is that what you really wanted?
 * Consider the following case:
  * You disposed `childContainer` and your `ServiceDependency` is disposable registered as `ContainerControlledLifetimeManager`.
  * **At this point you have invalid instance of `IService`** (it has injected disposed ServiceDependency).
  * **This is most likely a side effect of unwanted changes and you want to avoid it**.
   * **And this is exactly what is this extension trying to solve**.
    * **By enabling this extension the build operation will fail**.

## How to enable extension

* Reference the `UnityRegistrationValidator.dll` in your project.
* Call the registration below.
 * The extension is available as a [**Nuget package**](https://www.nuget.org/packages/UnityRegistrationValidator).

```csharp
var rootContainer = new UnityContainer();
rootContainer.AddNewExtension<EnsureRegistrationDepthOrderExtension>();
```

## Following rules are enforced after registering extension

* For each registration is tracked depth in containers (starting the container in which resolve starts).
* If you register an object which
 * has dependency resolvable only inside the child container
 * and has ContainerControlledLifetimeManager()
* the resolve will fail.
* If you do this without the extension the resolve will succeed but the dependencies were most likely resolved in unexpected way (unless you really know what are you doing).

## Conclusion

* This extension allows you to validate the expected behavior.
* Since it may have performance impact (it needs to track all the registrations as well as build operations) it may have negative performance impact.
* This extension is intended to help you ensure expected behavior - but you have to always consider your circumstanes.
* **You are using this extension on your own risk :-).**

## References
* [https://www.nuget.org/packages/UnityRegistrationValidator](https://www.nuget.org/packages/UnityRegistrationValidator)
* [https://github.com/voloda/UnityRegistrationValidator](https://github.com/voloda/UnityRegistrationValidator)
