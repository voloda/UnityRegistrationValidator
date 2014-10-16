using System;
using System.Collections.Generic;
using Microsoft.Practices.ObjectBuilder2;

namespace UnityRegistrationValidator
{
    public class TrackRegistrationsPolicy : ITrackRegistrationsPolicy
    {
        private TrackRegistrationsPolicy _parent;
        
        HashSet<NamedTypeBuildKey> _unityContainers = new HashSet<NamedTypeBuildKey>();

        public TrackRegistrationsPolicy(TrackRegistrationsPolicy parent)
        {
            _parent = parent;
        }

        public void Track(NamedTypeBuildKey key)
        {
            _unityContainers.Add(key);
        }

        public int GetRegistrationDepth(NamedTypeBuildKey key)
        {
            return GetRegistrationDepthInternal(key, 0);
        }

        private int GetRegistrationDepthInternal(NamedTypeBuildKey key, int currentDepth)
        {
            if (_unityContainers.Contains(key)) return currentDepth;

            if (_parent == null)
                throw new InvalidOperationException(string.Format("Cannot find registration depth of key {0}", key));

            return _parent.GetRegistrationDepthInternal(key, currentDepth + 1);
        }
    }
}