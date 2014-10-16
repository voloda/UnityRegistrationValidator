using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace UnityRegistrationValidator.Tests
{

    /// <summary>
    /// This set of tests ensures that the behavior for unity container with
    /// and without extension didn't change
    /// </summary>
    [TestFixture]
    public class UnityContainerCompabilityTests : UnityContainerCompabilityBaseTests
    {
        protected override UnityContainer CreateUnityContainer()
        {
            var rootContainer = new UnityContainer();
            return rootContainer;
        }
    }
}