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
}