using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.Creation;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.GameStates
{
    public class BombThrower : IFixedTickable
    {
        private Bomb _currentBomb;
        private Vector3 _targetPos;
        public void DragBomb(Pose pose)
        {
            _targetPos = pose.position;
        }

        public void FixedTick()
        {
            if (HasBomb())
            {
                Transform bombTransform = _currentBomb.Transform;
                bombTransform.position = Vector3.Lerp(bombTransform.position, _targetPos, 40f * Time.fixedDeltaTime);
            }
        }

        public void Throw()
        {
            _currentBomb.Transform.parent = null;
            _currentBomb.Rigidbody.useGravity = true;
            _currentBomb.Rigidbody.isKinematic = false;
            _currentBomb.Rigidbody.AddForce(Camera.main.transform.forward * 20f, ForceMode.Impulse);
        }

        public void ClearBomb() => _currentBomb = null;
        public void SetBomb(Bomb bomb)
        {
            _currentBomb = bomb;
        }
        public bool HasBomb() => _currentBomb != null;
    }
}
