using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.Creation;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.GameStates
{
    public class BombThrower : IFixedTickable
    {
        private Bomb _bomb;
        private Vector3 _targetPos;
        public void DragBomb(Pose pose)
        {
            _targetPos = pose.position;
        }

        public void FixedTick()
        {
            if (HasBomb())
            {
                Transform bombTransform = _bomb.Transform;
                bombTransform.position = Vector3.Lerp(bombTransform.position, _targetPos, 40f * Time.fixedDeltaTime);
            }
        }

        public void Throw()
        {
            _bomb.Transform.parent = null;
            _bomb.Rigidbody.useGravity = true;
            _bomb.Rigidbody.isKinematic = false;
            _bomb.Rigidbody.AddForce(Camera.main.transform.forward * 20f, ForceMode.Impulse);
        }

        public void ClearBomb() => _bomb = null;
        public void SetBomb(Bomb bomb)
        {
            _bomb = bomb;
        }
        public bool HasBomb() => _bomb != null;
    }
}
