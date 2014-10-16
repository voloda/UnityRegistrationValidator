# UnityRegistrationValidator

Microsoft Unity Extension which verifies registrations between parent and child containers.

## How to enable extension

* Reference the `UnityRegistrationValidator.dll` in your project
* Call the registration below

```cs
var rootContainer = new UnityContainer();
rootContainer.AddNewExtension<EnsureRegistrationDepthOrderExtension>();
```

## Following rules are enforced after registering extension

* For each registration is tracked depth in containers (from the root container thru all child containers)
* During the build operation all the dependencies must be correct:
 * If a registration in parent container depends on a registration from child container the build will fail

