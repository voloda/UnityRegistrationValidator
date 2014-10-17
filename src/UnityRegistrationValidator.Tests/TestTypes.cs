namespace UnityRegistrationValidator.Tests
{
    public interface IChild
    {

    }

    public interface IChild2
    {

    }

    public interface IRoot
    {

    }

    public class Child : IChild
    {
    }

    public class Child2 : IChild2
    {
        public Child2(IChild child)
        {
        }
    }

    public class Root : IRoot
    {
        private readonly IChild _child;

        public Root(IChild child, IChild2 child2)
        {
            _child = child;
        }
    }

    public class RootReferencingClass : IRoot
    {
        public RootReferencingClass(Child child)
        {
        }
    }

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
}