using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.ObjectBuilder2;

namespace UnityRegistrationValidator
{
    public class EnsureRegistrationDepthOrderStrategy : BuilderStrategy
    {
        private readonly Stack<int> _registrationDepths = new Stack<int>();

        public override void PreBuildUp(IBuilderContext context)
        {
            Debug.WriteLine("PreBuildUp operation: {0}", context.BuildKey);

            var trackRegistrationsPolicy = context.Policies.Get<ITrackRegistrationsPolicy>(context.BuildKey);

            int depth = trackRegistrationsPolicy.GetRegistrationDepth(context.BuildKey);
            int previousDepth = _registrationDepths.Count != 0 ? _registrationDepths.Peek() : int.MinValue;

            if (depth < previousDepth)
            {
                throw new InvalidOperationException(string.Format("Build key {0} with registration depth {1} has dependency on child registration on depth {2}", context.BuildKey, depth, previousDepth));
            }

            Debug.WriteLine("Pushed for {0}", context.BuildKey);
            _registrationDepths.Push(depth);
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            Debug.WriteLine("Poped for {0}", context.BuildKey);
            _registrationDepths.Pop();
        }
    }
}