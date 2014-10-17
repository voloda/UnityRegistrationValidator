using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace UnityRegistrationValidator
{
    public class EnsureRegistrationDepthOrderStrategy : BuilderStrategy, IRequiresRecovery
    {
        private readonly Stack<int> _registrationDepths = new Stack<int>();
        private readonly Stack<NamedTypeBuildKey> _builds = new Stack<NamedTypeBuildKey>();

        
        public override void PreBuildUp(IBuilderContext context)
        {
            Debug.WriteLine("PreBuildUp operation: {0}", context.BuildKey);

            var trackRegistrationsPolicy = context.Policies.Get<ITrackRegistrationsPolicy>(context.BuildKey);

            int depth = trackRegistrationsPolicy.GetRegistrationDepth(context.BuildKey);
            int previousDepth = _registrationDepths.Count != 0 ? _registrationDepths.Peek() : int.MinValue;

            int item = IsSingleton(context) ? Math.Max(depth, previousDepth) : previousDepth;

            Debug.WriteLine("Current max: " + item);

            context.RecoveryStack.Add(this);

            _registrationDepths.Push(item);
            _builds.Push(context.BuildKey);
        }

        public override void PostBuildUp(IBuilderContext context)
        {
            Debug.WriteLine("PostBuildUp for {0}", context.BuildKey);

            var trackRegistrationsPolicy = context.Policies.Get<ITrackRegistrationsPolicy>(context.BuildKey);

            int depth = trackRegistrationsPolicy.GetRegistrationDepth(context.BuildKey);
            Debug.WriteLine("Current depth: " + depth);
            int currentDepth = _registrationDepths.Peek();

            Debug.WriteLine("Peek depth: " + currentDepth);

            // unity itself is a bit tricky and we do not need to care about it here
            if (!typeof(IUnityContainer).IsAssignableFrom(context.BuildKey.Type) && depth < currentDepth)
            {
                var stack = new StringBuilder();

                foreach(var build in _builds) 
                {
                    stack.AppendLine(build.ToString());
                }
                
                Debug.WriteLine(stack.ToString());
                throw new InvalidOperationException(string.Format("Build key {0} with registration depth {1} has dependency on child registration on depth {2} ({3})", context.BuildKey, depth, currentDepth, stack));
            }

            _registrationDepths.Pop();
            _builds.Pop();
        }

        private static bool IsSingleton(IBuilderContext context)
        {
            var lifetimePolicy = context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey);
            var isSingleton = lifetimePolicy is ContainerControlledLifetimeManager;
            return isSingleton;
        }

        public void Recover()
        {
            Debug.WriteLine("Running Recover()");
            // let's clean up
            _registrationDepths.Pop();
            _builds.Pop();
        }
    }
}