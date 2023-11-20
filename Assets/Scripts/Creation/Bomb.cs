using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.Creation
{
    public class Bomb
    {
        public Transform Transform { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        private Bomb([Inject(Id = ComponentReference.ObjectTransform)] Transform transform,
                     Rigidbody rigidbody,
                     [Inject(Id = ComponentReference.BombParentTransform)] Transform parent)
        {
            Transform = transform;
            Rigidbody = rigidbody;
            Transform.SetParent(parent);
            Transform.localPosition = Vector3.zero;
        }

        public class Factory : PlaceholderFactory<Bomb>
        {
        }
    }

    public enum ComponentReference
    {
        ObjectTransform,
        BombParentTransform
    }
}

