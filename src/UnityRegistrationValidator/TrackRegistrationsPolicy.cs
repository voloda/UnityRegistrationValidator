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

            // Anything not found must be on current depth and happened somehow before
            // the extension registration
            if (_parent == null) return currentDepth;

            return _parent.GetRegistrationDepthInternal(key, currentDepth + 1);
        }
    }
}