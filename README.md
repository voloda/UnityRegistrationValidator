# UnityRegistrationValidator

Microsoft Unity Extension which verifies registrations between parent and child containers.

## Introduction

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
}

public class Service : IService
{
  public Service(IServiceDependency dependency)
  {
  }
}
```

Now let's setup some test case:

```cs
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

This actually will pass all the assertions. But let's analyze it a bit:
* Let's step thru the test in debugger
* Setup watch for `childContainerResult1._dependency` 
* And for `childContainer2Result1._dependency`

## How to enable extension

* Reference the `UnityRegistrationValidator.dll` in your project
* Call the registration below

```cs
var rootContainer = new UnityContainer();
rootContainer.AddNewExtension<EnsureRegistrationDepthOrderExtension>();
```

## Following rules are enforced after registering extension

* For each registration is tracked depth in containers (starting the container in which resolve starts)
* If you register an object which
 * has dependency resolvable only inside the child container
 * and has ContainerControlledLifetimeManager()
* the resolve will fail.
* If you do this without the extension the resolve will succeed but the dependencies were most likely resolved in unexpected way (unless you really know what are you doing)
