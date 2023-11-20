using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.Creation
{
    public class Fortress
    {
        public Transform Transform { get; private set; }

        private Fortress(Pose pose, Transform transform)
        {
            Transform = transform;
            Transform.SetPositionAndRotation(pose.position, pose.rotation);
        }

        public class Factory : PlaceholderFactory<Pose, Fortress>
        {
        }
    }
}
