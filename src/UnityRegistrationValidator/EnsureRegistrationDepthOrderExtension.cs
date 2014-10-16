using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace UnityRegistrationValidator
{
    public class EnsureRegistrationDepthOrderExtension : UnityContainerExtension
    {
        #region Private fields

        private int _childLevel;
        private TrackRegistrationsPolicy _trackRegistrationsPolicy;

        #endregion


        [InjectionConstructor]
        public EnsureRegistrationDepthOrderExtension() : this(0, new TrackRegistrationsPolicy(null))
        {
        }

        private EnsureRegistrationDepthOrderExtension(int childLevel, TrackRegistrationsPolicy trackRegistrationsPolicy)
        {
            _childLevel = childLevel;
            _trackRegistrationsPolicy = trackRegistrationsPolicy;
        }

        protected override void Initialize()
        {
            SubscribeEvents();

            Context.Strategies.AddNew<EnsureRegistrationDepthOrderStrategy>(UnityBuildStage.TypeMapping);
            Context.Policies.SetDefault(typeof(ITrackRegistrationsPolicy), _trackRegistrationsPolicy);
        }
        
        public override void Remove()
        {
            UnsubscribeEvents();

            base.Remove();
        }

        private void ContextRegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            _trackRegistrationsPolicy.Track(new NamedTypeBuildKey(e.RegisteredType, e.Name));
        }

        private void ContextRegistering(object sender, RegisterEventArgs e)
        {
            _trackRegistrationsPolicy.Track(new NamedTypeBuildKey(e.TypeFrom, e.Name));
            _trackRegistrationsPolicy.Track(new NamedTypeBuildKey(e.TypeTo, e.Name));
        }

        private void ContextChildContainerCreated(object sender, ChildContainerCreatedEventArgs e)
        {
            e.ChildContainer.AddExtension(new EnsureRegistrationDepthOrderExtension(_childLevel + 1, new TrackRegistrationsPolicy(_trackRegistrationsPolicy)));
        }

        private void SubscribeEvents()
        {
            Context.ChildContainerCreated += ContextChildContainerCreated;
            Context.Registering += ContextRegistering;
            Context.RegisteringInstance += ContextRegisteringInstance;
        }

        private void UnsubscribeEvents()
        {
            Context.RegisteringInstance -= ContextRegisteringInstance;
            Context.Registering -= ContextRegistering;
            Context.ChildContainerCreated -= ContextChildContainerCreated;
        }
    }
}