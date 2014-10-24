using Microsoft.Practices.ObjectBuilder2;

namespace UnityRegistrationValidator
{
    public interface IEnsureRegistrationDepthOrderPolicy: IBuilderPolicy
    {
        void PreBuildUp(IBuilderContext context);
        void PostBuildUp(IBuilderContext context);
    }
}