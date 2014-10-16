using Microsoft.Practices.ObjectBuilder2;

namespace UnityRegistrationValidator
{
    public interface ITrackRegistrationsPolicy : IBuilderPolicy
    {
        void Track(NamedTypeBuildKey key);
        int GetRegistrationDepth(NamedTypeBuildKey key);
    }
}