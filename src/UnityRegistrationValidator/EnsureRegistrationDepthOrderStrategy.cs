using System.Diagnostics;
using Microsoft.Practices.ObjectBuilder2;

namespace UnityRegistrationValidator
{
    public class EnsureRegistrationDepthOrderStrategy : BuilderStrategy
    {
        public override void PreBuildUp(IBuilderContext context)
        {
            var ensureRegistrationDepthOrderPolicy = context.Policies.Get<IEnsureRegistrationDepthOrderPolicy>(context.BuildKey);
            
            if (ensureRegistrationDepthOrderPolicy == null)
            {
                Debug.WriteLine("Registering policy");
                ensureRegistrationDepthOrderPolicy = new EnsureRegistrationDepthOrderPolicy();
                context.Policies.SetDefault(ensureRegistrationDepthOrderPolicy);
            }

            ensureRegistrationDepthOrderPolicy.PreBuildUp(context);
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            var ensureRegistrationDepthOrderPolicy = context.Policies.Get<IEnsureRegistrationDepthOrderPolicy>(context.BuildKey);
            ensureRegistrationDepthOrderPolicy.PostBuildUp(context);
        }
    }
}